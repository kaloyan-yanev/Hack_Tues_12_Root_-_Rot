#include <WiFi.h>
#include <WiFiManager.h>
#include <PubSubClient.h>
#include <LittleFS.h>
#include <ArduinoJson.h>

#define CONFIG_FILE "/config.json"

String module_name = "imeto se zadava vednuj pri proizvodstvo";

const char* mqtt_server = "10.1.85.205";
const int mqtt_port = 1883;

// MQTT topics
String pubTopic = "machines/sensors/" + module_name;
String subTopic = "machines/control/temp_tresh/" + module_name;

// ===== CONFIG STRUCT =====
struct Config {
  String ssid;
  String password;
  String ip;
  String gateway;
  String subnet;
};

WiFiClient espClient;
PubSubClient client(espClient);

// ===== LOAD CONFIG =====
bool loadConfig(Config &config) {
  if (!LittleFS.exists(CONFIG_FILE)) return false;

  File file = LittleFS.open(CONFIG_FILE, "r");
  if (!file) return false;

  StaticJsonDocument<256> doc;
  if (deserializeJson(doc, file)) {
    file.close();
    return false;
  }
  file.close();

  config.ssid     = doc["ssid"].as<String>();
  config.password = doc["password"].as<String>();
  config.ip       = doc["ip"].as<String>();
  config.gateway  = doc["gateway"].as<String>();
  config.subnet   = doc["subnet"].as<String>();

  return true;
}

// ===== SAVE CONFIG =====
bool saveConfig(const Config &config) {
  StaticJsonDocument<256> doc;

  doc["ssid"]     = config.ssid;
  doc["password"] = config.password;
  doc["ip"]       = config.ip;
  doc["gateway"]  = config.gateway;
  doc["subnet"]   = config.subnet;

  File file = LittleFS.open(CONFIG_FILE, "w");
  if (!file) return false;

  serializeJson(doc, file);
  file.close();
  return true;
}

// ===== MQTT CALLBACK (FIXED) =====
void callback(char* topic, byte* payload, unsigned int length) {

  /*String message = "";

  for (int i = 0; i < length; i++) {
    message += (char)payload[i];
  }*/
  Serial.println(topic);
  String message = "";

  for (int i = 0; i < length; i++) {
    message += (char)payload[i];
  }

  float temp = message.toFloat();

  Serial.println(temp);
}

// ===== MQTT RECONNECT (SAFE) =====
void reconnect() {

  while (!client.connected()) {

    Serial.print("Connecting to MQTT...");

    if (client.connect("ESP32TestClient")) {
      Serial.println("connected");
      client.subscribe(subTopic.c_str());
      Serial.println("Subscribed to LED topic");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" retry in 2 seconds");

      delay(2000);
    }
  }
}

// ===== SETUP =====
void setup() {
  Serial.begin(115200);
  delay(1000);

  // ===== LittleFS =====
  if (!LittleFS.begin(true)) {
    Serial.println("LittleFS mount failed");
    return;
  }

  Config config;
  bool hasConfig = loadConfig(config);

  WiFiManager wm;
  wm.setTimeout(180);

  // Custom WiFiManager fields
  WiFiManagerParameter custom_ip("ip", "Static IP", config.ip.c_str(), 16);
  WiFiManagerParameter custom_gw("gw", "Gateway", config.gateway.c_str(), 16);
  WiFiManagerParameter custom_sn("sn", "Subnet", config.subnet.c_str(), 16);

  wm.addParameter(&custom_ip);
  wm.addParameter(&custom_gw);
  wm.addParameter(&custom_sn);

  // Apply static IP if exists
  if (hasConfig && config.ip != "") {
    IPAddress ipAddr, gwAddr, snAddr;

    if (ipAddr.fromString(config.ip) &&
        gwAddr.fromString(config.gateway) &&
        snAddr.fromString(config.subnet)) {

      Serial.println("Using saved static IP...");
      wm.setSTAStaticIPConfig(ipAddr, gwAddr, snAddr);
    }
  }

  // Connect WiFi
  if (!wm.autoConnect("ESP32-AP")) {
    Serial.println("WiFi failed");
    ESP.restart();
  }

  Serial.println("WiFi connected");
  Serial.println(WiFi.localIP());

  // Save config
  config.ssid     = WiFi.SSID();
  config.password = WiFi.psk(); // WiFi.psk() unreliable on ESP32
  config.ip       = custom_ip.getValue();
  config.gateway  = custom_gw.getValue();
  config.subnet   = custom_sn.getValue();

  saveConfig(config);

  // MQTT setup
  client.setServer(mqtt_server, mqtt_port);
  client.setCallback(callback);
}

// ===== LOOP =====
void loop() {
  if (!client.connected()) {
    reconnect();
  }

  client.loop();


  static unsigned long lastMsg = 0;

  if (millis() - lastMsg > 10000) {
    lastMsg = millis();

    String msg = "Hello from " + module_name;

    Serial.print("Publishing: ");
    Serial.println(msg);

    client.publish(pubTopic.c_str(), msg.c_str());
  }
}

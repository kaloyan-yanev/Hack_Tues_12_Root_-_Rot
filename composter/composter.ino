#include <WiFi.h>
#include <WiFiManager.h>
#include <LittleFS.h>
#include <ArduinoJson.h>

#define CONFIG_FILE "/config.json"
String module_name = "mashina 1";

const char* mqtt_server = "10.97.107.205";  // or your broker IP
const int mqtt_port = 1883;

// Structure to hold config
struct Config {
  String ssid;
  String password;
  String ip;
  String gateway;
  String subnet;
};

String pubTopic = "machines/" + module_name;
String subTopic = "control/" + module_name + "/temp_treshhold";

// Load config from LittleFS
bool loadConfig(Config &config) {
  if (!LittleFS.exists(CONFIG_FILE)) return false;

  File file = LittleFS.open(CONFIG_FILE, "r");
  if (!file) return false;

  StaticJsonDocument<256> doc;
  DeserializationError error = deserializeJson(doc, file);
  file.close();

  if (error) {
    Serial.println("Failed to parse config");
    return false;
  }

  config.ssid     = doc["ssid"].as<String>();
  config.password = doc["password"].as<String>();
  config.ip       = doc["ip"].as<String>();
  config.gateway  = doc["gateway"].as<String>();
  config.subnet   = doc["subnet"].as<String>();

  return true;
}

// Save config to LittleFS
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

// Reset config
void resetLittleFS() {
  if (LittleFS.exists(CONFIG_FILE)) {
    LittleFS.remove(CONFIG_FILE);
    Serial.println("Config erased.");
  }
}

// ===== MQTT Callback (runs when message arrives) =====
void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");

  String message;
  for (int i = 0; i < length; i++) {
    message += (char)payload[i];
  }
  Serial.println(message);
}

// ===== Reconnect MQTT =====
void reconnect() {
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");

    String clientId = "ESPClient-";
    clientId += String(random(0xffff), HEX);

    if (client.connect(clientId.c_str(), mqtt_user, mqtt_pass)) {
      Serial.println("connected");

      // Subscribe
      client.subscribe(subTopic);

    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" retrying in 5 seconds...");
      delay(5000);
    }
  }
}


void setup() {
  Serial.begin(115200);
  delay(1000);

  if (!LittleFS.begin(true)) {
    Serial.println("LittleFS mount failed");
    return;
  }

  Config config;
  bool hasConfig = loadConfig(config);

  WiFiManager wm;
  wm.setTimeout(180);

  // Custom parameters
  WiFiManagerParameter custom_ip("ip", "Static IP", config.ip.c_str(), 16);
  WiFiManagerParameter custom_gw("gw", "Gateway", config.gateway.c_str(), 16);
  WiFiManagerParameter custom_sn("sn", "Subnet", config.subnet.c_str(), 16);

  wm.addParameter(&custom_ip);
  wm.addParameter(&custom_gw);
  wm.addParameter(&custom_sn);

  // If we have saved static IP → apply it BEFORE connect
  if (hasConfig && config.ip != "") {
    IPAddress ip, gateway, subnet;

    if (ip.fromString(config.ip) &&
        gateway.fromString(config.gateway) &&
        subnet.fromString(config.subnet)) {

      Serial.println("Applying saved static IP...");
      wm.setSTAStaticIPConfig(ip, gateway, subnet);
    }
  }

  // Try auto connect or open portal
  if (!wm.autoConnect("ESP32-AP")) {
    Serial.println("Failed to connect (timeout)");
    return;
  }

  Serial.println("Connected to WiFi!");
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());

  // Save config after connection
  config.ssid     = WiFi.SSID();
  config.password = WiFi.psk();
  config.ip       = custom_ip.getValue();
  config.gateway  = custom_gw.getValue();
  config.subnet   = custom_sn.getValue();

  if (saveConfig(config)) {
    Serial.println("Config saved.");
  } else {
    Serial.println("Failed to save config.");
  }

  client.setServer(mqtt_server, mqtt_port);
  client.setCallback(callback);
}

void loop() {
  if (!client.connected()) {
    reconnect();
  }

  client.loop();

  // ===== Publish every 5 seconds =====
  static unsigned long lastMsg = 0;
  if (millis() - lastMsg > 5000) {
    lastMsg = millis();

    String msg = "Hello from ESP";
    Serial.print("Publishing: ");
    Serial.println(msg);

    client.publish(pubTopic, msg.c_str());
  }
}

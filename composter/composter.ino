#include <WiFi.h>
#include <WiFiManager.h>
#include <PubSubClient.h>
#include <LittleFS.h>
#include <ArduinoJson.h>
#include <OneWire.h>
#include <DallasTemperature.h>

// =======================================================
// 1. PIN DEFINITIONS
// =======================================================

#define ONE_WIRE_BUS 2

#define METHANE_PIN 34
#define CO2_PIN     35
#define HUM_PIN     32

#define FLOW_PIN    27

// Actuators
#define PUMP_PIN 25
#define RELAY_PUMP_HEATER 26
#define RELAY_MOTOR 27


// =======================================================
// 2. CONSTANTS
// =======================================================

#define ADC_REF_VOLTAGE 3.3
#define ADC_RESOLUTION 4095.0

#define RL 10.0

float FLOW_K = 7.5;

float R0_MQ2 = 10.0;
float R0_MQ135 = 10.0;

#define MQ2_A 574.25
#define MQ2_B -2.222

#define MQ135_A 110.47
#define MQ135_B -2.862


// Pump PWM limits
#define PUMP_PWM_CHANNEL 0
#define PUMP_PWM_FREQ 5000
#define PUMP_PWM_RESOLUTION 8
#define PUMP_MAX_DUTY 153   // 60% of 255


// =======================================================
// 3. HARDWARE OBJECTS
// =======================================================

OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature sensors(&oneWire);

DeviceAddress tempSensor1, tempSensor2;

volatile int pulseCount = 0;


// =======================================================
// 4. FLOW INTERRUPT
// =======================================================

void IRAM_ATTR flowPulse() {
    pulseCount++;
}


// =======================================================
// 5. LOW LEVEL HELPERS
// =======================================================

float readVoltage(int pin) {
    int raw = analogRead(pin);
    return (raw / ADC_RESOLUTION) * ADC_REF_VOLTAGE;
}

float getRs(float voltage) {
    if (voltage <= 0.01) return 9999;
    return ((ADC_REF_VOLTAGE - voltage) / voltage) * RL;
}

float getPPM(float rs, float r0, float A, float B) {
    float ratio = rs / r0;
    return A * pow(ratio, B);
}


// =======================================================
// 6. FLOW SENSOR
// =======================================================

float getFlowRate() {

    static unsigned long lastTime = 0;
    static int lastPulseCount = 0;

    unsigned long now = millis();

    if (now - lastTime >= 1000) {

        noInterrupts();
        int pulses = pulseCount;
        pulseCount = 0;
        interrupts();

        lastPulseCount = pulses;
        lastTime = now;
    }

    return (float)lastPulseCount / FLOW_K;
}


// =======================================================
// 7. SENSOR API
// =======================================================

float getTemp1() {
    sensors.requestTemperatures();
    return sensors.getTempC(tempSensor1);
}

float getTemp2() {
    sensors.requestTemperatures();
    return sensors.getTempC(tempSensor2);
}

float getMethane() {
    float v = readVoltage(METHANE_PIN);
    return getPPM(getRs(v), R0_MQ2, MQ2_A, MQ2_B);
}

float getCO2() {
    float v = readVoltage(CO2_PIN);
    return getPPM(getRs(v), R0_MQ135, MQ135_A, MQ135_B);
}

float getHumidity() {
    float v = readVoltage(HUM_PIN);

    float h = (v - 0.8) * (100.0 / (3.0 - 0.8));

    if (h < 0) h = 0;
    if (h > 100) h = 100;

    return h;
}


// =======================================================
// 8. ACTUATORS (PUMP + RELAYS)
// =======================================================

void setupPump() {
    ledcSetup(PUMP_PWM_CHANNEL, PUMP_PWM_FREQ, PUMP_PWM_RESOLUTION);
    ledcAttachPin(PUMP_PIN, PUMP_PWM_CHANNEL);
}

void setPump(int dutyPercent) {

    if (dutyPercent < 0) dutyPercent = 0;
    if (dutyPercent > 100) dutyPercent = 100;

    int pwmValue = map(dutyPercent, 0, 100, 0, 255);

    if (pwmValue > PUMP_MAX_DUTY) {
        pwmValue = PUMP_MAX_DUTY;
    }

    ledcWrite(PUMP_PWM_CHANNEL, pwmValue);
}

void stopPump() {
    ledcWrite(PUMP_PWM_CHANNEL, 0);
}

void setupRelays() {
    pinMode(RELAY_PUMP_HEATER, OUTPUT);
    pinMode(RELAY_MOTOR, OUTPUT);

    digitalWrite(RELAY_PUMP_HEATER, LOW);
    digitalWrite(RELAY_MOTOR, LOW);
}

void setRelay(int relayPin, int state) {
    digitalWrite(relayPin, state ? HIGH : LOW);
}


// =======================================================
// 9. WIFI + MQTT + CONFIG (UNCHANGED LOGIC)
// =======================================================

#define CONFIG_FILE "/config.json"

String module_name = "default_name";

const char* mqtt_server = "10.1.85.205";
const int mqtt_port = 1883;

String pubTopic = "machines/sensors/" + module_name;
String subTopic = "machines/control/temp_tresh/" + module_name;

struct Config {
    String ssid;
    String password;
    String ip;
    String gateway;
    String subnet;
};

WiFiClient espClient;
PubSubClient client(espClient);


// CONFIG LOAD
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

    config.ssid = doc["ssid"].as<String>();
    config.password = doc["password"].as<String>();
    config.ip = doc["ip"].as<String>();
    config.gateway = doc["gateway"].as<String>();
    config.subnet = doc["subnet"].as<String>();

    return true;
}


// CONFIG SAVE
bool saveConfig(const Config &config) {
    StaticJsonDocument<256> doc;

    doc["ssid"] = config.ssid;
    doc["password"] = config.password;
    doc["ip"] = config.ip;
    doc["gateway"] = config.gateway;
    doc["subnet"] = config.subnet;

    File file = LittleFS.open(CONFIG_FILE, "w");
    if (!file) return false;

    serializeJson(doc, file);
    file.close();
    return true;
}


// MQTT CALLBACK
void callback(char* topic, byte* payload, unsigned int length) {

    String message;

    for (int i = 0; i < length; i++) {
        message += (char)payload[i];
    }

    float value = message.toFloat();
}


// MQTT RECONNECT
void reconnect() {

    while (!client.connected()) {

        if (client.connect("ESP32Client")) {
            client.subscribe(subTopic.c_str());
        } else {
            delay(2000);
        }
    }
}


// =======================================================
// 10. SETUP
// =======================================================

void setup() {

    Serial.begin(115200);

    if (!LittleFS.begin(true)) return;

    Config config;
    bool hasConfig = loadConfig(config);

    WiFiManager wm;
    wm.setTimeout(180);

    WiFiManagerParameter custom_ip("ip", "Static IP", config.ip.c_str(), 16);
    WiFiManagerParameter custom_gw("gw", "Gateway", config.gateway.c_str(), 16);
    WiFiManagerParameter custom_sn("sn", "Subnet", config.subnet.c_str(), 16);

    wm.addParameter(&custom_ip);
    wm.addParameter(&custom_gw);
    wm.addParameter(&custom_sn);

    if (hasConfig && config.ip != "") {

        IPAddress ipAddr, gwAddr, snAddr;

        if (ipAddr.fromString(config.ip) &&
            gwAddr.fromString(config.gateway) &&
            snAddr.fromString(config.subnet)) {

            wm.setSTAStaticIPConfig(ipAddr, gwAddr, snAddr);
        }
    }

    if (!wm.autoConnect("ESP32-AP")) {
        ESP.restart();
    }

    config.ssid = WiFi.SSID();
    config.password = WiFi.psk();
    config.ip = custom_ip.getValue();
    config.gateway = custom_gw.getValue();
    config.subnet = custom_sn.getValue();

    saveConfig(config);

    client.setServer(mqtt_server, mqtt_port);
    client.setCallback(callback);

    sensors.begin();

    sensors.getAddress(tempSensor1, 0);
    sensors.getAddress(tempSensor2, 1);

    analogSetAttenuation(ADC_11db);

    pinMode(FLOW_PIN, INPUT_PULLUP);
    attachInterrupt(digitalPinToInterrupt(FLOW_PIN), flowPulse, RISING);

    setupPump();
    setupRelays();
}


// =======================================================
// 11. LOOP
// =======================================================

void loop() {

    if (!client.connected()) {
        reconnect();
    }

    client.loop();

    static unsigned long lastMsg = 0;

    if (millis() - lastMsg > 10000) {
        lastMsg = millis();

        String msg = "system running";
        client.publish(pubTopic.c_str(), msg.c_str());
    }
}
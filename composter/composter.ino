#include <WiFi.h>
#include <WiFiManager.h>
#include <PubSubClient.h>
#include <LittleFS.h>
#include <Preferences.h>
#include <ArduinoJson.h>
#include <OneWire.h>
#include <DallasTemperature.h>

Preferences preferences;

#define CONFIG_FILE "/config.json"

const unsigned long MINUTE = 60UL * 1000UL;
String module_name = "default";//задава уникално име при произвеждане на самия компостер
uint8_t temp_treshhold; //в градуси целзий
uint8_t hum_treshhold; //в проценти

const char* mqtt_server = "192.168.0.138";
const int mqtt_port = 1883;

// MQTT topics
String pubTopic = "machines/sensors/" + module_name;
String subTopic = "machines/control/" + module_name;

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
  String message = "";

  for (int i = 0; i < length; i++) {
    message += (char)payload[i];
  }

  String tpc = String(topic);
  if (tpc == subTopic + "/temp_tresh") {
    temp_treshhold = message.toInt();
    if(temp_treshhold > 10 && temp_treshhold < 90){
      preferences.putInt("temp", temp_treshhold);
    }
    
  }
  else if (tpc == subTopic + "/hum_tresh") {
    hum_treshhold = message.toInt();
    if(hum_treshhold > 10 && hum_treshhold < 90){
      preferences.putInt("hum", hum_treshhold);
    }
  }
  else if (tpc == subTopic + "/stir") {
    MotorOnFor(MINUTE*0.1);
    Serial.println("Burkai tam torta i taka");
    
  }
}

// ===== MQTT RECONNECT (SAFE) =====
void reconnect() {

  while (!client.connected()) {

    Serial.print("Connecting to MQTT...");

    if (client.connect("ESP32TestClient")) {
      Serial.println("connected to mqtt server");
      client.subscribe((subTopic + "/temp_tresh").c_str());
      client.subscribe((subTopic+"/hum_tresh").c_str());
      client.subscribe((subTopic + "/stir").c_str());
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" retry in 2 seconds");

      delay(2000);
    }
  }
}

// =======================================================
// 1. 📌 PINS (HARDWARE LAYER)
// =======================================================

#include <OneWire.h>
#include <DallasTemperature.h>

// =======================================================
// 1. 📌 PIN DEFINITIONS
// =======================================================

#define TEMP1_PIN D4   // D4
//#define TEMP2_PIN D5   // D5

#define METHANE_PIN A1   // A1
#define CO2_PIN     A2   // A2
#define HUM_PIN     A0   // A0

#define FLOW_PIN    6    // D6 (⚠️ risky but as requested)


// =======================================================
// 2. ⚙️ CONSTANTS (CALIBRATION / MATH MODEL)
// =======================================================
#define ADC_REF_VOLTAGE 3.3
#define ADC_RESOLUTION 4095.0
#define RL 10.0  // Load resistor (kΩ)

// Flow sensor calibration
float FLOW_K = 7.5;

// MQ sensor calibration
float R0_MQ2 = 10.0;
float R0_MQ135 = 10.0;

// MQ curve constants
#define MQ2_A 574.25
#define MQ2_B -2.222
#define MQ135_A 110.47
#define MQ135_B -2.862

// =======================================================
// 3. 🔌 HARDWARE OBJECTS
// =======================================================

// Two separate OneWire buses
OneWire oneWire1(TEMP1_PIN);
//OneWire oneWire2(TEMP2_PIN);

DallasTemperature sensors1(&oneWire1);
//DallasTemperature sensors2(&oneWire2);

// Flow interrupt counter
volatile int pulseCount = 0;


// =======================================================
// 4. ⚙️ FLOW SENSOR INTERRUPT
// =======================================================

void IRAM_ATTR flowPulse() {
    pulseCount++;
}


// =======================================================
// 5. 🧠 LOW LEVEL HELPERS
// =======================================================

// ADC → voltage
float readVoltage(int pin) {
    int raw = analogRead(pin);
    return (raw / ADC_RESOLUTION) * ADC_REF_VOLTAGE;
}

// Voltage → MQ resistance (Rs)
float getRs(float voltage) {
    if (voltage <= 0.01) return 9999;
    return ((ADC_REF_VOLTAGE - voltage) / voltage) * RL;
}

// Rs → PPM model
float getPPM(float rs, float r0, float A, float B) {
    float ratio = rs / r0;
    return A * pow(ratio, B);
}


// =======================================================
// 6. 🔄 FLOW SENSOR LOGIC
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
// 7. 📡 PUBLIC API (CLEAN INTERFACE)
// =======================================================

// ---- TEMPERATURE ----
float getTemp1() {
    sensors1.requestTemperatures();
    return sensors1.getTempCByIndex(0);
}

//float getTemp2() {
//    sensors2.requestTemperatures();
//    return sensors2.getTempCByIndex(0);
//}

// ---- GAS SENSORS ----
float getMethane() {
    float v = readVoltage(METHANE_PIN);
    float rs = getRs(v);
    return getPPM(rs, R0_MQ2, MQ2_A, MQ2_B);
}

float getCO2() {
    float v = readVoltage(CO2_PIN);
    float rs = getRs(v);
    return getPPM(rs, R0_MQ135, MQ135_A, MQ135_B);
}

// ---- HUMIDITY ----
float getHumidity() {
    float v = readVoltage(HUM_PIN);

    float h = (v - 0.8) * (100.0 / (3.0 - 0.8));

    if (h < 0) h = 0;
    if (h > 100) h = 100;

    return h;
}

// ---- CH4 BUFFER / TREND ----
#define SIZE 10
float ch4_buffer[SIZE];
unsigned long ch4BufferIndex = 0;
bool buffer_full = false;

void addCH4Reading(float value) {
    ch4_buffer[ch4BufferIndex] = value;
    ch4BufferIndex = (ch4BufferIndex + 1) % SIZE;

    if (ch4BufferIndex == 0) {
        buffer_full = true;
    }
}

bool isMethaneRising() {
    if (!buffer_full) return false;

    int rising_count = 0;
    float epsilon = 2.0; // ignore small noise (ppm)

    for (int i = 0; i < SIZE - 1; i++) {
        int idx1 = (ch4BufferIndex + i) % SIZE;
        int idx2 = (ch4BufferIndex + i + 1) % SIZE;

        if (ch4_buffer[idx2] > ch4_buffer[idx1] + epsilon) {
            rising_count++;
        }
    }

    return rising_count >= (SIZE - 1) * 0.6;
}

double Kp_temp = 8.0, Ki_temp = 0.3, Kd_temp = 0.0;  // PI for temperature
double Kp_hum = 2.5, Ki_hum = 0.2, Kd_hum = 0.4;    // PID for humidity

// --- PID objects ---
PID tempPID(&currentTemp, &heaterOutput, &tempSetpoint, Kp_temp, Ki_temp, Kd_temp, DIRECT);
PID humPID(&currentHumidity, &pumpOutput, &humiditySetpoint, Kp_hum, Ki_hum, Kd_hum, DIRECT);

// --- Timing ---
const unsigned long TEMP_INTERVAL = 3000;     // 3 sec for temperature
const unsigned long HUM_INTERVAL = 1000;      // 1 sec for humidity
unsigned long lastTempUpdate = 0;
unsigned long lastHumUpdate = 0;

// --- Time-proportioned relay ---
const unsigned long RELAY_CYCLE = 5000; // 5-second relay cycle
unsigned long relayCycleStart = 0;

void setupPID() {
    tempPID.SetMode(AUTOMATIC);
    tempPID.SetOutputLimits(0, 255);  // 0-255 scaled for relay time proportion
    humPID.SetMode(AUTOMATIC);
    humPID.SetOutputLimits(0, 255);   // 0-255 PWM for pump
}

// Call this in your main loop
void updatePID() {
    unsigned long now = millis();

    // --- Temperature PI (slow) ---
    if (now - lastTempUpdate >= TEMP_INTERVAL) {
        lastTempUpdate = now;
        currentTemp = getTemp1();  // your abstract function

        tempPID.Compute();

        // Time-proportioned relay
        if (now - relayCycleStart >= RELAY_CYCLE) relayCycleStart = now;
        unsigned long onTime = (heaterOutput / 255.0) * RELAY_CYCLE;

        if (now - relayCycleStart < onTime) setPumpHeaterRelay(true);
        else setPumpHeaterRelay(false);
    }

    // --- Humidity PID (faster) ---
    if (now - lastHumUpdate >= HUM_INTERVAL) {
        lastHumUpdate = now;
        currentHumidity = getHumidity();  // your abstract function

        humPID.Compute();

        // Apply PWM to pump
        setPumpSpeed((uint8_t)pumpOutput);
    }
}


// =======================================================
// 8. 🛠 SETUP
// =======================================================


void setupSensors() {
    Serial.println("Initializing sensors...");

    sensors1.begin();
    //sensors2.begin();

    analogSetAttenuation(ADC_11db);
    analogSetPinAttenuation(CO2_PIN, ADC_11db);

    pinMode(FLOW_PIN, INPUT_PULLUP);
    attachInterrupt(digitalPinToInterrupt(FLOW_PIN), flowPulse, RISING);

    Serial.println("Sensors ready!");
}

#define PWM_PUMP_PIN D7

// Relay outputs
#define RELAY_PUMP_HEATER_PIN D9
#define RELAY_MOTOR_PIN D8


// =======================================================
// 2. ⚙️ PWM CONFIG (ESP32 LEDC)
// =======================================================

#define PWM_CHANNEL   0
#define PWM_FREQ      5000
#define PWM_RES_BITS  8   // 0–255


// =======================================================
// 3. 🧠 INITIALIZATION FUNCTION
// =======================================================

void setupActuators() {

    // PWM setup for pump
    ledcAttachChannel(PWM_PUMP_PIN, PWM_FREQ, PWM_RES_BITS, PWM_CHANNEL);
    Serial.println("1111------");
    ledcWrite(PWM_CHANNEL, 0);

    // Relay pins
    pinMode(RELAY_PUMP_HEATER_PIN, OUTPUT);
    Serial.println("1111------");
    delay(1000);
    pinMode(RELAY_MOTOR_PIN, OUTPUT);
    Serial.println("1111------");
    delay(1000);

    // default OFF state (important for safety)
    
    Serial.println("1111------");
    delay(1000);
  
    digitalWrite(RELAY_PUMP_HEATER_PIN, HIGH);
    Serial.println("1111------");
    digitalWrite(RELAY_MOTOR_PIN, HIGH);
}


// =======================================================
// 4. 🚰 PWM WATER PUMP ABSTRACTION
// =======================================================

void setPumpSpeed(uint8_t pwmValue) {

    // safety clamp (0–255)
    if (pwmValue > 255) pwmValue = 255;

    ledcWrite(PWM_CHANNEL, pwmValue);
}


// =======================================================
// 5. 🔌 RELAY ABSTRACTION (0 = OFF, 1 = ON)
// =======================================================

void setPumpHeaterRelay(bool state) {
    digitalWrite(RELAY_PUMP_HEATER_PIN, state ? LOW : HIGH);
}

void setMotorRelay(bool state) {
    digitalWrite(RELAY_MOTOR_PIN, state ? LOW : HIGH);
}

bool motorOn = false;
unsigned int motorStartTime;
unsigned int motorDuration;
void MotorOnFor(unsigned long time) {
    if (motorOn) {
        Serial.println("Motor already running");
        return;
    }

    motorOn = true;
    motorStartTime = millis();
    motorDuration = time;

    setMotorRelay(true);
    Serial.println("BACE MOTORA SE PUSKAAAAAAA");
}

// Call this repeatedly in loop()
void updateMotor() {
    if (motorOn && (millis() - motorStartTime >= motorDuration)) {
        setMotorRelay(false);
        motorOn = false;
        Serial.println("BACE MOTORA SPIRAAAAA");
    }
}


void setup() {
  Serial.begin(115200);
  delay(1000);

  setupSensors();
  //Serial.println("1111------");
  //delay(500);
  //Serial.println("1111------");
  setupActuators();  

  // ===== LittleFS =====
  if (!LittleFS.begin(true)) {
    Serial.println("LittleFS mount failed");
    return;
  }

  preferences.begin("config", false);
  temp_treshhold = preferences.getInt("temp", 30);
  hum_treshhold  = preferences.getInt("hum", 50);

  Serial.println(temp_treshhold);
  Serial.println(hum_treshhold);


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
    if (millis() - lastMsg > MINUTE*0.25) {
        lastMsg = millis();

        // =======================================================
        // CREATE JSON PACKET
        // =======================================================

        StaticJsonDocument<256> doc;

        doc["compost_ID"] = module_name;

        // ordered sensor data
        doc["temperature"] = getTemp1();
        doc["humidity"] = getHumidity();

        doc["methane"] = getMethane();
        doc["co2"] = getCO2();

        //doc["flow"] = getFlowRate();

        // =======================================================
        // SERIALIZE
        // =======================================================

        char buffer[256];
        size_t n = serializeJson(doc, buffer);

        // =======================================================
        // DEBUG PRINT
        // =======================================================

        Serial.println("Publishing JSON:");
        Serial.println(buffer);

        // =======================================================
        // MQTT PUBLISH
        // =======================================================

        client.publish(pubTopic.c_str(), buffer, n);
    }

    static unsigned long lastch4check = 0;
    if (millis() - lastch4check > MINUTE) {
        lastch4check = millis();

        float methaneLevel = getMethane();
        addCH4Reading(methaneLevel);
        bool check = isMethaneRising();
        Serial.print("Methane levelppm: ");
        Serial.println(methaneLevel);
        Serial.print("Is there a rise tendency: ");
        Serial.println(check);

         if (methaneLevel > 500) {
        // Severe anaerobic condition → long stir
            MotorOnFor(MINUTE*15);

        } else if (methaneLevel > 100) {
            // Strong anaerobic → medium stir
            MotorOnFor(MINUTE*5);
    
        } else if (methaneLevel > 50) {
            // Mild anaerobic → short stir
            MotorOnFor(MINUTE*2);
    
        } else if (check) {
            // Early warning + rising trend → preventive stir
            MotorOnFor(MINUTE);
        }
    }

    static unsigned long stirInterval = 0;
    if (millis() - stirInterval > MINUTE/6) {
        stirInterval = millis();
        MotorOnFor(MINUTE/12);
    }
  
    updateMotor();
}

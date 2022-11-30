// Load Wi-Fi library
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <Esp.h>
#include <ArduinoJson.h>

//#include "stm8s003.h"
//#include "gpio.h"

// Replace with your network credentials
const char* ssid     = "AST-I";
const char* password = "zaqwerdxs";

const char* admin_username = "root";
const char* admin_password = "IJ%z80$Ako";

const unsigned int RelayPin = 4;
const unsigned int SwitchInput = 5;

int uid = 0;
String sensorType = "light";
boolean state = false;

// Set web server port number to 80
ESP8266WebServer server(80);

void setup() {
  Serial.begin(74880);

  pinMode(RelayPin, OUTPUT);
  pinMode(SwitchInput, INPUT_PULLUP);

  Serial.print("Connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  // Print local IP address and start web server
  Serial.println("");
  Serial.println("WiFi connected.");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());

  server.on("/setValues", handleValues);
  server.on("/getValues", getValues);
  server.on("/invertBooleans", invertBooleans);
  
  server.begin();
}

boolean oldSwitchInput = false;
void loop() {
  server.handleClient();
  if ((boolean)digitalRead(SwitchInput) != oldSwitchInput) {
    Serial.println((boolean)digitalRead(SwitchInput));
    state = !state;
    delay(1);
  }

  if (state)
    digitalWrite(RelayPin, HIGH);
  else
    digitalWrite(RelayPin, LOW);

  oldSwitchInput = (boolean)digitalRead(SwitchInput);
  delay(10);
}

void handleValues() {
for (int i = 0; i < server.args(); i++) {
  if (server.argName(i).equals("onState"))
    if (server.arg(i).equals("1"))
      state = true;
    else
        state = false;
} 

DynamicJsonDocument doc(1024);
  String json = "";

  doc["status"] = "OK";
  doc["uid"]   = uid;
  doc["sensor"] = "light";
  doc["state"] = state;
  
  serializeJson(doc, json);

server.send(200, "text/plain", json);       //Response to the HTTP request
}

void getValues() {
  DynamicJsonDocument doc(1024);
  String json = "";

  doc["status"] = "OK";
  doc["uid"]   = uid;
  doc["sensor"] = "light";
  doc["state"] = state;
  
  serializeJson(doc, json);

  server.send(200, "text/plain", json);
}

void invertBooleans(){
  for (int i = 0; i < server.args(); i++) {
    if (server.argName(i).equals("onState"))
      state = !state;
  } 
  DynamicJsonDocument doc(1024);
  String json = "";

  doc["status"] = "OK";
  doc["uid"]   = uid;
  doc["sensor"] = "light";
  doc["state"] = state;
  
  serializeJson(doc, json);
  server.send(200, "text/plain", json);
}

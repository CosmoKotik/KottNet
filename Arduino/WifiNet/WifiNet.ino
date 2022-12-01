//Original

// Load Wi-Fi library
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <ESP8266HTTPClient.h>
#include <WiFiClient.h>
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
String deviceType = "light";
boolean state = false;

// Set web server port number to 80
ESP8266WebServer server(80);
void setup() {
  Serial.begin(9600);

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
  server.on("/checkStatus", checkStatus);
  
  server.begin();  
//  client.setInsecure();

  InitStatus();
}

void InitStatus(){
  WiFiClient client;
  HTTPClient http;
      
  DynamicJsonDocument doc(1024);
  String json = "";

  doc["uid"] = uid;
  doc["deviceType"] = "light";
  doc["state"] = state;
  doc["ip"] = WiFi.localIP();
  doc["status"] = "online";
  serializeJson(doc, json);
  
  http.begin(client, "http://10.0.1.3:5058/api/AddNewDevice?device=" + json);
  if (http.GET() > 0){
    DynamicJsonDocument doc(1024);
    deserializeJson(doc, http.getString());
    
    const int l_uid = doc["uid"];
    const String l_deviceType = doc["deviceType"];
    const boolean l_state = doc["state"];
    const String l_ip = doc["ip"];
    const String l_room = doc["room"];
    const String l_device_group = doc["device_group"];

    uid = l_uid;
    deviceType = l_deviceType;
    state = l_state;

    Serial.println("uid updated to: ");
    Serial.print(uid);
  }
  else
  {
    while (http.GET() < 0) {
      Serial.println("Failed to send to server, trying again");
      http.begin(client, "http://10.0.1.3:5058/api/AddNewDevice?device=" + json);
      delay(5000);
    }
    InitStatus();
  }
}

boolean oldSwitchInput = false;
void loop() {
  server.handleClient();
  if ((boolean)digitalRead(SwitchInput) != oldSwitchInput) {
    //Serial.println((boolean)digitalRead(SwitchInput));
    state = !state;
    sendEvent();
    delay(1);
  }

  if (state)
    digitalWrite(RelayPin, HIGH);
  else
    digitalWrite(RelayPin, LOW);

  oldSwitchInput = (boolean)digitalRead(SwitchInput);
  delay(10);
}

void sendEvent(){
  DynamicJsonDocument doc(1024);
  String json = "";
  doc["uid"]   = uid;
  doc["deviceType"] = "light";
  doc["state"] = state;
  doc["ip"] = WiFi.localIP();
  doc["status"] = "online";
  serializeJson(doc, json);

  WiFiClient client;
  HTTPClient http;
  
  http.begin(client, "http://10.0.1.3:5058/api/UpdateDevice?device=" + json + "&isEsp=true");
  Serial.println(http.GET());
}

void handleValues() {
for (int i = 0; i < server.args(); i++) {
  if (server.argName(i).equals("state"))
    if (server.arg(i).equals("1"))
      state = true;
    else
        state = false;
} 

DynamicJsonDocument doc(1024);
  String json = "";

  doc["uid"]   = uid;
  doc["sensor"] = "light";
  doc["state"] = state;
  doc["status"] = "online";
  
  serializeJson(doc, json);

server.send(200, "text/plain", json);       //Response to the HTTP request
}

void getValues() {
  DynamicJsonDocument doc(1024);
  String json = "";

  doc["uid"]   = uid;
  doc["sensor"] = "light";
  doc["state"] = state;
  doc["status"] = "online";
  
  serializeJson(doc, json);

  server.send(200, "text/plain", json);
}

void invertBooleans(){
  for (int i = 0; i < server.args(); i++) {
    if (server.argName(i).equals("state"))
      state = !state;
  } 
  DynamicJsonDocument doc(1024);
  String json = "";

  doc["uid"]   = uid;
  doc["sensor"] = "light";
  doc["state"] = state;
  doc["status"] = "online";
  
  serializeJson(doc, json);
  server.send(200, "text/plain", json);
}

void checkStatus(){
  server.send(200);
}

#include <RCSwitch.h>
#include <Ethernet.h>
#include <SPI.h>

#define tempPin   1
#define lightPin  0 

byte mac[] = {0x40, 0x6e, 0x9f, 0x06, 0xe4, 0x7a};
IPAddress ip(192, 168, 4, 10);
RCSwitch mySwitch = RCSwitch();
EthernetServer server(32545);
bool connected = false;

const float Freq[][2] = {{8262702, 8262703}, {8262700, 8262701}, {8262698, 8262699}, {8262694, 8262695}, {8262689, 8262690}};

bool States[] = {false, true, false, false};

void setup() {
  Serial.begin(9600);
  
  mySwitch.enableTransmit(5);
  setAll(0);

  Ethernet.begin(mac, ip);
  Serial.print("Adress: ");
  Serial.println(Ethernet.localIP());
  server.begin();
  connected = true;
}

void loop() {
  if(!connected) return;
  EthernetClient ethernetClient = server.available();

  while (ethernetClient.connected())
  {
    //Serial.println("Application connected");
    char buffer[128];
    int count = 0;
    while(ethernetClient.available())
    {
      buffer[count++] = ethernetClient.read();
    }
    buffer[count] = '\0';

    if(count > 0)
    {
      Serial.println(buffer);
      //Toggle 1
      if(String(buffer) == String("Ch1ON")) setSwitch(1, 1);
      else if(String(buffer) == String("Ch1OFF")) setSwitch(1, 0);
      //Toggle 2
      if(String(buffer) == String("Ch2ON")) setSwitch(2, 1);
      else if(String(buffer) == String("Ch2OFF")) setSwitch(2, 0);
      //Toggle 3
      if(String(buffer) == String("Ch3ON")) setSwitch(3, 1);
      else if(String(buffer) == String("Ch3OFF")) setSwitch(3, 0);
      //Toggle 4
      if(String(buffer) == String("Ch4ON")) setSwitch(4, 1);
      else if(String(buffer) == String("Ch4OFF")) setSwitch(4, 0);
      //Toggle All
      if(String(buffer) == String("ChAllON")) setAll(1);
      else if(String(buffer) == String("ChAllOFF")) setAll(0);
      //Return States of the Switches to app
      if(String(buffer) == String("States")) returnStates(ethernetClient);
      //Return values of the sensors
      if(String(buffer) == String("getVal")) returnValues(ethernetClient);
    }
  }
}

void returnValues(EthernetClient client)
{
  int tempPinValue = analogRead(tempPin);
  int lightPinValue = analogRead(lightPin);
  String valueString = String(tempPinValue);
  valueString = String(valueString + ",");
  valueString = String(valueString + lightPinValue);
  Serial.println(valueString);
  client.print(valueString);
  //Serial.println("1024,945");
  //client.print("1024,945");
}

void setSwitch(int adapter, int state)
{
  Serial.print(String("Toggling switch "));
  Serial.print(adapter);
  Serial.print(" ");
  Serial.println(state);
  mySwitch.send(Freq[adapter - 1][state], 24);
  if(state == 0) States[adapter - 1] = false;
  else States[adapter - 1] = true;
  delay(100);
  mySwitch.send(Freq[adapter - 1][state], 24);
  delay(100);
}

void setAll(int state)
{
  for(int i = 0; i < 4; i++)
  {
    if(state == 0) States[i] = false;
    else States[i] = true;
  }
  mySwitch.send(Freq[4][state], 24);
  delay(100);
  mySwitch.send(Freq[4][state], 24);
  delay(100);
}

void returnStates(EthernetClient client)
{
  String bools = "";
  for(int i = 0; i < 4; i++)
  {
    String temp;
    if(States[i]) temp = "true";
    else temp = "false"; 
    bools = String(bools + temp);
    if(i < 3) bools = String(bools + ",");
  }
  Serial.println(bools);
  client.print(bools);
}


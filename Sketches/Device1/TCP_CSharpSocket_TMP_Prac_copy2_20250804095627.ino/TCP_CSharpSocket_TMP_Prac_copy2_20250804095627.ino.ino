#include <SPI.h>
#include <Ethernet.h>
#include <string.h>
#include <unistd.h>


//Analog reference .voltage
//#define aref_voltage 3.3
#define aref_voltage 5.0

//Connect TMP36 to Analog pin 0
int tempPin = A5;

//Analog temperature result
int tempReading;

// MAC address for the Ethernet shield
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xEF };

//byte server[] = {169, 254, 102, 3};
IPAddress server(169,254,102,3);
//IPAddress server(169,254,178,156);

// IP configuration for Arduino
IPAddress ip(169, 254, 102, 22);

EthernetServer receiver = EthernetServer(8888);  //not a server, but functionally a 'receiver' functionalty for two-way communications
EthernetClient transmitter; //serves as the transmitter capability

/*---------------------------------------------------------------------------*/
void setup() {

  //Read voltage value from sensor
  tempReading = analogRead(tempPin);
  
  //Init serial monitor for testing
  Serial.begin(9600);

  //Serial connection test
  Serial.println("setup function in action");

  //Use external analog reference voltage
  //analogReference(EXTERNAL);

  Ethernet.begin(mac, ip);
  receiver.begin(); 

  Serial.print("Server is ");
  if(Ethernet.hardwareStatus() == 1){
    Serial.println("Functional");
  }else if(Ethernet.hardwareStatus() == 0){
    Serial.println("not functioning correctly");
  } 
}

/*---------------------------------------------------------------------------*/
void loop() {

  //Print to serial monitor
  //Serial.print("RAW input: ");
  //Serial.println(tempReading);

  //Voltage reading 
  float voltage = tempReading * aref_voltage / 1024.0;

  //Print voltage to serial monitor
  //Serial.print("Volts: ");
  //Serial.println(voltage);

  //Temperature conversion from voltage
  float temperatureC = (voltage - 0.5) * 100;
  float tempF = (temperatureC * 1.8) + 32; 
  //Serial.println(temperatureC);
  Serial.println(tempF);
  //delay(1000);
  

  transmitter.connect(server, 8888);  
  if(transmitter.connected()){
    transmitter.println(tempF);
    //transmitter.stop();
    //Serial.println("Connection successful");
    }
}

/*---------------------------------------------------------------------------*/
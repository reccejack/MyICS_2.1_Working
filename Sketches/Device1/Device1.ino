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
//byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };  //MAC Address for Device 1
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xEF };  //MAC Address for Device 2

//byte server[] = {169, 254, 102, 3};
IPAddress server(169,254,102,3);
//IPAddress server(169,254,178,156);

// IP configuration for Arduino
//IPAddress ip(169, 254, 102, 20);  //IPv4 address for Device 1
IPAddress ip(169, 254, 102, 22);  //IPv4 address for Device 2

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

  //Add 100 for 'Device 100'
  // float tempF = (temperatureC * 1.8) + 132;  //Original equation on adds +32

  //Add 100 for 'Device 200'
  float tempF = (temperatureC * 1.8) + 32;  //Original equation on adds +32

  //Serial.println(temperatureC);
  Serial.println(tempF);
  //delay(1000);
  

  transmitter.connect(server, 8888);  
  if(transmitter.connected()){
    //EXPERIMENTAL CODE
    byte deviceId1 = 1; //Example device id
    byte deviceId2 = 2;
    byte b = tempF;
    //tempF will remain the example 'sensor value' 
    //packet[0] should equal deviceId
    //packet[1] should equal (byte)tempF

    byte packet[2];

    //Device 100 byte array identifier (COM 3)
    //packet[0] = deviceId1;
    //packet[1] = b;

    //Device 200 byte array identifier (COM 6)
    packet[0] = deviceId2;
    packet[1] = b;   

    //Device 100 .write statement
    transmitter.write(packet, 2);
    //transmitter.println("COM 3 Connected");
    
    //Device 200 .write statement
    // transmitter.write(packet2, 2);
    //transmitter.println("COM 6 Connected");

    //Device 200 byte array identifier
    //packet2[0] = deviceId2;
    //packet2[1] = b;    

    //transmitter.println(tempF);
    /*
    for(int i = 0; i < 5; i++){
      transmitter.write(packet1, 2);
    };
    for(int i = 0; i < 5; i++){
      transmitter.write(packet2, 2);
    };
    */
    // for(int i = 0; i < 20; i++){
    //   if(i % 2 == 0){
    //     transmitter.write(packet1, 2);
    //   } else if(i % 2 != 0){
    //     transmitter.write(packet2,2);
    //   }
    // }
            
    
    //Serial.println("Connection successful");
    }
}

/*---------------------------------------------------------------------------*/
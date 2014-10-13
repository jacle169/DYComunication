#include <Stepper.h>
#include <DallasTemperature.h>
#include <IRremote.h>
#include <EEPROM.h>

String MDYid="";
String DYid="smart0002";

int current=511;

#define DHT11_PIN 0

int reset=0;
int setpwd=1;
int SetPinMode=4;
int SetDigital=5;
int SetPWM=6;
int SetServo=7;
int SetSteper=8;
int Get18B20=9;
int GetDigital=10;
int GetAnalog=11;
int GetAllDigitalAndAnalog=12;
int SendIRRemote=15;
int dht11=16;

void setup()
{
  DDRC |= _BV(DHT11_PIN);
  PORTC |= _BV(DHT11_PIN);
  Serial.begin(19200);
}

void loop()
{
  while(Serial.available())
  {
    byte rrc = Serial.read();
    if(rrc != 58)
    {
      EEPROM.write(current,rrc);
      current++;
    }else{
     byte rrc1=Serial.read();
     if(rrc1 == 124){
       takeMainDYid();
       if(checkPt(9)){
         controlOrder();
         current=511;
       }else{
       current=511;
       SendString("Err:pt");
       break;
       }
     }else{
       EEPROM.write(current,rrc);
       current++;
       EEPROM.write(current,rrc1);
       current++; 
     }
    }
   }
} 

void takeMainDYid()
{
  MDYid="";
  int ct=511;
  for(int i=0;i<9;i++)
  {
    MDYid+=EEPROM.read(ct);
    ct+=1;
  }
}








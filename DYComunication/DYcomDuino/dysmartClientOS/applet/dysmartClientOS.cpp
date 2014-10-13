#include <IRremote.h>
#include <aJSON.h>

#include "WProgram.h"
void setup();
void loop();
void sendToServer(char* data);
void ir(aJsonObject* jsonObject);
String netid="123";
String hid="123456";
int login=0;

char jsonString[1024];
int current=0;

void setup()
{
  Serial1.begin(19200);
  Serial.begin(9600);
  Serial.println("ok");
}

void loop()
{
  if(Serial1.available())
  {
    char c = Serial1.read();
    if(c != '\r')
    {
      jsonString[current]=c;
      current++;
    }else{
     aJsonObject* jsonObject = aJson.parse(jsonString);
     aJsonObject* nid = aJson.getObjectItem(jsonObject, "nid");
     aJsonObject* id = aJson.getObjectItem(jsonObject, "hid");
     aJsonObject* back = aJson.getObjectItem(jsonObject, "back");
     if(netid.equals(nid->valuestring) && hid.equals(id->valuestring)){
       aJsonObject* opera = aJson.getObjectItem(jsonObject, "op");  
       if(opera->valueint == login){
         ir(jsonObject);
         if(back){sendToServer("client ok");}
       }
     }
     current=0;     
    }
   }
}

void sendToServer(char* data)
{
      Serial1.print(data);
      Serial1.print('\r');
}

void ir(aJsonObject* jsonObject)
{
  aJsonObject* val = aJson.getObjectItem(jsonObject, "val");
  int len= aJson.getArraySize(val);
  unsigned int irData[len];
  for (int i=0;i<len;i++)
  {
    aJsonObject* subitem=aJson.getArrayItem(val,i);
    irData[i]=subitem->valueint;
  }
    //ir\u53d1\u9001pin3
  IRsend irsender;
  irsender.sendRaw(irData,len,38);
}




int main(void)
{
	init();

	setup();
    
	for (;;)
		loop();
        
	return 0;
}


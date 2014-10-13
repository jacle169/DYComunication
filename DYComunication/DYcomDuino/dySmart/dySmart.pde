#include <IRremote.h>
#include <DallasTemperature.h>
#include <Stepper.h>
#include <EEPROM.h>
#include <Ethernet.h>
#include "Dhcp.h"
#include "Dns.h"

String DYid="smart0001";

DnsClass dyDns;
String dnsIP="";
String clientIP="";
boolean DnsOK=false;

Server tcp(4533);
Server httpServer(80);
boolean dhcpReady=false;

boolean SerialOK=false;
String str="";

void setup()
{
  //Serial.begin(19200);
  CheckConnect(); 
}

void loop()
{
  if(dhcpReady)
  {
   tcpWork();
   httpWork();
   if(SerialOK){
     if(Serial.available()){
       char sc = Serial.read();
       if(sc != '\r'){ 
         str +=sc;
       }else{
       if(str.startsWith(DYid)){
        String sdt=str.replace(DYid,"");
        tcp.write(IntTob(sdt.length()+12),4);
        tcp.write(IntTob(16),4);//SerialOnData
        tcp.write(IntTob(sdt.length()),4);
        tcp.print(sdt);
       }
       str="";
     }
    }
   }
  }
}

void httpWork(){
  Client client = httpServer.available();
  if (client) {
    String httpString="";
    while (client.connected()) {
      if (client.available()) {
        if(!httpString.endsWith("\n\r\n")){
          //添加非指令字符到缓存器
          char hc  = client.read();
          httpString+=hc;
        }else{
          client.println("HTTP/1.1 200 OK");
          client.println("Content-Type: text/html");
          client.println();
          tackMsg(client);
          break;
         }
      }
    }
    // give the web browser time to receive the data
    delay(1);
    client.stop();
  }
}

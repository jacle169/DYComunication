#include <IRremote.h>
#include <DallasTemperature.h>
#include <Stepper.h>
#include <EEPROM.h>
#include <Ethernet.h>
#include "Dhcp.h"
#include "Dns.h"

#include "WProgram.h"
void setup();
void loop();
void httpWork();
void initReader();
int ReadInt();
String ReadString();
byte ReadByte();
void SendFloat(Client client,int operation,float ft);
void SendInt(Client client,int operation,int dt);
void SendString(Client client,int operation, String dt);
void AddLen(Client client,int lenth);
void sByte(Client client,byte bt);
void sInt(Client client,int lenth);
void sFloat(Client client,float ft);
byte* FloatTob(float ft);
float bToFloat(byte b0,byte b1,byte b2,byte b3);
byte* IntTob(unsigned long val);
unsigned long bToInt(byte b0,byte b1,byte b2,byte b3);
void resetPwd(String dt);
boolean checkPwd();
String readPwd(int pwdLen);
void getClientIP(Client client);
String ipToStr(byte* ip);
void CheckConnect();
void postId(byte dnsip[4]);
void startServer(byte* sip);
void RandomMac();
void runRrevcIR(Client client,byte pin);
void dump(decode_results *results,Client client);
void servoToPot(int servopin,int myangle);
void stepperGo(int Steps,byte pin1,byte pin2,byte pin3,byte pin4,int Speed,int doStep);
float get18b20(byte pin);
void tcpWork();
void tackMsg(Client client);
boolean checkPt();
boolean checkDnsOrder(Client client);
void controlOrder(Client client);
void doSendIRRemote(Client client);
void doGetAllDigitalAndAnalog(Client client);
void doSteper(Client client);
void doSetServo(Client client);
void doSetPwd(Client client);
void SendToSerial(Client client);
DnsClass dyDns;
String dnsIP="";
String clientIP="";
boolean DnsOK=false;

Server tcp(4533);
Server httpServer(80);
boolean dhcpReady=false;

boolean SerialOK=false;
String str="";
String httpString="";

int reset=0;
int setpwd=1;
int runSerial=2;
int sendSerial=3;
int SetPinMode=4;
int SetDigital=5;
int SetPWM=6;
int SetServo=7;
int SetSteper=8;
int Get18B20=9;
int GetDigital=10;
int GetAnalog=11;
int GetAllDigitalAndAnalog=12;
int runIRReader=13;
int IRReadOnData=14;
int SendIRRemote=15;
int SerialOnData=16;

char sc;
char hc;

void setup()
{
  Serial.begin(19200);
  CheckConnect(); 
}

void loop()
{
  if(dhcpReady)
  {
   tcpWork();
   delay(1);
   httpWork();
   delay(1);
   if(SerialOK){
     if(Serial.available()){
       sc = Serial.read();
       if(sc != '\r'){
         str +=sc;
       }else{
       tcp.write(IntTob(str.length()+12),4);
       tcp.write(IntTob(SerialOnData),4);
       tcp.write(IntTob(str.length()),4);
       tcp.print(str);
       str="";
     }
    }
   }
   delay(1);
  }
}

void httpWork(){
  Client client = httpServer.available();
  if (client) {
    httpString="";
    while (client.connected()) {
      if (client.available()) {
        if(!httpString.endsWith("\n\r\n")){
          //\u6dfb\u52a0\u975e\u6307\u4ee4\u5b57\u7b26\u5230\u7f13\u5b58\u5668
          hc  = client.read();
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
int Current=0;
int bytesLen=0;
int i;
//byte bytes[1];
byte rb;
int len;
String m="";

void initReader()
{
  Current=511+4;
}

int ReadInt()
{
  i = bToInt(EEPROM.read(Current),EEPROM.read(Current+1),EEPROM.read(Current+2),EEPROM.read(Current+3));
  Current+=4;
  return i;
}

String ReadString()
{
  m="";
  len = ReadInt();
  for(int i=0;i<len;i++)
  {
    m+=EEPROM.read(Current);
    Current+=1;
  }
  return m;
}

//byte* ReadBytes()
//{
//  free(bytes);
//  bytesLen = ReadInt();
//  bytes[bytesLen];
//  for(int i=0;i<bytesLen;i++)
//  {
//    bytes[i] = EEPROM.read(Current);
//    Current+=1;
//  }  
//  return bytes;
//}

byte ReadByte()
{
  rb=EEPROM.read(Current);
  Current+=1;
  return rb;
}


int msgLen;
byte* b;

void SendFloat(Client client,int operation,float ft)
{
  AddLen(client,8);
  sInt(client,operation);
  sFloat(client,ft);
}

void SendInt(Client client,int operation,int dt)
{
  AddLen(client,8);
  sInt(client,operation);
  sInt(client,dt);
}

void SendString(Client client,int operation, String dt)
{
   msgLen=dt.length()+8;
   AddLen(client,msgLen);
   sInt(client,operation);
   sInt(client,dt.length());
   client.print(dt);
}

void AddLen(Client client,int lenth)
{
  client.write(IntTob(lenth+4),4);
}

void sByte(Client client,byte bt)
{
  client.write(bt);
}

void sInt(Client client,int lenth)
{
  client.write(IntTob(lenth),4);
}

void sFloat(Client client,float ft)
{
  b=FloatTob(ft);
  client.write(b,4);
}
byte bs[4];
float fval;
byte bi[4];
unsigned long Intval;

union u_tag {
    byte b[4];
    float val;
} f; 

byte* FloatTob(float ft)
{
 f.val=ft;
 bs[0]=f.b[0];
 bs[1]=f.b[1];
 bs[2]=f.b[2];
 bs[3]=f.b[3];
 return bs;
}

float bToFloat(byte b0,byte b1,byte b2,byte b3){
f.b[0] = b0;
f.b[1] = b1;
f.b[2] = b2;
f.b[3] = b3;
fval=f.val;
return fval;
}

byte* IntTob(unsigned long val) {
  bi[3] = (byte )((val >> 24) & 0xff);
  bi[2] = (byte )((val >> 16) & 0xff);
  bi[1] = (byte )((val >> 8) & 0xff);
  bi[0] = (byte )(val & 0xff);
  return bi;
}

unsigned long bToInt(byte b0,byte b1,byte b2,byte b3) {
  Intval = b3 << 24;
  Intval |= b2 << 16;
  Intval |= b1 << 8;
  Intval |= b0;
  return Intval;
}
String rpwd="";
String rrpwd="";

void resetPwd(String dt)
{
  for(int i = 0;i < dt.length(); i++){
    EEPROM.write(i,dt.charAt(i));
  }
}

boolean checkPwd()
{
  rrpwd = ReadString();
  if(rrpwd.equals(readPwd(rrpwd.length()))){
   return true;
  }else { return false;}
}

String readPwd(int pwdLen)
{
  rpwd="";
  for(int i = 0;i < pwdLen; i++){
    rpwd += EEPROM.read(i);
  }
  return rpwd;
}
String ipstr="";
byte stip[4];
  
void getClientIP(Client client)
{
  client.getRemoteIP(stip);
  clientIP = ipToStr(stip);
}

String ipToStr(byte* ip)
{
  for(int i=0;i<4;i++){
   ipstr += (int)ip[i];
   if(i<3){
   ipstr +='.';
   }
  }
  return ipstr;
}
byte mac[6];
byte ip[4]; 
byte dyDnsIP[4];
int results;

void CheckConnect()
{
  RandomMac();
  if(Dhcp.beginWithDHCP(mac) == 1)
  {  
    Dhcp.getLocalIp(ip);
    Dhcp.getDnsServerIp(dyDnsIP);
    dyDns.init("dns.dy2com.com", dyDnsIP);  
    dyDns.resolve();
    while(!(results=dyDns.finished())) ;  
    if(results ==1){
      DnsOK=true;
      dyDns.getIP(dyDnsIP);
      dnsIP = ipToStr(dyDnsIP);
      postId(dyDnsIP); 
    }
    startServer(ip);
    //Serial.println("ok");
  }
}

void postId(byte dnsip[4])
{
 Client client(dnsip, 80);
 if (client.connect()) {
  client.println("GET /default.aspx?id=123456789 HTTP/1.0");
  client.println();
 }
 client.stop();
}

void startServer(byte* sip)
{
  sip[3]=254;
  Ethernet.begin(mac, sip);
  tcp.begin();
  httpServer.begin();
  dhcpReady=true;
}

void RandomMac()
{
 mac[0]=random(1, 254);
 mac[1]=random(1, 254);
 mac[2]=random(1, 254);
 mac[3]=random(1, 254);
 mac[4]=random(1, 254);
 mac[5]=random(1, 254);
}


int count;
int pulsewidth;
DallasTemperature tempSensor; 

void runRrevcIR(Client client,byte pin)
{
 IRrecv irrecv(pin);
 decode_results results;
 irrecv.enableIRIn(); 
 SendString(client,runIRReader,"IROnRead");
 while(true){
 if (irrecv.decode(&results)) {
  dump(&results,client);
  break; 
  }
 }
}

void dump(decode_results *results,Client client) {
  count = results->rawlen;
  AddLen(client,8+((count-1)*4));
  sInt(client,IRReadOnData);
  sInt(client,count-1);
  for (int i = 0; i < count; i++) {
    if(i>0){
     sInt(client,(int)results->rawbuf[i]*USECPERTICK);
    }
  }
}

void servoToPot(int servopin,int myangle)
{
  for(int i=0;i<=50;i++){
  pulsewidth=(myangle*11)+500;
  digitalWrite(servopin,HIGH);
  delayMicroseconds(pulsewidth);
  digitalWrite(servopin,LOW);
  delay(20-pulsewidth/1000);
  }
}

//\u6b65\u8fdb
void stepperGo(int Steps,byte pin1,byte pin2,byte pin3,byte pin4,int Speed,int doStep){
Stepper stepper(Steps,pin1,pin2,pin3,pin4);
stepper.setSpeed(Speed);
stepper.step(doStep); 
}

float get18b20(byte pin){
tempSensor.begin(pin);
return tempSensor.getTemperature();
}
int current=511;
byte rrc;
byte rrc1;
int opera;
int rlen;
unsigned int irData[1];
int steps;
byte pin1;
byte pin2;
byte pin3;
byte pin4;
int Speed;
int doStep;
byte spin;
byte pot;
String pwdst="";
int ml;

void tcpWork(){
  Client client = tcp.available();
  tackMsg(client);
}

void tackMsg(Client client){
  while(client.available()) {
     rrc = client.read();
     if(rrc != 13){
     EEPROM.write(current,rrc);
     current++; 
     }else{
      rrc1=client.read();
      if(rrc1 == 10){
       if(checkPt()){
       controlOrder(client);
       current=511;
       }else{
       current=511;
       SendString(client,reset,"Err:pt");
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

boolean checkPt(){
 ml=bToInt(EEPROM.read(511),EEPROM.read(512),EEPROM.read(513),EEPROM.read(514));
 //sub end mark -2 "\r\n"
 if(ml-2>=current-511){
  return true;
 }else{
  return false; 
 }
}

boolean checkDnsOrder(Client client)
{
  getClientIP(client);
  if(DnsOK && dnsIP.equals(clientIP))
  {return true;}else{return false;}
}

void controlOrder(Client client){
   initReader();
   opera= ReadInt();
   if(opera==reset){
    if(checkDnsOrder(client)){
      resetPwd("123456");
      SendString(client,reset,"RSETOK");
    }else{
    SendString(client,reset,"Err:Dns");
    client.stop();}
   }else if(opera==setpwd){
    if(checkPwd()){
      doSetPwd(client);
    }else{ client.stop(); }   
   }else if(opera == runSerial){
    if(checkPwd()){
     Serial.begin(ReadInt());
     SerialOK=true;
     SendString(client,runSerial,"TLLSOK");
    }else{ client.stop();}
   }else if(opera == sendSerial){
    if(checkPwd()){
     SendToSerial(client);
    }else{client.stop();} 
   }else if(opera == SetPinMode){
    if(checkPwd()){
     pinMode(ReadByte(),ReadByte()==0?INPUT:OUTPUT);
     SendString(client,SetPinMode,"SPMOK");
    } else {client.stop();}
   }else if(opera == SetDigital){
    if(checkPwd()){
     digitalWrite(ReadByte(),ReadByte()==0?LOW:HIGH);
     SendString(client,SetDigital,"SDTOK");
    }else{ client.stop();} 
   }else if(opera == SetPWM){
    if(checkPwd()){
     analogWrite(ReadByte(),ReadByte());
     SendString(client,SetPWM,"SPwmOK");
    }else{client.stop();} 
   }else if(opera == SetServo){
     if(checkPwd()){
      doSetServo(client);      
     }else{client.stop();} 
   }else if(opera == SetSteper){
    if(checkPwd()){
     doSteper(client);
    }else{client.stop();} 
   }else if(opera == Get18B20){
    if(checkPwd()){
     SendFloat(client,Get18B20,get18b20(ReadByte()));
    }else{client.stop();} 
   }else if(opera == GetDigital){
    if(checkPwd()){
     SendInt(client,GetDigital,digitalRead(ReadByte()));
    }else{client.stop();} 
   }else if(opera == GetAnalog){
    if(checkPwd()){
     SendInt(client,GetAnalog,analogRead(ReadByte()));
    } else {client.stop();}
   }else if(opera == GetAllDigitalAndAnalog){
    if(checkPwd()){
     doGetAllDigitalAndAnalog(client);
    }else{client.stop();} 
   }else if(opera == runIRReader){
    if(checkPwd()){
     runRrevcIR(client,ReadByte());
    } else {client.stop();}
   }else if(opera == SendIRRemote){
    if(checkPwd()){
     doSendIRRemote(client);
    }else{client.stop();} 
   }
   
   else{
     client.flush();
     client.stop();
   }
}

void doSendIRRemote(Client client)
{
  rlen= ReadInt();
  irData[rlen];
  for(int i=0;i<rlen;i++){
   irData[i]=ReadInt();
  }
  //ir\u53d1\u9001pin3
  IRsend irsender;
  irsender.sendRaw(irData,rlen,38);
  SendString(client,SendIRRemote,"IRSOK");
}

void doGetAllDigitalAndAnalog(Client client)
{
  AddLen(client,4+(6*4)+14);
  sInt(client,GetAllDigitalAndAnalog);
  for(int i=0;i<6;i++){
   sInt(client,analogRead(i)); 
  }
  for(int i=0;i<14;i++){
    sByte(client,digitalRead(i));
  }
}

void doSteper(Client client)
{
  steps=ReadInt();
  pin1=ReadByte();
  pin2=ReadByte();
  pin3=ReadByte();
  pin4=ReadByte();
  Speed=ReadInt();
  doStep=ReadInt();
  stepperGo(steps,pin1,pin2,pin3,pin4,Speed,doStep);
  SendString(client,SetSteper,"STOK");
}

void doSetServo(Client client)
{
  spin =ReadByte();
  pot =ReadByte();
  if(pot >=0 && pot <=180){
   servoToPot(spin,pot); 
  }
  SendString(client,SetServo,"SVOK");
}

void doSetPwd(Client client)
{
  pwdst = ReadString();
  if(pwdst.length() <=10){
  for(int i = 0;i < pwdst.length(); i++){
    EEPROM.write(i,pwdst.charAt(i));
  }
  SendString(client,setpwd,"SPOK");
  }else {
   SendString(client,setpwd,"Err:LenOut"); 
  }
}

void SendToSerial(Client client)
{
  if(SerialOK){
  bytesLen = ReadInt();
  for(int i=0;i<bytesLen;i++)
  {
    Serial.print(EEPROM.read(Current),BYTE);
    Current+=1;
  } 
  SendString(client,sendSerial,"TLLOK");
  }else{
  SendString(client,sendSerial,"Err:IsClose");    
  }
}

int main(void)
{
	init();

	setup();
    
	for (;;)
		loop();
        
	return 0;
}


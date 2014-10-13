int current=511;

void tcpWork(){
  Client client = tcp.available();
  tackMsg(client);
}

void tackMsg(Client client){
  while(client.available()) {
     byte rrc = client.read();
     if(rrc != 13){
     EEPROM.write(current,rrc);
     current++; 
     }else{
      byte rrc1=client.read();
      if(rrc1 == 10){
       if(checkPt()){
         controlOrder(client);
         current=511;
       }else{
       current=511;
       SendString(client,0,"Err:pt");
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
 int ml=bToInt(EEPROM.read(511),EEPROM.read(512),EEPROM.read(513),EEPROM.read(514));
 //sub end mark -2 "\r\n"
 if(ml-2==current-511){
  return true;
 }else{
  return false; 
 }
}

//boolean checkDnsOrder(Client client)
//{
//  getClientIP(client);
//  if(DnsOK && dnsIP.equals(clientIP))
//  {return true;}else{return false;}
//}

void controlOrder(Client client){
   initReader();
   int opera= ReadInt();
   if(opera==0){//reset
    if(ReadString().equals(DYid)){
      resetPwd("123456");
      SendString(client,0,"RSOK"); 
    }else{
    SendString(client,0,"Err:DYid");
    client.stop();}
   }else if(opera==1){//setpwd
    if(checkPwd()){
      String pwdst = ReadString();
      if(pwdst.length() <=10){
       for(int i = 0;i < pwdst.length(); i++){
         EEPROM.write(i,pwdst.charAt(i));
       }
       SendString(client,1,"SPOK");
      }else {
       SendString(client,1,"Err:LenOut"); 
      }
    }else{ disCon(client); }   
   }else if(opera == 2){//runSerial
    if(checkPwd()){
     Serial.begin(ReadInt());
     SerialOK=true;
     SendString(client,2,"TLLSOK");
    }else{ disCon(client);}
   }else if(opera == 3){//sendSerial
    if(checkPwd()){
     SendToSerial(client);
    }else{disCon(client);} 
   }else if(opera == 4){//SetPinMode
    if(checkPwd()){
     pinMode(ReadByte(),ReadByte()==0?INPUT:OUTPUT);
     SendString(client,4,"SPMOK");
    } else {disCon(client);}
   }else if(opera == 5){//SetDigital
    if(checkPwd()){
     digitalWrite(ReadByte(),ReadByte()==0?LOW:HIGH);
     SendString(client,5,"SDTOK");
    }else{ disCon(client);} 
   }else if(opera == 6){//SetPWM
    if(checkPwd()){
     analogWrite(ReadByte(),ReadByte());
     SendString(client,6,"SPwmOK");
    }else{disCon(client);} 
   }else if(opera == 7){//SetServo
     if(checkPwd()){
      byte spin =ReadByte();
      byte pot =ReadByte();
      if(pot >=0 && pot <=180){
       servoToPot(spin,pot); 
      }
      SendString(client,7,"SVOK");      
    }else{disCon(client);} 
   }else if(opera == 8){//SetSteper
    if(checkPwd()){
       int steps=ReadInt();
       int pin1=ReadByte();
       int pin2=ReadByte();
       int pin3=ReadByte();
       int pin4=ReadByte();
       int Speed=ReadInt();
       int doStep=ReadInt();
       stepperGo(steps,pin1,pin2,pin3,pin4,Speed,doStep);
       SendString(client,8,"STOK");
    }else{disCon(client);} 
   }else if(opera == 9){//Get18B20
    if(checkPwd()){
     DallasTemperature tempSensor; 
     tempSensor.begin(ReadByte());
     SendFloat(client,9,tempSensor.getTemperature());
    }else{disCon(client);} 
   }else if(opera == 10){//GetDigital
    if(checkPwd()){
     SendInt(client,10,digitalRead(ReadByte()));
    }else{disCon(client);} 
   }else if(opera == 11){//GetAnalog
    if(checkPwd()){
     SendInt(client,11,analogRead(ReadByte()));
    } else {disCon(client);}
   }else if(opera == 12){//GetAllDigitalAndAnalog
    if(checkPwd()){
     doGetAllDigitalAndAnalog(client);
    }else{disCon(client);} 
   }else if(opera == 13){//runIRReader
    if(checkPwd()){
     runRrevcIR(client,ReadByte());
    } else {disCon(client);}
   }else if(opera == 15){//SendIRRemote
    if(checkPwd()){
     doSendIRRemote(client);
    }else{disCon(client);} 
   
 
   }else{
     client.flush();
     client.stop();
   }
}

void disCon(Client client){
  SendString(client,0,"Err:PWD");
  client.stop();
}

void doSendIRRemote(Client client)
{
  int rlen= ReadInt();
  unsigned int irData[rlen];
  for(int i=0;i<rlen;i++){
   irData[i]=ReadInt();
  }
  //ir发送pin3
  IRsend irsender;
  irsender.sendRaw(irData,rlen,38);
  SendString(client,15,"IRSOK");//SendIRRemote
}

void doGetAllDigitalAndAnalog(Client client)
{
  int dMax=ReadInt();
  int aMax=ReadInt();
  String ios="{\"analogs\":[";
  for(int i=0;i<aMax;i++){
   ios += analogRead(i);
   if(i<aMax-1){ios +=',';}else {ios +=']';}
  }
  ios +=",\"digitals\":[";
  for(int i=0;i<dMax;i++){
    ios += digitalRead(i);
    if(i<dMax-1){ios +=',';}else {ios +=']';}
  }
  ios +='}';
  SendString(client,12,ios);
}

void SendToSerial(Client client)
{
  if(SerialOK){
  //send the MDYid to client
  Serial.print(DYid);
  //send order to client
  int bytesLen = ReadInt();
  for(int i=0;i<bytesLen;i++)
  {
    Serial.print(EEPROM.read(Current),BYTE);
    Current+=1;
  } 
  SendString(client,3,"TLLOK");
  }else{
  SendString(client,3,"Err:IsClose");    
  }
}

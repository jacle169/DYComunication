boolean checkPt(int beRun){
 int ml=bToInt(EEPROM.read(511+beRun),EEPROM.read(512+beRun),EEPROM.read(513+beRun),EEPROM.read(514+beRun));
 //sub end mark -2 "\:\|"
 if(ml-2==current-(511+beRun)){
  return true;
 }else{
  return false; 
 }
}

void controlOrder(){
   initReader(9);
   if(ReadString().equals(DYid)){
    int opera= ReadInt();
    if(opera==reset){
      resetPwd("123456");
      SendString("RSOK");
   }else if(opera==setpwd){
    if(checkPwd()){
      doSetPwd();
    }else{ disCon(); }   
   }else if(opera == SetPinMode){
    if(checkPwd()){
     pinMode(ReadByte(),ReadByte()==0?INPUT:OUTPUT);
     SendString("SPMOK");
    } else {disCon();}
   }else if(opera == SetDigital){
    if(checkPwd()){
     digitalWrite(ReadByte(),ReadByte()==0?LOW:HIGH);
     SendString("SDTOK");
    }else{ disCon();} 
   }else if(opera == SetPWM){
    if(checkPwd()){
     analogWrite(ReadByte(),ReadByte());
     SendString("SPwmOK");
    }else{disCon();} 
   }else if(opera == SetServo){
     if(checkPwd()){
      byte spin =ReadByte();
      byte pot =ReadByte();
      if(pot >=0 && pot <=180){
       servoToPot(spin,pot); 
      }
      SendString("SVOK");      
    }else{disCon();} 
   }else if(opera == SetSteper){
    if(checkPwd()){
     doSteper();
    }else{disCon();} 
   }else if(opera == Get18B20){
    if(checkPwd()){
     SendFloat(get18b20(ReadByte()));
    }else{disCon();} 
   }else if(opera == GetDigital){
    if(checkPwd()){
     SendInt(digitalRead(ReadByte()));
    }else{disCon();} 
   }else if(opera == GetAnalog){
    if(checkPwd()){
     SendInt(analogRead(ReadByte()));
    } else {disCon();}
   }else if(opera == GetAllDigitalAndAnalog){
    if(checkPwd()){
     doGetAllDigitalAndAnalog();
    }else{disCon();}   
   }else if(opera == SendIRRemote){
    if(checkPwd()){
     doSendIRRemote();
    }else{disCon();} 
   }else if(opera == dht11){
    if(checkPwd()){
     doDht11();
    }else{disCon();} 
   }
   
  }else{
   SendString("hid err");
  }
}

void disCon(){
SendString("Err:PWD");
}

void doDht11()
{
        byte dht11_dat[5];
	byte dht11_in;
	byte i;
	// start condition
	// 1. pull-down i/o pin from 18ms
	PORTC &= ~_BV(DHT11_PIN);
	delay(18);
	PORTC |= _BV(DHT11_PIN);
	delayMicroseconds(40);
	
	DDRC &= ~_BV(DHT11_PIN);
	delayMicroseconds(40);
	
	dht11_in = PINC & _BV(DHT11_PIN);
	
	if(dht11_in){
		SendString("1notmet");
		return;
	}
	delayMicroseconds(80);
	
	dht11_in = PINC & _BV(DHT11_PIN);
	
	if(!dht11_in){
		SendString("2notmet");
		return;
	}
	delayMicroseconds(80);
	// now ready for data reception
	for (i=0; i<5; i++)
		dht11_dat[i] = read_dht11_dat();
		
	DDRC |= _BV(DHT11_PIN);
	PORTC |= _BV(DHT11_PIN);
	
        byte dht11_check_sum = dht11_dat[0]+dht11_dat[1]+dht11_dat[2]+dht11_dat[3];
	// check check_sum
	if(dht11_dat[4]!= dht11_check_sum)
	{
		SendString("DHT11checksumerror");
	}
	
  SendMDYid();
  Serial.print(DYid+',');
	Serial.print("Current humdity = ");
	Serial.print(dht11_dat[0], DEC);
	Serial.print(".");
	Serial.print(dht11_dat[1], DEC);
	Serial.print("%  ");
	Serial.print("temperature = ");
	Serial.print(dht11_dat[2], DEC);
	Serial.print(".");
	Serial.print(dht11_dat[3], DEC);
	Serial.print("C  ");
 Serial.print('\r');
}

byte read_dht11_dat()
{
byte i = 0;
byte result=0;
for(i=0; i< 8; i++){	
while(!(PINC & _BV(DHT11_PIN)));  // wait for 50us
delayMicroseconds(30);	
if(PINC & _BV(DHT11_PIN)) 
result |=(1<<(7-i));
while((PINC & _BV(DHT11_PIN)));  // wait '1' finish        
}
return result;
}

void doSetPwd()
{
  String pwdst = ReadString();
  if(pwdst.length() <=10){
  for(int i = 0;i < pwdst.length(); i++){
    EEPROM.write(i,pwdst.charAt(i));
  }
   SendString("SPOK");
  }else {
   SendString("Err:LenOut"); 
  }
}

void doGetAllDigitalAndAnalog()
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
  SendString(ios);
}

void doSendIRRemote()
{
  //328 ir发送pin3| mega1280 pin9
  IRsend irsender;
  int rlen= ReadInt();
  unsigned int irData[rlen];
  for(int i=0;i<rlen;i++){
   irData[i]=ReadInt();
  }
  irsender.sendRaw(irData,rlen,38);
  SendString("IRSOK");
}

void doSteper()
{
  int steps=ReadInt();
  byte pin1=ReadByte();
  byte pin2=ReadByte();
  byte pin3=ReadByte();
  byte pin4=ReadByte();
  int Speed=ReadInt();
  int doStep=ReadInt();
  stepperGo(steps,pin1,pin2,pin3,pin4,Speed,doStep);
  SendString("STOK");
}

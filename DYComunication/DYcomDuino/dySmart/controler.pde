void runRrevcIR(Client client,byte pin)
{
 IRrecv irrecv(pin);
 decode_results results;
 irrecv.enableIRIn(); 
 SendString(client,13,"IROnRead");//runIRReader
 while(true){
 if (irrecv.decode(&results)) {
  dump(&results,client);
  break; 
  }
 }
}

void dump(decode_results *results,Client client) {
  int count = results->rawlen;
  AddLen(client,8+((count-1)*4));
  sInt(client,14);//IRReadOnData
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
  int pulsewidth=(myangle*11)+500;
  digitalWrite(servopin,HIGH);
  delayMicroseconds(pulsewidth);
  digitalWrite(servopin,LOW);
  delay(20-pulsewidth/1000);
  }
}

//步进
void stepperGo(int Steps,byte pin1,byte pin2,byte pin3,byte pin4,int Speed,int doStep){
Stepper stepper(Steps,pin1,pin2,pin3,pin4);
stepper.setSpeed(Speed);
stepper.step(doStep); 
}

//float get18b20(byte pin){
//DallasTemperature tempSensor; 
//tempSensor.begin(pin);
//return tempSensor.getTemperature();
//}

int pulsewidth;
DallasTemperature tempSensor; 

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

//步进
void stepperGo(int Steps,byte pin1,byte pin2,byte pin3,byte pin4,int Speed,int doStep){
Stepper stepper(Steps,pin1,pin2,pin3,pin4);
stepper.setSpeed(Speed);
stepper.step(doStep); 
}

float get18b20(byte pin){
tempSensor.begin(pin);
return tempSensor.getTemperature();
}

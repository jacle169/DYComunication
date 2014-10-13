int ir_pin = 10;				 //Sensor pin 1 wired through a 220 ohm resistor
int led_pin = 13;				   //"Ready to Receive" flag, not needed but nice
int debug = 1;				   //Serial connection must be started to debug
int start_bit = 2400;			 //Start bit threshold (Microseconds)
int bin_1 = 1200;				   //Binary 1 threshold (Microseconds)
int bin_0 = 600;				     //Binary 0 threshold (Microseconds)
int dataOut = 0;
int guardTime = 300;


void setup() {
  pinMode(led_pin, OUTPUT);		 //This shows when we're ready to recieve
  pinMode(ir_pin, OUTPUT);
  digitalWrite(led_pin, LOW);	 //not ready yet
  digitalWrite(ir_pin, LOW);	  //not ready yet
  Serial.begin(9600);
}

void loop() {
  if (Serial.available()) {
    int val = Serial.read();
    dataOut = val;
    int key = 0;
    for (int j = 0; j<10; j++) {
	key = sendIRKey(dataOut);		 //Fetch the key
    }
	Serial.print("Key Sent: ");
	Serial.println(key);
  }
}


int sendIRKey(int dataOut) {
  int data[12];
  digitalWrite(led_pin, HIGH);     //Ok, i'm ready to send
  for (int i=0; i<12; i++) {
    data[i] = dataOut>>i & B1;   //encode data as '1' or '0'
    }
  // send startbit
  oscillationWrite(ir_pin, start_bit);
  // send separation bit
  digitalWrite(ir_pin, HIGH);
  delayMicroseconds(guardTime);
  // send the whole string of data
  for (int i=11; i>=0; i--) {
    if (data[i] == 0) oscillationWrite(ir_pin, bin_0);
    else oscillationWrite(ir_pin, bin_1);
    // send separation bit
    digitalWrite(ir_pin, HIGH);
    delayMicroseconds(guardTime);
  }
  delay(20);
  return dataOut;				    //Return key number
}

// this will write an oscillation at 38KHz for a certain time in useconds
void oscillationWrite(int pin, int time) {
  for(int i = 0; i <= time/26; i++) {
    digitalWrite(pin, HIGH);
    delayMicroseconds(13);
    digitalWrite(pin, LOW);
    delayMicroseconds(13);
  }
}

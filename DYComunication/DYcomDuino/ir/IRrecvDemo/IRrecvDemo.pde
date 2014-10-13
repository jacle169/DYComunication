int ir_pin = 11;				 //Sensor pin 1 wired through a 220 ohm resistor
int led_pin = 13;				   //"Ready to Receive" flag, not needed but nice
int debug = 1;				   //Serial connection must be started to debug
int start_bit = 2000;			 //Start bit threshold (Microseconds)
int bin_1 = 1000;				   //Binary 1 threshold (Microseconds)
int bin_0 = 400;				     //Binary 0 threshold (Microseconds)


void setup() {
 pinMode(led_pin, OUTPUT);		 //This shows when we're ready to receive
 pinMode(ir_pin, INPUT);
 digitalWrite(led_pin, LOW);	 //not ready yet
 Serial.begin(9600);
}

void loop() {
 int key = getIRKey();		 //Fetch the key
 if (key != -1) {
   Serial.print("Key Recieved: ");
   Serial.println(key);
 }
}


int getIRKey() {
 int data[12];
 digitalWrite(led_pin, HIGH);     //Ok, i'm ready to recieve
 while(pulseIn(ir_pin, LOW) < 2200) { //Wait for a start bit
 }
 data[0] = pulseIn(ir_pin, LOW);	 //Start measuring bits, I only want low pulses
 data[1] = pulseIn(ir_pin, LOW);
 data[2] = pulseIn(ir_pin, LOW);
 data[3] = pulseIn(ir_pin, LOW);
 data[4] = pulseIn(ir_pin, LOW);
 data[5] = pulseIn(ir_pin, LOW);
 data[6] = pulseIn(ir_pin, LOW);
 data[7] = pulseIn(ir_pin, LOW);
 data[8] = pulseIn(ir_pin, LOW);
 data[9] = pulseIn(ir_pin, LOW);
 data[10] = pulseIn(ir_pin, LOW);
 data[11] = pulseIn(ir_pin, LOW);
 digitalWrite(led_pin, LOW);

 if(debug == 1) {
   Serial.println("-----");
 }
 for(int i=0;i<=11;i++) {		     //Parse them
   if (debug == 1) {
	   Serial.println(data[i]);
   }
   if(data[i] > bin_1) {		     //is it a 1?
	 data[i] = 1;
   }  else {
	 if(data[i] > bin_0) {	     //is it a 0?
	   data[i] = 0;
	 } else {
	  data[i] = 2;			   //Flag the data as invalid; I don't know what it is!
	 }
   }
 }

 for(int i=0;i<=11;i++) {		     //Pre-check data for errors
   if(data[i] > 1) {
	 return -1;				   //Return -1 on invalid data
   }
 }

 int result = 0;
 int seed = 1;
 for(int i=11;i>=0;i--) {		    //Convert bits to integer
   if(data[i] == 1) {
	 result += seed;
   }
   seed = seed * 2;
 }

 return result;				     //Return key number
}

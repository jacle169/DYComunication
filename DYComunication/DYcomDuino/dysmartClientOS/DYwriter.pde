void SendInt(int data)
{
  SendMDYid();
  Serial.print(DYid+',');
  Serial.print(data,DEC);
  Serial.print('\r');
}

void SendFloat(float data)
{
  SendMDYid();
  Serial.print(DYid+',');
  Serial.print(data,2);
  Serial.print('\r');
}

void SendString(String data)
{
  SendMDYid();
  Serial.print(DYid+',');
  Serial.print(data);
  Serial.print('\r');
}

void SendMDYid()
{
  Serial.print(MDYid);
}

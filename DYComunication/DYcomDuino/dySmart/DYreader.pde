int Current=0;
//int bytesLen=0;
//byte bytes[1];

void initReader()
{
  Current=511+4;
}

int ReadInt()
{
  int i = bToInt(EEPROM.read(Current),EEPROM.read(Current+1),EEPROM.read(Current+2),EEPROM.read(Current+3));
  Current+=4;
  return i;
}

String ReadString()
{
  String m="";
  int len = ReadInt();
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
  byte rb=EEPROM.read(Current);
  Current+=1;
  return rb;
}



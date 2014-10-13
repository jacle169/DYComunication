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
   int msgLen=dt.length()+8;
   AddLen(client,msgLen);
   sInt(client,operation);
   sInt(client,dt.length());
   for(int i=0;i<dt.length();i++){
     client.write(dt.charAt(i)); 
   }
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
  byte* b=FloatTob(ft);
  client.write(b,4);
}

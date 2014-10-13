void getClientIP(Client client)
{
  byte stip[4];
  client.getRemoteIP(stip);
  clientIP = ipToStr(stip);
}

String ipToStr(byte* ip)
{
  String ipstr="";
  for(int i=0;i<4;i++){
   ipstr += (int)ip[i];
   if(i<3){
   ipstr +='.';
   }
  }
  return ipstr;
}

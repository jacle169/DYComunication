byte mac[6];

void CheckConnect()
{
 mac[0]=random(1, 254);
 mac[1]=random(1, 254);
 mac[2]=random(1, 254);
 mac[3]=random(1, 254);
 mac[4]=random(1, 254);
 mac[5]=random(1, 254);  
  byte ip[4];
  byte dyDnsIP[4];
  if(Dhcp.beginWithDHCP(mac) == 1)
  {  
    Dhcp.getLocalIp(ip);
    Dhcp.getDnsServerIp(dyDnsIP);
    dyDns.init("dns.dy2com.com", dyDnsIP);  
    dyDns.resolve();
    int results;
    while(!(results=dyDns.finished())) ;  
    if(results ==1){
      DnsOK=true;
      dyDns.getIP(dyDnsIP);
      dnsIP = ipToStr(dyDnsIP);
      postId(dyDnsIP); 
    }
  ip[3]=254;
  Ethernet.begin(mac, ip);
  tcp.begin();
  httpServer.begin();
  dhcpReady=true;
    //Serial.println("ok");
  }
}

void postId(byte dnsip[4])
{
 Client client(dnsip, 80);
 if (client.connect()) {
  client.println("GET /default.aspx?id=" + DYid + " HTTP/1.0");
  client.println();
 }
 client.stop();
}



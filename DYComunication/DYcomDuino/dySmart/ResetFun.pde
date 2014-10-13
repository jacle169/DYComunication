void resetPwd(String dt)
{
  for(int i = 0;i < dt.length(); i++){
    EEPROM.write(i,dt.charAt(i));
  }
}

boolean checkPwd()
{
  String rrpwd = ReadString();
  if(rrpwd.equals(readPwd(rrpwd.length()))){
    return true;
  }else { 
  return false;}
}

String readPwd(int pwdLen)
{
  String rpwd="";
  for(int i = 0;i < pwdLen; i++){
    rpwd += EEPROM.read(i);
  }
  return rpwd;
}

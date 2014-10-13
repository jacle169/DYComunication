union u_tag {
    byte b[4];
    float val;
} f; 

byte* FloatTob(float ft)
{
 byte bs[4];
 f.val=ft;
 bs[0]=f.b[0];
 bs[1]=f.b[1];
 bs[2]=f.b[2];
 bs[3]=f.b[3];
 return bs;
}

float bToFloat(byte b0,byte b1,byte b2,byte b3){
float fval;
f.b[0] = b0;
f.b[1] = b1;
f.b[2] = b2;
f.b[3] = b3;
fval=f.val;
return fval;
}

byte* IntTob(unsigned long val) {
  byte bi[4];
  bi[3] = (byte )((val >> 24) & 0xff);
  bi[2] = (byte )((val >> 16) & 0xff);
  bi[1] = (byte )((val >> 8) & 0xff);
  bi[0] = (byte )(val & 0xff);
  return bi;
}

unsigned long bToInt(byte b0,byte b1,byte b2,byte b3) {
  unsigned long Intval;
  Intval = b3 << 24;
  Intval |= b2 << 16;
  Intval |= b1 << 8;
  Intval |= b0;
  return Intval;
}

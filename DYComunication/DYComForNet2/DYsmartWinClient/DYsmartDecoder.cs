
namespace DYsmartWinClient
{
    public class DYsmartDecoder
    {
        DYReader read;
        public void initData(byte[] data)
        {
            read = new DYReader(data);
        }

        public operations? getOperation()
        {
            int type;
            if (!read.ReadInt32(out type))
            {
                return null;
            }
            return (operations)type;
        }

        public object decode(operations opera)
        {
            if (opera == operations.BeginSerial || opera == operations.ResetAll || opera == operations.SetPwd || opera == operations.BeginSerial || opera == operations.SerialSend
                || opera == operations.PinMode || opera == operations.SetDigital || opera == operations.SetPWM || opera == operations.SetServo
                || opera == operations.SetSteper || opera == operations.runIRReader || opera == operations.SendIRRemote || opera == operations.SerialOnData || opera == operations.GetAllDigitalAndAnalog)
            {
                string message;
                if (read.ReadString(out message, System.Text.Encoding.UTF8))
                {
                    return message;
                }
            }
            else if (opera == operations.Get18B20)
            {
                float temp;
                if (read.ReadFloat(out temp))
                {
                    return temp;
                }
            }
            else if (opera == operations.GetDigital || opera == operations.GetAnalog)
            {
                int state;
                if (read.ReadInt32(out state))
                {
                    return state;
                }
            }
            else if (opera == operations.IRReadOnData)
            {
                int len;
                int[] irData;
                if (read.ReadInt32(out len))
                {
                    irData = new int[len];
                    for (int i = 0; i < len; i++)
                    {
                        read.ReadInt32(out irData[i]);
                    }
                    string irdata = string.Empty;
                    for (int i = 0; i < irData.Length; i++)
                    {
                        irdata += irData[i].ToString();
                        if (i < irData.Length - 1)
                        {
                            irdata += ",";
                        }
                    }
                    return irdata;
                }
            }
            return null;
        }

    }
}

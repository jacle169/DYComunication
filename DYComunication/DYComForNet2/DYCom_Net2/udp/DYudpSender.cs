using System.Net.Sockets;
using System.Net;

namespace DYCom_Net2
{
    public class DYudpSender
    {
        UdpClient uc;
        string serverIP;
        int serverPort;
        public DYudpSender(string ServerIP, int Port)
        {
            serverIP = ServerIP;
            serverPort = Port;
        }

        public void SendUpd(byte[] data)
        {
            if (uc == null)
            {
                uc = new UdpClient(0);
                IPEndPoint ipe;
                if (checkIPAddress(serverIP))
                {
                    ipe = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
                }
                else
                {
                    IPHostEntry ip = Dns.GetHostEntry(serverIP);
                    ipe = new IPEndPoint(ip.AddressList[0], serverPort);
                }
                uc.Connect(ipe);
            }
            byte[] buff = DYWriter.Merge(DYWriter.GetDYBytes(data.Length + 4), data);
            uc.Send(buff, buff.Length);
        }

        public void SendUpd(byte[] data, string ServerIP, int Port)
        {
            UdpClient suc = new UdpClient(0);
            IPEndPoint ipe;
            if (checkIPAddress(ServerIP))
            {
                ipe = new IPEndPoint(IPAddress.Parse(ServerIP), Port);
            }
            else
            {
                IPHostEntry ip = Dns.GetHostEntry(ServerIP);
                ipe = new IPEndPoint(ip.AddressList[0], Port);
            }
            suc.Connect(ipe);
            byte[] buff = DYWriter.Merge(DYWriter.GetDYBytes(data.Length + 4), data);
            suc.Send(buff, buff.Length);
            suc.Close();
        }

        bool checkIPAddress(string ipAdd)
        {
            try
            {
                var ip = IPAddress.Parse(ipAdd);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

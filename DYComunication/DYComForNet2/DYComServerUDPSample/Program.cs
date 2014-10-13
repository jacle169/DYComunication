using System;
using System.Collections.Generic;
using System.Text;

namespace DYComServerUDPSample
{
    class Program
    {
        static void Main(string[] args)
        {
            DYComServer server = new DYComServer();
            Console.WriteLine("DYCOM 服务 运行中!");
            Console.Read();
        }
    }
}

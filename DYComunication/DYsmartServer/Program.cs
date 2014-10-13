using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DYsmartServer
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

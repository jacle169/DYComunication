using System;

namespace DYComServerMonoSample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
		    DYComServer server = new DYComServer();
            Console.WriteLine("DYCOM 服务 运行中!");
            Console.Read();
		}
	}
}


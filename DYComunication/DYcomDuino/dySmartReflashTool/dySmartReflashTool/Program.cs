using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace dySmartReflashTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DySmart刷硬件系统工具");
            Console.WriteLine("Power by WWW.DySmart.NET... 黎东海");
            Console.WriteLine("Press enter to start reflash dysmart system!");
            Process avrprog = new Process();
            StreamReader avrstdout, avrstderr;
            StreamWriter avrstdin;
            ProcessStartInfo psI = new ProcessStartInfo("cmd");
            psI.UseShellExecute = false;
            psI.RedirectStandardInput = true;
            psI.RedirectStandardOutput = true;
            psI.RedirectStandardError = true;
            psI.CreateNoWindow = true;
            avrprog.StartInfo = psI;
            avrprog.Start();
            avrstdin = avrprog.StandardInput;
            avrstdout = avrprog.StandardOutput;
            avrstderr = avrprog.StandardError;
            avrstdin.AutoFlush = true;
            // 328p and uno
            avrstdin.WriteLine("avrdude.exe -Cavr/avrdude.conf -F -v -p m328p -c stk500v1 -P COM3 -b115200 -D -Uflash:w:CoolpyII.cpp.hex:i");
            //avrstdin.WriteLine("avrdude.exe -Cavr/avrdude.conf -patmega328p -cstk500v1 -P COM3 -b57600 -D -Uflash:w:CoolpyII.cpp.hex:i");
            //avrstdin.WriteLine("avrdude.exe -Cavr/avrdude.conf -pm1280 -cstk500v1 -P COM3 -b57600 -D -Uflash:w:dysmartMega.cpp.hex:i");
            avrstdin.Close();
            Console.WriteLine(avrstdout.ReadToEnd());
            Console.WriteLine(avrstderr.ReadToEnd());
            Console.Read();
        }
    }
}

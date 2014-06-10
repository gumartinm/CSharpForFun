using System;
using System.Text;

namespace ProcessLauncher
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            LinuxWindowsProcess process = new LinuxWindowsProcess();
            //Console.WriteLine("Linux");
            //var linuxOutput = process.Test("ls", "-lah");
            //Console.WriteLine("stdout: {0}", linuxOutput.ToString());

            // I need to change the console font :/ So even using UTF8 encoding this is not going to work. :(
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Windows");
            // UTF8 is 65001 code page. :O
            // see: http://ss64.com/nt/cmd.html
            var windowsOutput = process.Test("cmd.exe", "/C chcp 65001 && dir /A");
            Console.WriteLine("stdout: {0}", windowsOutput.ToString());
            Console.ReadLine();
        }
    }
}

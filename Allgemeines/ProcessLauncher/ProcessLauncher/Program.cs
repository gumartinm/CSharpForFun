using System;

namespace ProcessLauncher
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            LinuxWindowsProcess process = new LinuxWindowsProcess();
            Console.WriteLine("Linux");
            var linuxOutput = process.Test("ls", "-lah");
            Console.WriteLine("stdout: {0}", linuxOutput.ToString());

            Console.WriteLine("Windows");
            var windowsOutput = process.Test("cmd.exe", "/C dir /A");
            Console.WriteLine("stdout: {0}", windowsOutput.ToString());

            Console.ReadLine();
        }
    }
}

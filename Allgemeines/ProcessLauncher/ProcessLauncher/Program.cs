using System;

namespace ProcessLauncher
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            LinuxWindowsProcess process = new LinuxWindowsProcess();
            Console.WriteLine("Linux");
            var output = process.Test("ls", "-lah");
            Console.WriteLine("stdout: {0}", output.ToString());

            // Console.WriteLine("Windows");
            // output = process.Test("dir", "/a");
            // Console.WriteLine("stdout: {0}", output.ToString());
        }
    }
}

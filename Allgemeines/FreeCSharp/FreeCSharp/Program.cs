using System;
using System.Text.RegularExpressions;
using System.IO;
using Mono.Unix.Native;
using System.Diagnostics;

namespace FreeCSharp
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine ();
            Console.WriteLine ("USING free C#, just UNIX");
            FreeCSharp free = new FreeCSharp();
            free.GetValues();

            long buffersPlusCached = free.Buffers + free.Cached;
            long mainUsed = free.MemTotal - free.MemFree;

            // What you would get from free command:
            Console.WriteLine("-/+ buffers/cache:    {0}     {1}",
                (mainUsed - buffersPlusCached), (free.MemFree + buffersPlusCached));

            // What means:
            Console.WriteLine("Used physical memory: {0} kB", mainUsed - buffersPlusCached);
            Console.WriteLine("Available physical memory: {0} kB", free.MemFree + buffersPlusCached);

            Console.WriteLine ();
            Console.WriteLine ("USING SYSCALL, just UNIX");
            OperatingSystem os = Environment.OSVersion;
            PlatformID     pid = os.Platform;
            if (pid == PlatformID.Unix || pid == PlatformID.MacOSX) {
                long pages = Syscall.sysconf (SysconfName._SC_AVPHYS_PAGES);
                long page_size = Syscall.sysconf (SysconfName._SC_PAGESIZE);
                Console.WriteLine("The number of currently available pages of physical memory: {0}, " +
                    "Size of a page in bytes: {1} bytes", pages, page_size);
                Console.WriteLine("Mem: {0} bytes", pages * page_size);
            }

            if (pid == PlatformID.Unix || pid == PlatformID.MacOSX) {
                long pages = Syscall.sysconf (SysconfName._SC_PHYS_PAGES);
                long page_size = Syscall.sysconf (SysconfName._SC_PAGESIZE);
                Console.WriteLine("The number of pages of physical memory: {0}, " +
                    "Size of a page in bytes: {1} bytes", pages, page_size);
                Console.WriteLine("Mem: {0} bytes", pages * page_size);
            }
                

            Console.WriteLine ();
            Console.WriteLine ("USING PerformanceCounter, platform independent");
            string categoryName = "Mono Memory";
            string counterName = "Total Physical Memory";

            try {
                var pc = new PerformanceCounter (categoryName, counterName);
                Console.WriteLine ("Value of performance counter '{0}/{1}': {2}",
                    categoryName, counterName, pc.RawValue);
            } catch (InvalidOperationException ex) {
                Console.WriteLine ("Category name '{0}' does not exist. {1}",
                    categoryName, ex.Message);
            }
        }
    }

    /// <summary>
    /// FreeCSharp: quick implementation of free command (kind of) using C#
    /// </summary>
    public class FreeCSharp
    {
        public long MemTotal { get; private set; }
        public long MemFree { get; private set; }
        public long Buffers { get; private set; }
        public long Cached { get; private set; }

        public void GetValues()
        {
            string[] memInfoLines = File.ReadAllLines(@"/proc/meminfo");

            MemInfoMatch[] memInfoMatches =
            {
                new MemInfoMatch(@"^Buffers:\s+(\d+)", value => Buffers = Convert.ToInt64(value)),
                new MemInfoMatch(@"^Cached:\s+(\d+)", value => Cached = Convert.ToInt64(value)),
                new MemInfoMatch(@"^MemFree:\s+(\d+)", value => MemFree = Convert.ToInt64(value)),
                new MemInfoMatch(@"^MemTotal:\s+(\d+)", value => MemTotal = Convert.ToInt64(value))
            };

            foreach (string memInfoLine in memInfoLines)
            {
                foreach (MemInfoMatch memInfoMatch in memInfoMatches)
                {
                    Match match = memInfoMatch.regex.Match(memInfoLine);
                    if (match.Groups[1].Success)
                    {
                        string value = match.Groups[1].Value;
                        memInfoMatch.updateValue(value);
                    }
                }
            }
        }

        public class MemInfoMatch
        {
            public Regex regex;
            public Action<string> updateValue;

            public MemInfoMatch(string pattern, Action<string> update)
            {
                this.regex = new Regex(pattern, RegexOptions.Compiled);
                this.updateValue = update;
            }

        }
    }
}

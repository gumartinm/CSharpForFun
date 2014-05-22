using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace Threads
{
    /// <summary>
    /// 
    /// Parallel Programming with Microsoft .NET
    /// 
    /// Taken from http://parallelpatterns.codeplex.com/
    /// 
    /// </summary>
    class MainClass
    {
        public static void Main(string[] args)
        {
            Chapter2.Test();
            Chapter3.Test();
            Chapter4.Test();
            Chapter5.Test();
        }
    }
}

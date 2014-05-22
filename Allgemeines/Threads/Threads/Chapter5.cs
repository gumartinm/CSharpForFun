using System;
using System.Threading.Tasks;

namespace Threads
{
    /// <summary>
    /// 
    /// Chapter 5.
    /// Futures
    /// 
    /// Taken from http://msdn.microsoft.com/en-us/library/ff963556.aspx
    /// 
    /// </summary>
    public class Chapter5
    {
        public static void Test()
        {
            Console.WriteLine("Futures");
            int a = 1;
            Task<int> futureB = Task.Factory.StartNew<int>(() => F1(a));
            var c = F2(a); 
            var d = F3(c);
            try {
                var f = F4(futureB.Result, d);
                Console.WriteLine("Result {0}", f);
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Exception in parallel Task F1: {0} {1}", e.Message, e.StackTrace);
            }


            Console.WriteLine("Continuation Tasks");
            futureB = Task.Factory.StartNew<int>(() => F1(a));
            var futureD = Task.Factory.StartNew<int>(() => F3(F2(a)));
            var futureF = Task.Factory.ContinueWhenAll<int, int>(
                new[] { futureB, futureD },
                (tasks) => F4(futureB.Result, futureD.Result));
            // I do not know what could I use this thing for... :/
            futureF.ContinueWith((t) =>
                Console.WriteLine("Continuation Tasks result {0}", t)
            );
            Console.WriteLine ("Continuation Tasks result {0}", futureF.Result);
        }

        public static int F1(int number)
        {
            return 1 + number;
        }

        public static int F2(int number)
        {
            return 2 + number;
        }

        public static int F3(int number)
        {
            return 3 + number;
        }

        public static int F4(int numberA, int numberB)
        {
            return numberA + numberB;
        }
    }
}


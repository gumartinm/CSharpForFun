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
            Chapter1.Test();
            Chapter2.Test();
            Chapter3.Test();
            Chapter4.Test();
            Chapter5.Test();
            Chapter6 chapter6 = new Chapter6();
            var tree = new Threads.Chapter6.Tree<int>();
            // WITH 10 LEVELS MONO STOPS WORKING WITH 806 THREADS!!!! (in my machine with 6GB RAM)
            // DUNNO WHAT MAKES THE MONO VIRTUAL MACHINE STOP... :(
            // Using Windows 8 with Visual Studio Professional 2013 it works as expected. :(
            // What is wrong with my Mono? (My Mono version was built my me)
            var levels = 10;
            chapter6.FillTree(levels, tree, () => 40);

            Console.WriteLine("Sequential Walk");
            chapter6.SequentialWalk(tree, (data, level) => Console.WriteLine("Level: {0} Data: {1}", level, data));

            Console.WriteLine("Parallel Walk");
            // In my case the parallel walk is slower than the sequential one because the tasks are short and
            // we waste more time creating new threads and synchronizing than running these simple tasks.
            // IT IS NOT THE CASE IN WINDOWS 8!!!!! Using Windows 8 the parallel version is faster than the sequential one.
            // What is wrong with my Mono? (My Mono version was built my me)
            chapter6.ParallelWalk(tree, (data, level) => Console.WriteLine("Level: {0} Data: {1}", level, data));
            Console.Read();
        }
    }
}

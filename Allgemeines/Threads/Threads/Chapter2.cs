using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace Threads
{
    /// <summary>
    /// 
    /// Chapter2.
    /// Parallel Loops
    /// 
    /// Taken from http://msdn.microsoft.com/en-us/library/ff963552.aspx
    /// 
    /// </summary>
    public class Chapter2
    {
        public static void Test()
        {
            int n = 10;

            Console.WriteLine("For");
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine("For {0}", i);
                Thread.Sleep(500);
            }


            Console.WriteLine("Parallel.For");
            Parallel.For(0, n, i =>
            {
                Console.WriteLine("Parallel.For {0}", i);
                Thread.Sleep(500);
            });


            Console.WriteLine("Parallel.ForEach");
            List<string> listOfStrings = new List<string>();
            listOfStrings.Add("one");
            listOfStrings.Add("two");
            listOfStrings.Add("three");
            listOfStrings.Add("four");
            Parallel.ForEach(listOfStrings, stringnumber =>
            {
                Console.WriteLine("Parallel.ForEach {0}", stringnumber);
                Thread.Sleep(500);
            });


            Console.WriteLine("LINQ");
            var anonymousFromListOfStrings = from stringnumber in listOfStrings select new { Original = stringnumber };
            // Use ForAll if you need to iterate over a PLINQ result. Don't use ForEach in this case. :(
            // I should have used: anonymousFromListOfStrings.ForAll  See example at the end of this class :)
            Parallel.ForEach(anonymousFromListOfStrings, stringnumber =>
            {
                Console.WriteLine("LINQ {0}", stringnumber);
                Thread.Sleep(500);
            });
            var stringsFromListOfString = listOfStrings.Select(stringnumber => new String(stringnumber.ToCharArray()));
            // Use ForAll if you need to iterate over a PLINQ result. Don't use ForEach in this case. :(
            // I should have used: stringsFromListOfString.ForAll  See example at the end of this class :)
            Parallel.ForEach(stringsFromListOfString, stringnumber =>
            {
                Console.WriteLine("LINQ {0}", stringnumber);
                Thread.Sleep(500);
            });


            Console.WriteLine("PLINQ");
            anonymousFromListOfStrings = from stringnumber in listOfStrings.AsParallel() select new { Original = stringnumber };
            Parallel.ForEach(anonymousFromListOfStrings, stringnumber =>
            {
                Console.WriteLine("PLINQ {0}", stringnumber);
                Thread.Sleep(500);
            });
            stringsFromListOfString = listOfStrings.AsParallel().Select(stringnumber => new String(stringnumber.ToCharArray()));
            Parallel.ForEach(stringsFromListOfString, stringnumber =>
            {
                Console.WriteLine("PLINQ {0}", stringnumber);
                Thread.Sleep(500);
            });


            Console.WriteLine("PLINQ, ForAll");
            listOfStrings.AsParallel().ForAll(stringnumber =>
            {
                Console.WriteLine("PLINQ, ForAll {0}", stringnumber);
                Thread.Sleep(500);
            });


            Console.WriteLine("Parallel Break");
            // During the processing of a call to the Break method, iterations with an index
            // value less than the current index will be allowed to start (if they have not already started),
            // but iterations with an index value greater than the current index will not be started.
            // This ensures that all iterations below the break point will complete.
            n = 20;
            var loopResult = Parallel.For(0, n, (i, loopState) =>
            {
                // What should I use? LowestBreakIteration or ShouldExitCurrentIteration? :(
                if (loopState.LowestBreakIteration.HasValue || loopState.ShouldExitCurrentIteration)
                {
                    Console.WriteLine("Parallel Break Check State {0}", i);
                    loopState.Break();
                    return;
                }

                Console.WriteLine("Parallel Break {0}", i);
                Thread.Sleep(500);

                if (i == 10)
                {
                    loopState.Break();
                    return;
                }
            });
            if (!loopResult.IsCompleted && loopResult.LowestBreakIteration.HasValue)
            {
                // You will never see this code becasue the loopResult IS ALWAYS COMPLETED :/
                Console.WriteLine("Parallel Break (don't see me), loop encountered a break at {0}", loopResult.LowestBreakIteration.Value);
            }
            // loopResult IS ALWAYS COMPLETED!!! Break must be like a shutdown in Java Executor?
            // The Tasks in the Queue will be executed but the Queue does not accept more tasks after shutdown.
            // In this case are the Tasks the lambda expression?
            // Depending on where you check the loopState in the lambda expression this could be a problem,
            // you end up thinking the parallel for is completed when it is not. :/
            Console.WriteLine("Parallel Break IsCompleted {0}", loopResult.IsCompleted);
            Console.WriteLine("Parallel Break LowestBreakIteration {0}", loopResult.LowestBreakIteration.HasValue);
            if (loopResult.LowestBreakIteration.HasValue)
            {
                Console.WriteLine("Parallel Break, loop encountered a break at {0}", loopResult.LowestBreakIteration.Value);
            }


            Console.WriteLine("Parallel Stop");
            loopResult = Parallel.For(0, n, (i, loopState) =>
            {
                // What should I use? LowestBreakIteration, ShouldExitCurrentIteration or IsStopped? :(
                if (loopState.LowestBreakIteration.HasValue || loopState.ShouldExitCurrentIteration || loopState.IsStopped)
                {
                    Console.WriteLine("Parallel Stop Check State {0}", i);
                    loopState.Stop();
                    return;
                }

                Console.WriteLine("Parallel Stop {0}", i);
                Thread.Sleep(500);

                if (i == 10)
                {
                    loopState.Stop();
                    return;
                }
            });
            // loopResult IS ALWAYS NOT COMPLETED!!! Stop must be like a shutdownNow in Java Executor?
            // The Tasks in the Queue will NOT be executed and the Queue does not accept more tasks after shutdownNow.
            // In this case are the Tasks the lambda expression?
            if (!loopResult.IsCompleted && !loopResult.LowestBreakIteration.HasValue)
            {
                // When the Stop method is called, the index value of the iteration that caused the stop isn't available. :(
                Console.WriteLine("Parallel Stop, loop was stopped");
            }
            Console.WriteLine("Parallel Stop IsCompleted {0}", loopResult.IsCompleted);
            Console.WriteLine("Parallel Stop LowestBreakIteration {0}", loopResult.LowestBreakIteration.HasValue);


            Console.WriteLine("CancellationToken");
            var cts = new CancellationTokenSource();
            cts.CancelAfter(600);
            CancellationToken token = cts.Token;
            var options = new ParallelOptions { CancellationToken = token };
            try
            {
                Parallel.For(0, n, options, (i) =>
                {
                    // optionally check to see if cancellation happened
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("CancellationToken Check State {0}", i);
                        // optionally exit this iteration early
                        return;
                    }

                    Console.WriteLine("CancellationToken {0}", i);
                    Thread.Sleep(500);
                });
            }
            catch (OperationCanceledException ex)
            {
                // I never see this message. Where is my exception? :(
                Console.WriteLine("CancellationToken Exception {0}", ex.StackTrace);
            }
            // If external cancellation has been signaled and your loop has called either the
            // Break or the Stop method of the ParallelLoopState object, a race occurs to see which
            // will be recognized first. The parallel loop will either throw an OperationCanceledException
            // or it will terminate using the mechanism for Break and Stop that is described in the section,
            // "Breaking Out of Loops Early," earlier in this chapter.


            // If the body of a parallel loop throws an unhandled exception, the parallel loop no longer begins
            // any new steps. By default, iterations that are executing at the time of the exception,
            // other than the iteration that threw the exception, will complete. After they finish, the parallel
            // loop will throw an exception in the context of the thread that invoked it. Long-running iterations
            // may want to test to see whether an exception is pending in another iteration. They can do this
            // with the ParallelLoopState class's IsExceptional property. This property returns true if an exception is pending.

            // Because more than one exception may occur during parallel execution, exceptions are grouped using
            // an exception type known as an aggregate exception. The AggregateException class has an
            // InnerExceptions property that contains a collection of all the exceptions that occurred during
            // the execution of the parallel loop. Because the loop runs in parallel, there may be more than one exception.

            // Exceptions take priority over external cancellations and terminations of a loop initiated by calling the
            // Break or Stop methods of the ParallelLoopState object.


            Console.WriteLine("Partitioner Default");
            Parallel.ForEach(Partitioner.Create(0, n), range =>
            {
                Console.WriteLine("Partitioner Default range {0} {1}", range.Item1, range.Item2);
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    // very small, equally sized blocks of work
                    Console.WriteLine("Partitioner Default {0}", i);
                }
            });
            // The number of ranges that will be created by a Partitioner object depends on the number of cores in your computer.
            // The default number of ranges is approximately three times the number of those cores.


            // If you know how big you want your ranges to be, you can use an overloaded version of the Partitioner.Create method
            // that allows you to specify the size of each range. Here's an example.
            Console.WriteLine("Partitioner Custom");
            Parallel.ForEach(Partitioner.Create(0, n, n/2), range =>
            {
                Console.WriteLine("Partitioner Custom range {0} {1}", range.Item1, range.Item2);
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    // small, equally sized blocks of work
                    Console.WriteLine("Partitioner Custom {0}", i);
                }
            });


            // You can limit the maximum number of tasks used concurrently by specifying the MaxDegreeOfParallelism property of
            // a ParallelOptions object. Here is an example
            Console.WriteLine("Parallel.For MaxDegreeOfParallelism 2");
            options = new ParallelOptions() { MaxDegreeOfParallelism = 2 };
            Parallel.For(0, n, options, i =>
            {
                Console.WriteLine("Parallel.For MaxDegreeOfParallelism {0}", i);
                Thread.Sleep(500);
            });


            Console.WriteLine("PLINQ, ForAll WithDegreeOfParallelism 2");
            listOfStrings.AsParallel().WithDegreeOfParallelism(2).ForAll(stringnumber =>
            {
                Console.WriteLine("PLINQ, ForAll WithDegreeOfParallelism {0}", stringnumber);
                Thread.Sleep(500);
            });


            // Here's an example that uses one of the overloads of the Parallel.ForEach method. The example uses a Partitioner
            // object to decompose the work into relatively large pieces, because the amount of work performed by each step is small,
            // and there are a large number of steps.
            Console.WriteLine("Task-Local State in a Loop Body");
            int numberOfSteps = 10000000;
            double[] results = new double[numberOfSteps];
            Parallel.ForEach(Partitioner.Create(0, numberOfSteps/2),
                             new ParallelOptions(),
                             () => { return new Random(); },
                             (range, loopState, random) =>
                             {
                                for (int i = range.Item1; i < range.Item2; i++)
                                results[i] = random.NextDouble();
                                return random;
                             },
                             _ => {}
            );


            // Console.WriteLine("Custom Task Scheduler for a Parallel Loop");
            // TaskScheduler myScheduler = How???? :(
            // options = new ParallelOptions() { TaskScheduler = myScheduler };
            // Parallel.For(0, n, options, i =>
            // {
            //    Console.WriteLine("Custom Task Scheduler for a Parallel Loop {0}", i);
            //    Thread.Sleep(500);
            // });


            // Use ForAll if you need to iterate over a PLINQ result. Don't use ForEach in this case.
            Console.WriteLine("PLINQ's ForAll the same as Parallel.ForEach");
            var plinqforall = from stringnumber in listOfStrings.AsParallel() select new String(stringnumber.ToCharArray());
            // This is what I should have used instead :)
            plinqforall.ForAll(stringnumber =>
            {
                Console.WriteLine("PLINQ's ForAll the same as Parallel.ForEach {0}", stringnumber);
                Thread.Sleep(500);
            });



            // Be careful if you use parallel loops with individual steps that take several seconds or more.
            // This can occur with I/O-bound workloads as well as lengthy calculations. If the loops take a long
            // time, you may experience an unbounded growth of worker threads due to a heuristic for preventing thread starvation
            // that's used by the .NET ThreadPool class's thread injection logic. This heuristic steadily increases the number
            // of worker threads when work items of the current pool run for long periods of time. The motivation is to add
            // more threads in cases where everything in the thread pool is blocked. Unfortunately, if work is actually proceeding,
            // more threads may not necessarily be what you want. The .NET Framework can't distinguish between these two situations.

            //If the individual steps of your loop take a long time, you may see more worker threads than you intend.
        }
    }
}


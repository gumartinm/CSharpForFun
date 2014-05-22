using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Threads
{
    /// <summary>
    /// 
    /// Chapter 4.
    /// Parallel Aggregation
    /// 
    /// Taken from http://msdn.microsoft.com/en-us/library/ff963547.aspx
    /// 
    /// </summary>
    public class Chapter4
    {
        public static void Test()
        {
            Console.WriteLine("Chapter 4");

            Console.WriteLine("Parallel Aggregation, the basics");
            double[] sequence = { 2.22, 3.22, 4.33, 5.33, 6.33, 7.33, 8.33, 9.33, 10.33 };
            double sum = 0.0d;
            for (int i = 0; i < sequence.Length; i++)
            {
                sum += Normalize(sequence[i]);
            }
            Console.WriteLine("Result {0}", sum);


            Console.WriteLine("Parallel Aggregation, the basics, LINQ");
            sum = (from x in sequence select Normalize(x)).Sum();
            Console.WriteLine("Result {0}", sum);


            Console.WriteLine("Parallel Aggregation, the basics, PLINQ");
            sum = (from x in sequence.AsParallel() select Normalize(x)).Sum();
            Console.WriteLine("Result {0}", sum);


            Console.WriteLine("Parallel Aggregation, the basics, PLINQ. Custom aggreate.");
            sum = (from x in sequence.AsParallel() select Normalize(x)).Aggregate(1.0d, (y1, y2) => y1 * y2);
            Console.WriteLine("Result {0}", sum);

            /**
             * If PLINQ doesn't meet your needs or if you prefer a less declarative style of coding, you can also use Parallel.For
             * or Parallel.ForEach to implement the parallel aggregation pattern. The Parallel.For and Parallel.ForEach methods
             * require more complex code than PLINQ. For example, the Parallel.ForEach method requires your code to include
             * synchronization primitives to implement parallel aggregation. For examples and more information, see
             * "Using Parallel Loops for Aggregation".
             */


            Console.WriteLine("Using Parallel Loops for Aggregation");
            object lockObject = new object();
            Parallel.ForEach(
                // The values to be aggregated 
                sequence,

                // The local initial partial result
                () => 0.0d,

                // The loop body
                (x, loopState, partialResult) =>
                {
                    return Normalize(x) + partialResult;
                },

                // The final step of each local context            
                (localPartialSum) =>
                {
                    // Enforce serial access to single, shared result (sum is global)
                    lock (lockObject)
                    {
                        sum += localPartialSum;
                    }
                });
            Console.WriteLine("Result {0}", sum);


            Console.WriteLine("Using a Range Partitioner for Aggregation");
            var rangePartitioner = Partitioner.Create(0, sequence.Length);
            Parallel.ForEach(
                // The input intervals
                rangePartitioner, 

                // The local initial partial result
                () => 0.0d, 

                // The loop body for each interval
                (range, loopState, initialValue) =>
                {
                    Console.WriteLine("Partitioner Default range {0} {1}", range.Item1, range.Item2);
                    double partialSum = initialValue;
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        partialSum += Normalize(sequence[i]);
                    }
                    return partialSum;
                },

                // The final step of each local context
                (localPartialSum) =>
                {
                    // Use lock to enforce serial access to shared result
                    lock (lockObject)
                    {
                        sum += localPartialSum;
                    }
                });
            Console.WriteLine("Result {0}", sum);


            // Do not copy this code. This version will run much slower
            // than the sequential version. It's included here to 
            // illustrate what not to do.
            // BUG – don't do this
            Parallel.For(0, sequence.Length, i =>
                {
                    // BUG – don't do this
                    lock (lockObject)
                    {
                        sum += sequence[i];
                    }
                });
            Console.WriteLine("Result {0}", sum);

            // If you forget to add the lock statement, this code fails to calculate the correct sum on a multicore computer.
            // Adding the lock statement makes this code example correct with respect to serialization. If you run this code, it
            // produces the expected sum. However, it fails completely as an optimization. This code is many times slower than
            // the sequential version it attempted to optimize! The reason for the poor performance is the cost of synchronization.         
        }

        public static double Normalize(double number)
        {
            return number * 2;
        }
    }
}


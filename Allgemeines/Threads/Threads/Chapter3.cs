using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace Threads
{
    /// <summary>
    /// 
    /// Chapter 3.
    /// Parallel Tasks
    /// 
    /// Taken from http://msdn.microsoft.com/en-us/library/ff963549.aspx
    /// 
    /// </summary>
    public class Chapter3
    {
        public static void Test()
        {
            Console.WriteLine("Chapter 3");

            Console.WriteLine("Main Thread id {0}", Thread.CurrentThread.ManagedThreadId);


            Console.WriteLine("Parallel Invoke");
            Parallel.Invoke(() => DoLeft("parallelInvoke"), DoRight);
            // The Parallel.Invoke method includes an implicit call to WaitAll. Exceptions from all of the tasks are grouped
            // together in an AggregateException object and thrown in the calling context of the WaitAll or Wait method.


            Console.WriteLine("Simple Task Factory StartNew");
            Task t1 = Task.Factory.StartNew(() => DoLeft("simpleTaskFactory"));
            Task t2 = Task.Factory.StartNew(DoRight);
            Task.WaitAll(t1, t2);


            int n = 20;
            Console.WriteLine("Handling Exceptions 1/2: The Handle Method");
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            cts.CancelAfter(600);
            Task myTask = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < n; i++)
                {
                    Thread.Sleep(500);
                    // optionally check to see if cancellation happened
                    if (token.IsCancellationRequested)
                    {
                        // IsCancellationRequested property to test for a cancellation request. For example, you would do this
                        // if you need to perform local cleanup actions for a task that's in the process of being canceled.
                        Console.WriteLine("Cancelling a Task Check State {0}", i);
                        // optionally exit this iteration early
                        // return;
                    }
                    token.ThrowIfCancellationRequested();
                    Console.WriteLine("Cancelling a Task {0}", i);
                }
            }, token);
            // Invoking the faulted task's Wait method causes the task's unhandled exception to be observed. The exception is also
            // thrown in the calling context of the Wait method. The Task class's static WaitAll method allows you to observe the
            // unhandled exceptions of more than one task with a single method invocation.

            // ******************************************************** WTF ********************************************************
            // Special handling occurs if a faulted task's unhandled exceptions are not observed by the time the task object is
            // garbage-collected. For more information, see the section, "Unobserved Task Exceptions," later in this chapter.
            // You must take care to observe the unhandled exceptions of each task. If you don't do this, .NET's exception
            // escalation policy can terminate your process when the task is garbage collected.
            // *********************************************************************************************************************
            try {
                Task.WaitAll(myTask);
            }
            catch (AggregateException ae)
            {
                // When the Handle method invokes the user-provided delegate for each inner exception, it keeps track of the results of
                // the invocation. After it processes all inner exceptions, it checks to see if the results for one or more of the inner
                // exceptions were false, which indicates that they have not been handled. If there are any unhandled exceptions,
                // the Handle method creates and throws a new aggregate exception that contains the unhandled ones as inner exceptions.
                ae.Handle(e =>
                {
                    if (e is OperationCanceledException)
                    {
                        Console.WriteLine("Cancelling a Task, catching OperationCanceledException: {0} {1}", e.Message, e.StackTrace);
                        return true;
                    }
                    else
                    { 
                        Console.WriteLine("Cancelling a Task, dunno what are you: {0} {1}", e.Message, e.StackTrace);
                        return false;
                    }
                });
            }
            if (myTask.IsCanceled)
            {
                Console.WriteLine("Cancelling a Task, task was canceled");
            }

            Console.WriteLine("Handling Exceptions 2/2: The Flatten Method");
            t1 = Task.Factory.StartNew(() =>
            {
                t2 = Task.Factory.StartNew(() =>
                { 
                    Console.WriteLine("The Flatten Method, t2 throws exception");
                    throw new MyException("This is my exception from t1");
                });
                Console.WriteLine("The Flatten Method, t1 going to wait for t2");
                t2.Wait();
            });
            try {
                t1.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Flatten().Handle(e =>
                {
                    if (e is MyException)
                    {
                        Console.WriteLine("The Flatten Method, catching MyException {0} {1}", e.Message, e.StackTrace);
                        return true;
                    }
                    else
                    { 
                        Console.WriteLine("Cancelling a Task, dunno what are you: {0} {1}", e.Message, e.StackTrace);
                        return false;
                    }
                });
            }


            Console.WriteLine("Waiting for the First Task to Complete");
            var taskIndex = -1;
            Task[] tasks = new Task[]
            {
                Task.Factory.StartNew(() => DoLeft("waitingForTheFirstTask")),
                Task.Factory.StartNew(DoRight),
                Task.Factory.StartNew(DoCenter)
            };
            Task[] allTasks = tasks;
            // Print completion notices one by one as tasks finish.
            while (tasks.Length > 0)
            {
                taskIndex = Task.WaitAny(tasks);
                Console.WriteLine("Finished task {0}.", taskIndex + 1);
                tasks = tasks.Where((t) => t != tasks[taskIndex]).ToArray();
            }
            // You need to observe any exceptions that may have occurred, but note that exceptions are never observed
            // by the WaitAny method. Instead, you should add a step that checks for exceptions whenever you use the
            // WaitAny method. Although this example uses the WaitAll method to observe exceptions, you could also use
            // the Exception property or the Wait method for this purpose. The IsFaulted property of the Task class
            // can be used to check to see whether an unhandled exception occurred within a task.


            // Observe any exceptions that might have occurred.
            try
            {
                Task.WaitAll(allTasks);
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("Waiting for the First Task to Complete: {0} {1}", ae.Message, ae.StackTrace);
            }


            Console.WriteLine("SpeculativeInvoke");
            SpeculativeInvoke(DoActionFirst, DoActionSecond, DoActionThird);


            Console.WriteLine("Buggy Code");
            for (int i = 0; i < 4; i++)
            {   
                // WARNING: BUGGY CODE, i has unexpected value
                // i is outside the loop scope!!! The lambda may catch variables in all inherited scopes, typically
                // you will want to work just with the current scope (as in this case)
                // When lambda expression catches some variable, the lambda expression becomes a closure.
                Task.Factory.StartNew(() => Console.WriteLine(i));
            }
            Thread.Sleep(1000);
            Console.WriteLine("No Buggy Code");
            for (int i = 0; i < 4; i++)
            {
                // The closure catches the current value of tmp.
                var tmp = i;
                Task.Factory.StartNew(() => Console.WriteLine(tmp));
            }
            Thread.Sleep(1000);


            Console.WriteLine("Disposing a Resource Needed by a Task");
            // Be careful not to dispose resources needed by a pending task.
            Task<string> task;
            using (var file = new StringReader("text1\ntext2\ntext3"))
            {
                // 1. This task will run in some new thread.
                // 2. Concurrently the main thread executes an implicit finally block (remember the using declaration) for file variable,
                //    this finally block calls file.Dispose
                // 3. There is a high probability for the task thread of running after the finally block in the main thread
                // 4. Because of 3. there is a high probability for the task thread of seeing file after the main thread
                //    ran file.Dispose. After Dispose we can not use again some StringReader variable. So, if the main thread ran file.Dispose
                //    before the task thread ran file.ReadLine(), the task thread will throw ObjectDisposedException when trying to execute file.ReadLine()
                //    Then AggregateException wraps ObjectDisposedException. AggregateException will be caught by the main thread when
                //    using Task.WaitAny, Task.WaitAll, task.Result, task.Wait...
                //    QUESTION: what methods throw AggregateException from some Task? I miss Javadoc... :(
                // 5. SO, DISPOSE MUST NOT BE DONE OUTSIDE THE TASK THREAD!!! IN THIS CASE THERE IS A IMPLICIT DIPOSE (BECAUSE OF THE 
                //    using DECLARATION BEING DONE BY THE MAIN THREAD.
                task = Task<string>.Factory.StartNew(() => file.ReadLine());
            }
            // The main thread will run file.Dispose, probably before the task thread runs file.ReadLine() and in that case
            // the task will throw AggregateException when using some of the methods which enable us to catch exceptions coming from
            // task threads. QUESTION: what methods throw AggregateException from some Task? I miss Javadoc... :(
            // task.Result calls task.Wait and task.Wait throws AggregateException :)
            Console.WriteLine(task.Result);
        }


        public static void DoLeft(string doLeft)
        {
            Thread.Sleep(3000);
            Console.WriteLine("Parallel.Invoke DoLeft {0} {1}", Thread.CurrentThread.ManagedThreadId, doLeft);
        }

        public static void DoRight()
        {
            Thread.Sleep(2000);
            Console.WriteLine("Parallel.Invoke DoRight {0}", Thread.CurrentThread.ManagedThreadId);
        }

        public static void DoCenter()
        {
            Thread.Sleep(1500);
            Console.WriteLine("Parallel.Invoke DoCenter {0}", Thread.CurrentThread.ManagedThreadId);
        }


        public static void SpeculativeInvoke(params Action<CancellationToken>[] actions)
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var tasks = 
                (from a in actions
                    select Task.Factory.StartNew(() => a(token), token))
                    .ToArray();
            // Wait for fastest task to complete.
            Task.WaitAny(tasks);
            // Cancel all of the slower tasks.
            cts.Cancel();
            // Wait for cancellation to finish and observe exceptions.
            try 
            { 
                Task.WaitAll(tasks); 
            }
            catch (AggregateException ae)
            {
                // Filter out the exception caused by cancellation itself.
                ae.Flatten().Handle(e => e is OperationCanceledException);
            }
            finally
            {
                if (cts != null) cts.Dispose();
            }
        }

        public static void DoActionFirst(CancellationToken token)
        {
            var n = 10;
            for (int i = 0; i < n; i++)
            {
                Thread.Sleep(500);
                // optionally check to see if cancellation happened
                if (token.IsCancellationRequested)
                {
                    // IsCancellationRequested property to test for a cancellation request. For example, you would do this
                    // if you need to perform local cleanup actions for a task that's in the process of being canceled.
                    Console.WriteLine("DoActionFirst Cancelling a Task Check State {0}", i);
                    // optionally exit this iteration early
                    // return;
                }
                token.ThrowIfCancellationRequested();
                Console.WriteLine("DoActionFirst {0}", i);
            }
        }

        public static void DoActionSecond(CancellationToken token)
        {
            var n = 20;
            for (int i = 0; i < n; i++)
            {
                Thread.Sleep(2000);
                // optionally check to see if cancellation happened
                if (token.IsCancellationRequested)
                {
                    // IsCancellationRequested property to test for a cancellation request. For example, you would do this
                    // if you need to perform local cleanup actions for a task that's in the process of being canceled.
                    Console.WriteLine("DoActionSecond Cancelling a Task Check State {0}", i);
                    // optionally exit this iteration early
                    // return;
                }
                token.ThrowIfCancellationRequested();
                Console.WriteLine("DoActionSecond {0}", i);
            }
        }

        public static void DoActionThird(CancellationToken token)
        {
            var n = 20;
            for (int i = 0; i < n; i++)
            {
                Thread.Sleep(3000);
                // optionally check to see if cancellation happened
                if (token.IsCancellationRequested)
                {
                    // IsCancellationRequested property to test for a cancellation request. For example, you would do this
                    // if you need to perform local cleanup actions for a task that's in the process of being canceled.
                    Console.WriteLine("DoActionThird Cancelling a Task Check State {0}", i);
                    // optionally exit this iteration early
                    // return;
                }
                token.ThrowIfCancellationRequested();
                Console.WriteLine("DoActionThird {0}", i);
            }
        }
    }

    public class MyException : Exception
    {
        public MyException(string message) : base(message) { }
    }
}


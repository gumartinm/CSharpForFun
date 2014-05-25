using System;
using System.Threading;

namespace Threads
{
    /// <summary>
    /// 
    /// Chapter 1.
    /// Simple Threads
    /// 
    /// Taken from http://msdn.microsoft.com/en-us/library/system.threading.threadstart.aspx
    /// Taken from http://msdn.microsoft.com/en-us/library/6x4c42hc%28v=vs.110%29.aspx
    /// 
    /// </summary>
    public class Chapter1
    {
        public static void Test()
        {
            Console.WriteLine("Chapter 1");


            Console.WriteLine("ThreadStart");
            // To start a thread using a static thread procedure, use the 
            // class name and method name when you create the ThreadStart 
            // delegate. Beginning in version 2.0 of the .NET Framework, 
            // it is not necessary to create a delegate explicitly.  
            // Specify the name of the method in the Thread constructor,  
            // and the compiler selects the correct delegate. For example: 
            // 
            // Thread newThread = new Thread(Work.DoWork); 
            //
            WorkWithThreadStart workWith = new WorkWithThreadStart();
            ThreadStart threadDelegate = new ThreadStart(workWith.DoWork);
            Thread newThread = new Thread(threadDelegate);
            newThread.Start();
            Thread.Sleep(3000);
            // To start a thread using an instance method for the thread  
            // procedure, use the instance variable and method name when  
            // you create the ThreadStart delegate. Beginning in version 
            // 2.0 of the .NET Framework, the explicit delegate is not 
            // required. 
            //
            workWith = new WorkWithThreadStart();
            workWith.Data = 42;
            threadDelegate = new ThreadStart(workWith.DoMoreWork);
            newThread = new Thread(threadDelegate);
            newThread.Start();
            Thread.Sleep(3000);


            Console.WriteLine("ParameterizedThreadStart");
            WorkWithParameterizedThreadStart workWithout = new WorkWithParameterizedThreadStart();
            // To start a thread using a shared thread procedure, use 
            // the class name and method name when you create the  
            // ParameterizedThreadStart delegate. C# infers the  
            // appropriate delegate creation syntax: new ParameterizedThreadStart(Work.DoWork) 
            //
            // ParameterizedThreadStart parameterizedThreadDelegate = new ThreadStart(workWithout.DoWork);
            // newThread = new Thread(parameterizedThreadDelegate);
            newThread = new Thread(workWithout.DoWork);

            // Use the overload of the Start method that has a 
            // parameter of type Object. You can create an object that 
            // contains several pieces of data, or you can pass any  
            // reference type or value type. The following code passes 
            // the integer value 42. 
            //
            newThread.Start(42);
            Thread.Sleep(3000);

            // To start a thread using an instance method for the thread  
            // procedure, use the instance variable and method name when  
            // you create the ParameterizedThreadStart delegate. C# infers  
            // the appropriate delegate creation syntax: 
            //    new ParameterizedThreadStart(w.DoMoreWork) 
            //
            workWithout = new WorkWithParameterizedThreadStart();
            newThread = new Thread(workWithout.DoMoreWork);

            // Pass an object containing data for the thread. 
            //
            newThread.Start("The answer.");
            Thread.Sleep(3000);
        }

        private class WorkWithThreadStart 
        {
            public void DoWork() 
            {
                Console.WriteLine("Static thread procedure."); 
            }

            public int Data;

            public void DoMoreWork() 
            {
                Console.WriteLine("Instance thread procedure. Data={0}", Data); 
            }
        }

        private class WorkWithParameterizedThreadStart
        {
            public void DoWork(object data)
            {
                Console.WriteLine("Static thread procedure. Data='{0}'", data);
            }

            public void DoMoreWork(object data)
            {
                Console.WriteLine("Instance thread procedure. Data='{0}'", data);
            }
        }
    }
}


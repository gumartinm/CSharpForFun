using System;

//
// SEE FILE: under_the_scenes.txt
// (it should be in the same directory as this file)
//
namespace TryWithResourcesC
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("BEGIN FIRST EXAMPLE");
            Console.Out.Flush();

            using (ResourceFirst resourceOne = new ResourceFirst())
            using (ResourceSecond resourceTwo = new ResourceSecond())
            {
                resourceTwo.DoSomething();
                resourceOne.DoSomething();
            }

            Console.WriteLine("END FIRST EXAMPLE");
            Console.Out.Flush();


            Console.WriteLine("BEGIN SECOND EXAMPLE");
            Console.Out.Flush();

            using (ResourceFourth resourceFourth = new ResourceFourth())
            using (ResourceFifth resourceFifth = new ResourceFifth())
            {
                resourceFifth.DoSomething();
                resourceFourth.DoSomething();
            }

            Console.WriteLine("END SECOND EXAMPLE");
            Console.Out.Flush();
        }
    }


    public class ResourceFirst : IDisposable
    {
        public void DoSomething()
        {
            Console.WriteLine("ResourceFirst: DoSomething");
            Console.Out.Flush();
            throw new Exception("ResourceFirst DoSomething Exception!!!");
        }

        public void Dispose()
        {
            Console.WriteLine("I am the Dispose of ResourceFirst");
            Console.Out.Flush();
            throw new Exception("ResourceFirst Dispose Exception!!!");
        }
    }

    public class ResourceSecond : IDisposable
    {
        public void DoSomething()
        {
            Console.WriteLine("ResourceSecond: DoSomething");
            Console.Out.Flush();
        }

        public void Dispose()
        {
            Console.WriteLine("I am the Dispose of ResourceSecond");
            Console.Out.Flush();
            throw new Exception("ResourceSecond Dispose Exception!!!");
        }
    }


    public class ResourceFourth : IDisposable
    {
        public ResourceFourth()
        {
            throw new Exception("ResourceFourth Constructor Exception!!!");
        }

        public void DoSomething()
        {
            Console.WriteLine("ResourceFourth: DoSomething");
            Console.Out.Flush();
            throw new Exception("ResourceFourth DoSomething Exception!!!");
        }

        public void Dispose()
        {
            Console.WriteLine("I am the Dispose of ResourceFourth");
            Console.Out.Flush();
            throw new Exception("ResourceFourth Dispose Exception!!!");
        }
    }

    public class ResourceFifth: IDisposable
    {
        public ResourceFifth()
        {
            throw new Exception("ResourceFifth Constructor Exception!!!");
        }

        public void DoSomething()
        {
            Console.WriteLine("ResourceFifth: DoSomething");
        }

        public void Dispose()
        {
            Console.WriteLine("I am the Dispose of ResourceFifth");
            Console.Out.Flush();
            throw new Exception("ResourceFifth Dispose Exception!!!");
        }
    }
}

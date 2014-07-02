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

            using (ResourceFirst resourceOne = new ResourceFirst())
            using (ResourceSecond resourceTwo = new ResourceSecond())
            {
                resourceTwo.DoSomething();
                resourceOne.DoSomething();
            }

            Console.WriteLine("END FIRST EXAMPLE");


            Console.WriteLine("BEGIN SECOND EXAMPLE");

            using (ResourceFourth resourceFourth = new ResourceFourth())
            using (ResourceFifth resourceFifth = new ResourceFifth())
            {
                resourceFifth.DoSomething();
                resourceFourth.DoSomething();
            }

            Console.WriteLine("END SECOND EXAMPLE");
        }
    }


    public class ResourceFirst : IDisposable
    {
        public void DoSomething()
        {
            Console.WriteLine("ResourceFirst: DoSomething");
            throw new Exception("ResourceFirst DoSomething Exception!!!");
        }

        public void Dispose()
        {
            Console.WriteLine("I am the Dispose of ResourceFirst");
            throw new Exception("ResourceFirst Dispose Exception!!!");
        }
    }

    public class ResourceSecond : IDisposable
    {
        public void DoSomething()
        {
            Console.WriteLine("ResourceSecond: DoSomething");
        }

        public void Dispose()
        {
            Console.WriteLine("I am the Dispose of ResourceSecond");
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
            throw new Exception("ResourceFourth DoSomething Exception!!!");
        }

        public void Dispose()
        {
            Console.WriteLine("I am the Dispose of ResourceFourth");
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
            throw new Exception("ResourceFifth Dispose Exception!!!");
        }
    }
}

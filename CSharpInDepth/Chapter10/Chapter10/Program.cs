using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chapter10
{
    class Program
    {
        static void Main(string[] args)
        {

            /**
             * 
             * Listing 10.2 Using StreamUtil to copy a web response stream to a file.
             */
            Console.WriteLine("Listing 10.2 Using StreamUtil to copy a web response stream to a file.");
            WebRequest request1 = WebRequest.Create("http://gumartinm.name");
            using (WebResponse response1 = request1.GetResponse())
            using (Stream responseStream1 = response1.GetResponseStream())
            using (FileStream output1 = File.Create("response.dat"))
            {
                StreamUtil.Copy(responseStream1, output1);
            }

            /**
             * 
             * Listing 10.4 Copying a stream using an extension method.
             */
            Console.WriteLine("Listing 10.4 Copying a stream using an extension method.");
            WebRequest request2 = WebRequest.Create("http://gumartinm.name");
            using (WebResponse response2 = request2.GetResponse())
            using (Stream responseStream2 = response2.GetResponseStream())
            using (FileStream output2 = File.Create("response.dat"))
            {
                responseStream2.CopyToExtension(output2);
            }

            /**
             * 
             * Listing 10.5 Extension method being called on a null reference.
             */
            Console.WriteLine("Listing 10.5 Extension method being called on a null reference.");
            object y = null;
            Console.WriteLine(y.IsNull());
            y = new object();
            Console.WriteLine(y.IsNull());

            /**
             * 
             * Listing 10.6 Using Enumerable.Range to print out the numbers 0 to 9.
             */
            Console.WriteLine("Listing 10.6 Using Enumerable.Range to print out the numbers 0 to 9.");
            var collection1 = Enumerable.Range(0, 10);
            foreach (var element in collection1)
            {
                Console.WriteLine(element);
            }

            /**
             * 
             * Listing 10.7 Reversing a collection with the Reverse method.
             */
            Console.WriteLine("Listing 10.7 Reversing a collection with the Reverse method.");
            var collection2 = Enumerable.Range(0, 10).Reverse();
            foreach (var element in collection2)
            {
                Console.WriteLine(element);
            }

            /**
             * 
             * Listing 10.8 Using the Where method with a lambda expression to find odd numbers.
             */
            Console.WriteLine("Listing 10.8 Using the Where method with a lambda expression to find odd numbers.");
            var collection3 = Enumerable.Range(0, 10).Where(x => x % 2 != 0).Reverse();
            foreach (var element in collection3)
            {
                Console.WriteLine(element);
            }

            /**
             * 
             * Listing 10.9 Projection using a lambda expression and an anonymous type.
             */
            Console.WriteLine("Listing 10.9 Projection using a lambda expression and an anonymous type.");
            var collection4 = Enumerable.Range(0, 10)
                .Where(x => x % 2 != 0)
                .Reverse()
                .Select(x => new { Original = x, SquareRoot = Math.Sqrt(x) });
            foreach(var element in collection4)
            {
                Console.WriteLine(element);
            }

            /**
             * 
             * Listing 10.10 Ordering a sequence by two properties.
             */
            Console.WriteLine("Listing 10.10 Ordering a sequence by two properties.");
            var collection5 = Enumerable.Range(-5, 11)
                .Select(x => new { Original = x, Square = x * x })
                .OrderBy(x => x.Square)
                .ThenBy(x => x.Original);
            foreach (var element in collection5)
            {
                Console.WriteLine(element);
            }

            // Wait for key.
            Console.ReadKey();
        }
    }

    public static class StreamUtil
    {
        const int BufferSize = 8192;

        public static void Copy(Stream input, Stream output)
        {
            byte[] buffer = new byte[BufferSize];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream tempStream = new MemoryStream())
            {
                Copy(input, tempStream);
                return tempStream.ToArray();
            }
        }
    }

    public static class StreamUtilExtension
    {
        const int BufferSize = 8192;

        public static void CopyToExtension(this Stream input, Stream output)
        {
            byte[] buffer = new byte[BufferSize];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        public static byte[] ReadFully(this Stream input)
        {
            using (MemoryStream tempStream = new MemoryStream())
            {
                CopyToExtension(input, tempStream);
                return tempStream.ToArray();
            }
        }
    }

    public static class NullUtil
    {
        public static bool IsNull(this object x)
        {
            return x == null;
        }
    }
}

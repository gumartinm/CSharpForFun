using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServersExamples
{
    class Program
    {
        private static AutoResetEvent autoEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            HttpListener server = new HttpListener();

            server.Prefixes.Add("http://127.0.0.1:10002/");

            server.Start();

            while(server.IsListening)
            {
                var asyncResult = server.BeginGetContext(ListenerCallback, server);
                Thread.Sleep(3000);
            }

        }

        private static void ListenerCallback(IAsyncResult result)
        {
            autoEvent.Set();
            autoEvent.Reset();

            HttpListener server = (HttpListener)result.AsyncState;

            HttpListenerContext context = server.EndGetContext(result);

            HandleRemoteRequest(context);
        }

        private static void HandleRemoteRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;

            HttpListenerResponse response = context.Response;

            string responseString = "<HTML><BODY>Hello World!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using(Stream output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }

            int waitResult = WaitHandle.WaitAny(new[] { autoEvent }, TimeSpan.FromSeconds(30), false);

            if (waitResult == 0 || waitResult == 1)
            {
                Console.WriteLine("Done...");
            }
            else if (waitResult == WaitHandle.WaitTimeout)
            {
                Console.WriteLine("Done by timeout");
            }

        }
    }
}

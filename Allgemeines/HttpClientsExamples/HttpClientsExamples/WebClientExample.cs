using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientsExamples
{
    public class WebClientExample
    {
        public void Test()
        {
            Console.WriteLine("Synchronous WebClient");
            string line = null;
            while (line == null || line.Length == 0)
            {
                Console.WriteLine("Specify the URI of the resource to retrieve.");
                Console.WriteLine("Write URI: ");
                line = Console.ReadLine();
            }

            using (WebClient client = new WebClient())
            {
                // Add a user agent header in case the  
                // requested URI contains a query.
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; Linux; Mono .NET 4.5)");

                using (Stream data = client.OpenRead (line))
                using (StreamReader reader = new StreamReader(data))
                {
                    string s = reader.ReadToEnd();
                    Console.WriteLine(s);
                }
                // data.Dispose();
                // data.Close();
                // reader.Dispose();
                // reader.Close();
            }
            // client.Dispose();


            line = null;
            Console.WriteLine("Asynchronous WebClient With Events");
            while (line == null || line.Length == 0)
            {
                Console.WriteLine("Specify the URI of the resource to retrieve.");
                Console.WriteLine("Write URI: ");
                line = Console.ReadLine();
            }

            using (WebClient client = new WebClient())
            {
                // Another way, using DownloadDataAsync and its delegate.
                // client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadDataCallback);
                // client.DownloadDataAsync (uri, waiter);
                client.OpenReadCompleted += new OpenReadCompletedEventHandler(OpenReadCallback);
                client.OpenReadAsync(new Uri(line), null /*What is this for? :( */);
            }
            // How to wait for client end without using sleep? :(
            Thread.Sleep(3000);


            line = null;
            Console.WriteLine("Asynchronous WebClient With Tasks");
            while (line == null || line.Length == 0)
            {
                Console.WriteLine("Specify the URI of the resource to retrieve.");
                Console.WriteLine("Write URI: ");
                line = Console.ReadLine();
            }
            using (WebClient client = new WebClient())
            using (Task<Stream> data = client.OpenReadTaskAsync(line))
                //using (Stream stream = data.Result)
            {
                data.Start();
                try {
                    Task.WaitAll(data);
                }
                catch (AggregateException ae)
                {
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
                Stream dataStream = data.Result;
                if (dataStream != null)
                {

                }
            }
        }

        private void OpenReadCallback (Object sender, OpenReadCompletedEventArgs e)
        {
            Stream reply = null;
            StreamReader s = null;

            // Old fashined way (without using)
            // NOTE: using statement is the same as this code (it also checks for null value before calling
            // the Dispose method)
            try
            {
                reply = (Stream)e.Result;
                s = new StreamReader (reply);
                Console.WriteLine (s.ReadToEnd ());
            }
            finally
            {
                if (s != null)
                {
                    s.Close ();
                }

                if (reply != null)
                {
                    reply.Close ();
                }
            }
        }
    }
}


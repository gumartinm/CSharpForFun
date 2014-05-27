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
                // WebClient.DownloadStringCompleted
                // client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadDataCallback);
                // client.DownloadDataAsync (uri, waiter);
                // We may retrieve the data using WebClient.DownloadDataCompletedEventArgs.Result in the
                // delegate implementation (our callback/lambda expression/delegate implementation)
                client.OpenReadCompleted += new OpenReadCompletedEventHandler(OpenReadCallback);

                // Taken from: http://msdn.microsoft.com/en-us/library/wewwczdw.aspx
                // Asynchronous Method Overloads:
                // There are potentially two overloads for the asynchronous operations: single-invocation and
                // multiple-invocation. You can distinguish these two forms by their method signatures: the
                // multiple-invocation form has an extra parameter called userState. This form makes it possible
                // for your code to call Method1Async(string param, object userState) multiple times without
                // waiting for any pending asynchronous operations to finish. If, on the other hand, you try to
                // call Method1Async(string param) before a previous invocation has completed, the method
                // raises an InvalidOperationException.

                // The userState parameter for the multiple-invocation overloads allows you to distinguish
                // among asynchronous operations. You provide a unique value (for example, a GUID or hash code)
                // for each call to Method1Async(string param, object userState), and when each operation
                // is completed, your event handler can determine which instance of the operation raised
                // the completion event.

                // Tracking Pending Operations:
                // If you use the multiple-invocation overloads, your code will need to keep track of
                // the userState objects (task IDs) for pending tasks. For each call to
                // Method1Async(string param, object userState), you will typically generate a new,
                // unique userState object and add it to a collection. When the task corresponding to this
                // userState object raises the completion event, your completion method implementation will
                // examine AsyncCompletedEventArgs.UserState and remove it from your collection. Used this way,
                // the userState parameter takes the role of a task ID.

                // You must be careful to provide a unique value for userState in your calls to multiple-invocation
                // overloads. Non-unique task IDs will cause the asynchronous class throw an ArgumentException.
                var taskId = Guid.NewGuid();
                client.OpenReadAsync(new Uri(line), taskId);
                client.Dispose ();
                // I could make more calls in this way:
                // taskId = Guid.NewGuid();
                // client.OpenReadAsync(new Uri(line), taskId);
                // taskId = Guid.NewGuid();
                // client.OpenReadAsync(new Uri(line), taskId);
            }
            // How to wait for client end without using sleep? I am afraid, there is no way :(
            Console.WriteLine ("Press any key to continue after receiving data.");
            Console.Read();


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

        private void OpenReadCallback (Object sender, OpenReadCompletedEventArgs eventData)
        {
            Stream replyStream = null;
            StreamReader replyStreamReader = null;

            // Old fashined way (without using)
            // NOTE: using statement is the same as this code (it also checks for null value before calling
            // the Dispose method)

            var taskId = (Guid) eventData.UserState;

            if (eventData.Cancelled)
            {
                Console.WriteLine ("Task Cancelled. Taks Id: {0}", taskId);
                return;
            }

            Exception errorException = eventData.Error;
            if (errorException != null)
            {
                Console.WriteLine ("Task with Exception. Taks Id: {0}, Exception: ", taskId, errorException);
                return;
            }


            try
            {
                // throw new Exception("It will become an UnHandled Exception if the main thread does not cath it" +
                //    "and it will kill my application :(");
                replyStream = (Stream) eventData.Result;
                replyStreamReader = new StreamReader (replyStream);
                Console.WriteLine (replyStreamReader.ReadToEnd ());
                // throw new Exception("If I throw exception here, weird things will happen. Shouldn't it become" +
                //    "a UnHandled Exception?");
            }
            finally
            {
                if (replyStreamReader != null)
                {
                    replyStreamReader.Close ();
                }

                if (replyStream != null)
                {
                    replyStream.Close ();
                }
            }
        }
    }
}


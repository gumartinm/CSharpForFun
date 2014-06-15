using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientsExamples
{
    /**
     * BE CAREFUL!!! If calling Dispose may throw exception, you could hide
     * exceptions being thrown inside the using blocks. If the Dispose methods
     * do not throw any exception there is no problem,
     * otherwise see: http://msdn.microsoft.com/en-us/library/aa355056%28v=vs.110%29.aspx
     */

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

                try {
                    // HOW MAY I CHECK THE HTTP RESPONSE CODE? THIS SUCKS!!!
                    // From this answer: http://stackoverflow.com/questions/3574659/how-to-get-status-code-from-webclient
                    // When are we receiving WebException? When HTTP status code is not 200? I do not think so.
                    // What if I just want to read data when HTTP OK? I am getting really angry.
                    // C# is a great language but the API sucks IMHO.

                    // Be careful!!! If calling Dispose may throw exception, you could hide
                    // exceptions being thrown inside the using blocks. In this case this Dispose methods
                    // do not throw any exception but in other cases it could be different, so never forget it.
                    // See: http://msdn.microsoft.com/en-us/library/aa355056%28v=vs.110%29.aspx
                    using (Stream replyStream = client.OpenRead (line))
                    using (StreamReader replyStreamReader = new StreamReader(replyStream))
                    {
                        string s = replyStreamReader.ReadToEnd();
                        Console.WriteLine(s);
                    }
                }
                catch(WebException e) {
                    Console.WriteLine ("Synchronous WebClient, WebException: ", e);
                }
                catch(IOException e) {
                    Console.WriteLine ("Synchronous WebClient, IOException: ", e);
                }
            }


            line = null;
            Console.WriteLine("Asynchronous WebClient With Events");
            while (line == null || line.Length == 0)
            {
                Console.WriteLine("Specify the URI of the resource to retrieve.");
                Console.WriteLine("Write URI: ");
                line = Console.ReadLine();
            }
            /**
             * IT IS IMPOSSIBLE TO CALL WebClient.Dispose() without creating a complete mess. :/
             * Besides, calling Dispose() does not seem really useful (AFAIU after reading the Mono implementation)
             * and this answer from SE: http://codereview.stackexchange.com/questions/9714/using-statement-in-context-of-streams-and-webclients
             * seems to match what I have already seen in the Mono code. So, I will use the using statement
             * when working with WebClient class in a synchronous way even when I know it probably does not do anything useful
             * and will not bother calling WebClient.Dispose() when using WebClient in asynchronous way because
             * the code ends up being too complex.
             * Hopefully this will work smoothly :)
             */
            WebClient webClientAsync = new WebClient();

            // Another way, using DownloadDataAsync and its delegate.
            // WebClient.DownloadStringCompleted
            // client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadDataCallback);
            // client.DownloadDataAsync (uri, waiter);
            // We may retrieve the data using WebClient.DownloadDataCompletedEventArgs.Result in the
            // delegate implementation (our callback/lambda expression/delegate implementation)
            webClientAsync.OpenReadCompleted += new OpenReadCompletedEventHandler(OpenReadCallback);

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
            try {
                webClientAsync.OpenReadAsync(new Uri(line), taskId);
            } catch(WebException e) {
                Console.WriteLine ("Asynchronous WebClient With Events, WebException: ", e);
            }

            /**
             * I WILL NOT CALL Dispose when using WebClient in asynchronous way.
             * webClientAsync.Dispose ();
             */

            // I could make more calls in this way BUT JUST ONCE THE LAST ASYNC OPERATION WAS COMPLETED!!!!
            // From here: http://stackoverflow.com/questions/9765109/webclient-does-not-support-concurrent-i-o-operations
            // And here: http://stackoverflow.com/questions/7905445/wb-downloadfileasync-throw-webclient-does-not-support-concurrent-i-o-operations
            // When calling DownloadFileAsync method you have to make sure it completes before trying to download again.
            //
            // So, I should wait until the async operation is completed and then I could use OpenReadAsync again, but not before. :(
            // taskId = Guid.NewGuid();
            // webClientAsync.OpenReadAsync(new Uri(line), taskId);
            // taskId = Guid.NewGuid();
            // webClientAsync.OpenReadAsync(new Uri(line), taskId);

            // How to wait for client end without using sleep? I am afraid, there is no nice way. Anyhow if you are using Async,
            // why would you want to wait?
            Console.Read();


            line = null;
            Console.WriteLine("Asynchronous WebClient With Tasks");
            while (line == null || line.Length == 0)
            {
                Console.WriteLine("Specify the URI of the resource to retrieve.");
                Console.WriteLine("Write URI: ");
                line = Console.ReadLine();
            }
            using (WebClient client = new WebClient ())
            {
                // AFAIK (see comments in some place above) I do not need to call WebClient.Dispose. Anyhow, in this case, the code
                // is not too complicated so, I will write the using statement.

                /**
                 * Should I call Task.Dispose()? Answer:
                 * DO NOT BOTHER DISPOSING OF YOUR TASKS: http://blogs.msdn.com/b/pfxteam/archive/2012/03/25/10287435.aspx
                 */
                Task<Stream> task = null;
                try {
                    task = client.OpenReadTaskAsync (line);
                } catch(WebException e) {
                    Console.WriteLine ("Asynchronous WebClient With Tasks, WebException: ", e);
                }

                if (task == null) {
                    return;
                }

                // Don't do this. OpenReadTaskAsync is already launching a new Thread (OpenReadTaskAsync is intended to be used with async/await)
                //task.Start ();
                try {
                    // DO NOT DO THIS. THERE COULD BE DEADLOCK. ALL DEPENDS ON THE SynchronizationContext
                    // How may I know what SynchronizationContext is going to be used? :/
                    // See: http://msdn.microsoft.com/en-us/magazine/gg598924.aspx
                    //      http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
                    //      http://stackoverflow.com/questions/22699048/why-does-task-waitall-not-block-or-cause-a-deadlock-here
                    // AFAIK, I SHOULD USE Task.WhenAll INSTEAD!!!! Because it creates a task (a new thread instead of blocking the current one)
                    // http://msdn.microsoft.com/en-us/library/system.threading.tasks.task.whenall%28v=vs.110%29.aspx
                    Task.WaitAll (task);
                } catch (AggregateException ae) {
                    ae.Handle (e => {
                        if (e is OperationCanceledException) {
                            Console.WriteLine ("Cancelling a Task, catching OperationCanceledException: {0}", e);
                            return true;
                        } else {
                            Console.WriteLine ("Cancelling a Task, dunno what are you: {0}", e);
                            return true;
                        }
                    });
                }
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    // HOW MAY I CHECK THE HTTP RESPONSE CODE? THIS SUCKS!!!
                    // From this answer: http://stackoverflow.com/questions/3574659/how-to-get-status-code-from-webclient
                    // When are we receiving WebException? When HTTP status code is not 200? I do not think so.
                    // What if I just want to read data when HTTP OK? I am getting really angry.
                    // C# is a great language but the API sucks IMHO.

                    try {
                        using (Stream replyStream = task.Result)
                        using (StreamReader replyStreamReader = new StreamReader (replyStream))
                        {
                            string s = replyStreamReader.ReadToEnd ();
                            Console.WriteLine (s);
                        }
                    }
                    catch(IOException e) {
                        Console.WriteLine ("Asynchronous WebClient With Tasks, IOException: ", e);
                    }
                }
            }
        }

        private void OpenReadCallback (Object sender, OpenReadCompletedEventArgs eventData)
        {
            // HOW MAY I CHECK THE HTTP RESPONSE CODE? THIS SUCKS!!!
            // From this answer: http://stackoverflow.com/questions/3574659/how-to-get-status-code-from-webclient
            // When are we receiving WebException? When HTTP status code is not 200? I do not think so.
            // What if I just want to read data when HTTP OK? I am getting really angry.
            // The Mono implementation does not seem to throw WebException... so, the stack overflow answer must be wrong... FUUUUU
            // C# is a great language but the API sucks IMHO.

            var taskId = (Guid) eventData.UserState;

            if (eventData.Cancelled)
            {
                // Be careful!!! If you throw exception from this point your program will finish with "Unhandled Exception".
                Console.WriteLine ("Task Cancelled. Taks Id: {0}", taskId);
                return;
            }

            Exception errorException = eventData.Error;
            if (errorException != null)
            {
                // Be careful!!! If you throw exception from this point your program will finish with "Unhandled Exception".
                Console.WriteLine ("Task with Exception. Taks Id: {0}, Exception: {1}", taskId, errorException);
                return;
            }
                
            // Old fashined way (without using)
            // NOTE: using statement is the same as this code (it also checks for null value before calling
            // the Dispose method)
            Stream replyStream = null;
            StreamReader replyStreamReader = null;

            Console.WriteLine ("Taks Id: {0}", taskId);
            try
            {
                // WE MUST ALWAYS CLOSE THE STREAM RETURNED BY WebClient!!!!
                replyStream = (Stream) eventData.Result;
                replyStreamReader = new StreamReader (replyStream);
                Console.WriteLine (replyStreamReader.ReadToEnd ());
                //throw new Exception("My Exception from Async CallBack");
                // IF YOU SEE THE MONO IMPLEMENTATION OF OpenReadAsync YOU WILL SEE THERE IS A try/catch Exception
                // AND THAT CATCH EXCEPTION IS CALLING AGAIN MY OpenReadCallback. When calling OpenReadCallback in the catch
                // Exception block eventData.Error will not be null. So, my OpenReadCallback could be called more than once if
                // there are multiple Exceptions being thrown from my OpenReadCallback method. :/
                // BECAUSE OF THAT YOU MUST ALWAYS CHECK IN THE CALLBACK IMPLEMENTATION THE eventData.Cancelled AND eventData.Error VALUES!!!!
                // (As I've done here) 
            }
            // If there is any exception it will be caught later when checking the eventData.Error value.
            // SEE THE MONO IMPLEMENTATION OF OpenReadAsync.
            // catch(IOException e)
            // {
            //     Console.WriteLine ("OpenReadCallback, IOException: ", e);
            // }
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


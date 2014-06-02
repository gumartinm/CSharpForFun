using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Text;

namespace HttpClientsExamples
{
    /**
     * BE CAREFUL!!! If calling Dispose may throw exception, you could hide
     * exceptions being thrown inside the using blocks. If the Dispose methods
     * do not throw any exception there is no problem,
     * otherwise see: http://msdn.microsoft.com/en-us/library/aa355056%28v=vs.110%29.aspx
     */

    public class HttpWebRequestExample
    {
        public void Test()
        {
            Console.WriteLine("Synchronous HttpWebRequest");
            string uri = null;
            while (uri == null || uri.Length == 0)
            {
                Console.WriteLine("Specify the URI of the resource to retrieve.");
                Console.WriteLine("Write URI: ");
                uri = Console.ReadLine();
            }
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(uri);
            try {
                using(HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse())
                {
                    // May httpWebResponse be null? API says nothing (as usual...) API sucks.
                    if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using(Stream replyStream = httpWebResponse.GetResponseStream())
                        using(StreamReader replyStreamReader = new StreamReader(replyStream))
                        {
                            Console.WriteLine(replyStreamReader.ReadToEnd());
                            httpWebResponse.Dispose ();
                        }
                    }
                }

            }
            catch(ProtocolViolationException e) {
                Console.WriteLine ("Synchronous HttpWebRequest, ProtocolViolationException: ", e);
            }
            catch(NotSupportedException e) {
                Console.WriteLine ("Synchronous HttpWebRequest, NotSupportedException: ", e);
            }
            catch(WebException e) {
                Console.WriteLine ("Synchronous HttpWebRequest, WebException: ", e);
            }
            catch(IOException e) {
                Console.WriteLine ("Synchronous HttpWebRequest, IOException: ", e);
            }


            Console.WriteLine("Asynchronous HttpWebRequest");
            uri = null;
            while (uri == null || uri.Length == 0)
            {
                Console.WriteLine("Specify the URI of the resource to retrieve.");
                Console.WriteLine("Write URI: ");
                uri = Console.ReadLine();
            }
            httpWebRequest = (HttpWebRequest) WebRequest.Create(uri);
            Task<WebResponse> task = httpWebRequest.GetResponseAsync ();
            try {
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
                using(HttpWebResponse httpWebResponse = (HttpWebResponse) task.Result)
                {
                    // May httpWebResponse be null? API says nothing (as usual...) API sucks.
                    if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        try {
                            using(Stream replyStream = httpWebResponse.GetResponseStream())
                            using (StreamReader replyStreamReader = new StreamReader (replyStream))
                            {
                                string s = replyStreamReader.ReadToEnd ();
                                Console.WriteLine (s);
                            }
                        }
                        catch(ProtocolViolationException e) {
                            Console.WriteLine ("Asynchronous HttpWebRequest, ProtocolViolationException: ", e);
                        }
                        catch(IOException e) {
                            Console.WriteLine ("Asynchronous HttpWebRequest, IOException: ", e);
                        }
                    }
                }
            }
        }
    }
}


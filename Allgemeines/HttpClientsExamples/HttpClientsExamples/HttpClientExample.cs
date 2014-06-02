using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace HttpClientsExamples
{
    public class HttpClientExample
    {
        public void Test()
        {
            Console.WriteLine("HttpClient");
            string uri = null;
            while (uri == null || uri.Length == 0)
            {
                Console.WriteLine("Specify the URI of the resource to retrieve.");
                Console.WriteLine("Write URI: ");
                uri = Console.ReadLine();
            }
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds (5) })
            {
                Task<HttpResponseMessage> task =
                    client.GetAsync (uri, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
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
                    using(HttpResponseMessage httpWebResponse = task.Result)
                    {
                        // May httpWebResponse be null? API says nothing (as usual...) API sucks.
                        if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                        {
                            using(HttpContent content = httpWebResponse.Content)
                            {
                                // May content be null? API says nothing (as usual...) API sucks.
                                Task<Stream> taskStream = content.ReadAsStreamAsync ();
                                try {
                                    Task.WaitAll (taskStream);
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

                                if (taskStream.Status == TaskStatus.RanToCompletion)
                                {
                                    try {
                                        using(Stream replyStream = taskStream.Result)
                                        // May replyStream be null? API says nothing (as usual...) API sucks.
                                        using (StreamReader replyStreamReader = new StreamReader (replyStream))
                                        {
                                            string s = replyStreamReader.ReadToEnd ();
                                            Console.WriteLine (s);
                                        }
                                    }
                                    catch(IOException e) {
                                        Console.WriteLine ("HttpClient, IOException: ", e);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}


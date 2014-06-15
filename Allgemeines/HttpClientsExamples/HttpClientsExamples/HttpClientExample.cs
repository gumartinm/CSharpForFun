using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text;

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
                /**
                 * WHEN USING HttpCompletionOption.ResponseContentRead!!!
                 * The Mono implementation is using response.Content.LoadIntoBufferAsync. So, it seems as if it is going
                 * to read the full data and it will be stored in a buffer. So, when using HttpCompletionOption.ResponseContentRead
                 * (it is the default option for HttpClient) it does not matter if you want to use a stream instead of the full remote
                 * answer because you have already received the full data :/
                 * 
                 * SO, I GUESS IF YOU WANT TO SAVE MEMORY YOU SHOULD USE HttpCompletionOption.ResponseHeadersRead and
                 * content.ReadAsStreamAsync or client.GetStreamAsync (whatever with stream and HttpCompletionOption.ResponseHeadersRead option)
                 * 
                 * For example, in this implementation I am using the content.ReadAsStreamAsync method, this stream comes
                 * (I guess) from the buffer which already stored the full data. So even if you think you are
                 * saving memory because you are using a stream you are not. The stream does not come from the TCP connection but
                 * from the buffer (I guess)
                 */
                Task<HttpResponseMessage> task =
                    client.GetAsync (uri, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
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
                    using(HttpResponseMessage httpWebResponse = task.Result)
                    {
                        // May httpWebResponse be null? API says nothing (as usual...) API sucks.
                        // See GetStringAsync Mono implementation of HttpClient.cs, when using HttpCompletionOption.ResponseContentRead
                        // option I do not think httpWebResponse will be null.
                        httpWebResponse.EnsureSuccessStatusCode ();

                        using(HttpContent content = httpWebResponse.Content)
                        {
                            // May content be null? API says nothing (as usual...) API sucks.
                            // See GetStringAsync Mono implementation of HttpClient.cs, when using HttpCompletionOption.ResponseContentRead
                            // option I do not think content will be null.
                            Task<Stream> taskStream = content.ReadAsStreamAsync ();
                            try {
                                // DO NOT DO THIS. THERE COULD BE DEADLOCK. ALL DEPENDS ON THE SynchronizationContext
                                // How may I know what SynchronizationContext is going to be used? :/
                                // See: http://msdn.microsoft.com/en-us/magazine/gg598924.aspx
                                //      http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
                                //      http://stackoverflow.com/questions/22699048/why-does-task-waitall-not-block-or-cause-a-deadlock-here
                                // AFAIK, I SHOULD USE Task.WhenAll INSTEAD!!!! Because it creates a task (a new thread instead of blocking the current one)
                                // http://msdn.microsoft.com/en-us/library/system.threading.tasks.task.whenall%28v=vs.110%29.aspx
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
                                    // When using HttpCompletionOption.ResponseContentRead option I do not think replyStream will be null.
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


            Console.WriteLine("HttpClient with async statement");
            // No AggregateException when using async (you could retrieve AggregateException if you use Task.Exception but if you are
            // using async is because you want to forget about Task.Wait, Task.Result, etc, etc ,etc)
            // We receive only what would be the first exception in the AggregateException!!!
            // See: http://msmvps.com/blogs/jon_skeet/archive/2011/06/22/eduasync-part-11-more-sophisticated-but-lossy-exception-handling.aspx
            //      http://blogs.msdn.com/b/pfxteam/archive/2011/09/28/10217876.aspx
            uri = null;
            while (uri == null || uri.Length == 0)
            {
                Console.WriteLine("Specify the URI of the resource to retrieve.");
                Console.WriteLine("Write URI: ");
                uri = Console.ReadLine();
            }
            Task<string> taskHttpClient = this.DoGetAsync (uri);
            try {
                // DO NOT DO THIS. THERE COULD BE DEADLOCK. ALL DEPENDS ON THE SynchronizationContext
                // How may I know what SynchronizationContext is going to be used? :/
                // See: http://msdn.microsoft.com/en-us/magazine/gg598924.aspx
                //      http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
                //      http://stackoverflow.com/questions/22699048/why-does-task-waitall-not-block-or-cause-a-deadlock-here
                // AFAIK, I SHOULD USE Task.WhenAll INSTEAD!!!! Because it creates a task (a new thread instead of blocking the current one)
                // http://msdn.microsoft.com/en-us/library/system.threading.tasks.task.whenall%28v=vs.110%29.aspx
                Task.WaitAll (taskHttpClient);
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
            if (taskHttpClient.Status == TaskStatus.RanToCompletion)
            {
                string result = taskHttpClient.Result;
                Console.WriteLine (result);
            }


            Console.WriteLine("HttpClient GetStringAsync with async statement");
            // No AggregateException when using async (you could retrieve AggregateException if you use Task.Exception but if you are
            // using async is because you want to forget about Task.Wait, Task.Result, etc, etc ,etc)
            // We receive only what would be the first exception in the AggregateException!!!
            // See: http://msmvps.com/blogs/jon_skeet/archive/2011/06/22/eduasync-part-11-more-sophisticated-but-lossy-exception-handling.aspx
            //      http://blogs.msdn.com/b/pfxteam/archive/2011/09/28/10217876.aspx
            uri = null;
            while (uri == null || uri.Length == 0)
            {
                Console.WriteLine("Specify the URI of the resource to retrieve.");
                Console.WriteLine("Write URI: ");
                uri = Console.ReadLine();
            }
            taskHttpClient = this.DoGetStringAsync (uri);
            try {
                // DO NOT DO THIS. THERE COULD BE DEADLOCK. ALL DEPENDS ON THE SynchronizationContext
                // How may I know what SynchronizationContext is going to be used? :/
                // See: http://msdn.microsoft.com/en-us/magazine/gg598924.aspx
                //      http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
                //      http://stackoverflow.com/questions/22699048/why-does-task-waitall-not-block-or-cause-a-deadlock-here
                // AFAIK, I SHOULD USE Task.WhenAll INSTEAD!!!! Because it creates a task (a new thread instead of blocking the current one)
                // http://msdn.microsoft.com/en-us/library/system.threading.tasks.task.whenall%28v=vs.110%29.aspx
                Task.WaitAll (taskHttpClient);
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
            if (taskHttpClient.Status == TaskStatus.RanToCompletion)
            {
                string result = taskHttpClient.Result;
                Console.WriteLine (result);
            }


            // TODO: you must write another example with HttpCompletionOption.ResponseHeadersRead
        }

        private async Task<string> DoGetAsync(string uri)
        {
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds (5) })
            {
                /**
                 * WHEN USING HttpCompletionOption.ResponseContentRead!!!
                 * The Mono implementation is using response.Content.LoadIntoBufferAsync. So, it seems as if it is going
                 * to read the full data and it will be stored in a buffer. So, when using HttpCompletionOption.ResponseContentRead
                 * (it is the default option for HttpClient) it does not matter if you want to use a stream instead of the full remote
                 * answer because you have already received the full data :/
                 * 
                 * SO, I GUESS IF YOU WANT TO SAVE MEMORY YOU SHOULD USE HttpCompletionOption.ResponseHeadersRead and
                 * content.ReadAsStreamAsync or client.GetStreamAsync (whatever with stream and HttpCompletionOption.ResponseHeadersRead option)
                 * 
                 * For example, in this implementation I am using in my ReadResponseAsync the content.ReadAsStreamAsync method,
                 * this stream comes (I guess) from the buffer which already stored the full data. So even if you think you are
                 * saving memory because you are using a stream you are not. The stream does not come from the TCP connection but
                 * from the buffer (I guess)
                 */
                using (HttpResponseMessage httpWebResponse =
                    await client.GetAsync (uri, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    // May httpWebResponse be null? API says nothing (as usual...) API sucks.
                    // See GetStringAsync Mono implementation of HttpClient.cs, when using HttpCompletionOption.ResponseContentRead
                    // option I do not think httpWebResponse will be null.
                    httpWebResponse.EnsureSuccessStatusCode ();

                    using (HttpContent content = httpWebResponse.Content)
                    {
                        // May content be null? API says nothing (as usual...) API sucks.
                        // See GetStringAsync Mono implementation of HttpClient.cs, when using HttpCompletionOption.ResponseContentRead
                        // option I do not think content will be null.
                        return await ReadResponseAsync(content);
                    }
                }
            }
        }

        private async Task<string> ReadResponseAsync(HttpContent content)
        {
            /**
             * Taken from HttpContent.cs ReadAsStringAsync() Mono implementation.
             */
            Encoding encoding;
            if (content.Headers != null && content.Headers.ContentType != null && content.Headers.ContentType.CharSet != null) {
                encoding = Encoding.GetEncoding (content.Headers.ContentType.CharSet);
            } else {
                encoding = Encoding.UTF8;
            }

            using (Stream replyStream = await content.ReadAsStreamAsync ())
            // May replyStream be null? API says nothing (as usual...) API sucks.
            // When using HttpCompletionOption.ResponseContentRead option I do not think replyStream will be null.
            using (StreamReader replyStreamReader = new StreamReader (replyStream, encoding))
            {
                return await replyStreamReader.ReadToEndAsync();
            }
        }


        private async Task<string> DoGetStringAsync(string uri)
        {
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds (5) })
            {
                /**
                 * This is the best method (in my case). It makes many things for me (see its Mono implementation):
                 * 1. It checks for the HTTP return status code (it is using httpWebResponse.EnsureSuccessStatusCode)
                 * 2. It is using the remote received encoding to encode data.
                 * 
                 * So, I can rely on this method when I do not need to save memory by means of using stream and
                 * HttpCompletionOption.ResponseHeadersRead option. :)
                 */
                return await client.GetStringAsync (uri);
            }
        }
    }
}


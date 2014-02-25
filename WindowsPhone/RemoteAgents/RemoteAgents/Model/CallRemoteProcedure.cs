using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAgents.WindowsPhone.Model
{
    public class CallRemoteProcedure
    {

        async public Task<TResult> callRemoteService<TResult>(string uri, string method)
        {
            var postData = new POST();
            postData.id = "2114567586433855105";
            postData.jsonrpc = "2.0";
            postData.method = method;
            var jsonSettings = new JsonSerializerSettings
            {
                Error = delegate(object sender, ErrorEventArgs args)
                {
                    //TODO: logger for Windows Phone 8 :(
                    Console.WriteLine(args.ErrorContext.Error.Message);
                    args.ErrorContext.Handled = true;
                }
            };

            string data = JsonConvert.SerializeObject(postData, jsonSettings);
            HttpContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json-rpc");

            HttpResponseMessage response = await this.doCall(uri, content);

            TResult result = default(TResult);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Task<byte[]> responseBytes = response.Content.ReadAsByteArrayAsync();
                string responseString = Encoding.UTF8.GetString(responseBytes.Result, 0, responseBytes.Result.Length);
                POSTResult<TResult> postResult = JsonConvert.DeserializeObject<POSTResult<TResult>>(responseString, jsonSettings);
                result = postResult.result;
            }

            return result;
        }


        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <exception cref="System.InvalidOperationException">When some error.</exception>
        /// <returns>System.Threading.Tasks.Task<![CDATA[<TResult>]]>.The task object representing the asynchronous operation.</returns>
        async private Task<HttpResponseMessage> doCall(string uri, HttpContent content)
        {
            using (HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) })
            {
                return await client.PostAsync(uri, content);
            }
        }

        private class POST
        {
            public string id { get; set; }
            public string jsonrpc { get; set; }
            public string method { get; set; }
        }


        private class POSTResult<TResult>
        {
            public string id { get; set; }
            public string jsonrpc { get; set; }
            public TResult result { get; set; }
        }

        // TODO: When error I receive from JSONRPC: {"jsonrpc":"2.0","id":"null","error":{"code":-32600,"message":"Invalid Request"}}
        private class Error
        {
            public int code { get; set; }
            public string message { get; set; }
        }

        private class ERRORResult
        {
            public string jsonrpc { get; set; }
            public string id { get; set; }
            public Error error { get; set; }
        }
    }

}

using Newtonsoft.Json;
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

            string data = JsonConvert.SerializeObject(postData);
            HttpContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json-rpc");

            // TODO: find out why HttpClient is sendig two times the same POST. Is there something wrong with the .NET implementation?
            // At least it is doing that, when error response from web server.
            HttpResponseMessage response = await this.doCall(uri, content);

            TResult result = default(TResult);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Task<byte[]> responseBytes = response.Content.ReadAsByteArrayAsync();
                string responseString = Encoding.UTF8.GetString(responseBytes.Result, 0, responseBytes.Result.Length);
                POSTResult<TResult> postResult = JsonConvert.DeserializeObject<POSTResult<TResult>>(responseString);
                result = postResult.result;
            }

            return result;
        }


        async private Task<HttpResponseMessage> doCall(string uri, HttpContent content)
        {
            using (HttpClient client = new HttpClient())
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

using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Gtk;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Example.RemoteAgents.GTKLinux.Model
{
  public class CallRemoteProcedure
  {

    async public Task<TResult> callRemoteService<TResult>(string uri, string method)
    {
      TResult result = default(TResult);
      var postData = new POST();
      postData.id = "2114567586433855105";
      postData.jsonrpc = "2.0";
      postData.method = method;
      var jsonSettings = new JsonSerializerSettings{
        Error = delegate(object sender, ErrorEventArgs args)
        {
          Console.WriteLine(args.ErrorContext.Error.Message);
          args.ErrorContext.Handled = true;
        }
      };

      try {
        string data = JsonConvert.SerializeObject(postData, jsonSettings);
        HttpContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json-rpc");

        HttpResponseMessage response = await this.issueCall(uri, content);

          if (response.StatusCode == HttpStatusCode.OK) {
            Task<byte[]> responseBytes = response.Content.ReadAsByteArrayAsync();
            string responseString = System.Text.Encoding.UTF8.GetString(responseBytes.Result);

            POSTResult<TResult> postResult = JsonConvert.DeserializeObject<POSTResult<TResult>>(responseString, jsonSettings);
            result = postResult.result;
          }
      }
      catch (Exception e)
      {
          Console.WriteLine("callRemoteService exception. Message: {0}  Stacktrace: {1} ", e.Message, e.StackTrace);
      }

      return result;
    }

    async private Task<HttpResponseMessage> issueCall(string uri, HttpContent content)
    {
        using (HttpClient client = new HttpClient{ Timeout = TimeSpan.FromSeconds(5)})
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


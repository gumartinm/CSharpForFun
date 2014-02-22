using System;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.Remoting.Lifetime;
using System.Threading.Tasks;
using Gtk;
using System.Reflection;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Example.RemoteAgents.GTKLinux.Model
{
  public class CallRemoteProcedure
  {

    async public Task<TResult> callRemoteService<TResult>(string uri, string method)
    {
      var postData = new POST();
      postData.Id = "2114567586433855105";
      postData.JSONrpc = "2.0";
      postData.Method = method;

      string data = JsonConvert.SerializeObject(postData);
      HttpContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json-rpc");

      HttpResponseMessage response = await this.issueCall(uri, content);

      TResult result = default(TResult);

      if (response.StatusCode == HttpStatusCode.OK) {
        Task<byte[]> responseBytes = response.Content.ReadAsByteArrayAsync();
        string responseString = System.Text.Encoding.UTF8.GetString(responseBytes.Result);
        POSTResult<TResult> postResult = JsonConvert.DeserializeObject<POSTResult<TResult>>(responseString);
        result = postResult.Result;
      }

      return result;
    }


    async private Task<HttpResponseMessage> issueCall(string uri, HttpContent content)
    {
        using (HttpClient client = new HttpClient())
        {
            return await client.PostAsync(uri, content);
        }
    }

    private class POST
    {
      public string Id { get; set; }
      public string JSONrpc { get; set; }
      public string Method { get; set; }
    }
      

    private class POSTResult<TResult>
    {
      public string Id { get; set; }
      public string JSONrpc { get; set; }
      public TResult Result { get; set; }
    }
  }
}


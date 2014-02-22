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
      POSTResult<TResult> postResult;
      POST postData = new POST();
      postData.id = "2114567586433855105";
      postData.jsonrpc = "2.0";
      postData.method = method;

      string data = JsonConvert.SerializeObject(postData);
      HttpContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json-rpc");

      HttpResponseMessage response = await this.issueCall(uri, content);

      //if (response.StatusCode == HttpStatusCode.OK) {
        Task<byte[]> responseBytes = response.Content.ReadAsByteArrayAsync();
        string responseString = System.Text.Encoding.UTF8.GetString(responseBytes.Result);
        postResult = JsonConvert.DeserializeObject<POSTResult<TResult>>(responseString);
      //}

      return postResult.result;
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
  }
}


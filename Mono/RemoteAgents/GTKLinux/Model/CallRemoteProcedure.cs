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
using System.Threading;
using log4net;

namespace Example.RemoteAgents.GTKLinux.Model
{
  public class CallRemoteProcedure
  {
    private static readonly ILog logger = LogManager.GetLogger(
      System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    async public Task<TResult> PostRemoteServiceAsync<TResult>(string uri, string method)
    {
      POSTResult<TResult> postResult = await this.PostAsync<TResult>(uri, method, CancellationToken.None);

      return postResult.result;
    }

    async private Task<POSTResult<TResult>> PostAsync<TResult>(string uri, string method, CancellationToken cancellation)
    {
      var postData = new POST();
      postData.id = "2114567586433855105";
      postData.jsonrpc = "2.0";
      postData.method = method;
      var jsonSettings = new JsonSerializerSettings{
        Error = delegate(object sender, ErrorEventArgs args)
        {
          logger.Error(args.ErrorContext.Error.Message);
          args.ErrorContext.Handled = true;
        }
      };

      string data = JsonConvert.SerializeObject(postData, jsonSettings);

      // see: http://stackoverflow.com/questions/1329739/nested-using-statements-in-c-sharp
      // see: http://stackoverflow.com/questions/5895879/when-do-we-need-to-call-dispose-in-dot-net-c
      //TODO: Am I really sure I have to call the Dispose method of HttpContent content. In this case shouldn't it be stupid?
      // for HttpResponseMessage response I am sure I have to do it but I am not for HttpContent content.
      using (HttpContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json-rpc"))
      using (HttpResponseMessage response = await this.PostAsync(uri, content, cancellation))
      {
        POSTResult<TResult> postResult = new POSTResult<TResult>();

        if (response.StatusCode == HttpStatusCode.OK) {
          byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
          string responseString = System.Text.Encoding.UTF8.GetString(responseBytes);

          postResult = JsonConvert.DeserializeObject<POSTResult<TResult>>(responseString, jsonSettings);
        }

        return postResult;
      }
    }

    /// <summary>
    /// Send a POST request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <param name="uri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="System.Threading.CancellationToken">Cancellation token.</param>
    /// <exception cref="System.InvalidOperationException">When some error.</exception>
    /// <returns>System.Threading.Tasks.Task<![CDATA[<TResult>]]>.The task object representing the asynchronous operation.</returns>
    async private Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, CancellationToken cancellation)
    {
        using (HttpClient client = new HttpClient{ Timeout = TimeSpan.FromSeconds(5)})
        {
            return await client.PostAsync(uri, content, cancellation);
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


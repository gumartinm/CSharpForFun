﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace GumartinM.JsonRPC4Mono
{
  public class JsonRpcHttpAsyncClient
  {
    private long _nextId;

    /// <summary>
    /// The logger.
    /// </summary>
    private static readonly ILog logger = LogManager.GetLogger(
      System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// The _json settings.
    /// </summary>
    private readonly JsonSerializerSettings _jsonSettings = 
      new JsonSerializerSettings{
      Error = delegate(object sender, ErrorEventArgs args)
      {
        logger.Error(args.ErrorContext.Error.Message);
        args.ErrorContext.Handled = true;
      }
    };

    /// <summary>
    /// The _exception resolver.
    /// </summary>
    private readonly ExceptionResolver _exceptionResolver = new ExceptionResolver();



    /// <summary>
    /// Posts the remote service async.
    /// </summary>
    /// <returns>The remote service async.</returns>
    /// <param name="uri">URI.</param>
    /// <param name="method">Method.</param>
    /// <typeparam name="TResult">The 1st type parameter.</typeparam>
    async public Task<TResult> PostRemoteServiceAsync<TResult>(string uri, string method)
    {
      POSTResult<TResult> postResult = await this.PostAsync<TResult>(uri, method, CancellationToken.None);

      return postResult.result;
    }

    /// <summary>
    /// Posts the async.
    /// </summary>
    /// <returns>The async.</returns>
    /// <param name="uri">URI.</param>
    /// <param name="method">Method.</param>
    /// <param name="cancellation">Cancellation.</param>
    /// <typeparam name="TResult">The 1st type parameter.</typeparam>
    async private Task<POSTResult<TResult>> PostAsync<TResult>(string uri, string method, CancellationToken cancellation)
    {
      var postData = new POST();
      postData.id = Interlocked.Increment(ref _nextId).ToString();
      postData.jsonrpc = "2.0";
      postData.method = method;

      string data = JsonConvert.SerializeObject(postData, _jsonSettings);

      // see: http://stackoverflow.com/questions/1329739/nested-using-statements-in-c-sharp
      // see: http://stackoverflow.com/questions/5895879/when-do-we-need-to-call-dispose-in-dot-net-c
      //TODO: Am I really sure I have to call the Dispose method of HttpContent content? In this case, shouldn't it be stupid?
      // For HttpResponseMessage response I am sure I have to do it but I am not for HttpContent content.
      using (HttpContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json-rpc"))
      using (HttpResponseMessage response = await this.PostAsync(uri, content, cancellation))
      {

        if (response.StatusCode == HttpStatusCode.OK) {
          byte[] jsonBytes = await response.Content.ReadAsByteArrayAsync();

          return this.ReadResponse<TResult>(jsonBytes);
        }

        throw new Exception("Unexpected response code: " + response.StatusCode);
      }
    }

    /// <summary>
    /// Reads the response.
    /// </summary>
    /// <returns>The response.</returns>
    /// <param name="jsonBytes">Json bytes.</param>
    /// <typeparam name="TResult">The 1st type parameter.</typeparam>
    private POSTResult<TResult> ReadResponse<TResult>(byte[] jsonBytes)
    {
      POSTResult<TResult> postResult = new POSTResult<TResult>();

      string json = System.Text.Encoding.UTF8.GetString(jsonBytes);

      JObject jsonObjects = JObject.Parse(json);
      IDictionary<string, JToken> jsonTokens = jsonObjects;


      if (jsonTokens.ContainsKey("error"))
      {
        throw _exceptionResolver.ResolveException(jsonObjects["error"]);
      }

      if (jsonTokens.ContainsKey("result"))
      {
        postResult = JsonConvert.DeserializeObject<POSTResult<TResult>>(json, _jsonSettings);
      }

      return postResult;
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
        // TODO: cancellation
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
  }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace GumartinM.JsonRPC4NET
{
    public class JsonRpcHttpAsyncClient
    {
        /// <summary>
        /// RPC call id.
        /// </summary>
        private long _nextId;

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The _json settings.
        /// </summary>
        private readonly JsonSerializerSettings _jsonSettings =
          new JsonSerializerSettings
          {
            Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
              {
                  _logger.Error(args.ErrorContext.Error.Message);
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

                if (response.StatusCode == HttpStatusCode.OK)
                {
					//byte[] jsonBytes = await response.Content.ReadAsByteArrayAsync();
                    Stream stream = await response.Content.ReadAsStreamAsync();

                    //return this.ReadResponse<TResult>(jsonBytes);
                    return await this.ReadResponseAsync<TResult>(stream);

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
            string json = System.Text.Encoding.UTF8.GetString(jsonBytes, 0, jsonBytes.Length);

            return this.ReadResponse<TResult>(json);
        }

        /// <summary>
        /// Reads the response.
        /// </summary>
        /// <returns>The response.</returns>
        /// <param name="stream">Stream.</param>
        /// <typeparam name="TResult">The 1st type parameter.</typeparam>
        async private Task<POSTResult<TResult>> ReadResponseAsync<TResult>(Stream stream)
		{
			using (StreamReader streamReader = new StreamReader (stream, System.Text.Encoding.UTF8))
			{
                // This line makes this method useless (IMHO it is the same as the one working with bytes)
                // How could I work with JSON saving memory?
				string json = await streamReader.ReadToEndAsync();

                return this.ReadResponse<TResult>(json);
			}
		}

        private POSTResult<TResult> ReadResponse<TResult>(string json)
        {
            JObject jsonObjects = JObject.Parse(json);
            IDictionary<string, JToken> jsonTokens = jsonObjects;


            if (jsonTokens.ContainsKey("error"))
            {
                throw _exceptionResolver.ResolveException(jsonObjects["error"]);
            }

            if (jsonTokens.ContainsKey("result"))
            {
                return JsonConvert.DeserializeObject<POSTResult<TResult>>(json, _jsonSettings);
            }

            throw new JsonRpcClientException(0, "There is not error nor result in JSON response data.", jsonObjects);
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
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
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

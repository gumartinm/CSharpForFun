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
using System.Text;

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
        /// The JSON RPC version.
        /// </summary>
        private readonly string _JSON_RPC_VERSION = "2.0";



        /// <summary>
        /// Posts the with parameters remote service async.
        /// </summary>
        /// <returns>The with parameters remote service async.</returns>
        /// <param name="uri">URI.</param>
        /// <param name="method">Method.</param>
        /// <param name="arguments">Arguments.</param>
        /// <typeparam name="TResult">The 1st type parameter.</typeparam>
        async public Task<TResult> PostRemoteServiceAsync<TResult>(string uri, string method, params object[] arguments)
        {
            string jsonData;

            if (arguments != null)
            {
                var inputParameters = new List<object>(arguments);
                var postData = new POSTWithParameters();
                postData.id = Interlocked.Increment(ref _nextId).ToString();
                postData.jsonrpc = _JSON_RPC_VERSION;
                postData.method = method;
                postData.@params = inputParameters;
                jsonData = JsonConvert.SerializeObject(postData, _jsonSettings);
            } else
            {
                var postData = new POST();
                postData.id = Interlocked.Increment(ref _nextId).ToString();
                postData.jsonrpc = _JSON_RPC_VERSION;
                postData.method = method;
                jsonData = JsonConvert.SerializeObject(postData, _jsonSettings);
            }

            POSTResult<TResult> postResult = await this.PostAsync<TResult>(uri, method, jsonData, CancellationToken.None).ConfigureAwait(false);

            return postResult.result;
        }

        /// <summary>
        /// Posts the with parameters remote service async.
        /// </summary>
        /// <returns>The with parameters remote service async.</returns>
        /// <param name="uri">URI.</param>
        /// <param name="method">Method.</param>
        /// <param name="parameters">Parameters.</param>
        async public Task PostRemoteServiceAsync(string uri, string method, params object[] parameters)
        {
            await this.PostRemoteServiceAsync<object>(uri, method, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Posts the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="uri">URI.</param>
        /// <param name="method">Method.</param>
        /// <param name="cancellation">Cancellation.</param>
        /// <typeparam name="TResult">The 1st type parameter.</typeparam>
        async private Task<POSTResult<TResult>> PostAsync<TResult>(string uri, string method, string jsonData, CancellationToken cancellation)
        {
            // see: http://stackoverflow.com/questions/1329739/nested-using-statements-in-c-sharp
            // see: http://stackoverflow.com/questions/5895879/when-do-we-need-to-call-dispose-in-dot-net-c
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
            using (HttpContent contentPOST = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json-rpc"))
            // TODO: HttpCompletionOption, without it, by default, I am buffering the received data.
            // ConfigureAwait(false): This API will always return data in a different context (different thread) to the calling one.
            // In this way upper layers may decide what context to use for returning data (the calling context or a diferent one)
            using (HttpResponseMessage response = await client.PostAsync(uri, contentPOST, cancellation).ConfigureAwait(false))
            {
                //TODO: What if response is null? :(
                response.EnsureSuccessStatusCode();

                using (HttpContent contentRESULT = response.Content)
                {
                    //TODO: What if contentRESULT is null? :(
                    return await this.ReadResponseAsync<TResult>(contentRESULT).ConfigureAwait(false);
                }
            }
        }

        async private Task<POSTResult<TResult>> ReadResponseAsync<TResult>(HttpContent content)
        {
            /**
             * Taken from HttpContent.cs ReadAsStringAsync() Mono implementation.
             */
            Encoding encoding;
            if (content.Headers != null && content.Headers.ContentType != null && content.Headers.ContentType.CharSet != null)
            {
                encoding = Encoding.GetEncoding(content.Headers.ContentType.CharSet);
            }
            else
            {
                encoding = Encoding.UTF8;
            }

            // Option a) with bytes
            //byte[] jsonBytes = await contentRESULT.ReadAsByteArrayAsync();
            //return this.ReadResponse<TResult>(jsonBytes);

            // Option b) with stream
            using (Stream stream = await content.ReadAsStreamAsync().ConfigureAwait(false))
            using (StreamReader streamReader = new StreamReader(stream, encoding))
            {
                // This line makes this method useless (IMHO it is the same as the one working with bytes)
                // How could I work with JSON saving memory?
                string json = await streamReader.ReadToEndAsync().ConfigureAwait(false);

                return this.ReadResponse<TResult>(json);
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

        private class POST
        {
            public string id { get; set; }
            public string jsonrpc { get; set; }
            public string method { get; set; }
        }

        private class POSTWithParameters
        {
            public string id { get; set; }
            public string jsonrpc { get; set; }
            public string method { get; set; }
            public List<object> @params { get; set; }
        }

        private class POSTResult<TResult>
        {
            public string id { get; set; }
            public string jsonrpc { get; set; }
            public TResult result { get; set; }
        }
    }
}

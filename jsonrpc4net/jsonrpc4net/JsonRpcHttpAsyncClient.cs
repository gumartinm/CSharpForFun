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

            POSTResult<TResult> postResult = await this.PostAsync<TResult>(uri, method, jsonData, CancellationToken.None);

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
            await this.PostRemoteServiceAsync<object>(uri, method, parameters);
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
            //TODO: Am I really sure I have to call the Dispose method of HttpContent content? In this case, shouldn't it be stupid?
            // For HttpResponseMessage response I am sure I have to do it but I am not for HttpContent content.
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
            using (HttpContent contentPOST = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json-rpc"))
            using (HttpResponseMessage response = await client.PostAsync(uri, contentPOST, cancellation))
            //TODO: What if response is null? :(
            using (HttpContent contentRESULT = response.Content)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //TODO: What if contentRESULT is null? :(
                    return await this.ReadResponseAsync<TResult>(contentRESULT);
                }

                throw new Exception("Unexpected response code: " + response.StatusCode);
            }
        }

        async private Task<POSTResult<TResult>> ReadResponseAsync<TResult>(HttpContent content)
        {
            // Option a) with bytes
            //byte[] jsonBytes = await contentRESULT.ReadAsByteArrayAsync();
            //return this.ReadResponse<TResult>(jsonBytes);

            // Option b) with stream
            using (Stream stream = await content.ReadAsStreamAsync ())
            using (StreamReader streamReader = new StreamReader (stream, System.Text.Encoding.UTF8))
            {
                // This line makes this method useless (IMHO it is the same as the one working with bytes)
                // How could I work with JSON saving memory?
                string json = await streamReader.ReadToEndAsync();

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

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WeatherInformation.Model.Services
{
    class CustomHTTPClient
    {
        async public Task<string> GetWeatherDataAsync(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("Missing argument", "uri");
            }

            // TODO: it would be nice to use the same HttpClient for the the 2 requests instead of creating
            //       a new one for each connection. :( Not easy with using statement and async :(
            using(HttpClientHandler handler = new HttpClientHandler
            {
                // TODO: check if this really works when receiving compressed data.
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })
            using (HttpClient client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) })
            {
                HttpRequestHeaders headers = client.DefaultRequestHeaders;

                headers.UserAgent.Clear();
                headers.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("WeatherInformation", "WP8")));

                headers.AcceptCharset.Clear();
                headers.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));

                headers.Accept.Clear();
                headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                headers.AcceptEncoding.Clear();
                headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

                // Bypassing Windows cache
                string uriWindowsCacheSucks = String.Concat(uri, "&time=", DateTime.UtcNow.Ticks);

                // TODO: HttpCompletionOption, without it, by default, I am buffering the received data.
                //       in this case it is not a problem but when receiving loads of bytes I do not
                //       think it is a great idea to buffer all of them... :(
                using (HttpResponseMessage response = await client.GetAsync(uriWindowsCacheSucks).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();

                    using (HttpContent contentRESULT = response.Content)
                    {
                        return await this.ReadResponseAsync(contentRESULT).ConfigureAwait(false);
                    }
                }
            }
        }

        async private Task<string> ReadResponseAsync(HttpContent content)
        {
            Encoding encoding;
            if (content.Headers != null && content.Headers.ContentType != null && content.Headers.ContentType.CharSet != null)
            {
                encoding = Encoding.GetEncoding(content.Headers.ContentType.CharSet);
            }
            else
            {
                encoding = Encoding.UTF8;
            }

            using (Stream stream = await content.ReadAsStreamAsync().ConfigureAwait(false))
            using (StreamReader streamReader = new StreamReader(stream, encoding))
            {
                return await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }  
}

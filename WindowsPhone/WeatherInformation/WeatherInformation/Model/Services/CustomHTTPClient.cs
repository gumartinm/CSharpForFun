using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WeatherInformation.Model.Services
{
    class CustomHTTPClient
    {
        async public Task<string> GetWeatherDataAsync(string url)
        {
            
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
            {
                // TODO: I wish my string to be converted to UTF-8. WTH is doing HttpClient? Dunno :(
                // How do I control the encoding used by HttpClient?

                // Disable WindowsPhone cache
                HttpRequestHeaders headers = client.DefaultRequestHeaders;
                headers.IfModifiedSince = DateTime.UtcNow;

                // TODO: THIS IS FUCKED UP. IT IS WORKING RANDOMLY... THE MOST OF THE TIMES IT STOPS HERE FOREVER...
                string jsonData = await client.GetStringAsync(url);

                return jsonData;
            }
        } 
    }  
}

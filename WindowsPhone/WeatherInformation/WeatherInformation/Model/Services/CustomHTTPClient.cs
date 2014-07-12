using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                return await client.GetStringAsync(url);
            }
        } 
    }  
}

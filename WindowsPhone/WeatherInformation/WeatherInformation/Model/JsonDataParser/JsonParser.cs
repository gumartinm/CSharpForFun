using Newtonsoft.Json;
using WeatherInformation.Model.CurrentWeatherParser;
using WeatherInformation.Model.ForecastWeatherParser;
using System;

namespace WeatherInformation.Model.JsonDataParser
{
    class JsonParser
    {
        /// <summary>
        /// The _json settings.
        /// </summary>
        private static readonly JsonSerializerSettings _jsonSettings =
            new JsonSerializerSettings
            {
                Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                {
                    //Console.WriteLine(args.ErrorContext.Error.Message); No logs for WP8 :(
                    args.ErrorContext.Handled = true;
                }
            };

        public TWeatherData ParserWeatherData<TWeatherData>(String jsonData)
        {
            return JsonConvert.DeserializeObject<TWeatherData>(jsonData, _jsonSettings);
        }
    }
}

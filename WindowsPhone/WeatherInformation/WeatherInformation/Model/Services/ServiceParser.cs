using WeatherInformation.Model.CurrentWeatherParser;
using WeatherInformation.Model.ForecastWeatherParser;
using WeatherInformation.Model.JsonDataParser;
using System;

namespace WeatherInformation.Model.Services
{
    class ServiceParser
    {
        private readonly JsonParser _jsonParser;

        public ServiceParser(JsonParser jsonParser)
        {
            this._jsonParser = jsonParser;
        }

        public CurrentWeather GetCurrentWeather(String jsonData)
        {
            return this._jsonParser.ParserWeatherData<CurrentWeather>(jsonData);
        }

        public ForecastWeather GetForecastWeather(String jsonData)
        {
            return this._jsonParser.ParserWeatherData<ForecastWeather>(jsonData);
        }

        public WeatherData WeatherDataParser(
            string jsonForecast, string jsonCurrent, string city, string country)
        {
            if (string.IsNullOrEmpty(jsonForecast))
            {
                throw new ArgumentException("Missing argument", "JSONForecast");
            }
            if (string.IsNullOrEmpty(jsonCurrent))
            {
                throw new ArgumentException("Missing argument", "JSONCurrent");
            }

            var forecast = GetForecastWeather(jsonForecast);
            var current = GetCurrentWeather(jsonCurrent);
            return new WeatherData(forecast, jsonForecast, current, jsonCurrent, city, country);
        }
    }
}

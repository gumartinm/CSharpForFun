using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherInformation.Model.CurrentWeatherParser;
using WeatherInformation.Model.ForecastWeatherParser;

namespace WeatherInformation.Model
{
    public class WeatherData
    {
        private readonly ForecastWeather _forecast;
        private readonly string _jsonForecast;
        private readonly CurrentWeather _current;
        private readonly string _jsonCurrent;
        private readonly string _city;
        private readonly string _country;

        public WeatherData(ForecastWeather forecast, string jsonForecast, CurrentWeather current,
                           string jsonCurrent, string city, string country)
        {
            _forecast = forecast;
            _jsonForecast = jsonForecast;
            _current = current;
            _jsonCurrent = jsonCurrent;
            _city = city;
            _country = country;
        }

        public ForecastWeather Forecast { get { return _forecast; } }
        public string JSONForecast { get { return _jsonForecast; } }
        public CurrentWeather Current { get { return _current; } }
        public string JSONCurrent { get { return _jsonCurrent; } }
        public string City { get { return _city; } }
        public string Country { get { return _country; } }
    }
}

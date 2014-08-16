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
        public ForecastWeather RemoteForecast
        {
            get;
            set;
        }
        public string JSONRemoteForecast
        {
            get;
            set;
        }


        public CurrentWeather RemoteCurrent
        {
            get;
            set;
        }
        public string JSONRemoteCurrent
        {
            get;
            set;
        }

        public string City { get; set;}

        public string Country { get; set; }
    }
}

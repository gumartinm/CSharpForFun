using System;
using System.Collections.Generic;

namespace WeatherInformation.Model.ForecastWeatherParser
{
    public class ForecastWeather
    {
        public string cod { get; set; }
        public double message { get; set; }
        public City city { get; set; }
        public int cnt { get; set; }
        public List<WeatherInformation.Model.ForecastWeatherParser.List> list { get; set; }
    }
}


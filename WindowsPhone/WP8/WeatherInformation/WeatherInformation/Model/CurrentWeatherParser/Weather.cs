﻿
namespace WeatherInformation.Model.CurrentWeatherParser
{
    public class Weather
    {
        public long? id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }
}


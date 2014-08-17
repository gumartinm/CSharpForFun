using System;
using System.Collections.Generic;

namespace jsonparser.forecastweather
{
    public class ForecastWeather
    {
        public string cod { get; set; }
        public double? message { get; set; }
        public City city { get; set; }
        public int? cnt { get; set; }
        public List<jsonparser.forecastweather.List> list { get; set; }
    }
}


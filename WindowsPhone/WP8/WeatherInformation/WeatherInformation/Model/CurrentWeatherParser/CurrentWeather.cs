using System.Collections.Generic;

namespace WeatherInformation.Model.CurrentWeatherParser
{
    public class CurrentWeather
    {
        public Coord coord { get; set; }
        public Sys sys { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public Wind wind { get; set; }
        public Snow snow { get; set; }
        public Rain rain { get; set; }
        public Clouds clouds { get; set; }
        public long? dt { get; set; }
        public long? id { get; set; }
        public string name { get; set; }
        public long? cod { get; set; }
    }
}


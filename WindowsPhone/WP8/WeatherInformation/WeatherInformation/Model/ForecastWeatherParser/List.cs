using System.Collections.Generic;

namespace WeatherInformation.Model.ForecastWeatherParser
{
    public class List
    {
        public long? dt { get; set; }
        public Temp temp { get; set; }
        public double? pressure { get; set; }
        public double? humidity { get; set; }
        public List<Weather> weather { get; set; }
        public double? speed { get; set; }
        public double? deg { get; set; }
        public double? clouds { get; set; }
        public double? rain { get; set; }
    }
}


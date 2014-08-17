
namespace WeatherInformation.Model.ForecastWeatherParser
{
    public class City
    {
        public double? id { get; set; }
        public string name { get; set; }
        public Coord coord { get; set; }
        public string country { get; set; }
        public double? population { get; set; }
    }
}


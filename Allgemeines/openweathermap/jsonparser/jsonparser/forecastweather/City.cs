using System;

namespace jsonparser.forecastweather
{
    public class City
    {
        public long? id { get; set; }
        public string name { get; set; }
        public Coord coord { get; set; }
        public string country { get; set; }
        public long? population { get; set; }
    }
}


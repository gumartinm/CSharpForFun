using System;

namespace jsonparser.currentweather
{
    public class Sys
    {
        public double? message { get; set; }
        public string country { get; set; }
        public long? sunrise { get; set; }
        public long? sunset { get; set; }
    }
}


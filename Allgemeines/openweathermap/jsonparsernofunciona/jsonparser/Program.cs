using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using jsonparser.currentweather;

namespace jsonparser
{
    class MainClass
    {
        /// <summary>
        /// The _json settings.
        /// </summary>
        private static readonly JsonSerializerSettings _jsonSettings =
            new JsonSerializerSettings
        {
            Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
                Console.WriteLine(args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
            }
        };

        public static void Main(string[] args)
        {
            Console.WriteLine("LOL");
            string jsonCurrentWeatherData = @"{""coord"":{""lon"":-2.15,""lat"":57},""sys"":{""message"":0.0076,""country"":""GB"",""sunrise"":1398227897,""sunset"":1398281723},""weather"":[{""id"":741,""main"":""Fog"",""description"":""fog"",""icon"":""50n""}],""base"":""cmc stations"",""main"":{""temp"":281.15,""pressure"":1013,""humidity"":100,""temp_min"":281.15,""temp_max"":281.15},""wind"":{""speed"":3.6,""deg"":130},""rain"":{""3h"":0.5},""clouds"":{""all"":90},""dt"":1398223200,""id"":2636814,""name"":""Stonehaven"",""cod"":200}";


            CurrentWeather currentWeather = ReadResponse(jsonCurrentWeatherData);

        }


        private static CurrentWeather ReadResponse(string json)
        {
            return JsonConvert.DeserializeObject<CurrentWeather>(json, _jsonSettings);
        }

    }
}

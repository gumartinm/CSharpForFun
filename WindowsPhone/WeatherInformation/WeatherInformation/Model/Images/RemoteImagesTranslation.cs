using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherInformation.Model.Images
{
    class RemoteImagesTranslation
    {
        // C# Language Specification§10.11
        // If a class contains any static fields with initializers, those initializers are executed
        // in textual order immediately prior to executing the static constructor.
        private readonly static Dictionary<string, string> images = new Dictionary<string, string>();

        static RemoteImagesTranslation()
        {
            images.Add("01d", "weather_clear");
            images.Add("01n", "weather_clear_night");
            images.Add("02d", "weather_few_clouds");
            images.Add("02n", "weather_few_clouds_night");
            images.Add("03d", "weather_few_clouds");
            images.Add("03n", "weather_few_clouds");
            images.Add("04d", "weather_overcast");
            images.Add("04n", "weather_overcast");
            images.Add("09d", "weather_showers");
            images.Add("09n", "weather_showers");
            images.Add("10d", "weather_showers_scattered");
            images.Add("10n", "weather_showers_scattered");
            images.Add("11d", "weather_storm");
            images.Add("11n", "weather_storm");
            images.Add("13d", "weather_snow");
            images.Add("13n", "weather_snow");
            images.Add("50d", "weather_fog");
            images.Add("50n", "weather_fog");
        }

        public static string GetTransaltedImage(string id)
        {
            string value;
            images.TryGetValue(id, out value);

            return value;
        }
    }
}

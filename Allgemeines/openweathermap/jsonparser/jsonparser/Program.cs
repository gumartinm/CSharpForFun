using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using jsonparser.currentweather;
using jsonparser.forecastweather;

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

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            // Searching by geographic coordinates:
            // http://api.openweathermap.org/data/2.5/weather?lat=57&lon=-2.15&cnt=1
            string jsonCurrentWeatherData =
                "{" +
                "\"coord\":{\"lon\":-2.15,\"lat\":57}," +
                "\"sys\":{\"message\":0.0076,\"country\":\"GB\",\"sunrise\":1398227897,\"sunset\":1398281723}," +
                "\"weather\":[{\"id\":741,\"main\":\"Fog\",\"description\":\"fog\",\"icon\":\"50n\"}]," +
                "\"base\":\"cmc stations\"," +
                "\"main\":{\"temp\":281.15,\"pressure\":1013,\"humidity\":100,\"temp_min\":281.15,\"temp_max\":281.15}," +
                "\"wind\":{\"speed\":3.6,\"deg\":130}," +
                "\"rain\":{\"3h\":0.5}," +
                "\"clouds\":{\"all\":90}," +
                "\"dt\":1398223200," +
                "\"id\":2636814," +
                "\"name\":" +
                "\"Stonehaven\"," +
                "\"cod\":200}";

            // Getting daily forecast weather data: Searching 15 days forecast by
            // geographic coordinates at JSON format
            // http://api.openweathermap.org/data/2.5/forecast/daily?lat=57&lon=-2.15&cnt=15&mode=json
            // By default units metric (meters per second)
            // http://api.openweathermap.org/data/2.5/forecast/daily?lat=57&lon=-2.15&cnt=15&mode=json&units=imperial
            // Imperial: miles per hour
            // imperial value is multiplied by ~2.1605 which is close to conversion from m/s to mph (~2.236)
            // http://bugs.openweathermap.org/issues/111
            string jsonForeCastWeatherData =
                "{"
                + "\"cod\":\"200\","
                + "\"message\":0.0048,"
                + "\"city\":{\"id\":2641549,\"name\":\"Newtonhill\",\"coord\":{\"lon\":-2.15,\"lat\":57.033329},\"country\":\"GB\",\"population\":0},"
                + "\"cnt\":15,"
                + "\"list\":["
                + "{\"dt\":1397304000,\"temp\":{\"day\":286.15,\"min\":284.62,\"max\":286.15,\"night\":284.62,\"eve\":285.7,\"morn\":286.15},\"pressure\":1016.67,\"humidity\":84,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":7.68,\"deg\":252,\"clouds\":0,\"rain\":0.25},"
                + "{\"dt\":1397390400,\"temp\":{\"day\":284.92,\"min\":282.3,\"max\":284.92,\"night\":282.3,\"eve\":283.79,\"morn\":284.24},\"pressure\":1021.62,\"humidity\":84,\"weather\":[{\"id\":804,\"main\":\"Clouds\",\"description\":\"overcast clouds\",\"icon\":\"04d\"}],\"speed\":7.91,\"deg\":259,\"clouds\":92},"
                + "{\"dt\":1397476800,\"temp\":{\"day\":282.1,\"min\":280.32,\"max\":282.1,\"night\":280.32,\"eve\":281.51,\"morn\":281.65},\"pressure\":1033.84,\"humidity\":92,\"weather\":[{\"id\":801,\"main\":\"Clouds\",\"description\":\"few clouds\",\"icon\":\"02d\"}],\"speed\":8.37,\"deg\":324,\"clouds\":20},"
                + "{\"dt\":1397563200,\"temp\":{\"day\":280.73,\"min\":280.11,\"max\":281.4,\"night\":281.4,\"eve\":280.75,\"morn\":280.11},\"pressure\":1039.27,\"humidity\":97,\"weather\":[{\"id\":801,\"main\":\"Clouds\",\"description\":\"few clouds\",\"icon\":\"02d\"}],\"speed\":7.31,\"deg\":184,\"clouds\":12},"
                + "{\"dt\":1397649600,\"temp\":{\"day\":281.73,\"min\":281.03,\"max\":282.22,\"night\":281.69,\"eve\":282.22,\"morn\":281.03},\"pressure\":1036.05,\"humidity\":90,\"weather\":[{\"id\":803,\"main\":\"Clouds\",\"description\":\"broken clouds\",\"icon\":\"04d\"}],\"speed\":7.61,\"deg\":205,\"clouds\":68},"
                + "{\"dt\":1397736000,\"temp\":{\"day\":282.9,\"min\":281.45,\"max\":283.21,\"night\":282.71,\"eve\":283.06,\"morn\":281.49},\"pressure\":1029.39,\"humidity\":83,\"weather\":[{\"id\":803,\"main\":\"Clouds\",\"description\":\"broken clouds\",\"icon\":\"04d\"}],\"speed\":6.17,\"deg\":268,\"clouds\":56},"
                + "{\"dt\":1397822400,\"temp\":{\"day\":285.26,\"min\":281.55,\"max\":285.26,\"night\":282.48,\"eve\":285.09,\"morn\":281.55},\"pressure\":1025.83,\"humidity\":0,\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"sky is clear\",\"icon\":\"01d\"}],\"speed\":5.31,\"deg\":221,\"clouds\":10},"
                + "{\"dt\":1397908800,\"temp\":{\"day\":284.29,\"min\":281.5,\"max\":284.29,\"night\":282.53,\"eve\":283.58,\"morn\":281.5},\"pressure\":1024.55,\"humidity\":0,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":5.51,\"deg\":192,\"clouds\":6},"
                + "{\"dt\":1397995200,\"temp\":{\"day\":283.36,\"min\":281.62,\"max\":284.34,\"night\":284.04,\"eve\":284.34,\"morn\":281.62},\"pressure\":1019.58,\"humidity\":0,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":7.66,\"deg\":149,\"clouds\":0,\"rain\":0.48},"
                + "{\"dt\":1398081600,\"temp\":{\"day\":282.24,\"min\":280.51,\"max\":282.41,\"night\":280.51,\"eve\":282.41,\"morn\":280.9},\"pressure\":1027.35,\"humidity\":0,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":8.17,\"deg\":221,\"clouds\":10,\"rain\":0.94},"
                + "{\"dt\":1398168000,\"temp\":{\"day\":282.28,\"min\":279.76,\"max\":282.28,\"night\":280.69,\"eve\":281.13,\"morn\":279.76},\"pressure\":1038.31,\"humidity\":0,\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"sky is clear\",\"icon\":\"01d\"}],\"speed\":6.33,\"deg\":172,\"clouds\":1},"
                + "{\"dt\":1398254400,\"temp\":{\"day\":281.54,\"min\":280.52,\"max\":281.54,\"night\":281.44,\"eve\":281.23,\"morn\":280.52},\"pressure\":1022.4,\"humidity\":0,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":7.84,\"deg\":140,\"clouds\":91,\"rain\":1.24},"
                + "{\"dt\":1398340800,\"temp\":{\"day\":282.1,\"min\":280.66,\"max\":282.78,\"night\":280.97,\"eve\":282.78,\"morn\":280.66},\"pressure\":1013.39,\"humidity\":0,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":9.43,\"deg\":164,\"clouds\":98,\"rain\":1.03},"
                + "{\"dt\":1398427200,\"temp\":{\"day\":282.11,\"min\":280.72,\"max\":282.32,\"night\":282.32,\"eve\":280.99,\"morn\":280.72},\"pressure\":1018.65,\"humidity\":0,\"weather\":[{\"id\":502,\"main\":\"Rain\",\"description\":\"heavy intensity rain\",\"icon\":\"10d\"}],\"speed\":5.26,\"deg\":158,\"clouds\":83,\"rain\":14.4},"
                + "{\"dt\":1398513600,\"temp\":{\"day\":282.75,\"min\":280.61,\"max\":282.75,\"night\":280.61,\"eve\":281.75,\"morn\":281.96},\"pressure\":1007.4,\"humidity\":0,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":9.18,\"deg\":198,\"clouds\":35,\"rain\":0.55}"
                + "]}";

            CurrentWeather current = JsonConvert.DeserializeObject<CurrentWeather>(jsonCurrentWeatherData, _jsonSettings);

            Console.WriteLine("lat: " + current.coord.lat ?? "");
            Console.WriteLine("lon: " + current.coord.lon ?? "");
            foreach (jsonparser.currentweather.Weather weather in current.weather)
            {
                Console.WriteLine("description: " + weather.description ?? "");
                Console.WriteLine("main: " + weather.main ?? "");
                Console.WriteLine("id: " + weather.id ?? "");
                Console.WriteLine("icon: " + weather.icon ?? "");
            }
            Console.WriteLine("humidity: " + current.main.humidity ?? "");
            Console.WriteLine("pressure: " + current.main.pressure ?? "");
            Console.WriteLine("temp: " + current.main.temp ?? "");
            Console.WriteLine("temp_min: " + current.main.temp_min ?? "");
            Console.WriteLine("temp_max: " + current.main.temp_max ?? "");
            Console.WriteLine("rain: " + current.rain.get3h() ?? "");
            Console.WriteLine("clouds: " + current.clouds.all ?? "");

            Console.ReadLine();



            ForecastWeather forecast = JsonConvert.DeserializeObject<ForecastWeather>(jsonForeCastWeatherData, _jsonSettings);

            Console.WriteLine("lat: " + forecast.city.coord.lat ?? "");
            Console.WriteLine("lon: " + forecast.city.coord.lon ?? "");
            Console.WriteLine("name: " + forecast.city.name ?? "");

            foreach (jsonparser.forecastweather.List list in forecast.list)
            {
                Console.WriteLine("temp day: " + list.temp.day ?? "");
                Console.WriteLine("temp eve: " + list.temp.eve ?? "");
                Console.WriteLine("temp max: " + list.temp.max ?? "");
                Console.WriteLine("temp min: " + list.temp.min ?? "");
            }

            Console.ReadLine();
        }
    }
}

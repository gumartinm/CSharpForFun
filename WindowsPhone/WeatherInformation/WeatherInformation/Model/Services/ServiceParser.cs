﻿using WeatherInformation.Model.CurrentWeatherParser;
using WeatherInformation.Model.ForecastWeatherParser;
using WeatherInformation.Model.JsonDataParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherInformation.Model.Services
{
    class ServiceParser
    {
        private readonly JsonParser _jsonParser;

        public ServiceParser(JsonParser jsonParser)
        {
            this._jsonParser = jsonParser;
        }

        public CurrentWeather GetCurrentWeather(String jsonData)
        {
            return this._jsonParser.ParserWeatherData<CurrentWeather>(jsonData);
        }

        public ForecastWeather GetForecastWeather(String jsonData)
        {
            return this._jsonParser.ParserWeatherData<ForecastWeather>(jsonData);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherInformation.Model.CurrentWeatherParser;
using WeatherInformation.Model.ForecastWeatherParser;

namespace WeatherInformation.Model
{
    public class WeatherData
    {
        public ForecastWeather RemoteForecastWeatherData
        {
            get;
            set;
        }
        public string JSONRemoteForecastWeatherData
        {
            get;
            set;
        }


        public CurrentWeather RemoteCurrentWeatherData
        {
            get;
            set;
        }
        public string JSONRemoteCurrentWeatherData
        {
            get;
            set;
        }

        public bool WasThereRemoteError
        {
            get;
            set;
        }
    }
}

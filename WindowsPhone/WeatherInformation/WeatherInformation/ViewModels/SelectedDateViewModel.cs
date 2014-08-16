using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WeatherInformation.Model;
using WeatherInformation.Model.Images;
using WeatherInformation.Model.Services;
using WeatherInformation.Resources;

namespace WeatherInformation.ViewModels
{
    public class SelectedDateViewModel : INotifyPropertyChanged
    {

        [DataMember]
        public Int32 SelectedDateIndex { get; set; }

        public String TitleTextCityCountry { get; private set; }

        public String SelectedDate { get; private set; }
        public String SelectedDateWeatherImagePath { get; private set; }
        public String SelectedDateMaxTemp { get; private set; }
        public String SelectedDateMaxTempUnits { get; private set; }
        public String SelectedDateMinTemp { get; private set; }
        public String SelectedDateMinTempUnits { get; private set; }
        public String SelectedDateMorningTempText { get; private set; }
        public String SelectedDateMorningTemp { get; private set; }
        public String SelectedDateMorningTempUnits { get; private set; }
        public String SelectedDateDayTempText { get; private set; }
        public String SelectedDateDayTemp { get; private set; }
        public String SelectedDateDayTempUnits { get; private set; }
        public String SelectedDateEveningTempText { get; private set; }
        public String SelectedDateEveningTemp { get; private set; }
        public String SelectedDateEveningTempUnits { get; private set; }
        public String SelectedDateNightTempText { get; private set; }
        public String SelectedDateNightTemp { get; private set; }
        public String SelectedDateNightTempUnits { get; private set; }
        public String SelectedDateConditions { get; private set; }
        public String SelectedDateHumidityText { get; private set; }
        public String SelectedDateHumidity { get; private set; }
        public String SelectedDateHumidityUnits { get; private set; }
        public String SelectedDateRainText { get; private set; }
        public String SelectedDateRain { get; private set; }
        public String SelectedDateRainUnits { get; private set; }
        public String SelectedDateWindText { get; private set; }
        public String SelectedDateWind { get; private set; }
        public String SelectedDateWindUnits { get; private set; }
        public String SelectedDateCloudsText { get; private set; }
        public String SelectedDateClouds { get; private set; }
        public String SelectedDateCloudsUnits { get; private set; }
        public String SelectedDatePressureText { get; private set; }
        public String SelectedDatePressure { get; private set; }
        public String SelectedDatePressureUnits { get; private set; }

        public void LoadData(WeatherData weatherData)
        {
            var remoteForecastWeatherData = weatherData.RemoteForecast;

            WeatherInformation.Model.ForecastWeatherParser.List forecast = remoteForecastWeatherData.list[this.SelectedDateIndex];
            DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = unixTime.AddSeconds(forecast.dt).ToLocalTime();
            this.SelectedDate = date.ToString("m", CultureInfo.InvariantCulture);
            NotifyPropertyChanged("SelectedDate");

            // TODO: units :(
            bool isFahrenheit = false;
            double tempUnits = isFahrenheit ? 0 : 273.15;
            string symbol = isFahrenheit ? AppResources.TemperatureUnitsFahrenheitSymbol : AppResources.TemperatureUnitsCentigradeSymbol;

            string weatherImage;
            if (forecast.weather.Count > 0 &&
                forecast.weather[0].icon != null &&
                RemoteImagesTranslation.GetTransaltedImage(forecast.weather[0].icon) != null)
            {
                weatherImage = RemoteImagesTranslation.GetTransaltedImage(forecast.weather[0].icon);
            }
            else
            {
                weatherImage = "weather_severe_alert";
            }
            this.SelectedDateWeatherImagePath = String.Format(CultureInfo.InvariantCulture, "/Images/{0}.png", weatherImage);
            NotifyPropertyChanged("SelectedDateWeatherImagePath");

            var selectedDateMaxTemp = "";
            var selectedDateTempUnits = "";
            if (forecast.temp != null)
            {
                var conversion = forecast.temp.max;
                conversion -= tempUnits;
                selectedDateMaxTemp = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", conversion);
                selectedDateTempUnits = symbol;
            }
            this.SelectedDateMaxTemp = selectedDateMaxTemp;
            NotifyPropertyChanged("SelectedDateMaxTemp");
            this.SelectedDateMaxTempUnits = selectedDateTempUnits;
            NotifyPropertyChanged("SelectedDateMaxTempUnits");

            var selectedDateMinTemp = "";
            selectedDateTempUnits = "";
            if (forecast.temp != null)
            {
                var conversion = forecast.temp.min;
                conversion -= tempUnits;
                selectedDateMinTemp = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", conversion);
                selectedDateTempUnits = symbol;
            }
            this.SelectedDateMinTemp = selectedDateMinTemp;
            NotifyPropertyChanged("SelectedDateMinTemp");
            this.SelectedDateMaxTempUnits = selectedDateTempUnits;
            NotifyPropertyChanged("SelectedDateMinTempUnits");

            // TODO: static resource :(
            var selectedDateConditions = "no description available";
            if (forecast.weather.Count > 0)
            {
                selectedDateConditions = forecast.weather[0].description;
            }
            this.SelectedDateConditions = selectedDateConditions;
            NotifyPropertyChanged("SelectedDateConditions");

            // TODO: nullables para distinguir cuando hay datos o no. Ahora me llega 0 si no datos (supongo) cuando double/integer
            this.SelectedDateHumidityText = AppResources.MainPageCurrentHumidity;
            this.SelectedDateHumidity = forecast.humidity.ToString(CultureInfo.InvariantCulture);
            this.SelectedDateHumidityUnits = AppResources.MainPageCurrentHumidityUnits;
            NotifyPropertyChanged("SelectedDateHumidity");
            NotifyPropertyChanged("SelectedDateHumidityUnits");
            NotifyPropertyChanged("SelectedDateHumidityText");

            this.SelectedDateRainText = AppResources.MainPageCurrentRain;
            this.SelectedDateRain = forecast.rain.ToString(CultureInfo.InvariantCulture);
            this.SelectedDateRainUnits = AppResources.MainPageCurrentRainUnits;
            NotifyPropertyChanged("SelectedDateRain");
            NotifyPropertyChanged("SelectedDateRainUnits");
            NotifyPropertyChanged("SelectedDateRainText");

            this.SelectedDateWindText = AppResources.MainPageCurrentWind;
            this.SelectedDateWind = forecast.speed.ToString(CultureInfo.InvariantCulture);
            this.SelectedDateWindUnits = AppResources.MainPageCurrentWindUnits;
            NotifyPropertyChanged("SelectedDateWind");
            NotifyPropertyChanged("SelectedDateWindUnits");
            NotifyPropertyChanged("SelectedDateWindText");

            this.SelectedDateCloudsText = AppResources.MainPageCurrentClouds;
            this.SelectedDateClouds = forecast.clouds.ToString(CultureInfo.InvariantCulture);
            this.SelectedDateCloudsUnits = AppResources.MainPageCurrentCloudsUnits;
            NotifyPropertyChanged("SelectedDateClouds");
            NotifyPropertyChanged("SelectedDateCloudsUnits");
            NotifyPropertyChanged("SelectedDateCloudsText");

            this.SelectedDatePressureText = AppResources.MainPageCurrentPressure;
            this.SelectedDatePressure = forecast.pressure.ToString(CultureInfo.InvariantCulture);
            this.SelectedDatePressureUnits = AppResources.MainPageCurrentPressureUnits;
            NotifyPropertyChanged("SelectedDatePressure");
            NotifyPropertyChanged("SelectedDatePressureUnits");
            NotifyPropertyChanged("SelectedDatePressureText");

            var selectedDateMorningTemp = "";
            selectedDateTempUnits = "";
            if (forecast.temp != null)
            {
                var conversion = forecast.temp.morn;
                conversion -= tempUnits;
                selectedDateMorningTemp = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", conversion);
                selectedDateTempUnits = symbol;
            }
            this.SelectedDateMorningTempText = AppResources.SelectedDatePageMorning;
            NotifyPropertyChanged("SelectedDateMorningTempText");
            this.SelectedDateMorningTemp = selectedDateMorningTemp;
            NotifyPropertyChanged("SelectedDateMorningTemp");
            this.SelectedDateMorningTempUnits = selectedDateTempUnits;
            NotifyPropertyChanged("SelectedDateMorningTempUnits");

            var selectedDateDayTemp = "";
            selectedDateTempUnits = "";
            if (forecast.temp != null)
            {
                var conversion = forecast.temp.day;
                conversion -= tempUnits;
                selectedDateDayTemp = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", conversion);
                selectedDateTempUnits = symbol;
            }
            this.SelectedDateDayTempText = AppResources.SelectedDatePageDay;
            NotifyPropertyChanged("SelectedDateDayTempText");
            this.SelectedDateDayTemp = selectedDateDayTemp;
            NotifyPropertyChanged("SelectedDateDayTemp");
            this.SelectedDateDayTempUnits = selectedDateTempUnits;
            NotifyPropertyChanged("SelectedDateDayTempUnits");

            var selectedDateEveningTemp = "";
            selectedDateTempUnits = "";
            if (forecast.temp != null)
            {
                var conversion = forecast.temp.eve;
                conversion -= tempUnits;
                selectedDateEveningTemp = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", conversion);
                selectedDateTempUnits = symbol;
            }
            this.SelectedDateEveningTempText = AppResources.SelectedDatePageEvening;
            NotifyPropertyChanged("SelectedDateEveningTempText");
            this.SelectedDateEveningTemp = selectedDateEveningTemp;
            NotifyPropertyChanged("SelectedDateEveningTemp");
            this.SelectedDateEveningTempUnits = selectedDateTempUnits;
            NotifyPropertyChanged("SelectedDateEveningTempUnits");

            var selectedDateNightTemp = "";
            selectedDateTempUnits = "";
            if (forecast.temp != null)
            {
                var conversion = forecast.temp.night;
                conversion -= tempUnits;
                selectedDateNightTemp = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", conversion);
                selectedDateTempUnits = symbol;
            }
            this.SelectedDateNightTempText = AppResources.SelectedDatePageNight;
            NotifyPropertyChanged("SelectedDateNightTempText");
            this.SelectedDateNightTemp = selectedDateNightTemp;
            NotifyPropertyChanged("SelectedDateNightTemp");
            this.SelectedDateNightTempUnits = selectedDateTempUnits;
            NotifyPropertyChanged("SelectedDateNightTempUnits");

            string country = weatherData.Country;
            string city = weatherData.City;
            string cityCountry = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", city, country);
            this.TitleTextCityCountry = cityCountry;
            NotifyPropertyChanged("TitleTextCityCountry");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

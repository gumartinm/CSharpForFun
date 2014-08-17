using System;
using System.ComponentModel;
using System.Globalization;
using WeatherInformation.Model;
using WeatherInformation.Model.Images;
using WeatherInformation.Resources;

namespace WeatherInformation.ViewModels
{
    public class SelectedDateViewModel : INotifyPropertyChanged
    {
        private readonly SettingsViewModel _settings;

        public SelectedDateViewModel(SettingsViewModel settings)
        {
            _settings = settings;
        }

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

        public void LoadData(WeatherData weatherData, int index)
        {
            var remoteForecastWeatherData = weatherData.Forecast;

            WeatherInformation.Model.ForecastWeatherParser.List forecast = remoteForecastWeatherData.list[index];
            DateTime baseUnixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            string selectedDate = "";
            if (forecast.dt != null)
            {
                var date = baseUnixTime.AddSeconds(forecast.dt.Value).ToLocalTime();
                selectedDate = date.ToString("m", CultureInfo.InvariantCulture);
            }
            this.SelectedDate = selectedDate;
            NotifyPropertyChanged("SelectedDate");

            bool isFahrenheit = true;
            int temperatureUnitsSelection = _settings.TemperaruteUnitsSelectionSetting;
            if (temperatureUnitsSelection != 0)
            {
                isFahrenheit = false;
            }
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
            if (forecast.temp != null && forecast.temp.max != null)
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
            if (forecast.temp != null && forecast.temp.min != null)
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

            var selectedDateConditions = AppResources.SelectedDatePageDefaultDescription;
            if (forecast.weather.Count > 0 && !String.IsNullOrEmpty(forecast.weather[0].description))
            {
                selectedDateConditions = forecast.weather[0].description;
            }
            this.SelectedDateConditions = selectedDateConditions;
            NotifyPropertyChanged("SelectedDateConditions");

            this.SelectedDateHumidityText = AppResources.MainPageCurrentHumidity;
            var selectedDateHumidity = "";
            if (forecast.humidity != null)
            {
                selectedDateHumidity = forecast.humidity.Value.ToString(CultureInfo.InvariantCulture);
            }
            this.SelectedDateHumidity = selectedDateHumidity;
            this.SelectedDateHumidityUnits = AppResources.MainPageCurrentHumidityUnits;
            NotifyPropertyChanged("SelectedDateHumidity");
            NotifyPropertyChanged("SelectedDateHumidityUnits");
            NotifyPropertyChanged("SelectedDateHumidityText");

            this.SelectedDateRainText = AppResources.MainPageCurrentRain;
            var selectedDateRain = "";
            if (forecast.rain != null)
            {
                selectedDateRain = forecast.rain.Value.ToString(CultureInfo.InvariantCulture);
            }
            this.SelectedDateRain = selectedDateRain;
            this.SelectedDateRainUnits = AppResources.MainPageCurrentRainUnits;
            NotifyPropertyChanged("SelectedDateRain");
            NotifyPropertyChanged("SelectedDateRainUnits");
            NotifyPropertyChanged("SelectedDateRainText");

            this.SelectedDateWindText = AppResources.MainPageCurrentWind;
            var selectedDateSpeed = "";
            if (forecast.speed != null)
            {
                selectedDateSpeed = forecast.speed.Value.ToString(CultureInfo.InvariantCulture);
            }
            this.SelectedDateWind = selectedDateSpeed;
            this.SelectedDateWindUnits = AppResources.MainPageCurrentWindUnits;
            NotifyPropertyChanged("SelectedDateWind");
            NotifyPropertyChanged("SelectedDateWindUnits");
            NotifyPropertyChanged("SelectedDateWindText");

            this.SelectedDateCloudsText = AppResources.MainPageCurrentClouds;
            var selectedDateClouds = "";
            if (forecast.clouds != null)
            {
                selectedDateClouds = forecast.clouds.Value.ToString(CultureInfo.InvariantCulture);
            }
            this.SelectedDateClouds = selectedDateClouds;
            this.SelectedDateCloudsUnits = AppResources.MainPageCurrentCloudsUnits;
            NotifyPropertyChanged("SelectedDateClouds");
            NotifyPropertyChanged("SelectedDateCloudsUnits");
            NotifyPropertyChanged("SelectedDateCloudsText");

            this.SelectedDatePressureText = AppResources.MainPageCurrentPressure;
            var selectedDatePressure = "";
            if (forecast.pressure != null)
            {
                selectedDatePressure = forecast.pressure.Value.ToString(CultureInfo.InvariantCulture);
            }
            this.SelectedDatePressure = selectedDatePressure;
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

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using WeatherInformation.Model;
using WeatherInformation.Model.CurrentWeatherParser;
using WeatherInformation.Model.ForecastWeatherParser;
using WeatherInformation.Model.Images;
using WeatherInformation.Resources;

namespace WeatherInformation.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SettingsViewModel _settings;

        public MainViewModel(SettingsViewModel settings)
        {
            this.ForecastItems = new ObservableCollection<ItemViewModel>();
            this.CurrentItems = new ObservableCollection<ItemViewModel>();

            // Get the _settings for this application.
            _settings = settings;
        }

        /// <summary>
        /// Colección para objetos ItemViewModel.
        /// </summary>
        public ObservableCollection<ItemViewModel> ForecastItems{ get; private set; }
        public ObservableCollection<ItemViewModel> CurrentItems { get; private set; }
        public String TitleTextCityCountry { get; private set; }
        public String CurrentWeatherImagePath { get; private set; }
        public String CurrentMaxTemp { get; private set; }
        public String CurrentMaxTempUnits { get; private set; }
        public String CurrentMinTemp { get; private set; }
        public String CurrentMinTempUnits { get; private set; }
        public String CurrentConditions { get; private set; }
        public String CurrentFeelsLikeText { get; private set; }
        public String CurrentFeelsLikeTemp { get; private set; }
        public String CurrentFeelsLikeTempUnits { get; private set; }
        public String CurrentHumidityText { get; private set; }
        public String CurrentHumidity { get; private set; }
        public String CurrentHumidityUnits { get; private set; }
        public String CurrentRainText { get; private set; }
        public String CurrentRain { get; private set; }
        public String CurrentRainUnits { get; private set; }
        public String CurrentSnowText { get; private set; }
        public String CurrentSnow { get; private set; }
        public String CurrentSnowUnits { get; private set; }
        public String CurrentWindText { get; private set; }
        public String CurrentWind { get; private set; }
        public String CurrentWindUnits { get; private set; }
        public String CurrentCloudsText { get; private set; }
        public String CurrentClouds { get; private set; }
        public String CurrentCloudsUnits { get; private set; }
        public String CurrentPressureText { get; private set; }
        public String CurrentPressure { get; private set; }
        public String CurrentPressureUnits { get; private set; }
        public String CurrentSunRiseText { get; private set; }
        public String CurrentSunRise { get; private set; }
        public String CurrentSunSetText { get; private set; }
        public String CurrentSunSet { get; private set; }

        /// <summary>
        /// Update UI
        /// </summary>
        public void LoadData(WeatherData weatherData)
        {
            // TODO: there must be a better way than using the index value :(
            bool isFahrenheit = true;
            int temperatureUnitsSelection = _settings.TemperaruteUnitsSelectionSetting;
            if (temperatureUnitsSelection != 0)
            {
                isFahrenheit = false;
            }
            double tempUnits = isFahrenheit ? 0 : 273.15;
            string symbol = isFahrenheit ? AppResources.TemperatureUnitsFahrenheitSymbol : AppResources.TemperatureUnitsCentigradeSymbol;
            DateTime baseUnixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            string country = weatherData.Country;
            string city = weatherData.City;
            string cityCountry = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", city, country);
            this.TitleTextCityCountry = cityCountry;
            NotifyPropertyChanged("TitleTextCityCountry");

            if (weatherData.Forecast != null)
            {
                UpdateForecastUI(weatherData.Forecast, baseUnixTime, symbol, tempUnits);
            }

            if (weatherData.Current != null)
            {
                UpdateCurrentUI(weatherData.Current, baseUnixTime, symbol, tempUnits);
            }
        }

        private void UpdateForecastUI(ForecastWeather forecast, DateTime baseUnixTime, string symbol, double tempUnits)
        {
            // TODO: there must be a better way than using the index value :(
            int forecastDayNumbers = _settings.ForecastDayNumbersSelectionSetting;
            int count;
            switch (forecastDayNumbers)
            {
                case (0):
                    count = 5;
                    break;
                case (1):
                    count = 10;
                    break;
                case (2):
                    count = 14;
                    break;
                default:
                    count = 14;
                    break;
            }

            this.ForecastItems.Clear();

            foreach (WeatherInformation.Model.ForecastWeatherParser.List item in forecast.list)
            {
                var lineOne = "";
                var lineTwo = "";
                if (item.dt != null)
                {
                    var date = baseUnixTime.AddSeconds(item.dt.Value);
                    lineOne = date.ToString("ddd", CultureInfo.InvariantCulture);
                    lineTwo = date.ToString("dd", CultureInfo.InvariantCulture);
                }

                string lineThree = symbol;
                if (item.temp.max != null)
                {
                    var maxTemp = item.temp.max;
                    maxTemp = maxTemp - tempUnits;
                    lineThree = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", maxTemp) + symbol;
                }

                string lineFour = symbol;
                if (item.temp.min != null)
                {
                    var minTemp = item.temp.min;
                    minTemp = minTemp - tempUnits;
                    lineFour = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", minTemp) + symbol;
                }

                string lineFive;
                if (item.weather.Count > 0 &&
                    item.weather[0].icon != null &&
                    RemoteImagesTranslation.GetTransaltedImage(item.weather[0].icon) != null)
                {
                    lineFive = RemoteImagesTranslation.GetTransaltedImage(item.weather[0].icon);
                }
                else
                {
                    lineFive = "weather_severe_alert";
                }

                this.ForecastItems.Add(new ItemViewModel()
                {
                    LineOne = lineOne,
                    LineTwo = lineTwo,
                    LineThree = lineThree,
                    LineFour = lineFour,
                    LineFive = String.Format(CultureInfo.InvariantCulture, "/Images/{0}.png", lineFive),
                });

                count--;
                if (count == 0)
                {
                    break;
                }
            }
        }

        private void UpdateCurrentUI(CurrentWeather current, DateTime baseUnixTime, string symbol, double tempUnits)
        {
            string weatherImage;
            if (current.weather.Count > 0 &&
                current.weather[0].icon != null &&
                RemoteImagesTranslation.GetTransaltedImage(current.weather[0].icon) != null)
            {
                weatherImage = RemoteImagesTranslation.GetTransaltedImage(current.weather[0].icon);
            }
            else
            {
                weatherImage = "weather_severe_alert";
            }
            this.CurrentWeatherImagePath = String.Format(CultureInfo.InvariantCulture, "/Images/{0}.png", weatherImage);
            NotifyPropertyChanged("CurrentWeatherImagePath");

            var currentMaxTemp = "";
            if (current.main != null && current.main.temp_max != null)
            {
                var conversion = current.main.temp_max;
                conversion -= tempUnits;
                currentMaxTemp = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", conversion);
            }
            this.CurrentMaxTemp = currentMaxTemp;
            this.CurrentMaxTempUnits = symbol;
            NotifyPropertyChanged("CurrentMaxTemp");
            NotifyPropertyChanged("CurrentMaxTempUnits");

            var currentMinTemp = "";
            if (current.main != null && current.main.temp_min != null)
            {
                var conversion = current.main.temp_min;
                conversion -= tempUnits;
                currentMinTemp = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", conversion);
            }
            this.CurrentMinTemp = currentMinTemp;
            this.CurrentMinTempUnits = symbol;
            NotifyPropertyChanged("CurrentMinTemp");
            NotifyPropertyChanged("CurrentMinTempUnits");

            var currentConditions = AppResources.MainPageCurrentDefaultDescription;
            if (current.weather.Count > 0 && !String.IsNullOrEmpty(current.weather[0].description))
            {
                currentConditions = current.weather[0].description;
            }
            this.CurrentConditions = currentConditions;
            NotifyPropertyChanged("CurrentConditions");

            this.CurrentFeelsLikeText = AppResources.MainPageCurrentFeelsLike;
            var currentFeelsLikeTemp = "";
            if (current.main != null && current.main.temp != null)
            {
                var conversion = current.main.temp;
                conversion -= tempUnits;
                currentFeelsLikeTemp = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", conversion);
            }
            this.CurrentFeelsLikeTemp = currentFeelsLikeTemp;
            this.CurrentFeelsLikeTempUnits = symbol;
            NotifyPropertyChanged("CurrentFeelsLikeTemp");
            NotifyPropertyChanged("CurrentFeelsLikeTempUnits");
            NotifyPropertyChanged("CurrentFeelsLikeText");

            this.CurrentHumidityText = AppResources.MainPageCurrentHumidity;
            var currentHumidity = "";
            if (current.main != null && current.main.humidity != null)
            {
                currentHumidity = current.main.humidity.Value.ToString(CultureInfo.InvariantCulture);
            }
            this.CurrentHumidity = currentHumidity;
            this.CurrentHumidityUnits = AppResources.MainPageCurrentHumidityUnits;
            NotifyPropertyChanged("CurrentHumidity");
            NotifyPropertyChanged("CurrentHumidityUnits");
            NotifyPropertyChanged("CurrentHumidityText");

            this.CurrentRainText = AppResources.MainPageCurrentRain;
            var currentRain = "";
            if (current.rain != null && current.rain.get3h() != null)
            {
                currentRain = current.rain.get3h().Value.ToString(CultureInfo.InvariantCulture);
            }
            this.CurrentRain = currentRain;
            this.CurrentRainUnits = AppResources.MainPageCurrentRainUnits;
            NotifyPropertyChanged("CurrentRain");
            NotifyPropertyChanged("CurrentRainUnits");
            NotifyPropertyChanged("CurrentRainText");

            this.CurrentSnowText = AppResources.MainPageCurrentSnow;
            var currentSnow = "";
            if (current.snow != null && current.snow.get3h() != null)
            {
                currentSnow = current.snow.get3h().Value.ToString(CultureInfo.InvariantCulture);
            }
            this.CurrentSnow = currentSnow;
            this.CurrentSnowUnits = AppResources.MainPageCurrentSnowUnits;
            NotifyPropertyChanged("CurrentSnow");
            NotifyPropertyChanged("CurrentSnowUnits");
            NotifyPropertyChanged("CurrentSnowText");

            this.CurrentWindText = AppResources.MainPageCurrentWind;
            var currentWind = "";
            if (current.wind != null && current.wind.speed != null)
            {
                currentWind = current.wind.speed.Value.ToString(CultureInfo.InvariantCulture);
            }
            this.CurrentWind = currentWind;
            this.CurrentWindUnits = AppResources.MainPageCurrentWindUnits;
            NotifyPropertyChanged("CurrentWind");
            NotifyPropertyChanged("CurrentWindUnits");
            NotifyPropertyChanged("CurrentWindText");

            this.CurrentCloudsText = AppResources.MainPageCurrentClouds;
            var currentClouds = "";
            if (current.clouds != null && current.clouds.all != null)
            {
                currentClouds = current.clouds.all.Value.ToString(CultureInfo.InvariantCulture);
            }
            this.CurrentClouds = currentClouds;
            this.CurrentCloudsUnits = AppResources.MainPageCurrentCloudsUnits;
            NotifyPropertyChanged("CurrentClouds");
            NotifyPropertyChanged("CurrentCloudsUnits");
            NotifyPropertyChanged("CurrentCloudsText");

            this.CurrentPressureText = AppResources.MainPageCurrentPressure;
            var currentPressure = "";
            if (current.main != null && current.main.pressure != null)
            {
                currentPressure = current.main.pressure.Value.ToString(CultureInfo.InvariantCulture);
            }
            this.CurrentPressure = currentPressure;
            this.CurrentPressureUnits = AppResources.MainPageCurrentPressureUnits;
            NotifyPropertyChanged("CurrentPressure");
            NotifyPropertyChanged("CurrentPressureUnits");
            NotifyPropertyChanged("CurrentPressureText");

            this.CurrentSunRiseText = AppResources.MainPageCurrentSunRise;
            string sunRise = "";
            if (current.sys.sunrise != null)
            {
                var sunRiseTime = baseUnixTime.AddSeconds(current.sys.sunrise.Value);
                sunRise = sunRiseTime.ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture);
            }
            this.CurrentSunRise = sunRise;
            NotifyPropertyChanged("CurrentSunRise");
            NotifyPropertyChanged("CurrentSunRiseText");

            this.CurrentSunSetText = AppResources.MainPageCurrentSunSet;
            string sunSet = "";
            if (current.sys.sunset != null)
            {
                var sunSetTime = baseUnixTime.AddSeconds(current.sys.sunset.Value);
                sunSet = sunSetTime.ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture);
            }

            this.CurrentSunSet = sunSet;
            NotifyPropertyChanged("CurrentSunSet");
            NotifyPropertyChanged("CurrentSunSetText");
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
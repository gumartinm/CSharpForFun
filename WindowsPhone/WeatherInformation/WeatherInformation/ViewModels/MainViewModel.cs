using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO.IsolatedStorage;
using WeatherInformation.Model;
using WeatherInformation.Model.Images;
using WeatherInformation.Resources;

namespace WeatherInformation.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // The key names of _settings
        // TODO: reuse settings object instead of using the same code here again...
        private const string _temperatureUnitsSelectionSettingKeyName = "TemperatureUnitsSelection";
        private const string _forecastDayNumbersSelectionSelectionSettingKeyName = "ForecastDayNumbersSelection";

        // The default value of _settings
        // TODO: reuse settings object instead of using the same code here again...
        private const int _temperatureUnitsSelectionSettingDefault = 0;
        private const int _forecastDayNumbersSelectionSettingDefault = 0;

        // Settings
        private readonly IsolatedStorageSettings _settings;

        public MainViewModel()
        {
            this.ForecastItems = new ObservableCollection<ItemViewModel>();
            this.CurrentItems = new ObservableCollection<ItemViewModel>();

            // Get the _settings for this application.
            _settings = IsolatedStorageSettings.ApplicationSettings;
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
        /// Crear y agregar unos pocos objetos ItemViewModel a la colección Items.
        /// </summary>
        public void LoadData(WeatherData weatherData)
        {
            // TODO: there must be a better way than using the index value :(
            int forecastDayNumbers =
                    GetValueOrDefault<int>(_forecastDayNumbersSelectionSelectionSettingKeyName, _forecastDayNumbersSelectionSettingDefault);
            int count;
            switch(forecastDayNumbers)
            {
                case(0):
                    count = 5;
                    break;
                case(1):
                    count = 10;
                    break;
                case(2):
                    count = 14;
                    break;
                default:
                    count = 14;
                    break;
            }

            // TODO: there must be a better way than using the index value :(
            bool isFahrenheit = true;
            int temperatureUnitsSelection =
                GetValueOrDefault<int>(_temperatureUnitsSelectionSettingKeyName, _temperatureUnitsSelectionSettingDefault);
            if (temperatureUnitsSelection != 0)
            {
                isFahrenheit = false;
            }
            double tempUnits = isFahrenheit ? 0 : 273.15;
            string symbol = isFahrenheit ? AppResources.TemperatureUnitsFahrenheitSymbol : AppResources.TemperatureUnitsCentigradeSymbol;
            DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var remoteForecastWeatherData = weatherData.RemoteForecast;
            if (remoteForecastWeatherData != null)
            {
                this.ForecastItems.Clear();
                
                foreach (WeatherInformation.Model.ForecastWeatherParser.List item in remoteForecastWeatherData.list)
                {
                    DateTime date = unixTime.AddSeconds(item.dt);

                    // TODO: if I do not receive max temp or min temp... Am I going to receive item.temp.max=0 or item.temp.min=0 (I guess because
                    // double has no null value)
                    // I need something that is not 0 value in order to find out if OpenWeatherMap sent me values or not :/
                    // I guess; I am going to need nullable but I will have to modify my Json Parser :(
                    double maxTemp = item.temp.max;
                    maxTemp = maxTemp - tempUnits;

                    double minTemp = item.temp.min;
                    minTemp = minTemp - tempUnits;

                    string weatherImage;
                    if (item.weather.Count > 0 &&
                        item.weather[0].icon != null &&
                        RemoteImagesTranslation.GetTransaltedImage(item.weather[0].icon) != null)
                    {
                        weatherImage = RemoteImagesTranslation.GetTransaltedImage(item.weather[0].icon);
                    }
                    else
                    {
                        weatherImage = "weather_severe_alert";
                    }
                    string weatherImagePath = String.Format(CultureInfo.InvariantCulture, "/Images/{0}.png", weatherImage);

                    this.ForecastItems.Add(new ItemViewModel()
                    {
                        LineOne = date.ToString("ddd", CultureInfo.InvariantCulture),
                        LineTwo = date.ToString("dd", CultureInfo.InvariantCulture),
                        LineThree = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", maxTemp) + symbol,
                        LineFour = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", minTemp) + symbol,
                        LineFive = weatherImagePath
                    });

                    count--;
                    if (count == 0)
                    {
                        break;
                    }
                }
            }

            // TODO: nullables?
            // TODO: nullables para distinguir cuando hay datos o no. Ahora me llega 0 si no datos (supongo) cuando double/integer

            var remoteCurrentWeatherData = weatherData.RemoteCurrent;
            if (remoteCurrentWeatherData != null)
            {
                string weatherImage;
                if (remoteCurrentWeatherData.weather.Count > 0 &&
                    remoteCurrentWeatherData.weather[0].icon != null &&
                    RemoteImagesTranslation.GetTransaltedImage(remoteCurrentWeatherData.weather[0].icon) != null)
                {
                    weatherImage = RemoteImagesTranslation.GetTransaltedImage(remoteCurrentWeatherData.weather[0].icon);
                }
                else
                {
                    weatherImage = "weather_severe_alert";
                }
                this.CurrentWeatherImagePath = String.Format(CultureInfo.InvariantCulture, "/Images/{0}.png", weatherImage);
                NotifyPropertyChanged("CurrentWeatherImagePath");

                var currentMaxTemp = "";
                if (remoteCurrentWeatherData.main != null)
                {
                    var conversion = remoteCurrentWeatherData.main.temp_max;
                    conversion -= tempUnits;
                    currentMaxTemp = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", conversion);
                }
                this.CurrentMaxTemp = currentMaxTemp;
                this.CurrentMaxTempUnits = symbol;
                NotifyPropertyChanged("CurrentMaxTemp");
                NotifyPropertyChanged("CurrentMaxTempUnits");

                var currentMinTemp = "";
                if (remoteCurrentWeatherData.main != null)
                {
                    var conversion = remoteCurrentWeatherData.main.temp_min;
                    conversion -= tempUnits;
                    currentMinTemp = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", conversion);
                }
                this.CurrentMinTemp = currentMinTemp;
                this.CurrentMinTempUnits = symbol;
                NotifyPropertyChanged("CurrentMinTemp");
                NotifyPropertyChanged("CurrentMinTempUnits");

                // TODO: static resource :(
                var currentConditions = "no description available";
                if (remoteCurrentWeatherData.weather.Count > 0)
                {
                    currentConditions = remoteCurrentWeatherData.weather[0].description;
                }
                this.CurrentConditions = currentConditions;
                NotifyPropertyChanged("CurrentConditions");

                this.CurrentFeelsLikeText = AppResources.MainPageCurrentFeelsLike;
                var currentFeelsLikeTemp = "";
                if (remoteCurrentWeatherData.main != null)
                {
                    var conversion = remoteCurrentWeatherData.main.temp;
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
                if (remoteCurrentWeatherData.main != null)
                {
                    currentHumidity = remoteCurrentWeatherData.main.humidity.ToString(CultureInfo.InvariantCulture);
                }
                this.CurrentHumidity = currentHumidity;
                this.CurrentHumidityUnits = AppResources.MainPageCurrentHumidityUnits;
                NotifyPropertyChanged("CurrentHumidity");
                NotifyPropertyChanged("CurrentHumidityUnits");
                NotifyPropertyChanged("CurrentHumidityText");

                this.CurrentRainText = AppResources.MainPageCurrentRain;
                var currentRain = "";
                if (remoteCurrentWeatherData.rain != null)
                {
                    currentRain = remoteCurrentWeatherData.rain.get3h().ToString(CultureInfo.InvariantCulture);
                }
                this.CurrentRain = currentRain;
                this.CurrentRainUnits = AppResources.MainPageCurrentRainUnits;
                NotifyPropertyChanged("CurrentRain");
                NotifyPropertyChanged("CurrentRainUnits");
                NotifyPropertyChanged("CurrentRainText");

                this.CurrentSnowText = AppResources.MainPageCurrentSnow;
                var currentSnow = "";
                if (remoteCurrentWeatherData.snow != null)
                {
                    currentSnow = remoteCurrentWeatherData.snow.get3h().ToString(CultureInfo.InvariantCulture);
                }
                this.CurrentSnow = currentSnow;
                this.CurrentSnowUnits = AppResources.MainPageCurrentSnowUnits;
                NotifyPropertyChanged("CurrentSnow");
                NotifyPropertyChanged("CurrentSnowUnits");
                NotifyPropertyChanged("CurrentSnowText");

                this.CurrentWindText = AppResources.MainPageCurrentWind;
                var currentWind = "";
                if (remoteCurrentWeatherData.wind != null)
                {
                    currentWind = remoteCurrentWeatherData.wind.speed.ToString(CultureInfo.InvariantCulture);
                }
                this.CurrentWind = currentWind;
                this.CurrentWindUnits = AppResources.MainPageCurrentWindUnits;
                NotifyPropertyChanged("CurrentWind");
                NotifyPropertyChanged("CurrentWindUnits");
                NotifyPropertyChanged("CurrentWindText");

                this.CurrentCloudsText = AppResources.MainPageCurrentClouds;
                var currentClouds = "";
                if (remoteCurrentWeatherData.clouds != null)
                {
                    currentClouds = remoteCurrentWeatherData.clouds.all.ToString(CultureInfo.InvariantCulture);
                }
                this.CurrentClouds = currentClouds;
                this.CurrentCloudsUnits = AppResources.MainPageCurrentCloudsUnits;
                NotifyPropertyChanged("CurrentClouds");
                NotifyPropertyChanged("CurrentCloudsUnits");
                NotifyPropertyChanged("CurrentCloudsText");

                this.CurrentPressureText = AppResources.MainPageCurrentPressure;
                var currentPressure = "";
                if (remoteCurrentWeatherData.main != null)
                {
                    currentPressure = remoteCurrentWeatherData.main.pressure.ToString(CultureInfo.InvariantCulture);
                }
                this.CurrentPressure = currentPressure;
                this.CurrentPressureUnits = AppResources.MainPageCurrentPressureUnits;
                NotifyPropertyChanged("CurrentPressure");
                NotifyPropertyChanged("CurrentPressureUnits");
                NotifyPropertyChanged("CurrentPressureText");

                this.CurrentSunRiseText = AppResources.MainPageCurrentSunRise;
                var sunRiseTime = unixTime.AddSeconds(remoteCurrentWeatherData.sys.sunrise);
                this.CurrentSunRise = sunRiseTime.ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture);
                NotifyPropertyChanged("CurrentSunRise");
                NotifyPropertyChanged("CurrentSunRiseText");

                this.CurrentSunSetText = AppResources.MainPageCurrentSunSet;
                var sunSetTime = unixTime.AddSeconds(remoteCurrentWeatherData.sys.sunset);
                this.CurrentSunSet = sunSetTime.ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture);
                NotifyPropertyChanged("CurrentSunSet");
                NotifyPropertyChanged("CurrentSunSetText");

                string country = weatherData.Country;
                string city = weatherData.City;
                string cityCountry = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", city, country);
                this.TitleTextCityCountry = cityCountry;
                NotifyPropertyChanged("TitleTextCityCountry");
            }

        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // You need to add a check to DesignerProperties.IsInDesignTool to that code since accessing
            // IsolatedStorageSettings in Visual Studio or Expression Blend is invalid.
            // see: http://stackoverflow.com/questions/7294461/unable-to-determine-application-identity-of-the-caller
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                return defaultValue;
            }

            // If the key exists, retrieve the value.
            if (_settings.Contains(Key))
            {
                value = (T)_settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
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
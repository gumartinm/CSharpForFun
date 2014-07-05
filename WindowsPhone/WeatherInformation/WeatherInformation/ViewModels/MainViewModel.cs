using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows;
using WeatherInformation.Model.ForecastWeatherParser;
using WeatherInformation.Model.JsonDataParser;
using WeatherInformation.Model.Services;
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
        private readonly ServiceParser _serviceParser;

        public MainViewModel()
        {
            this.ForecastItems = new ObservableCollection<ItemViewModel>();
            this.CurrentItems = new ObservableCollection<ItemViewModel>();
            this._serviceParser = new ServiceParser(new JsonParser());

            // Get the _settings for this application.
            _settings = IsolatedStorageSettings.ApplicationSettings;
        }

        /// <summary>
        /// Colección para objetos ItemViewModel.
        /// </summary>
        public ObservableCollection<ItemViewModel> ForecastItems { get; private set; }
        public ObservableCollection<ItemViewModel> CurrentItems { get; private set; }

        

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Crear y agregar unos pocos objetos ItemViewModel a la colección Items.
        /// </summary>
        async public Task LoadData()
        {
            if (!_settings.Contains("CurrentLatitude") || !_settings.Contains("CurrentLongitude"))
            {
                 MessageBox.Show(
                     AppResources.NoticeThereIsNotCurrentLocation,
                     AppResources.AskForLocationConsentMessageBoxCaption,
                     MessageBoxButton.OK);
                return;
            }

            CustomHTTPClient httpClient = new CustomHTTPClient();

            int resultsNumber = 14;
            string formattedForecastURL = String.Format(
                CultureInfo.InvariantCulture, AppResources.URIAPIOpenWeatherMapForecast,
                AppResources.APIVersionOpenWeatherMap, (double)_settings["CurrentLatitude"],
                (double)_settings["CurrentLongitude"], resultsNumber);
            string jsonData = await httpClient.getWeatherData(formattedForecastURL);

            ForecastWeather weather = this._serviceParser.GetForecastWeather(jsonData);


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

            foreach (WeatherInformation.Model.ForecastWeatherParser.List item in weather.list)
            {
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
                DateTime date = unixTime.AddSeconds(item.dt).ToLocalTime();

                // TODO: if I do not receive max temp or min temp... Am I going to receive item.temp.max=0 or item.temp.min=0 (I guess because
                // double has no null value)
                // I need something that is not 0 value in order to find out if OpenWeatherMap sent me values or not :/
                // I guess; I am going to need nullable but I will have to modify my Json Parser :(
                double maxTemp = item.temp.max;
                maxTemp = maxTemp - tempUnits;

                double minTemp = item.temp.min;
                minTemp = minTemp - tempUnits;

                this.ForecastItems.Add(new ItemViewModel()
                {
                    LineOne = date.ToString("ddd", CultureInfo.InvariantCulture),
                    LineTwo = date.ToString("dd", CultureInfo.InvariantCulture),
                    LineThree = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", maxTemp) + symbol,
                    LineFour = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", minTemp) + symbol,
                    LineFive = "/Assets/Tiles/IconicTileMediumLarge.png"
                });

                count--;
                if (count == 0)
                {
                    break;
                }
            }

            this.IsDataLoaded = true;
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
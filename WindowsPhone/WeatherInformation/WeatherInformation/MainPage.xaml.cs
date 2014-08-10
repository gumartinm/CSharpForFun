using Microsoft.Phone.Controls;
using System;
using System.Linq;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WeatherInformation.Model;
using WeatherInformation.Model.Services;
using WeatherInformation.Resources;
using WeatherInformation.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using WeatherInformation.Model.ForecastWeatherParser;
using WeatherInformation.Model.CurrentWeatherParser;
using WeatherInformation.Model.JsonDataParser;

namespace WeatherInformation
{
    public partial class MainPage : PhoneApplicationPage
    {
        private MainViewModel _mainViewModel;
        private bool _isNewPageInstance = false;
        // Data context for the local database
        private LocationDataContext _locationDB;
        private Location _locationItem;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _isNewPageInstance = true;

            // Set the event handler for when the application data object changes.
            // TODO: doing this, when is the GC going to release this object? I do not think it is going to be able... This is weird...
            // Shouldn't I release this even handler when the MainPage is not used anymore. In my case is not a big problem because
            // the MainPage should be always active (it is the "mainpage") but if this was not the mainpage... Would the GC be able
            // to release this object when the page is not active... I DO NOT THINK SO...
            (Application.Current as WeatherInformation.App).ApplicationDataObjectChanged +=
                          new EventHandler(MainPage_ApplicationDataObjectChanged);

            // Connect to the database and instantiate data context.
            _locationDB = new LocationDataContext(LocationDataContext.DBConnectionString);

            _locationItem = _locationDB.Locations.Where(location => location.IsSelected).FirstOrDefault();
            // Código de ejemplo para traducir ApplicationBar
            //BuildLocalizedApplicationBar();
        }


        // Cargar datos para los elementos MainViewModel
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // If _isNewPageInstance is true, the page constuctor has been called, so
            // state may need to be restored.
            if (_isNewPageInstance)
            {
                if (_mainViewModel == null)
                {
                    _mainViewModel = new MainViewModel();
                }

                DataContext = _mainViewModel;
            }
            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            _isNewPageInstance = false;

            UpdateApplicationDataUI();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Call the base method.
            base.OnNavigatedFrom(e);

            // Save changes to the database.
            _locationDB.SubmitChanges();

            // No calling _locationDB.Dispose? :/
        }

        private void UpdateApplicationDataUI()
        {
            if (_locationItem == null)
            {
                // Nothing to do.
                return;
            }

            // If the application member variable is not empty,
            // set the page's data object from the application member variable.
            // TODO: I am setting and getting ApplicationDataObject from different threads!!!! What if I do not see its last value? Do I need synchronization? :/
            WeatherData weatherData = (Application.Current as WeatherInformation.App).ApplicationDataObject;
            if (weatherData != null && !_locationItem.IsNewLocation && IsStoredDataFresh())
            {
                UpdateUI();
            }
            else
            {
                // Otherwise, call the method that loads data.
                GetDataAsync();
            }
        }

        // The event handler called when the ApplicationDataObject changes.
        void MainPage_ApplicationDataObjectChanged(object sender, EventArgs e)
        {
            // Call UpdateApplicationData on the UI thread.
            Dispatcher.BeginInvoke(() => UpdateUI());
        }

        void UpdateUI()
        {
            // Set the ApplicationData and ApplicationDataStatus members of the ViewModel
            WeatherData weatherData = (Application.Current as WeatherInformation.App).ApplicationDataObject;

            if (weatherData != null)
            {
                if (weatherData.WasThereRemoteError)
                {
                    MessageBox.Show(
                         AppResources.NoticeThereIsNotCurrentLocation,
                         AppResources.UnavailableAutomaticCurrentLocationMessageBox,
                         MessageBoxButton.OK);
                    return;
                }

                _mainViewModel.LoadData(weatherData);

                var locationItem = _locationDB.Locations.Where(location => location.IsSelected).FirstOrDefault();
                if (locationItem != null)
                {
                    locationItem.IsNewLocation = false;
                }           
            }
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector longListSelector = sender as LongListSelector;

            // TODO: with LINQ :(
            ItemViewModel element = longListSelector.SelectedItem as ItemViewModel;
            int index = longListSelector.ItemsSource.IndexOf(element);
            String uri = string.Format(CultureInfo.InvariantCulture, "/SelectedDatePage.xaml?parameter={0}", index);
            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }

        public void GetDataAsync()
        {
            // Call the GetData method on a new thread.
            // TODO: Are there too many threads? HttpClient is going to create more... (threadpools and stuff like that...)
            Thread t = new Thread(new ThreadStart(GetData));
            t.Start();
        }

        async private void GetData()
        {
            // Check to see if data exists in storage and see if the data is fresh.
            WeatherData weatherData = GetIsolatedStoredData();

            if ((weatherData != null) && IsStoredDataFresh() && !_locationItem.IsNewLocation)
            {
                (Application.Current as WeatherInformation.App).ApplicationDataObject = weatherData;
            }
            else
            {
                // Otherwise, it gets the data from the web.
                (Application.Current as WeatherInformation.App).ApplicationDataObject = await LoadDataAsync();
            }
        }

        /// <summary>
        /// Retrieve remote weather data.
        /// </summary>
        async public Task<WeatherData> LoadDataAsync()
        {
            double latitude = _locationItem.Latitude;
            double longitude = _locationItem.Longitude;
            int resultsNumber = Convert.ToInt32(AppResources.APIOpenWeatherMapResultsNumber);

            CustomHTTPClient httpClient = new CustomHTTPClient();

            string formattedForecastURL = String.Format(
                CultureInfo.InvariantCulture, AppResources.URIAPIOpenWeatherMapForecast,
                AppResources.APIVersionOpenWeatherMap, latitude, longitude, resultsNumber);
            string JSONRemoteForecastWeather = await httpClient.GetWeatherDataAsync(formattedForecastURL);

            string formattedCurrentURL = String.Format(
                CultureInfo.InvariantCulture, AppResources.URIAPIOpenWeatherMapCurrent,
                AppResources.APIVersionOpenWeatherMap, latitude, longitude, resultsNumber);
            string JSONRemoteCurrentWeather = await httpClient.GetWeatherDataAsync(formattedCurrentURL);

            return WeatherDataParser(JSONRemoteForecastWeather, JSONRemoteCurrentWeather);
        }

        // ESTE METODO ERA PARA CARGAR LOS DATOS JSON NO LOS DE LA BASE DE DATOS. HAY QUE REPENSAR ESTO :(
        private bool IsStoredDataFresh()
        {
            // Check to see if the data is fresh.
            // This example uses 30 seconds as the valid time window to make it easy to test. 
            // Real apps will use a larger window.

            // Check the time elapsed since data was last saved to storage.
            DateTime dataLastSaveTime = _locationItem.StoredTime;
            TimeSpan timeSinceLastSave = DateTime.Now - dataLastSaveTime;

            if (timeSinceLastSave.TotalSeconds < 30)
            {
                return true;
            }

            return false;
        }

        private WeatherData GetIsolatedStoredData()
        {
            string JSONRemoteCurrentWeather = null;
            string JSONRemoteForecastWeather = null;

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists("JSONRemoteCurrentWeatherFile.txt") &&
                    isoStore.FileExists("JSONRemoteForecastWeatherFile.txt"))
                {
                    using (IsolatedStorageFileStream file = isoStore.OpenFile("JSONRemoteCurrentWeatherFile.txt", FileMode.Open))
                    using (StreamReader sr = new StreamReader(file))
                    {
                        // This method loads the data from isolated storage, if it is available.
                        JSONRemoteCurrentWeather = sr.ReadLine();
                    }

                    using (IsolatedStorageFileStream file = isoStore.OpenFile("JSONRemoteCurrentWeatherFile.txt", FileMode.Open))
                    using (StreamReader sr = new StreamReader(file))
                    {
                        // This method loads the data from isolated storage, if it is available.
                        JSONRemoteForecastWeather = sr.ReadLine();
                    }
                }
            }

            if (!string.IsNullOrEmpty(JSONRemoteCurrentWeather) && !string.IsNullOrEmpty(JSONRemoteForecastWeather))
            {
                return WeatherDataParser(JSONRemoteForecastWeather, JSONRemoteCurrentWeather);
            }

            return null;
        }

        private ForecastWeather ForecastWeatherParser(string remoteForecastWeatherData)
        {
            ServiceParser parser = new ServiceParser(new JsonParser());
            return parser.GetForecastWeather(remoteForecastWeatherData);
        }

        private CurrentWeather CurrentWeatherParser(string remoteCurrentWeatherData)
        {
            ServiceParser parser = new ServiceParser(new JsonParser());
            return parser.GetCurrentWeather(remoteCurrentWeatherData);
        }

        private WeatherData WeatherDataParser(string JSONRemoteForecastWeather, string JSONRemoteCurrentWeather)
        {
            if (string.IsNullOrEmpty(JSONRemoteForecastWeather))
            {
                throw new ArgumentException("Missing argument", "JSONRemoteForecastWeather");
            }
            if (string.IsNullOrEmpty(JSONRemoteCurrentWeather))
            {
                throw new ArgumentException("Missing argument", "JSONRemoteCurrentWeather");
            }

            return new WeatherData
            {
                JSONRemoteCurrent = JSONRemoteCurrentWeather,
                JSONRemoteForecast = JSONRemoteForecastWeather,
                RemoteCurrent = CurrentWeatherParser(JSONRemoteCurrentWeather),
                RemoteForecast = ForecastWeatherParser(JSONRemoteForecastWeather),
                WasThereRemoteError = false
            };
        }

        private void Location_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MapPage.xaml", UriKind.Relative));
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        // Código de ejemplo para compilar una ApplicationBar traducida
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Establecer ApplicationBar de la página en una nueva instancia de ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Crear un nuevo botón y establecer el valor de texto en la cadena traducida de AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Crear un nuevo elemento de menú con la cadena traducida de AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}
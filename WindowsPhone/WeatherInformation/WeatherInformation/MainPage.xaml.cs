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

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _isNewPageInstance = true;

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
                    _mainViewModel = new MainViewModel(new SettingsViewModel());
                }

                DataContext = _mainViewModel;
            }
            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            _isNewPageInstance = false;

            UpdateApplicationDataUI();
        }

        async private void UpdateApplicationDataUI()
        {
            Location locationItem = null;
            using (var db = new LocationDataContext(LocationDataContext.DBConnectionString))
            {
                locationItem = db.Locations.Where(location => location.IsSelected).FirstOrDefault();
            }

            if (locationItem == null)
            {
                // Nothing to do.
                return;
            }

            // If the application member variable is not empty,
            // set the page's data object from the application member variable.
            WeatherData weatherData = (Application.Current as WeatherInformation.App).ApplicationDataObject;
            if (!IsDataFresh(locationItem.LastRemoteDataUpdate) || weatherData == null)
            {
                // Load remote data (aynchronous way by means of async/await)

                // Gets the data from the web.
                (Application.Current as WeatherInformation.App).ApplicationDataObject =
                    await GetRemoteDataAsync(locationItem);

                using (var db = new LocationDataContext(LocationDataContext.DBConnectionString))
                {
                    locationItem = db.Locations.Where(location => location.IsSelected).FirstOrDefault();
                    locationItem.LastRemoteDataUpdate = DateTime.UtcNow;
                    db.SubmitChanges();
                }
            }

            // Call UpdateUI on the UI thread.
            // Without ConfigureAwait(false) await returns data on the calling thread. In this case the calling one
            // is the UI thread. So, I can save the call to Dispatcher.BeginInvoke.
            //Dispatcher.BeginInvoke(() => UpdateUI());
            UpdateUI();
        }

        void UpdateUI()
        {
            // Set the ApplicationData and ApplicationDataStatus members of the ViewModel
            WeatherData weatherData = (Application.Current as WeatherInformation.App).ApplicationDataObject;

            if (weatherData != null)
            {
                _mainViewModel.LoadData(weatherData);
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

        /// <summary>
        /// Retrieve remote weather data.
        /// </summary>
        async public Task<WeatherData> GetRemoteDataAsync(Location locationItem)
        {
            double latitude = locationItem.Latitude;
            double longitude = locationItem.Longitude;
            int resultsNumber = Convert.ToInt32(AppResources.APIOpenWeatherMapResultsNumber);

            CustomHTTPClient httpClient = new CustomHTTPClient();

            string formattedForecastURL = String.Format(
                CultureInfo.InvariantCulture, AppResources.URIAPIOpenWeatherMapForecast,
                AppResources.APIVersionOpenWeatherMap, latitude, longitude, resultsNumber);
            string jsonForecast = await httpClient.GetWeatherDataAsync(formattedForecastURL).ConfigureAwait(false);

            string formattedCurrentURL = String.Format(
                CultureInfo.InvariantCulture, AppResources.URIAPIOpenWeatherMapCurrent,
                AppResources.APIVersionOpenWeatherMap, latitude, longitude, resultsNumber);
            string jsonCurrent = await httpClient.GetWeatherDataAsync(formattedCurrentURL).ConfigureAwait(false);

            var parser = new ServiceParser(new JsonParser());
            var weatherData = parser.WeatherDataParser(jsonForecast, jsonCurrent, locationItem.City, locationItem.Country);

            return weatherData;
        }

        private bool IsDataFresh(DateTime? lastUpdate)
        {
            if (lastUpdate == null)
            {
                return false;
            }

            // Check the time elapsed since data was last saved to isolated storage.
            TimeSpan TimeSinceLastSave = DateTime.UtcNow - lastUpdate.Value;

            if (TimeSinceLastSave.TotalSeconds < 30)
            {
                return true;
            }

            return false;
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
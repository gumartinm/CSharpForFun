using Microsoft.Phone.Controls;
using System;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WeatherInformation.Model;
using WeatherInformation.Resources;

namespace WeatherInformation
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool _isNewPageInstance = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();


            // Establecer el contexto de datos del control ListBox control en los datos de ejemplo
            DataContext = App.MainViewModel;


            _isNewPageInstance = true;

            // Set the event handler for when the application data object changes.
            // TODO: doing this, when is the GC going to release this object? I do not think it is going to be able... This is weird...
            // Shouldn't I release this even handler when the MainPage is not used anymore. In my case is not a big problem because
            // the MainPage should be always active (it is the "mainpage") but if this was not the mainpage... Would the GC be able
            // to release this object when the page is not active... I DO NOT THINK SO...
            (Application.Current as WeatherInformation.App).ApplicationDataObjectChanged +=
                          new EventHandler(MainPage_ApplicationDataObjectChanged);


            // Código de ejemplo para traducir ApplicationBar
            //BuildLocalizedApplicationBar();
        }


        // Cargar datos para los elementos MainViewModel
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // If _isNewPageInstance is true, the page constructor has been called, so
            // state may need to be restored.
            if (_isNewPageInstance)
            {
                if (!App.MainViewModel.IsThereCurrentLocation())
                {
                    MessageBox.Show(
                        AppResources.NoticeThereIsNotCurrentLocation,
                        AppResources.AskForLocationConsentMessageBoxCaption,
                        MessageBoxButton.OK);
                }
                else
                {
                    // If the application member variable is not empty,
                    // set the page's data object from the application member variable.
                    // TODO: I am setting and getting ApplicationDataObject from different threads!!!! What if I do not see its last value? Do I need synchronization? :/
                    WeatherData weatherData = (Application.Current as WeatherInformation.App).ApplicationDataObject;
                    if (weatherData != null && !(Application.Current as WeatherInformation.App).IsNewLocation)
                    {
                        UpdateApplicationDataUI();
                    }
                    else
                    {
                        // Otherwise, call the method that loads data.
                        (Application.Current as WeatherInformation.App).GetDataAsync();
                    }
                }
            }
            else
            {
                if (!App.MainViewModel.IsThereCurrentLocation())
                {
                    MessageBox.Show(
                        AppResources.NoticeThereIsNotCurrentLocation,
                        AppResources.AskForLocationConsentMessageBoxCaption,
                        MessageBoxButton.OK);
                }
                else
                {
                    // If the application member variable is not empty,
                    // set the page's data object from the application member variable.
                    // TODO: I am setting and getting ApplicationDataObject from different threads!!!! What if I do not see the its last state? Do I need synchronization? :/
                    WeatherData weatherData = (Application.Current as WeatherInformation.App).ApplicationDataObject;
                    if (weatherData != null && !(Application.Current as WeatherInformation.App).IsNewLocation)
                    {
                        UpdateApplicationDataUI();  
                    }
                    else
                    {
                        // Otherwise, call the method that loads data.
                        (Application.Current as WeatherInformation.App).GetDataAsync();
                    }
                }
            }

            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            _isNewPageInstance = false;
        }

        // The event handler called when the ApplicationDataObject changes.
        void MainPage_ApplicationDataObjectChanged(object sender, EventArgs e)
        {
            // Call UpdateApplicationData on the UI thread.
            Dispatcher.BeginInvoke(() => UpdateApplicationDataUI());
        }

        void UpdateApplicationDataUI()
        {
            // Set the ApplicationData and ApplicationDataStatus members of the ViewModel
            WeatherData weatherData = (Application.Current as WeatherInformation.App).ApplicationDataObject;

            if (weatherData.WasThereRemoteError)
            {
                MessageBox.Show(
                     AppResources.NoticeThereIsNotCurrentLocation,
                     AppResources.AskForLocationConsentMessageBoxCaption,
                     MessageBoxButton.OK);
                return;
            }
    
            App.MainViewModel.LoadData(weatherData);

            (Application.Current as WeatherInformation.App).IsNewLocation = false;

            // TODO: Should I try to move this code to MainViewModel. It seems so but how?
            string country = (string)IsolatedStorageSettings.ApplicationSettings["Country"];
            string city = (string)IsolatedStorageSettings.ApplicationSettings["City"];
            string cityCountry = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", city, country);
            this.TitleTextCityCountry.Title = cityCountry;
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
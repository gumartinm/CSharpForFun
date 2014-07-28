using Microsoft.Phone.Controls;
using System;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Windows;
using WeatherInformation.Model;
using WeatherInformation.Resources;
using WeatherInformation.ViewModels;

namespace WeatherInformation
{
    public partial class SelectedDate : PhoneApplicationPage
    {
        SelectedDateViewModel _selectedDateViewModel;
        bool _isNewPageInstance = false;

        public SelectedDate()
        {
            InitializeComponent();

            _isNewPageInstance = true;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // If _isNewPageInstance is true, the page constuctor has been called, so
            // state may need to be restored.
            if (_isNewPageInstance)
            {
                if (_selectedDateViewModel == null)
                {
                    _selectedDateViewModel = new SelectedDateViewModel();
                    // I always receive my parameter even if page was tombstoned. :)
                    string stringIndex = NavigationContext.QueryString["parameter"];
                    _selectedDateViewModel.SelectedDateIndex = Convert.ToInt32(stringIndex, CultureInfo.InvariantCulture);    
                }

                DataContext = _selectedDateViewModel;
            }
            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            _isNewPageInstance = false;

            UpdateApplicationDataUI();
        }

        void UpdateApplicationDataUI()
        {
            WeatherData weatherData = (Application.Current as WeatherInformation.App).ApplicationDataObject;

            if (weatherData.WasThereRemoteError)
            {
                MessageBox.Show(
                     AppResources.NoticeThereIsNotCurrentLocation,
                     AppResources.AskForLocationConsentMessageBoxCaption,
                     MessageBoxButton.OK);
                return;
            }

            _selectedDateViewModel.LoadData(weatherData);

            // TODO: Should I try to move this code to MainViewModel. It seems so but how?
            // TODO: What if the address is not available? I should show something like "Address not found" by default...
            string country = (string)IsolatedStorageSettings.ApplicationSettings["Country"];
            string city = (string)IsolatedStorageSettings.ApplicationSettings["City"];
            string cityCountry = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", city, country);
            this.TitleTextCityCountry.Title = cityCountry;
        }
    }
}
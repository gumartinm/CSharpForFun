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
        private SelectedDateViewModel _selectedDateViewModel;
        private bool _isNewPageInstance = false;

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
                }

                DataContext = _selectedDateViewModel;
            }
            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            _isNewPageInstance = false;

            // I always receive my parameter even if page was tombstoned. :)
            string stringIndex = NavigationContext.QueryString["parameter"];
            int index = Convert.ToInt32(stringIndex, CultureInfo.InvariantCulture);

            UpdateApplicationDataUI(index);
        }

        void UpdateApplicationDataUI(int index)
        {
            WeatherData weatherData = (Application.Current as WeatherInformation.App).ApplicationDataObject;

            if (weatherData != null)
            {
                _selectedDateViewModel.LoadData(weatherData, index);
            }
        }
    }
}
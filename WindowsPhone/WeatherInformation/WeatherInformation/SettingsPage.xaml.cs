using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WeatherInformation.ViewModels;
using System.IO.IsolatedStorage;

namespace WeatherInformation
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        // Our settings
        private IsolatedStorageSettings _settings;

        public SettingsPage()
        {
            InitializeComponent();

            _settings = IsolatedStorageSettings.ApplicationSettings;

            // Establecer el contexto de datos del control ListBox control en los datos de ejemplo
            DataContext = App.SettingsViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.SettingsViewModel.IsDataLoaded)
            {
                App.SettingsViewModel.LoadData();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WeatherInformation
{
    public partial class SettingsTemperatureUnitsPage : PhoneApplicationPage
    {
        public SettingsTemperatureUnitsPage()
        {
            InitializeComponent();

            // Establecer el contexto de datos del control ListBox control en los datos de ejemplo
            DataContext = App.TemperatureUnitsViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.TemperatureUnitsViewModel.IsDataLoaded)
            {
                App.TemperatureUnitsViewModel.LoadData();
            }
        }
    }
}
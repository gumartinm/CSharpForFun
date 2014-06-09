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

namespace WeatherInformation
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();

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

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            ItemViewModel item = selector.SelectedItem as ItemViewModel;
            switch(item.LineOne)
            {
                case "Temperature units":
                    NavigationService.Navigate(new Uri("/SettingsTemperatureUnitsPage.xaml", UriKind.Relative));
                    break;
                default:
                    break;
            }
        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
           
        }
    }
}
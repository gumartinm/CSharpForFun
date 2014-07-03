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
using System.Collections;
using System.Text;

namespace WeatherInformation
{
    public partial class SettingsPage : PhoneApplicationPage
    {

        public SettingsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Update temperature units selection when change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemperatureUnitsSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker listPicker = sender as ListPicker;

            // TODO: with LINQ :(
            int index = listPicker.SelectedIndex;
            var item = listPicker.Items[index];
            listPicker.SelectedItem = item;
        }

        /// <summary>
        /// Update language selection when change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker listPicker = sender as ListPicker;

            // TODO: with LINQ :(
            int index = listPicker.SelectedIndex;
            var item = listPicker.Items[index];
            listPicker.SelectedItem = item;
        }
    }
}
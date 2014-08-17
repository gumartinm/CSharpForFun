using System.Windows.Controls;
using Microsoft.Phone.Controls;

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

            int index = listPicker.SelectedIndex;
            var item = listPicker.Items[index];
            listPicker.SelectedItem = item;
        }

        private void ForecastDayNumbersSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker listPicker = sender as ListPicker;

            int index = listPicker.SelectedIndex;
            var item = listPicker.Items[index];
            listPicker.SelectedItem = item;
        }
    }
}
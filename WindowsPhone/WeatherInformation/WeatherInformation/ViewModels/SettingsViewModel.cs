using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherInformation.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel()
        {
            this.TemperatureUnitsSelection = new List<string>();
            this.LanguageSelection = new List<string>();
        }

        public List<string> TemperatureUnitsSelection { get; private set; }
        public List<string> LanguageSelection { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Crear y agregar unos pocos objetos a la colección Items.
        /// </summary>
        public void LoadData()
        {
            // TODO: How to do the same using StaticResources? :/ What the translator should do with this :(
            // There must be some way to refernce static resources from here or something like that, otherwise
            // the translator is going to complain A LOT!
            TemperatureUnitsSelection.Add("fahrenheit");
            TemperatureUnitsSelection.Add("centigrade");

            LanguageSelection.Add("english");
            LanguageSelection.Add("spanish");

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

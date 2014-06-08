using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherInformation.ViewModels
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel()
        {
            this.SettingsItems = new ObservableCollection<ItemViewModel>();
        }

        private string _sampleProperty = "Sample Runtime Property Value";

        public ObservableCollection<ItemViewModel> SettingsItems { get; private set; }
        /// <summary>
        /// Propiedad Sample ViewModel; esta propiedad se usa en la vista para mostrar su valor mediante un enlace
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
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

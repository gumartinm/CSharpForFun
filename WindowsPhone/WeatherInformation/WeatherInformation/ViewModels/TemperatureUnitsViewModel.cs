using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherInformation.ViewModels
{
    public class TemperatureUnitsViewModel : INotifyPropertyChanged
    {
        public TemperatureUnitsViewModel()
        {
            this.TemperatureUnitsItems = new ObservableCollection<ItemViewModel>();
        }

        private string _sampleProperty = "Sample Runtime Property Value";

        public ObservableCollection<ItemViewModel> TemperatureUnitsItems { get; private set; }
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

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Crear y agregar unos pocos objetos ItemViewModel a la colección Items.
        /// </summary>
        public void LoadData()
        {
                // TODO: How to do the same using StaticResources? :/
                this.TemperatureUnitsItems.Add(new ItemViewModel()
                {
                    LineOne = "fahrenheit"
                });
                this.TemperatureUnitsItems.Add(new ItemViewModel()
                {
                    LineOne = "centigrade"
                });


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

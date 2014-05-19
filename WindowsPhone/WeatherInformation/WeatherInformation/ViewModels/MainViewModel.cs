using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using WeatherInformation.Model.ForecastWeatherParser;
using WeatherInformation.Model.JsonDataParser;
using WeatherInformation.Model.Services;
using WeatherInformation.Resources;

namespace WeatherInformation.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.ForecastItems = new ObservableCollection<ItemViewModel>();
            this.CurrentItems = new ObservableCollection<ItemViewModel>();
            this._serviceParser = new ServiceParser(new JsonParser());
        }

        /// <summary>
        /// Colección para objetos ItemViewModel.
        /// </summary>
        public ObservableCollection<ItemViewModel> ForecastItems { get; private set; }
        public ObservableCollection<ItemViewModel> CurrentItems { get; private set; }
        //public string ForecastHeader { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";

        private readonly ServiceParser _serviceParser;

        string _jsonForeCastWeatherData =
    "{"
    + "\"cod\":\"200\","
    + "\"message\":0.0048,"
    + "\"city\":{\"id\":2641549,\"name\":\"Newtonhill\",\"coord\":{\"lon\":-2.15,\"lat\":57.033329},\"country\":\"GB\",\"population\":0},"
    + "\"cnt\":15,"
    + "\"list\":["
    + "{\"dt\":1397304000,\"temp\":{\"day\":286.15,\"min\":284.62,\"max\":286.15,\"night\":284.62,\"eve\":285.7,\"morn\":286.15},\"pressure\":1016.67,\"humidity\":84,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":7.68,\"deg\":252,\"clouds\":0,\"rain\":0.25},"
    + "{\"dt\":1397390400,\"temp\":{\"day\":284.92,\"min\":282.3,\"max\":284.92,\"night\":282.3,\"eve\":283.79,\"morn\":284.24},\"pressure\":1021.62,\"humidity\":84,\"weather\":[{\"id\":804,\"main\":\"Clouds\",\"description\":\"overcast clouds\",\"icon\":\"04d\"}],\"speed\":7.91,\"deg\":259,\"clouds\":92},"
    + "{\"dt\":1397476800,\"temp\":{\"day\":282.1,\"min\":280.32,\"max\":282.1,\"night\":280.32,\"eve\":281.51,\"morn\":281.65},\"pressure\":1033.84,\"humidity\":92,\"weather\":[{\"id\":801,\"main\":\"Clouds\",\"description\":\"few clouds\",\"icon\":\"02d\"}],\"speed\":8.37,\"deg\":324,\"clouds\":20},"
    + "{\"dt\":1397563200,\"temp\":{\"day\":280.73,\"min\":280.11,\"max\":281.4,\"night\":281.4,\"eve\":280.75,\"morn\":280.11},\"pressure\":1039.27,\"humidity\":97,\"weather\":[{\"id\":801,\"main\":\"Clouds\",\"description\":\"few clouds\",\"icon\":\"02d\"}],\"speed\":7.31,\"deg\":184,\"clouds\":12},"
    + "{\"dt\":1397649600,\"temp\":{\"day\":281.73,\"min\":281.03,\"max\":282.22,\"night\":281.69,\"eve\":282.22,\"morn\":281.03},\"pressure\":1036.05,\"humidity\":90,\"weather\":[{\"id\":803,\"main\":\"Clouds\",\"description\":\"broken clouds\",\"icon\":\"04d\"}],\"speed\":7.61,\"deg\":205,\"clouds\":68},"
    + "{\"dt\":1397736000,\"temp\":{\"day\":282.9,\"min\":281.45,\"max\":283.21,\"night\":282.71,\"eve\":283.06,\"morn\":281.49},\"pressure\":1029.39,\"humidity\":83,\"weather\":[{\"id\":803,\"main\":\"Clouds\",\"description\":\"broken clouds\",\"icon\":\"04d\"}],\"speed\":6.17,\"deg\":268,\"clouds\":56},"
    + "{\"dt\":1397822400,\"temp\":{\"day\":285.26,\"min\":281.55,\"max\":285.26,\"night\":282.48,\"eve\":285.09,\"morn\":281.55},\"pressure\":1025.83,\"humidity\":0,\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"sky is clear\",\"icon\":\"01d\"}],\"speed\":5.31,\"deg\":221,\"clouds\":10},"
    + "{\"dt\":1397908800,\"temp\":{\"day\":284.29,\"min\":281.5,\"max\":284.29,\"night\":282.53,\"eve\":283.58,\"morn\":281.5},\"pressure\":1024.55,\"humidity\":0,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":5.51,\"deg\":192,\"clouds\":6},"
    + "{\"dt\":1397995200,\"temp\":{\"day\":283.36,\"min\":281.62,\"max\":284.34,\"night\":284.04,\"eve\":284.34,\"morn\":281.62},\"pressure\":1019.58,\"humidity\":0,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":7.66,\"deg\":149,\"clouds\":0,\"rain\":0.48},"
    + "{\"dt\":1398081600,\"temp\":{\"day\":282.24,\"min\":280.51,\"max\":282.41,\"night\":280.51,\"eve\":282.41,\"morn\":280.9},\"pressure\":1027.35,\"humidity\":0,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":8.17,\"deg\":221,\"clouds\":10,\"rain\":0.94},"
    + "{\"dt\":1398168000,\"temp\":{\"day\":282.28,\"min\":279.76,\"max\":282.28,\"night\":280.69,\"eve\":281.13,\"morn\":279.76},\"pressure\":1038.31,\"humidity\":0,\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"sky is clear\",\"icon\":\"01d\"}],\"speed\":6.33,\"deg\":172,\"clouds\":1},"
    + "{\"dt\":1398254400,\"temp\":{\"day\":281.54,\"min\":280.52,\"max\":281.54,\"night\":281.44,\"eve\":281.23,\"morn\":280.52},\"pressure\":1022.4,\"humidity\":0,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":7.84,\"deg\":140,\"clouds\":91,\"rain\":1.24},"
    + "{\"dt\":1398340800,\"temp\":{\"day\":282.1,\"min\":280.66,\"max\":282.78,\"night\":280.97,\"eve\":282.78,\"morn\":280.66},\"pressure\":1013.39,\"humidity\":0,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":9.43,\"deg\":164,\"clouds\":98,\"rain\":1.03},"
    + "{\"dt\":1398427200,\"temp\":{\"day\":282.11,\"min\":280.72,\"max\":282.32,\"night\":282.32,\"eve\":280.99,\"morn\":280.72},\"pressure\":1018.65,\"humidity\":0,\"weather\":[{\"id\":502,\"main\":\"Rain\",\"description\":\"heavy intensity rain\",\"icon\":\"10d\"}],\"speed\":5.26,\"deg\":158,\"clouds\":83,\"rain\":14.4},"
    + "{\"dt\":1398513600,\"temp\":{\"day\":282.75,\"min\":280.61,\"max\":282.75,\"night\":280.61,\"eve\":281.75,\"morn\":281.96},\"pressure\":1007.4,\"humidity\":0,\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10d\"}],\"speed\":9.18,\"deg\":198,\"clouds\":35,\"rain\":0.55}"
    + "]}";

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

        /// <summary>
        /// Propiedad de ejemplo que devuelve una cadena traducida
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
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
            ForecastWeather weather = this._serviceParser.GetForecastWeather(this._jsonForeCastWeatherData);

            foreach (WeatherInformation.Model.ForecastWeatherParser.List item in weather.list)
            {
                DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime date = unixTime.AddSeconds(item.dt).ToLocalTime();
                this.ForecastItems.Add(new ItemViewModel()
                {
                    LineOne = date.ToString("ddd", CultureInfo.InvariantCulture),
                    LineTwo = date.ToString("dd", CultureInfo.InvariantCulture),
                    LineThree = String.Format("{0:0.##}", item.temp.max),
                    LineFour = String.Format("{0:0.##}", item.temp.min),
                    LineFive = "/Assets/Tiles/IconicTileMediumLarge.png"
                });
            }

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
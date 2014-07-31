using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Windows.Devices.Geolocation;
using System.Device.Location;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Phone.Maps.Controls;
using WeatherInformation.Resources;
using System.Globalization;
using Microsoft.Phone.Maps.Services;
using WeatherInformation.Model.Services;

namespace WeatherInformation
{
    public partial class MapPage : PhoneApplicationPage
    {
        // Settings
        private readonly IsolatedStorageSettings _settings;

        public MapPage()
        {
            InitializeComponent();

            // Get the _settings for this application.
            _settings = IsolatedStorageSettings.ApplicationSettings;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!_settings.Contains("LocationConsent"))
            {
                MessageBoxResult result = MessageBox.Show(
                    AppResources.AskForLocationConsentMessageBox,
                    AppResources.AskForLocationConsentMessageBoxCaption,
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    _settings["LocationConsent"] = true;
                }
                else
                {
                    _settings["LocationConsent"] = false;
                }

                _settings.Save();
            }

            if ((bool)_settings["LocationConsent"] != true)
            {
                // The user has opted out of Location.
                return;
            }

            this.GetLocation();
        }

        private async void GetLocation()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );
                GeoCoordinate currentGeoCoordinate = CoordinateConverter.ConvertGeocoordinate(geoposition.Coordinate);

                ReverseGeocodeQuery currentReverseGeocodeQuery = new ReverseGeocodeQuery();
                currentReverseGeocodeQuery.GeoCoordinate = currentGeoCoordinate;
                currentReverseGeocodeQuery.QueryCompleted += QueryCompletedCallback;
                currentReverseGeocodeQuery.QueryAsync();  
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {

                    // the application does not have the right capability or the location master switch is off
                    MessageBox.Show(
                        AppResources.NoticeErrorLocationAutodetection,
                        AppResources.AskForLocationConsentMessageBoxCaption,
                        MessageBoxButton.OK);
                }
                else
                {
                    // something else happened acquring the location
                    MessageBox.Show(
                        AppResources.NoticeErrorLocationAutodetection,
                        AppResources.AskForLocationConsentMessageBoxCaption,
                        MessageBoxButton.OK);
                }
            }
        }


        private void QueryCompletedCallback(object sender, QueryCompletedEventArgs<IList<MapLocation>> eventData)
        {
            if (eventData.Cancelled)
            {
                // Be careful!!! If you throw exception from this point your program will finish with "Unhandled Exception".
                return;
            }

            Exception errorException = eventData.Error;
            if (errorException != null)
            {
                // TODO: Show some log. I need to use remote logging :(
                return;
            }
            else
            {
                if (eventData.Result.Count > 0)
                {
                    MapAddress address = eventData.Result[0].Information.Address;
                    GeoCoordinate currentGeoCoordinate = eventData.Result[0].GeoCoordinate;

                    string cityCountry = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", address.City, address.Country);
                    this.LocationTextCityCountry.Text = cityCountry;

                    // TODO: If I want to store more than one place I should use a database :(
                    StoredLocation.CurrentLatitude = currentGeoCoordinate.Latitude;
                    StoredLocation.CurrentLongitude = currentGeoCoordinate.Longitude;
                    // TODO: If I want to store more thant one place I should use a database :(
                    StoredLocation.City = address.City;
                    StoredLocation.Country = address.Country;
                    StoredLocation.IsNewLocation = true;

                    // Create a small circle to mark the current location.
                    Ellipse myCircle = new Ellipse();
                    myCircle.Fill = new SolidColorBrush(Colors.Blue);
                    myCircle.Height = 20;
                    myCircle.Width = 20;
                    myCircle.Opacity = 50;

                    // Create a MapOverlay to contain the circle.
                    MapOverlay myLocationOverlay = new MapOverlay();
                    myLocationOverlay.Content = myCircle;
                    myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
                    myLocationOverlay.GeoCoordinate = currentGeoCoordinate;

                    // Create a MapLayer to contain the MapOverlay.
                    MapLayer myLocationLayer = new MapLayer();
                    myLocationLayer.Add(myLocationOverlay);

                    this.mapWeatherInformation.Center = currentGeoCoordinate;
                    this.mapWeatherInformation.ZoomLevel = 13;

                    // Add the MapLayer to the Map.
                    this.mapWeatherInformation.Layers.Add(myLocationLayer);  
                }
            }
        }


        public static class CoordinateConverter
        {
            public static GeoCoordinate ConvertGeocoordinate(Geocoordinate geocoordinate)
            {
                return new GeoCoordinate
                    (
                    geocoordinate.Latitude,
                    geocoordinate.Longitude,
                    geocoordinate.Altitude ?? Double.NaN,
                    geocoordinate.Accuracy,
                    geocoordinate.AltitudeAccuracy ?? Double.NaN,
                    geocoordinate.Speed ?? Double.NaN,
                    geocoordinate.Heading ?? Double.NaN
                    );
            }
        }

    }
}
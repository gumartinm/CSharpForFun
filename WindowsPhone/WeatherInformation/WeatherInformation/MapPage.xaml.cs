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

            if (!StoredLocation.IsThereCurrentLocation)
            {
                this.GetLocation();
            }
            else
            {
                GeoCoordinate geoCoordinate = CoordinateHelper.GetStoredGeoCoordinate();

                this.UpdateMap(geoCoordinate, StoredLocation.City, StoredLocation.Country);
            }
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
                GeoCoordinate currentGeoCoordinate = CoordinateHelper.ConvertGeocoordinate(geoposition.Coordinate);

                ReverseGeocodeAndUpdateMap(currentGeoCoordinate);
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

        // TODO: problems updating Map because this method may be called when automatically retrieving
        //       the current user's location or when user taps on map tyring to choose by herself her location.
        //       There could be 2 threads updating Map at the same time. Solution: remove the feature
        //       of getting the current user's location (user must always choose her/his location instead of doing
        //       it automatically)
        private void ReverseGeocodeAndUpdateMap(GeoCoordinate currentGeoCoordinate)
        {
            ReverseGeocodeQuery currentReverseGeocodeQuery = new ReverseGeocodeQuery();
            currentReverseGeocodeQuery.GeoCoordinate = currentGeoCoordinate;
            currentReverseGeocodeQuery.QueryCompleted += QueryCompletedCallback;
            currentReverseGeocodeQuery.QueryAsync();
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
                    // TODO: What if there is no city or country. Is there null value or empty string?
                    MapAddress address = eventData.Result[0].Information.Address;
                    GeoCoordinate currentGeoCoordinate = eventData.Result[0].GeoCoordinate;

                    UpdateMap(currentGeoCoordinate, address.City, address.Country);
                }
            }
        }

        private void UpdateMap(GeoCoordinate geoCoordinate, string city, string country)
        {
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
            myLocationOverlay.GeoCoordinate = geoCoordinate;

            // Create a MapLayer to contain the MapOverlay.
            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(myLocationOverlay);

            this.mapWeatherInformation.Layers.Clear();

            this.mapWeatherInformation.Center = geoCoordinate;
            this.mapWeatherInformation.ZoomLevel = 13;

            // TODO: What if there is no city or country. Is there null value or empty string?
            string cityCountry = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", city, country);
            this.LocationTextCityCountry.Text = cityCountry;
            // Add the MapLayer to the Map.
            this.mapWeatherInformation.Layers.Add(myLocationLayer);
        }

        private static class CoordinateHelper
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

            // TODO: database
            // TODO: What if Double.NAN or null... Am I going to have some problem storing data in IsolatedStorageSettings?
            public static void StoreGeoCoordinate(GeoCoordinate geocoordinate)
            {
                StoredLocation.CurrentLatitude = geocoordinate.Latitude;
                StoredLocation.CurrentLongitude = geocoordinate.Longitude;
                StoredLocation.IsNewLocation = true;
                StoredLocation.CurrentAltitude = geocoordinate.Altitude;
                StoredLocation.CurrentHorizontalAccuracy = geocoordinate.HorizontalAccuracy;
                StoredLocation.CurrentVerticalAccuracy = geocoordinate.VerticalAccuracy;
                StoredLocation.CurrentSpeed = geocoordinate.Speed;
                StoredLocation.CurrentCourse = geocoordinate.Course;
            }

            public static GeoCoordinate GetStoredGeoCoordinate()
            {
                return new GeoCoordinate
                    (
                    StoredLocation.CurrentLatitude,
                    StoredLocation.CurrentLongitude,
                    StoredLocation.CurrentAltitude,
                    StoredLocation.CurrentHorizontalAccuracy,
                    StoredLocation.CurrentVerticalAccuracy,
                    StoredLocation.CurrentSpeed,
                    StoredLocation.CurrentCourse
                    );
            }
        }

        private void mapWeatherInformation_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var point = e.GetPosition(this.mapWeatherInformation);
            GeoCoordinate geocoordinate = this.mapWeatherInformation.ConvertViewportPointToGeoCoordinate(point);
            ReverseGeocodeAndUpdateMap(geocoordinate);
        }
      
        private void SaveLocationButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Could there some problem if user clicks button and thread is in this very moment updating map?
            var geoCoordinate = this.mapWeatherInformation.Center;

            // TODO: What if there is no city or country. Is there null value or empty string?
            //StoredLocation.City = address.City;
            //StoredLocation.Country = address.Country;
            CoordinateHelper.StoreGeoCoordinate(geoCoordinate);
        }
    }
}
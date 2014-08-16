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
using System.Threading.Tasks;
using WeatherInformation.Model;

namespace WeatherInformation
{
    public partial class MapPage : PhoneApplicationPage
    {
        // TODO anything better than these two instance fields? :(
        private string _city;
        private string _country;

        public MapPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            Location locationItem = null;
            using (var db = new LocationDataContext(LocationDataContext.DBConnectionString))
            {
                // Define the query to gather all of the to-do items.
                // var toDoItemsInDB = from Location location in _locationDB.Locations where location.IsSelected select location;
                locationItem = db.Locations.Where(location => location.IsSelected).FirstOrDefault();
            }
            
            if (locationItem != null)
            {
                GeoCoordinate geoCoordinate = ConvertLocation(locationItem);

                this.UpdateMap(geoCoordinate, locationItem.City, locationItem.Country);
            }
        }

        private async Task GetCurrentLocationAndUpdateMap()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            Geoposition geoposition = await geolocator.GetGeopositionAsync(
                maximumAge: TimeSpan.FromSeconds(10),
                timeout: TimeSpan.FromSeconds(10)
                );
            GeoCoordinate currentGeoCoordinate = CoordinateHelper.ConvertGeocoordinate(geoposition.Coordinate);

            ReverseGeocodeAndUpdateMap(currentGeoCoordinate);
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
                MessageBox.Show(
                    AppResources.NoticeErrorLocationAutodetection,
                    AppResources.UnavailableAutomaticCurrentLocationMessageBox,
                    MessageBoxButton.OK);
            }
            else
            {
                if (eventData.Result.Count > 0)
                {
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

            if (string.IsNullOrEmpty(city))
            {
                city = AppResources.DefaultCity;
            }
            if (string.IsNullOrEmpty(country))
            {
                country = AppResources.DefaultCountry;
            }
            this.LocationTextCityCountry.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", city, country);
            _city = city;
            _country = country;
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
        }

        // TODO: check data before storing :(
        // http://stackoverflow.com/questions/4521435/what-specific-values-can-a-c-sharp-double-represent-that-a-sql-server-float-can
        private void StoreLocation(GeoCoordinate geocoordinate)
        {
            Location locationItem = null;
            using (var db = new LocationDataContext(LocationDataContext.DBConnectionString))
            {
                // Define the query to gather all of the to-do items.
                // var toDoItemsInDB = from Location location in _locationDB.Locations where location.IsSelected select location;
                locationItem = db.Locations.Where(location => location.IsSelected).FirstOrDefault();

                if (locationItem != null)
                {
                    locationItem.Latitude = geocoordinate.Latitude;
                    locationItem.Longitude = geocoordinate.Longitude;
                    locationItem.Altitude = geocoordinate.Altitude;
                    locationItem.City = _city ?? "";
                    locationItem.Country = _country ?? "";
                    locationItem.HorizontalAccuracy = geocoordinate.HorizontalAccuracy;
                    locationItem.VerticalAccuracy = geocoordinate.VerticalAccuracy;
                    locationItem.Speed = geocoordinate.Speed;
                    locationItem.Course = geocoordinate.Course;
                    locationItem.IsSelected = true;
                    locationItem.LastRemoteDataUpdate = null;
                }
                else
                {
                    locationItem = new Location()
                    {
                        Latitude = geocoordinate.Latitude,
                        Longitude = geocoordinate.Longitude,
                        Altitude = geocoordinate.Altitude,
                        City = _city ?? "",
                        Country = _country ?? "",
                        HorizontalAccuracy = geocoordinate.HorizontalAccuracy,
                        VerticalAccuracy = geocoordinate.VerticalAccuracy,
                        Speed = geocoordinate.Speed,
                        Course = geocoordinate.Course,
                        IsSelected = true,
                        LastRemoteDataUpdate = null,
                    };

                    // Add a location item to the local database.
                    db.Locations.InsertOnSubmit(locationItem);
                }

                db.SubmitChanges();
            }
        }

        private GeoCoordinate ConvertLocation(Location locationItem)
        {
            return new GeoCoordinate
                (
                locationItem.Latitude,
                locationItem.Longitude,
                locationItem.Altitude,
                locationItem.HorizontalAccuracy,
                locationItem.VerticalAccuracy,
                locationItem.Speed,
                locationItem.Course
                );
        }

        private void mapWeatherInformation_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var point = e.GetPosition(this.mapWeatherInformation);
            GeoCoordinate geocoordinate = this.mapWeatherInformation.ConvertViewportPointToGeoCoordinate(point);
            ReverseGeocodeAndUpdateMap(geocoordinate);
        }

        private async void GetCurrentLocationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await this.GetCurrentLocationAndUpdateMap();
            }
            catch (Exception ex)
            {
                // TODO: make sure when exception in GetCurrentLocationAndUpdateMap we catch it here.
                MessageBox.Show(
                    AppResources.NoticeErrorLocationAutodetection,
                    AppResources.UnavailableAutomaticCurrentLocationMessageBox,
                    MessageBoxButton.OK);
            }
        }

        private void SaveLocationButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Could there some problem if user clicks button and thread is in this very moment updating map?
            var geoCoordinate = this.mapWeatherInformation.Center;

            // TODO: What if there is no city or country. Is there null value or empty string?
            //StoredLocation.City = address.City;
            //StoredLocation.Country = address.Country;
            StoreLocation(geoCoordinate);
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            this.mapWeatherInformation.ZoomLevel = this.mapWeatherInformation.ZoomLevel - 1;
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            this.mapWeatherInformation.ZoomLevel = this.mapWeatherInformation.ZoomLevel + 1;
        }
    }
}
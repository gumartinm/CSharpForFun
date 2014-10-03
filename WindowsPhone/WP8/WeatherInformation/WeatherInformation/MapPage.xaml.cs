using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Windows.Devices.Geolocation;
using System.Device.Location;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Phone.Maps.Controls;
using System.Threading.Tasks;
using WeatherInformation.Model;

namespace WeatherInformation
{
    public partial class MapPage : PhoneApplicationPage, ReverseGeoCode.IReverseGeoCode
    {
        private bool _isNewPageInstance;
        private ReverseGeoCode _reverseGeoCodeOnProgress;
        // TODO: how big is a MapLayer object?
        private MapOverlay _locationOverlay;

        public MapPage()
        {
            InitializeComponent();

            _isNewPageInstance = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var geolocator = new Geolocator();
            if (geolocator.LocationStatus != PositionStatus.Ready)
            {
                GetCurrentLocationButton.IsEnabled = false;
            }

            GeoCoordinate restoreReverseOnProgress = null;
            Location restoreLocation = null;
            if (_isNewPageInstance)
            {
                if (State.Count > 0)
                {
                    restoreReverseOnProgress = (GeoCoordinate)State["ReverseGeoCoorDinateOnProgress"];
                    restoreLocation = (Location)State["CurrentChosenMapLocation"];
                }
            }
            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            _isNewPageInstance = false;

            Location location;
            if (restoreLocation != null)
            {
                location = restoreLocation;
            }
            else if (_locationOverlay != null)
            {
                location = CoordinateHelper.GeoCoordinateToLocation(
                    _locationOverlay.GeoCoordinate,
                    LocationTextCity.Text,
                    LocationTextCountry.Text);
            }
            else
            {
                using (var db = new LocationDataContext(LocationDataContext.DBConnectionString))
                {
                    // Define the query to gather all of the to-do items.
                    // var toDoItemsInDB = from Location location in _locationDB.Locations where location.IsSelected select location;
                    location = db.Locations.Where(locationItem => locationItem.IsSelected).FirstOrDefault();
                }
            }


            if (location != null)
            {
                UpdateMap(CoordinateHelper.LocationToGeoCoordinate(location),
                          location.City, location.Country);
            }

            if (restoreReverseOnProgress != null)
            {
                ShowProgressBar();
                ReverseGeocodeAndUpdateMap(restoreReverseOnProgress);
            }
            else
            {
                RemoveProgressBar();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                // TODO: Could Center be null?
                State["CurrentChosenMapLocation"] = CoordinateHelper.GeoCoordinateToLocation(
                    mapWeatherInformation.Center,
                    LocationTextCity.Text,
                    LocationTextCountry.Text);

                if (_reverseGeoCodeOnProgress != null)
                {
                    State["ReverseGeoCoorDinateOnProgress"] = _reverseGeoCodeOnProgress.CoorDinate;
                }  
            }
        }

        public void OnCompletedReverseGeoCode(GeoCoordinate geoCoordinate, string city, string country)
        {
            UpdateMap(geoCoordinate, city, country);
            RemoveProgressBar();
            _reverseGeoCodeOnProgress = null;
        }

        private async Task GetCurrentLocationAndUpdateMap()
        {
            var geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            geolocator.ReportInterval = 1000;

            var geoposition = await geolocator.GetGeopositionAsync(
                maximumAge: TimeSpan.FromSeconds(1),
                timeout: TimeSpan.FromSeconds(10)
                );
            // TODO: check if the following is true:
            // Without ConfigureAwait(false) await returns data on the calling thread. (AFAIK for this Context)
            // otherwise I should call: Dispatcher.BeginInvoke(() => ReverseGeocodeAndUpdateMap(currentGeoCoordinate));

            // TODO: What is going to happend when Timeout? Exception or geposition will be null value.
            //       Should I check for null value in case of Timeout?
            var currentGeoCoordinate = CoordinateHelper.ConvertGeocoordinate(geoposition.Coordinate);
            ShowProgressBar();
            ReverseGeocodeAndUpdateMap(currentGeoCoordinate);
        }

        private void ReverseGeocodeAndUpdateMap(GeoCoordinate geoCoordinate)
        {
            var reverseGeoCode = new ReverseGeoCode()
            {
                Page = this,
                CoorDinate = geoCoordinate
            };

            if (_reverseGeoCodeOnProgress != null)
            {
                // GC may release old object.
                _reverseGeoCodeOnProgress.Page = null;
            }

            _reverseGeoCodeOnProgress = reverseGeoCode;
            _reverseGeoCodeOnProgress.DoReverseGeocode(geoCoordinate);
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
            _locationOverlay = new MapOverlay();
            _locationOverlay.Content = myCircle;
            _locationOverlay.PositionOrigin = new Point(0.5, 0.5);
            _locationOverlay.GeoCoordinate = geoCoordinate;

            // Create a MapLayer to contain the MapOverlay.
            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(_locationOverlay);

            mapWeatherInformation.Layers.Clear();

            // TODO: problems? user could press save location and if she is fast enough she
            // could not realize the location changed.
            // But well... She will realize later... So.. Is this really a problem?
            mapWeatherInformation.Center = geoCoordinate;
            mapWeatherInformation.ZoomLevel = 13;

            LocationTextCity.Text = city;
            LocationTextCountry.Text = country;
            // Add the MapLayer to the Map.
            mapWeatherInformation.Layers.Add(myLocationLayer);
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

            public static GeoCoordinate LocationToGeoCoordinate(Location locationItem)
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

            public static Location GeoCoordinateToLocation(GeoCoordinate geoCoordinate, string city, string country)
            {
                return new Location()
                {
                    Latitude = geoCoordinate.Latitude,
                    Longitude = geoCoordinate.Longitude,
                    Altitude = geoCoordinate.Altitude,
                    City = city,
                    Country = country,
                    HorizontalAccuracy = geoCoordinate.HorizontalAccuracy,
                    VerticalAccuracy = geoCoordinate.VerticalAccuracy,
                    Speed = geoCoordinate.Speed,
                    Course = geoCoordinate.Course,
                    IsSelected = true,
                    LastRemoteDataUpdate = null,
                };
            }
        }

        // TODO: check data before storing :(
        // http://stackoverflow.com/questions/4521435/what-specific-values-can-a-c-sharp-double-represent-that-a-sql-server-float-can
        private void StoreLocation(GeoCoordinate geocoordinate, string city, string country)
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
                    locationItem.City = city;
                    locationItem.Country = country;
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
                        City = city,
                        Country = country,
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

        private void mapWeatherInformation_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // TODO: if exception from here application will crash (I guess)
            var point = e.GetPosition(this.mapWeatherInformation);
            GeoCoordinate geocoordinate = this.mapWeatherInformation.ConvertViewportPointToGeoCoordinate(point);
            ShowProgressBar();
            ReverseGeocodeAndUpdateMap(geocoordinate);
        }

        private async void GetCurrentLocationButton_Click(object sender, RoutedEventArgs e)
        {
            //Geolocator geolocator = new Geolocator();
            //geolocator.DesiredAccuracyInMeters = 50;
            //if (geolocator.LocationStatus != PositionStatus.Ready)
            //{
            //    // TODO: to use ToastPrompt from the Coding4Fun Toolkit (using NuGet)
            //    return;
            //}
            
            // TODO: if exception from here application will crash (I guess)
            await this.GetCurrentLocationAndUpdateMap();
        }

        private void SaveLocationButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Could Center be null?
            StoreLocation(this.mapWeatherInformation.Center, this.LocationTextCity.Text, this.LocationTextCountry.Text);
        }

        private void ShowProgressBar()
        {
            GetCurrentLocationButton.IsEnabled = false;
            GetCurrentLocationButton.Visibility = Visibility.Collapsed;
            SaveLocationButton.IsEnabled = false;
            SaveLocationButton.Visibility = Visibility.Collapsed;
            ProgressBarRemoteData.IsEnabled = true;
            ProgressBarRemoteData.Visibility = Visibility.Visible;
        }

        private void RemoveProgressBar()
        {
            GetCurrentLocationButton.IsEnabled = true;
            GetCurrentLocationButton.Visibility = Visibility.Visible;
            SaveLocationButton.IsEnabled = true;
            SaveLocationButton.Visibility = Visibility.Visible;
            ProgressBarRemoteData.IsEnabled = false;
            ProgressBarRemoteData.Visibility = Visibility.Collapsed;
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
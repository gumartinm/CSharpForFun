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
using System.IO.IsolatedStorage;

namespace WeatherInformation
{
    public partial class MapPage : PhoneApplicationPage, ReverseGeoCode.IReverseGeoCode
    {
        private bool _isNewPageInstance;
        private ReverseGeoCode _reverseOnProgress;
        private Geolocator _geolocator;
        // TODO: how big is a MapLayer object?
        private MapOverlay _locationOverlay;
        private Location _restoreLocation;
        private GeoCoordinate _restoreReverseOnProgress;

        public MapPage()
        {
            InitializeComponent();

            _isNewPageInstance = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AskForLocationConsent();

            if (_isNewPageInstance)
            {
                if (State.Count > 0)
                {
                    if (State.ContainsKey("ReverseGeoCoorDinateOnProgress"))
                    {
                        this._restoreReverseOnProgress = (GeoCoordinate)State["ReverseGeoCoorDinateOnProgress"];
                    }
                    this._restoreLocation = (Location)State["CurrentChosenMapLocation"];
                }

                UpdateLocationStatus();
            }
            else
            {
                if (this._geolocator != null)
                {
                    this._geolocator.StatusChanged += GeolocationStatusCallback;
                }
            }
            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            _isNewPageInstance = false;

            RestoreUI();
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

                if (_reverseOnProgress != null)
                {
                    State["ReverseGeoCoorDinateOnProgress"] = _reverseOnProgress.CoorDinate;
                }
            }

            if (_geolocator != null)
            {
                _geolocator.StatusChanged -= GeolocationStatusCallback;
            }

            if (_reverseOnProgress != null)
            {
                _reverseOnProgress.Page = null;
            }
        }

        public void OnCompletedReverseGeoCode(GeoCoordinate geoCoordinate, string city, string country)
        {
            UpdateMap(geoCoordinate, city, country);
            RemoveProgressBar();
            _reverseOnProgress = null;
        }

        private async Task GetCurrentLocationAndUpdateMap(Geolocator geolocator)
        {
            try
            {
                var geoposition = await geolocator.GetGeopositionAsync(
                                    maximumAge: TimeSpan.FromSeconds(1),
                                    timeout: TimeSpan.FromSeconds(10)
                                );

                var currentGeoCoordinate = CoordinateHelper.ConvertGeocoordinate(geoposition.Coordinate);
                ShowProgressBar();
                ReverseGeocodeAndUpdateMap(currentGeoCoordinate);
            }
            catch (Exception ex)
            {
                // TODO: hopefully using main thread when Exception.
                if ((uint)ex.HResult == 0x80004004)
                {
                    // Location is disabled in phone settings.
                }

                // TODO: not sure if when exception I will be using the calling thread calling the await method.
                Dispatcher.BeginInvoke(new UpdateLocationButtonDelegate(this.UpdateLocationButton), false);
            }
        }

        private void ReverseGeocodeAndUpdateMap(GeoCoordinate geoCoordinate)
        {
            var reverseGeoCode = new ReverseGeoCode()
            {
                Page = this,
                CoorDinate = geoCoordinate
            };

            if (_reverseOnProgress != null)
            {
                // GC may release old object.
                _reverseOnProgress.Page = null;
            }

            _reverseOnProgress = reverseGeoCode;
            _reverseOnProgress.DoReverseGeocode(geoCoordinate);
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
            using (var db = new LocationDataContext(LocationDataContext.DBConnectionString))
            {
                // Define the query to gather all of the to-do items.
                // var toDoItemsInDB = from Location location in _locationDB.Locations where location.IsSelected select location;
                var locationItem = db.Locations.Where(location => location.IsSelected).FirstOrDefault();

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
            if (_geolocator == null)
            {
                // Nothing to do.
                return;
            }
            
            // TODO: if exception from here application will crash (I guess)
            await this.GetCurrentLocationAndUpdateMap(_geolocator);
        }

        private void SaveLocationButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Could Center be null?
            StoreLocation(this.mapWeatherInformation.Center, this.LocationTextCity.Text, this.LocationTextCountry.Text);
        }

        private async void UpdateLocationStatus()
        {
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
            {
                // The user has opted out of Location.
                GetCurrentLocationButton.IsEnabled = false;
                return;
            }

            _geolocator = new Geolocator();
            _geolocator.DesiredAccuracyInMeters = 50;
            _geolocator.ReportInterval = 1000;

            try
            {
                _geolocator.StatusChanged += GeolocationStatusCallback;
                var geoposition = await _geolocator.GetGeopositionAsync(
                                    maximumAge: TimeSpan.FromSeconds(1),
                                    timeout: TimeSpan.FromSeconds(10)
                                );
            }
            catch(Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // Location is disabled in phone settings.
                }

                // TODO: not sure if when exception I will be using the calling thread calling the await method.
                Dispatcher.BeginInvoke(new UpdateLocationButtonDelegate(this.UpdateLocationButton), false);
            }
        }

        // TODO: how to do it just with lambda expressions?
        delegate void UpdateLocationButtonDelegate(bool isEnabled);
        private void UpdateLocationButton(bool isEnabled)
        {
            GetCurrentLocationButton.IsEnabled = isEnabled;
        }

        private void GeolocationStatusCallback(Geolocator sender, StatusChangedEventArgs eventData)
        {
            if (eventData.Status == PositionStatus.Ready)
            {
                Dispatcher.BeginInvoke(new UpdateLocationButtonDelegate(this.UpdateLocationButton), true);
            }
            else
            {
                Dispatcher.BeginInvoke(new UpdateLocationButtonDelegate(this.UpdateLocationButton), false);
            }
        }

        private void AskForLocationConsent()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        private void RestoreUI()
        {
            Location location;
            if (this._restoreLocation != null)
            {
                location = this._restoreLocation;
            }
            else if (this._locationOverlay != null)
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

            if (this._restoreReverseOnProgress != null)
            {
                ShowProgressBar();
                ReverseGeocodeAndUpdateMap(this._restoreReverseOnProgress);
            }
            else
            {
                RemoveProgressBar();
            }
        }

        private void ShowProgressBar()
        {
            GetCurrentLocationButton.Visibility = Visibility.Collapsed;
            SaveLocationButton.IsEnabled = false;
            SaveLocationButton.Visibility = Visibility.Collapsed;
            ProgressBarRemoteData.IsEnabled = true;
            ProgressBarRemoteData.Visibility = Visibility.Visible;
        }

        private void RemoveProgressBar()
        {
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
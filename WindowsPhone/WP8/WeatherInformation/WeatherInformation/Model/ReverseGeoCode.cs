using Microsoft.Phone.Maps.Services;
using System;
using System.Collections.Generic;
using System.Device.Location;
using WeatherInformation.Resources;

namespace WeatherInformation.Model
{
    class ReverseGeoCode
    {
        public interface IReverseGeoCode
        {
            void OnCompletedReverseGeoCode(GeoCoordinate geoCoordinate, string city, string country);
        }

        public IReverseGeoCode Page { get; set; }
        public GeoCoordinate CoorDinate { get; set; }

        public void DoReverseGeocode(GeoCoordinate geoCoordinate)
        {
            var currentReverseGeocodeQuery = new ReverseGeocodeQuery();
            currentReverseGeocodeQuery.GeoCoordinate = geoCoordinate;
            currentReverseGeocodeQuery.QueryCompleted += QueryCompletedCallback;
            currentReverseGeocodeQuery.QueryAsync();
        }

        /// <summary>
        /// It must be running in main thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventData"></param>
        private void QueryCompletedCallback(object sender, QueryCompletedEventArgs<IList<MapLocation>> eventData)
        {
            // Commenting out because I have to disable progress dialog even if the async task was cancelled.
            //if (eventData.Cancelled)
            //{
            //    // Be careful!!! If you throw exception from this point your program will finish with "Unhandled Exception".  
            //    return;
            //}

            ReverseGeocodeQuery reverseGeocodeQuery = sender as ReverseGeocodeQuery;

            // Default values
            var city = AppResources.DefaultCity;
            var country = AppResources.DefaultCountry;

            Exception errorException = eventData.Error;
            if (errorException == null)
            {
                if (eventData.Result.Count > 0)
                {
                    if (eventData.Result[0].Information != null
                        && eventData.Result[0].Information.Address != null)
                    {
                        var address = eventData.Result[0].Information.Address;
                        city = string.IsNullOrEmpty(address.City) ? AppResources.DefaultCity : address.City;
                        country = string.IsNullOrEmpty(address.Country) ? AppResources.DefaultCountry : address.Country;
                    }
                }
            }

            if (Page != null)
            {
                Page.OnCompletedReverseGeoCode(reverseGeocodeQuery.GeoCoordinate, city,country);
            }
        }
    }
}

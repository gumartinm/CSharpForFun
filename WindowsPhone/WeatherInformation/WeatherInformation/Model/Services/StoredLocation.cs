using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherInformation.Model.Services
{
    // TODO: If I want to store more than one place I should use a database :(
    class StoredLocation
    {
        public static double CurrentLatitude
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("CurrentLatitude"))
                {
                    return (double)IsolatedStorageSettings.ApplicationSettings["CurrentLatitude"];
                }
                // TODO: what if settings does not contain this value? :/
                return 0;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["CurrentLatitude"] = value;
            }
        }

        public static double CurrentLongitude
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("CurrentLongitude"))
                {
                    return (double)IsolatedStorageSettings.ApplicationSettings["CurrentLongitude"];
                }
                // TODO: what if settings does not contain this value? :/
                return 0;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["CurrentLongitude"] = value;
            }
        }

        public static string City
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("City"))
                {
                    return (string)IsolatedStorageSettings.ApplicationSettings["City"];
                }
                return null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["City"] = value;
            }
        }

        public static string Country
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("Country"))
                {
                    return (string)IsolatedStorageSettings.ApplicationSettings["Country"];
                }
                return null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["Country"] = value;
            }
        }

        public static bool IsNewLocation
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("IsNewLocation"))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings["IsNewLocation"];
                }
                return false;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["IsNewLocation"] = value;
            }
        }
    }
}

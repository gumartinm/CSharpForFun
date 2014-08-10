using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherInformation.Model
{
    public class LocationDataContext : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Data Source=isostore:/Locations.sdf";

        // Pass the connection string to the base class.
        public LocationDataContext(string connectionString)
            : base(connectionString)
        { }

        // Specify a single table for the location items.
        public Table<Location> Locations;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherInformation.Model
{
    [Table]
    public class Location : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        private int _itemId;
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ItemId
        {
            get
            {
                return _itemId;
            }
            set
            {
                if (_itemId != value)
                {
                    NotifyPropertyChanging("ItemId");
                    _itemId = value;
                    NotifyPropertyChanged("ItemId");
                }
            }
        }

        private bool _isSelected;
        [Column(CanBeNull = false)]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    NotifyPropertyChanging("IsSelected");
                    _isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        private double _latitude;
        [Column(CanBeNull = false)]
        public double Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                if (_latitude != value)
                {
                    NotifyPropertyChanging("Latitude");
                    _latitude = value;
                    NotifyPropertyChanged("Latitude");
                }
            }
        }

        private double _longitude;
        [Column(CanBeNull = false)]
        public double Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                if (_longitude != value)
                {
                    NotifyPropertyChanging("Longitude");
                    _longitude = value;
                    NotifyPropertyChanged("Longitude");
                }
            }
        }

        private double _altitude;
        [Column(CanBeNull = false)]
        public double Altitude
        {
            get
            {
                return _altitude;
            }
            set
            {
                if (_altitude != value)
                {
                    NotifyPropertyChanging("Altitude");
                    _altitude = value;
                    NotifyPropertyChanged("Altitude");
                }
            }
        }

        private double _horizontalAccuracy;
        [Column(CanBeNull = false)]
        public double HorizontalAccuracy
        {
            get
            {
                return _horizontalAccuracy;
            }
            set
            {
                if (_horizontalAccuracy != value)
                {
                    NotifyPropertyChanging("HorizontalAccuracy");
                    _horizontalAccuracy = value;
                    NotifyPropertyChanged("HorizontalAccuracy");
                }
            }
        }

        private double _verticalAccuracy;
        [Column(CanBeNull = false)]
        public double VerticalAccuracy
        {
            get
            {
                return _verticalAccuracy;
            }
            set
            {
                if (_verticalAccuracy != value)
                {
                    NotifyPropertyChanging("VerticalAccuracy");
                    _verticalAccuracy = value;
                    NotifyPropertyChanged("VerticalAccuracy");
                }
            }
        }

        private double _speed;
        [Column(CanBeNull = false)]
        public double Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                if (_speed != value)
                {
                    NotifyPropertyChanging("Speed");
                    _speed = value;
                    NotifyPropertyChanged("Speed");
                }
            }
        }

        private double _course;
        [Column(CanBeNull = false)]
        public double Course
        {
            get
            {
                return _course;
            }
            set
            {
                if (_course != value)
                {
                    NotifyPropertyChanging("Course");
                    _course = value;
                    NotifyPropertyChanged("Course");
                }
            }
        }

        private string _city;
        [Column(CanBeNull = false)]
        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                if (_city != value)
                {
                    NotifyPropertyChanging("City");
                    _city = value;
                    NotifyPropertyChanged("City");
                }
            }
        }

        private string _country;
        [Column(CanBeNull = false)]
        public string Country
        {
            get
            {
                return _country;
            }
            set
            {
                if (_country != value)
                {
                    NotifyPropertyChanging("Country");
                    _country = value;
                    NotifyPropertyChanged("Country");
                }
            }
        }

        private bool _isNewLocation;
        [Column(CanBeNull = false)]
        public bool IsNewLocation
        {
            get
            {
                return _isNewLocation;
            }
            set
            {
                if (_isNewLocation != value)
                {
                    NotifyPropertyChanging("IsNewLocation");
                    _isNewLocation = value;
                    NotifyPropertyChanged("IsNewLocation");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}

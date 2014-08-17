using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WeatherInformation.Resources;

namespace WeatherInformation.ViewModels
{
    // TODO: IMHO INotifyPropertyChanged does not do anything useful in this class... NotifyPropertyChanged has no effect :(
    public class SettingsViewModel : INotifyPropertyChanged
    {
        // Settings
        private readonly IsolatedStorageSettings _settings;

        // The key names of _settings
        private const string _languageSelectionSettingKeyName = "LanguageSelection";
        private const string _temperatureUnitsSelectionSettingKeyName = "TemperatureUnitsSelection";
        private const string _forecastDayNumbersSelectionSelectionSettingKeyName = "ForecastDayNumbersSelection";

        // The default value of ListPicker _settings
        private const int _languageSelectionSettingDefault = 0;
        private const int _temperatureUnitsSelectionSettingDefault = 0;
        private const int _forecastDayNumbersSelectionSettingDefault = 0;


        public SettingsViewModel()
        {
            // You need to add a check to DesignerProperties.IsInDesignTool to that code since accessing
            // IsolatedStorageSettings in Visual Studio or Expression Blend is invalid.
            // see: http://stackoverflow.com/questions/7294461/unable-to-determine-application-identity-of-the-caller
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                // Get the _settings for this application.
                _settings = IsolatedStorageSettings.ApplicationSettings;
            }
        }

        /// <summary>
        /// Property to get and set language selection Setting Key.
        /// </summary>
        public int LanguageSelectionSetting
        {
            get
            {
                return GetValueOrDefault<int>(_languageSelectionSettingKeyName, _languageSelectionSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(_languageSelectionSettingKeyName, value))
                {
                    Save();
                }
                NotifyPropertyChanged(_languageSelectionSettingKeyName);
            }
        }

        /// <summary>
        /// Property to get and set temperature units selection Setting Key.
        /// </summary>
        public int TemperaruteUnitsSelectionSetting
        {
            get
            {
                return GetValueOrDefault<int>(_temperatureUnitsSelectionSettingKeyName, _temperatureUnitsSelectionSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(_temperatureUnitsSelectionSettingKeyName, value))
                {
                    Save();
                }
                NotifyPropertyChanged(_temperatureUnitsSelectionSettingKeyName);
            }
        }

        /// <summary>
        /// Property to get and set forecast day numbers selection Setting Key.
        /// </summary>
        public int ForecastDayNumbersSelectionSetting
        {
            get
            {
                return GetValueOrDefault<int>(_forecastDayNumbersSelectionSelectionSettingKeyName, _forecastDayNumbersSelectionSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(_forecastDayNumbersSelectionSelectionSettingKeyName, value))
                {
                    Save();
                }
                NotifyPropertyChanged(_forecastDayNumbersSelectionSelectionSettingKeyName);
            }
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // You need to add a check to DesignerProperties.IsInDesignTool to that code since accessing
            // IsolatedStorageSettings in Visual Studio or Expression Blend is invalid.
            // see: http://stackoverflow.com/questions/7294461/unable-to-determine-application-identity-of-the-caller
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                return defaultValue;
            }

            // If the key exists, retrieve the value.
            if (_settings.Contains(Key))
            {
                value = (T)_settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Update a setting value for application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // You need to add a check to DesignerProperties.IsInDesignTool to that code since accessing
            // IsolatedStorageSettings in Visual Studio or Expression Blend is invalid.
            // see: http://stackoverflow.com/questions/7294461/unable-to-determine-application-identity-of-the-caller
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                return false;
            }

            // If the key exists
            if (_settings.Contains(Key))
            {
                // If the value has changed
                if (_settings[Key] != value)
                {
                    // Store the new value
                    _settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                _settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Save the _settings.
        /// </summary>
        private void Save()
        {
            // You need to add a check to DesignerProperties.IsInDesignTool to that code since accessing
            // IsolatedStorageSettings in Visual Studio or Expression Blend is invalid.
            // see: http://stackoverflow.com/questions/7294461/unable-to-determine-application-identity-of-the-caller
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                return;
            }

            _settings.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

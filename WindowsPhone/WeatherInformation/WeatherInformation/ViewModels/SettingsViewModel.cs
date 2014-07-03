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
    public class SettingsViewModel
    {
        // Settings
        private IsolatedStorageSettings settings;

        // The key names of settings
        private const string _languageSelectionSettingKeyName = "LanguageSelection";
        private const string _temperatureUnitsSelectionSettingKeyName = "TemperatureUnitsSelection";

        // The default value of ListPicker settings
        private const int _languageSelectionSettingDefault = 0;
        private const int _temperatureUnitsSelectionSettinDefault = 0;


        public SettingsViewModel()
        {
            // You need to add a check to DesignerProperties.IsInDesignTool to that code since accessing
            // IsolatedStorageSettings in Visual Studio or Expression Blend is invalid.
            // see: http://stackoverflow.com/questions/7294461/unable-to-determine-application-identity-of-the-caller
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                // Get the settings for this application.
                settings = IsolatedStorageSettings.ApplicationSettings;
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
            }
        }

        /// <summary>
        /// Property to get and set temperature units selection Setting Key.
        /// </summary>
        public int TemperaruteUnitsSelectionSetting
        {
            get
            {
                return GetValueOrDefault<int>(_temperatureUnitsSelectionSettingKeyName, _temperatureUnitsSelectionSettinDefault);
            }
            set
            {
                if (AddOrUpdateValue(_temperatureUnitsSelectionSettingKeyName, value))
                {
                    Save();
                }
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
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
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
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Save the settings.
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

            settings.Save();
        }
    }
}

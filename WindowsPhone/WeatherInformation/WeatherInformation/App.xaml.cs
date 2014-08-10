﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading; 
using System.Globalization; 
using WeatherInformation.Resources;
using WeatherInformation.ViewModels;
using System.IO.IsolatedStorage;
using System.IO;
using WeatherInformation.Model;
using WeatherInformation.Model.Services;
using System.Threading.Tasks;
using WeatherInformation.Model.ForecastWeatherParser;
using WeatherInformation.Model.JsonDataParser;
using WeatherInformation.Model.CurrentWeatherParser;

namespace WeatherInformation
{
    public partial class App : Application
    {
        // Locale to force CurrentCulture to in InitializeLanguage(). 
        // Use "qps-PLOC" to deploy pseudolocalized strings. 
        // Use "" to let user Phone Language selection determine locale. 
        // public static String appForceCulture = "qps-PLOC";
        private const String appForceCulture = "en"; 

        // Declare a private variable to store application state.
        private WeatherData _remoteWeatherData;

        // Declare an event for when the application data changes.
        public event EventHandler ApplicationDataObjectChanged;

        // Declare a public property to access the application data variable.
        public WeatherData ApplicationDataObject
        {
            get { return _remoteWeatherData; }
            set
            {
                if (value != _remoteWeatherData)
                {
                    _remoteWeatherData = value;
                    OnApplicationDataObjectChanged(EventArgs.Empty);
                }
            }
        }

        // Create a method to raise the ApplicationDataObjectChanged event.
        protected void OnApplicationDataObjectChanged(EventArgs e)
        {
            EventHandler handler = ApplicationDataObjectChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Proporcionar acceso sencillo al marco raíz de la aplicación telefónica.
        /// </summary>
        /// <returns>Marco raíz de la aplicación telefónica.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor para el objeto Application.
        /// </summary>
        public App()
        {
            // Controlador global para excepciones no detectadas.
            UnhandledException += Application_UnhandledException;

            // Inicialización XAML estándar
            InitializeComponent();

            // Inicialización especifica del teléfono
            InitializePhoneApplication();

            // Inicialización del idioma
            InitializeLanguage();

            // Create the database if it does not exist.
            using (LocationDataContext db = new LocationDataContext(LocationDataContext.DBConnectionString))
            {
                if (db.DatabaseExists() == false)
                {
                    //Create the database
                    db.CreateDatabase();
                }
            }

            // Mostrar información de generación de perfiles gráfica durante la depuración.
            if (Debugger.IsAttached)
            {
                // Mostrar los contadores de velocidad de marcos actual
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Mostrar las áreas de la aplicación que se están volviendo a dibujar en cada marco.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Habilitar el modo de visualización de análisis de no producción,
                // que muestra áreas de una página que se entregan a la GPU con una superposición coloreada.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Impedir que la pantalla se apague mientras se realiza la depuración deshabilitando
                // la detección de inactividad de la aplicación.
                // Precaución: solo debe usarse en modo de depuración. Las aplicaciones que deshabiliten la detección de inactividad del usuario seguirán en ejecución
                // y consumirán energía de la batería cuando el usuario no esté usando el teléfono.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        // no way of moving temporary file atomically with IsolatedStorageFile. LINUX OWNS MICROSOFT.
        private void SaveDataToIsolatedStorage(string fileName, string value)
        {
            string pathToTemporaryFile = CreateTemporaryFile(fileName);

            SaveDataToTemporaryFile(pathToTemporaryFile, value);

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // NO FUCKING WAY, MoveFile throws exception if destination file exists!!!!
                // And documentation doesn't say anything... ROFL
                // Linux OWNS Microsoft...
                // If there are corrupted files do not blame me.... Unbelievable....
                if (isoStore.FileExists(fileName))
                {
                    isoStore.DeleteFile(fileName);
                }
                isoStore.MoveFile(pathToTemporaryFile, fileName);
            }
        }

        private string CreateTemporaryFile(string fileName)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isoStore.CreateDirectory("tmp");
            }

            return Path.Combine("tmp", fileName);
        }

        private void SaveDataToTemporaryFile(string pathToTemporaryFile, string value)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            using (IsolatedStorageFileStream fileStream = isoStore.OpenFile(pathToTemporaryFile, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fileStream))
            {
                sw.Write(value);
                fileStream.Flush(true);
            }
        }



        // Código para ejecutar cuando la aplicación se inicia (p.ej. a partir de Inicio)
        // Este código no se ejecutará cuando la aplicación se reactive
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Código para ejecutar cuando la aplicación se activa (se trae a primer plano)
        // Este código no se ejecutará cuando la aplicación se inicie por primera vez
        // Coming from TOMBSTONED or DORMANT
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (e.IsApplicationInstancePreserved)
            {
                // Coming from DORMANT
                return;
            }

            // Coming from TOMBSTONED
            WeatherData weatherData = null;

            // Check to see if the key for the application state data is in the State dictionary.
            string JSONRemoteForecastWeather = null;
            string JSONRemoteCurrentWeather = null;
            if (PhoneApplicationService.Current.State.ContainsKey("JSONRemoteForecastWeather") &&
                PhoneApplicationService.Current.State.ContainsKey("JSONRemoteCurrentWeather"))
            {
                // If it exists, assign the data to the application member variable.
                JSONRemoteForecastWeather = PhoneApplicationService.Current.State["JSONRemoteForecastWeather"] as string;

                // If it exists, assign the data to the application member variable.
                JSONRemoteCurrentWeather = PhoneApplicationService.Current.State["JSONRemoteCurrentWeather"] as string;
            }
            
            if (!string.IsNullOrEmpty(JSONRemoteCurrentWeather) && !string.IsNullOrEmpty(JSONRemoteForecastWeather))
            {
                weatherData = WeatherDataParser(JSONRemoteForecastWeather, JSONRemoteCurrentWeather);
            }

            ApplicationDataObject = weatherData;
        }

        // Código para ejecutar cuando la aplicación se desactiva (se envía a segundo plano)
        // Este código no se ejecutará cuando la aplicación se cierre
        // TODO: no siempre pasa por aqui cuando se lanza una nueva ventana de la misma aplicación.
        //       Y POR ESO HAY PROBLEMAS CON GetDataAsync PORQUE ESTO NO TIENE NADA DE NADA Y DEVUELVE NULL
        //       Y SIEMPRE HACE UNA PETICIÓN AL SERVIDOR :(
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // If there is data in the application member variable...
            var weatherData = ApplicationDataObject;
            if (weatherData != null)
            {
                if (!string.IsNullOrEmpty(weatherData.JSONRemoteCurrent) &&
                    !string.IsNullOrEmpty(weatherData.JSONRemoteForecast))
                {
                    // Store it in the State dictionary.
                    PhoneApplicationService.Current.State["JSONRemoteForecastWeather"] = weatherData.JSONRemoteForecast;

                    // Also store it in isolated storage, in case the application is never reactivated.
                    SaveDataToIsolatedStorage("JSONRemoteForecastWeatherFile.txt", weatherData.JSONRemoteForecast);

                    // Store it in the State dictionary.
                    PhoneApplicationService.Current.State["JSONRemoteCurrentWeather"] = weatherData.JSONRemoteCurrent;

                    // Also store it in isolated storage, in case the application is never reactivated.
                    SaveDataToIsolatedStorage("JSONRemoteCurrentWeatherFile.txt", weatherData.JSONRemoteCurrent);

                    IsolatedStorageSettings.ApplicationSettings["DataLastSavedTime"] = DateTime.Now;
                }
            }
        }

        // Código para ejecutar cuando la aplicación se cierra (p.ej., al hacer clic en Atrás)
        // Este código no se ejecutará cuando la aplicación se desactive
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            // Asegurarse de que el estado de la aplicación requerida persiste aquí.
            // The application will not be tombstoned, so save only to isolated storage.
            var weatherData = ApplicationDataObject;
            if (weatherData != null)
            {
                if (!string.IsNullOrEmpty(weatherData.JSONRemoteForecast) &&
                    !string.IsNullOrEmpty(weatherData.JSONRemoteCurrent))
                {
                    // Also store it in isolated storage, in case the application is never reactivated.
                    SaveDataToIsolatedStorage("JSONRemoteForecastWeatherFile.txt", weatherData.JSONRemoteForecast);

                    // Also store it in isolated storage, in case the application is never reactivated.
                    SaveDataToIsolatedStorage("JSONRemoteCurrentWeatherFile.txt", weatherData.JSONRemoteCurrent);

                    IsolatedStorageSettings.ApplicationSettings["DataLastSavedTime"] = DateTime.Now;
                }
            }
        }     

        // Código para ejecutar si hay un error de navegación
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // Ha habido un error de navegación; interrumpir el depurador
                Debugger.Break();
            }
        }

        // Código para ejecutar en excepciones no controladas
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // Se ha producido una excepción no controlada; interrumpir el depurador
                Debugger.Break();
            }
        }

        #region Inicialización de la aplicación telefónica

        // Evitar inicialización doble
        private bool phoneApplicationInitialized = false;

        // No agregar ningún código adicional a este método
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Crear el marco pero no establecerlo como RootVisual todavía; esto permite que
            // la pantalla de presentación permanezca activa hasta que la aplicación esté lista para la presentación.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Controlar errores de navegación
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Controlar solicitudes de restablecimiento para borrar la pila de retroceso
            RootFrame.Navigated += CheckForResetNavigation;

            // Asegurarse de que no volvemos a inicializar
            phoneApplicationInitialized = true;
        }

        // No agregar ningún código adicional a este método
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Establecer el objeto visual raíz para permitir que la aplicación se presente
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Quitar este controlador porque ya no es necesario
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // Si la aplicación ha recibido una navegación 'reset', tenemos que comprobarlo
            // en la siguiente navegación para ver si se debe restablecer la pila de páginas
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Anular registro del evento para que no se vuelva a llamar
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Borrar solo la pila de navegaciones 'new' (hacia delante) y 'refresh'
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // Por coherencia de la IU, borrar toda la pila de páginas
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // no hacer nada
            }
        }

        #endregion

        // Inicializar la fuente y la dirección de flujo de la aplicación según se define en sus cadenas de recursos traducidas.
        //
        // Para asegurarse de que la fuente de la aplicación está alineada con sus idiomas admitidos y que
        // FlowDirection para todos esos idiomas sigue su dirección tradicional, ResourceLanguage
        // y ResourceFlowDirection se debe inicializar en cada archivo resx para que estos valores coincidan con ese
        // referencia cultural del archivo. Por ejemplo:
        //
        // AppResources.es-ES.resx
        //    El valor de ResourceLanguage debe ser "es-ES"
        //    El valor de ResourceFlowDirection debe ser "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     El valor de ResourceLanguage debe ser "ar-SA"
        //     El valor de ResourceFlowDirection debe ser "RightToLeft"
        //
        // Para obtener más información sobre cómo traducir aplicaciones para Windows Phone, consulta http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Force CurrentUICulture to locale defined by appForceCulture.
                // An empty string allows the user's Phone Language setting to 
                // determine the locale.
                //if (Debugger.IsAttached && String.IsNullOrWhiteSpace(appForceCulture) == false)
                //{
                Thread.CurrentThread.CurrentCulture = new CultureInfo(appForceCulture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(appForceCulture);
                //}

                // Establecer la fuente para que coincida con el idioma definido por
                // Cadena de recursos ResourceLanguage para cada idioma admitido.
                //
                // Recurrir a la fuente del idioma neutro si el idioma
                // del teléfono no se admite.
                //
                // Si se produce un error del compilador, falta ResourceLanguage
                // el archivo de recursos.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Establecer FlowDirection de todos los elementos del marco raíz según
                // en la cadena de recursos ResourceFlowDirection para cada
                // idioma admitido.
                //
                // Si se produce un error del compilador, falta ResourceFlowDirection
                // el archivo de recursos.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // Si se detecta aquí una excepción, lo más probable es que se deba a
                // ResourceLanguage no se ha establecido correctamente en un idioma admitido
                // o ResourceFlowDirection se ha establecido en un valor distinto de LeftToRight
                // o RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}
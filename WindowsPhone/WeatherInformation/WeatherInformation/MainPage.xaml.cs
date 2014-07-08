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
using System.Windows.Controls;
using System.Windows.Navigation;
using WeatherInformation.Resources;
using WeatherInformation.ViewModels;

namespace WeatherInformation
{
    public partial class MainPage : PhoneApplicationPage
    {
        private MainViewModel _mainViewModel;
        private bool _isNewPageInstance = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();


            // Establecer el contexto de datos del control ListBox control en los datos de ejemplo
            DataContext = App.MainViewModel;


            _isNewPageInstance = true;

            // Set the event handler for when the application data object changes.
            (Application.Current as WeatherInformation.App).ApplicationDataObjectChanged +=
                          new EventHandler(MainPage_ApplicationDataObjectChanged);


            // Código de ejemplo para traducir ApplicationBar
            //BuildLocalizedApplicationBar();
        }


        // Cargar datos para los elementos MainViewModel
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.MainViewModel.IsDataLoaded)
            {
                App.MainViewModel.LoadData();
            }


            // If _isNewPageInstance is true, the page constructor has been called, so
            // state may need to be restored.
            if (_isNewPageInstance)
            {
                // If the application member variable is not empty,
                // set the page's data object from the application member variable.
                if ((Application.Current as WeatherInformation.App).ApplicationDataObject != null)
                {
                    UpdateApplicationDataUI();
                }
                else
                {
                    // Otherwise, call the method that loads data.
                    //statusTextBlock.Text = "getting data...";
                    (Application.Current as WeatherInformation.App).GetDataAsync();
                }
            }

            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            _isNewPageInstance = false;
        }

        // The event handler called when the ApplicationDataObject changes.
        void MainPage_ApplicationDataObjectChanged(object sender, EventArgs e)
        {
            // Call UpdateApplicationData on the UI thread.
            Dispatcher.BeginInvoke(() => UpdateApplicationDataUI());
        }
        void UpdateApplicationDataUI()
        {
            // Set the ApplicationData and ApplicationDataStatus members of the ViewModel
            // class to update the UI.
            // dataTextBlock.Text = (Application.Current as ExecutionModelApplication.App).ApplicationDataObject;
            // statusTextBlock.Text = (Application.Current as ExecutionModelApplication.App).ApplicationDataStatus;
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Location_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MapPage.xaml", UriKind.Relative));
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        // Código de ejemplo para compilar una ApplicationBar traducida
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Establecer ApplicationBar de la página en una nueva instancia de ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Crear un nuevo botón y establecer el valor de texto en la cadena traducida de AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Crear un nuevo elemento de menú con la cadena traducida de AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}
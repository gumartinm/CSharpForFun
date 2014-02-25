using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Example.RemoteAgents.WindowsPhone.Resources;
using RemoteAgents.WindowsPhone.View;

namespace RemoteAgents
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly ViewImpl view;

        // Constructor
        public MainPage()
        {
            view = new ViewImpl();

            InitializeComponent();

            this.registerEvents();
            // Código de ejemplo para traducir ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        async private void ButtonRetrieveRemoteData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string currentDate = await view.getCurrentDate();
                if (currentDate != null)
                {
                    this.CurrentDateTextBox.Text = currentDate;
                }
            }
            catch (Exception exception)
            {
                //TODO: logger for Windows Phone 8 :(
                Console.WriteLine("ButtonGetDateClicked. Message: {0}  Stacktrace: {1}", exception.Message, exception.StackTrace);
            }
        }

        private void registerEvents ()
        {
            this.ButtonRetrieveRemoteData.Click += ButtonRetrieveRemoteData_Click;
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
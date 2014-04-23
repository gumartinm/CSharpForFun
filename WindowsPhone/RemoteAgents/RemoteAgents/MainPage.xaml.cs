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
using NLog;

namespace RemoteAgents
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly ViewImpl _view;

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        // Constructor
        public MainPage()
        {
            _view = new ViewImpl();

            InitializeComponent();

            this.registerEvents();
            // Código de ejemplo para traducir ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        async private void ButtonRetrieveRemoteData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CurrentDateTextBox.Text = await _view.GetCurrentDateAsync();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("ButtonGetDateClicked error: ", exception);
            }
        }

        async private void SendDataButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                int number = 666;
                await _view.SetWriteTextAsync(this.TextBoxSendText.Text,
                                              number);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("SendDataButtonClicked error: ", exception);
            }
        }

        private void registerEvents ()
        {
            this.ButtonRetrieveRemoteData.Click += ButtonRetrieveRemoteData_Click;
            this.ButtonSendRemoteData.Click += SendDataButtonClicked;
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
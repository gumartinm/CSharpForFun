using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MVVMTestApp.Resources;
using MVVMTestApp.ViewModelNamespace;

namespace MVVMTestApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ViewModel vm;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            vm = new ViewModel();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!StateUtilities.IsLaunching && this.State.ContainsKey("Accomplishments"))
            {
                // Old instance of the application
                // The user started the application from the Back button.

                vm = (ViewModel)this.State["Accomplishments"];
                MessageBox.Show("Got data from state");
            }
            else
            {
                // New instance of the application
                // The user started the application from the application list,
                // or there is no saved state available.

                vm.GetAccomplishments();
                MessageBox.Show("Did not get data from state");
            }


            // There are two different views, but only one view model.
            // So, use LINQ queries to populate the views.

            // Set the data context for the Item view.
            ItemViewOnPage.DataContext = from Accomplishment in vm.Accomplishments where Accomplishment.Type == "Item" select Accomplishment;

            // Set the data context for the Level view.
            LevelViewOnPage.DataContext = from Accomplishment in vm.Accomplishments where Accomplishment.Type == "Level" select Accomplishment;

            // If there is only one view, you could use the following code
            // to populate the view.
            //AccomplishmentViewOnPage.DataContext = vm.Accomplishments;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (this.State.ContainsKey("Accomplishments"))
            {
                this.State["Accomplishments"] = vm;
            }
            else
            {
                this.State.Add("Accomplishments", vm);
            }

            StateUtilities.IsLaunching = false;
        }

        private void AppBarSave_Click(object sender, EventArgs e)
        {
            vm.SaveAccomplishments();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}
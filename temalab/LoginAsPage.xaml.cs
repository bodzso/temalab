using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using temalab.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginAsPage : Page
    {
        private KnownUser _knownUser = new KnownUser();

        public LoginAsPage()
        {
            this.InitializeComponent();
        }

        private void ToRegisterPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterPage), typeof(LoginAsPage));
        }

        private void ToLoginPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            knownUsersList.ItemsSource = _knownUser.GetKnownUsers();
        }

        private void knownUsersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            controls.Visibility = Visibility.Visible;
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            var app = ((App)Application.Current);
            if (await app.Login(((KnownUser)knownUsersList.SelectedItem).Username, password.Password))
                this.Frame.Navigate(typeof(MainPage));
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Web.Http;
using System.Text;
using System.Diagnostics;
using temalab.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private void ToRegisterPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterPage), typeof(LoginPage));
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            var app = ((App)Application.Current);

            if(await app.Login(username.Text, password.Password))
            {
                if (rememberMeCheck.IsChecked == true)
                {
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    string rememberedUsers = (string)localSettings.Values["rememberedUsers"];

                    if (string.IsNullOrEmpty(rememberedUsers))
                    {
                        var user = JsonSerializer.Serialize(new List<UserModel> { app.user });
                        localSettings.Values["rememberedUsers"] = user;
                    }
                    else
                    {
                        var users = JsonSerializer.Deserialize<List<UserModel>>(rememberedUsers);

                        if (users.Any(u => u.id == app.user.id))
                            return;
                        else
                        {
                            users.Add(app.user);
                            localSettings.Values["rememberedUsers"] = JsonSerializer.Serialize(users);
                        }
                    }
                }

                this.Frame.Navigate(typeof(MainPage));
            }
        }

        private void password_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                Login(sender, e);
        }
    }
}

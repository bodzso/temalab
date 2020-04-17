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
using System.Text.Json;
using System.Text.Json.Serialization;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        App app = Application.Current as App;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            app.Logout();
        }

        private async void updateButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };

            var updatedUser = new UserModel { id = app.user.id, firstName = firstName.Text, lastName = lastName.Text, email = email.Text, password = password.Password };
            var res = await app.PutJson(new Uri("http://localhost:60133/users/" + app.user.id), JsonSerializer.Serialize(updatedUser, options));
            
            if(res)
            {
                app.user.firstName = updatedUser.firstName;
                app.user.lastName = updatedUser.lastName;
                app.user.email = updatedUser.email;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Windows.Web.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text;
using temalab.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        private Type prevPage;
        App app = Application.Current as App;

        public RegisterPage()
        {
            this.InitializeComponent();
        }

        private async void registerclick(object sender, RoutedEventArgs e)
        {
            var user = new UserModel
            {
                username = username.Text,
                firstName = firstName.Text,
                lastName = lastName.Text,
                email = email.Text,
                password = password.Password
            };
            
            var res = await app.PostJson(new Uri($"{app.baseuri}/users/register"), JsonSerializer.Serialize(user));
            Debug.WriteLine(res);

            if (!string.IsNullOrEmpty(res))
                this.Frame.Navigate(typeof(LoginPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            prevPage = (Type)e.Parameter;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(prevPage);
        }

        private void password_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                registerclick(sender, e);
        }
    }
}

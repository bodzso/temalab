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
                        using (var stream = new MemoryStream())
                        {
                            using (var writer = new Utf8JsonWriter(stream))
                            {
                                writer.WriteStartArray();
                                writer.WriteStringValue(username.Text);
                                writer.WriteEndArray();
                            }

                            rememberedUsers = Encoding.UTF8.GetString(stream.ToArray());
                            localSettings.Values["rememberedUsers"] = rememberedUsers;
                        }
                    }
                    else
                    {
                        using (JsonDocument document = JsonDocument.Parse(rememberedUsers))
                        {
                            var root = document.RootElement;
                            using (var stream = new MemoryStream())
                            {
                                bool skip = false;
                                using (var writer = new Utf8JsonWriter(stream))
                                {
                                    writer.WriteStartArray();

                                    foreach (var value in root.EnumerateArray())
                                    {
                                        if (value.GetString() == username.Text)
                                        {
                                            skip = true;
                                            break;
                                        }

                                        value.WriteTo(writer);
                                    }

                                    if (!skip)
                                    {
                                        writer.WriteStringValue(username.Text);
                                        writer.WriteEndArray();
                                    }
                                }

                                if (!skip)
                                {
                                    rememberedUsers = Encoding.UTF8.GetString(stream.ToArray());
                                    localSettings.Values["rememberedUsers"] = rememberedUsers;
                                }
                            }
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

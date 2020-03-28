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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        private Type prevPage;

        public RegisterPage()
        {
            this.InitializeComponent();
        }

        private async void registerclick(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                Uri uri = new Uri("http://localhost:60133/users/register");

                string json;
                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream))
                    {
                        writer.WriteStartObject();
                        writer.WriteString("username", username.Text);
                        writer.WriteString("firstName", firstName.Text);
                        writer.WriteString("lastName", lastName.Text);
                        writer.WriteString("email", email.Text);
                        writer.WriteString("password", password.Password);
                        writer.WriteEndObject();
                    }

                    json = Encoding.UTF8.GetString(stream.ToArray());
                    Console.WriteLine(json);
                }

                HttpStringContent content = new HttpStringContent(json, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(uri, content);
                httpResponseMessage.EnsureSuccessStatusCode();
                var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                Debug.WriteLine(httpResponseBody);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            prevPage = (Type)e.Parameter;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(prevPage);
        }
    }
}

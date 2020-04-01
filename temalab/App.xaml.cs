using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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
using Windows.Web.Http.Headers;

namespace temalab
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public string currentUsername { get; set; }
        public bool currentuserIsNew { get; set; } = true;
        public int currentUserId { get; set; }
        public string currentToken { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter

                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                    if (localSettings.Values["rememberedUsers"] == null)
                        rootFrame.Navigate(typeof(LoginPage), e.Arguments);
                    else
                        rootFrame.Navigate(typeof(LoginAsPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        public async Task<bool> Login(string username, string password)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                Uri uri = new Uri("http://localhost:60133/users/authenticate");

                string json;
                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream))
                    {
                        writer.WriteStartObject();
                        writer.WriteString("username", username);
                        writer.WriteString("password", password);
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

                string token;
                int userId;
                using (JsonDocument document = JsonDocument.Parse(httpResponseBody))
                {
                    var root = document.RootElement;

                    token = root.GetProperty("token").GetString();
                    userId = root.GetProperty("id").GetInt32();
                }

                Debug.WriteLine(token);

                currentUsername = username;
                currentToken = token;
                currentUserId = userId;

                uri = new Uri("http://localhost:60133/transactions/latest");
                httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("Bearer", token);

                httpResponseMessage = await httpClient.GetAsync(uri);
                httpResponseMessage.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                
                using (JsonDocument document = JsonDocument.Parse(httpResponseBody))
                {
                    if (document.RootElement.GetArrayLength() != 0)
                        currentuserIsNew = false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}

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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterScreen : Page
    {
        public RegisterScreen()
        {
            this.InitializeComponent();
        }

        private void registerclick(object sender, RoutedEventArgs e)
        {
            //codebehind register
        }

        private void fnamebox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO
        }

        private void lnamebox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO
        }

        private void usernam_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO
        }

        private void email_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO
        }

        private void passw_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //TODO
        }
    }
}

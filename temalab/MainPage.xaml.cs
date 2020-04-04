using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("overview", typeof(UserMainPage)),
            ("revenues", typeof(RevenuesPage)),
            ("expenses", typeof(ExpensesPage)),
            ("transactions", typeof(TransactionsPage)),
        };

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void contentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void navView_Loaded(object sender, RoutedEventArgs e)
        {
            navView.SelectedItem = navView.MenuItems[0];

            navView_Navigate("overview", new EntranceNavigationTransitionInfo());

            var goBack = new KeyboardAccelerator { Key = VirtualKey.GoBack };
            goBack.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(goBack);

            var altLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left,
                Modifiers = VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(altLeft);
        }

        private void navView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                navView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                navView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void navView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                _page = typeof(SettingsPage);
            }
            else if(navItemTag == "overview")
            {
                _page = ((App)Application.Current).user.isNew ? typeof(FreshUserMainPage) : typeof(UserMainPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }

            var preNavPageType = contentFrame.CurrentSourcePageType;

            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                contentFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void navView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }

        private void BackInvoked(KeyboardAccelerator sender,
                         KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private bool On_BackRequested()
        {
            if (!contentFrame.CanGoBack)
                return false;

            if (navView.IsPaneOpen &&
                (navView.DisplayMode == NavigationViewDisplayMode.Compact ||
                 navView.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;

            contentFrame.GoBack();
            return true;
        }

        private void contentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            navView.IsBackEnabled = contentFrame.CanGoBack;

            if (contentFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of navView.MenuItems, and doesn't have a Tag.
                navView.SelectedItem = (NavigationViewItem)navView.SettingsItem;
                navView.Header = "Settings";
            }
            else if(contentFrame.SourcePageType == typeof(FreshUserMainPage))
            {
                navView.Header = "Overview";
                navView.SelectedItem = navView.MenuItems[0];
            }
            else if (contentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                navView.SelectedItem = navView.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(n => n.Tag.Equals(item.Tag));

                navView.Header =
                    ((NavigationViewItem)navView.SelectedItem)?.Content?.ToString();
            }
        }
    }
}

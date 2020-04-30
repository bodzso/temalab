using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.ComponentModel;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoriesPage : Page
    {
        App app = Application.Current as App;
        ObservableCollection<Category> categories;
        Category currentCategory;

        public CategoriesPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            categories = JsonSerializer.Deserialize<ObservableCollection<Category>>(await app.GetHttpContent(new Uri($"{app.baseuri}/categories")));
            categoriesListView.ItemsSource = categories;
        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            var res = await app.PostJson(new Uri($"{app.baseuri}/categories"), JsonSerializer.Serialize(new { categoryName = categoryName.Text }));

            if (!string.IsNullOrEmpty(res))
                categories.Add(JsonSerializer.Deserialize<Category>(res));
        }

        private async void edit_Click(object sender, RoutedEventArgs e)
        {
            categoryNameDialogInput.Text = currentCategory.categoryName;
            var res = await categoryNameDialog.ShowAsync();

            if (res == ContentDialogResult.Primary)
            {
                var httpRes = await app.PutJson(new Uri($"{app.baseuri}/categories/" + currentCategory.categoryId), JsonSerializer.Serialize(new CategoryModel { categoryId = currentCategory.categoryId, categoryName = categoryNameDialogInput.Text }));

                if (httpRes)
                {
                    currentCategory.categoryName = categoryNameDialogInput.Text;
                    int idx = categories.IndexOf(currentCategory);
                    categories.Remove(currentCategory);
                    categories.Insert(idx, currentCategory);
                }
            }
        }

        private async void remove_Click(object sender, RoutedEventArgs e)
        {
            var result = await app.SendDeleteHttp(new Uri($"{app.baseuri}/categories/" + currentCategory.categoryId));

            if (result)
                categories.Remove(currentCategory);
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            currentCategory = senderElement.DataContext as Category;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }
    }

    public class Category : CategoryModel
    { }
}

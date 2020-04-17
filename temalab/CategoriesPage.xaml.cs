﻿using System;
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

            categories = JsonSerializer.Deserialize<ObservableCollection<Category>>(await app.GeHttpContent(new Uri("http://localhost:60133/categories")));
            categoriesListView.ItemsSource = categories;
        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            var res = await app.PostJson(new Uri("http://localhost:60133/categories"), JsonSerializer.Serialize(new CategoryModel { name = categoryName.Text }));

            if (!string.IsNullOrEmpty(res))
                categories.Add(JsonSerializer.Deserialize<Category>(res));
        }

        private async void edit_Click(object sender, RoutedEventArgs e)
        {
            categoryNameDialogInput.Text = currentCategory.name;
            var res = await categoryNameDialog.ShowAsync();

            if (res == ContentDialogResult.Primary)
            {
                var httpRes = await app.PutJson(new Uri("http://localhost:60133/categories/" + currentCategory.id), JsonSerializer.Serialize(new Category { id = currentCategory.id, name = categoryNameDialogInput.Text }));

                if (httpRes)
                {
                    currentCategory.name = categoryNameDialogInput.Text;
                    int idx = categories.IndexOf(currentCategory);
                    categories.Remove(currentCategory);
                    categories.Insert(idx, currentCategory);
                }
            }
        }

        private async void remove_Click(object sender, RoutedEventArgs e)
        {
            var result = await app.SendDeleteHttp(new Uri("http://localhost:60133/categories/" + currentCategory.id));

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

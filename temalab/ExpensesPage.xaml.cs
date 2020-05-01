using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExpensesPage : Page
    {
        public ObservableCollection<EditableTransactionModel> expenses = new ObservableCollection<EditableTransactionModel>();
        public ObservableCollection<CategoryModel> categories = new ObservableCollection<CategoryModel>();
        App app = (App)Application.Current;
        public ExpensesPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            expenses = JsonSerializer.Deserialize<ObservableCollection<EditableTransactionModel>>(await app.GetHttpContent(new Uri($"{app.baseuri}/transactions/expenses")));
            expensesGrid.ItemsSource = expenses;
            categories = JsonSerializer.Deserialize<ObservableCollection<CategoryModel>>(await app.GetHttpContent(new Uri($"{app.baseuri}/categories")));

            // Add null category
            categories.Insert(0, new CategoryModel());

            categComboBox.ItemsSource = categories;
            categoryColumn.ItemsSource = categories;
        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryModel category = null;
            if (categComboBox.SelectedValue != null)
            {
                category = ((CategoryModel)categComboBox.SelectedValue);
            }

            if (string.IsNullOrEmpty(name.Text) || string.IsNullOrEmpty(cost.Text))
                return;

            var dateTime = DateTime.Now;

            if (dueDate.Date.HasValue && time.SelectedTime.HasValue)
            {
                dateTime = dueDate.Date.Value.Date + time.SelectedTime.Value;
            }
            else if (dueDate.Date.HasValue && !time.SelectedTime.HasValue)
            {
                dateTime = dueDate.Date.Value.Date;
            }

            var expense = new EditableTransactionModel() { name = name.Text, amount = cost.Value * -1, categoryId = category?.categoryId, description = description.Text, date = dateTime };
            var json = JsonSerializer.Serialize(expense);
            var res = await app.PostJson(new Uri($"{app.baseuri}/transactions"), json);

            if(!string.IsNullOrEmpty(res))
                expenses.Add(JsonSerializer.Deserialize<EditableTransactionModel>(res));

            name.Text = "";
            cost.Text = "";
            categComboBox.SelectedValue = null;
            description.Text = "";
            dueDate.Date = null;
            time.SelectedTime = null;
        }

        private async void addCategory_Click(object sender, RoutedEventArgs e)
        {
            TextBox input = new TextBox();
            input.PlaceholderText = "Category name";
            ContentDialog dialog = new ContentDialog
            {
                Title = "Add new category",
                Content = input,
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Add"
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary && !String.IsNullOrEmpty(input.Text))
            {
                var json = JsonSerializer.Serialize(new { categoryName = input.Text });
                var category = JsonSerializer.Deserialize<CategoryModel>(await app.PostJson(new Uri($"{app.baseuri}/categories"), json));
                categories.Add(category);
                categComboBox.SelectedItem = category;
            }
        }

        private void expensesGrid_Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
        {
            var column = e.Column.ClipboardContentBinding.Path.Path.ToString();
            var pi = typeof(EditableTransactionModel).GetProperty(column);

            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
            {
                expensesGrid.ItemsSource = expenses.OrderBy(t => pi.GetValue(t));
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            }
            else
            {
                expensesGrid.ItemsSource = expenses.OrderByDescending(t => pi.GetValue(t));
                e.Column.SortDirection = DataGridSortDirection.Descending;
            }

            foreach (var c in expensesGrid.Columns)
            {
                if (c.Header.ToString() != e.Column.Header.ToString())
                {
                    c.SortDirection = null;
                }
            }
        }

        private async void expensesGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var transaction = e.Row.DataContext as EditableTransactionModel;
                if (transaction.categoryId == -1)
                    transaction.categoryId = null;

                if (!await app.PutJson(new Uri($"{app.baseuri}/transactions/" + transaction.id), JsonSerializer.Serialize(transaction)))
                    e.Cancel = true;
            }
        }
    }

    public class NegateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (double)value * -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

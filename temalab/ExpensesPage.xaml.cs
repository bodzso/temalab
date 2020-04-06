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
        public ObservableCollection<ExpenseModel> expenses = new ObservableCollection<ExpenseModel>();
        public ObservableCollection<CategoryModel> categories = new ObservableCollection<CategoryModel>();
        App app = (App)Application.Current;
        public ExpensesPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            expenses = JsonSerializer.Deserialize<ObservableCollection<ExpenseModel>>(await app.GetJson(new Uri("http://localhost:60133/transactions/expenses")));
            expensesGrid.ItemsSource = expenses;
            categories = JsonSerializer.Deserialize<ObservableCollection<CategoryModel>>(await app.GetJson(new Uri("http://localhost:60133/categories")));
            categories.Add(new CategoryModel() { id = -1, name = "Uncategorized" });
            categComboBox.ItemsSource = categories;

        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryModel category = null;
            if (categComboBox.SelectedValue != null && ((CategoryModel)categComboBox.SelectedValue).id != -1)
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

            var expense = new ExpenseModel() { name = name.Text, amount = cost.Value * -1, categoryId = category?.id, description = description.Text, date = dateTime };
            var json = JsonSerializer.Serialize(expense);
            var res = await app.PostJson(new Uri("http://localhost:60133/transactions"), json);

            if (!string.IsNullOrEmpty(res))
                expense.categoryName = category?.name;
            expenses.Add(expense);

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
                var category = new CategoryModel() { name = input.Text };
                var json = JsonSerializer.Serialize<CategoryModel>(category);
                await app.PostJson(new Uri("http://localhost:60133/categories"), json);
                categories = JsonSerializer.Deserialize<ObservableCollection<CategoryModel>>(await app.GetJson(new Uri("http://localhost:60133/categories")));
                categComboBox.ItemsSource = categories;
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

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
        public ExpensesPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var app = (App)Application.Current;
            expenses = JsonSerializer.Deserialize<ObservableCollection<ExpenseModel>>(await app.GetJson(new Uri("http://localhost:60133/transactions/expenses")));
            expensesGrid.ItemsSource = expenses;
            categories = JsonSerializer.Deserialize<ObservableCollection<CategoryModel>>(await app.GetJson(new Uri("http://localhost:60133/categories")));
            categComboBox.ItemsSource = categories.Select(c => c.name).ToList();
            
        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryModel category;
            try
            {
                category = categories.Single(c => c.name == (string)categComboBox.SelectedValue);
            }
            catch (InvalidOperationException)
            {
                category = null;
            }

            if (string.IsNullOrEmpty(name.Text) || string.IsNullOrEmpty(cost.Text))
                return;

            var app = (App)Application.Current;
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

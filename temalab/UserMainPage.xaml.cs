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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserMainPage : Page
    {
        public ObservableCollection<TransactionModel> latestTransactions = new ObservableCollection<TransactionModel>();
        public ObservableCollection<TransactionModel> upcomingTransactions = new ObservableCollection<TransactionModel>();
        App app = (App)Application.Current;

        public List<TransactionModel> expenses = new List<TransactionModel>();
        public List<TransactionModel> revenues = new List<TransactionModel>();

        public UserMainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await app.UpdateUserBalance();
            balanceValue.Text = Convert.ToString(app.user.GetBalance());
            latestTransactions = JsonSerializer.Deserialize<ObservableCollection<TransactionModel>>(await app.GetHttpContent(new Uri("http://localhost:60133/transactions/latest")));
            latestTransactionsDataGrid.ItemsSource = latestTransactions;
            upcomingTransactions = JsonSerializer.Deserialize<ObservableCollection<TransactionModel>>(await app.GetHttpContent(new Uri("http://localhost:60133/transactions/pending")));
            upcomingTransactionsDataGrid.ItemsSource = upcomingTransactions;

            expenses = JsonSerializer.Deserialize<List<TransactionModel>>(await app.GetHttpContent(new Uri("http://localhost:60133/transactions/expenses")));
            revenues = JsonSerializer.Deserialize<List<TransactionModel>>(await app.GetHttpContent(new Uri("http://localhost:60133/transactions/revenues")));
            UpdateChartData();
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RevenuesPage));
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ExpensesPage));
        }

        private void latestTransactionsDataGrid_Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
        {
            var column = e.Column.ClipboardContentBinding.Path.Path.ToString();
            var pi = typeof(TransactionModel).GetProperty(column);

            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
            {
                latestTransactionsDataGrid.ItemsSource = latestTransactions.OrderBy(t => pi.GetValue(t));
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            }
            else
            {
                latestTransactionsDataGrid.ItemsSource = latestTransactions.OrderByDescending(t => pi.GetValue(t));
                e.Column.SortDirection = DataGridSortDirection.Descending;
            }

            foreach (var c in latestTransactionsDataGrid.Columns)
            {
                if (c.Header.ToString() != e.Column.Header.ToString())
                {
                    c.SortDirection = null;
                }
            }
        }

        private void upcomingTransactionsDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            var column = e.Column.ClipboardContentBinding.Path.Path.ToString();
            var pi = typeof(TransactionModel).GetProperty(column);

            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
            {
                upcomingTransactionsDataGrid.ItemsSource = upcomingTransactions.OrderBy(t => pi.GetValue(t));
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            }
            else
            {
                upcomingTransactionsDataGrid.ItemsSource = upcomingTransactions.OrderByDescending(t => pi.GetValue(t));
                e.Column.SortDirection = DataGridSortDirection.Descending;
            }

            foreach (var c in upcomingTransactionsDataGrid.Columns)
            {
                if (c.Header.ToString() != e.Column.Header.ToString())
                {
                    c.SortDirection = null;
                }
            }
        }


        public class ChartData
        {
            public string Label { get; set; }
            public double Value { get; set; }
        }

        public void UpdateChartData()
        {
            List<ChartData> values = new List<ChartData>();
            values.Add(new ChartData()
            {
                Label = "Expenses",
                Value = expenses.Sum(e => Math.Abs(e.amount))
            });
            values.Add(new ChartData()
            {   
                Label = "Revenues",
                Value = revenues.Sum(r => r.amount)
            });

            (pieCh.Series[0] as PieSeries).ItemsSource = values;
            
        }

    }
}

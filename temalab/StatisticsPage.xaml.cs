using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
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
using WinRTXamlToolkit.Tools;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StatisticsPage : Page
    {
        App app = Application.Current as App;
        List<ChartData> revenuesSource = new List<ChartData>();
        List<ChartData> expensesSource = new List<ChartData>();

        public StatisticsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            typeComboBox.SelectedItem = monthly;
        }

        class ChartData
        {
            public string date { get; set; }
            public double amount { get; set; }

            public int getMonths()
            {
                if (date.Contains("/"))
                {
                    return Convert.ToInt32(date.Split('/').ElementAt(0)) * 12 + Convert.ToInt32(date.Split('/').ElementAt(1));
                }
                return Convert.ToInt32(date) * 12;
            }
            public int getYear()
            {
                if (date.Contains("/"))
                {
                    return Convert.ToInt32(date.Split('/').ElementAt(0));
                }
                return Convert.ToInt32(date);
            }
            public static string getDate(int months)
            {
                if (months % 12 == 0)
                {
                    return $"{(months / 12)-1}/12";
                }
                return $"{months / 12}/{months % 12}";
            }
        }

        private void typeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.AddedItems.First() as ComboBoxItem;
            if (selected == yearly)
            {
                monthYearText.Text = "years";
            }
            else
            {
                monthYearText.Text = "months";
            }
            UpdateCharts(selected, limitNum.Value);
        }

        private async void UpdateCharts(ComboBoxItem type, double limitValue)
        {
            int limit = double.IsNaN(limitValue) ? 0 : Convert.ToInt32(limitValue);
            List<ChartData> revenues = new List<ChartData>();
            List<ChartData> expenses = new List<ChartData>();
            if (type == yearly)
            {
                revenues = JsonSerializer.Deserialize<List<ChartData>>(await app.GetHttpContent(new Uri($"{app.baseuri}/transactions/revenues/yearly")));
                expenses = JsonSerializer.Deserialize<List<ChartData>>(await app.GetHttpContent(new Uri($"{app.baseuri}/transactions/expenses/yearly")));

                (int startDate, int endDate) = InitVars(type, limit, revenues, expenses);
                revenuesSource = IncludeYearsWithNoTransactions(revenues, startDate, endDate);
                expensesSource = IncludeYearsWithNoTransactions(expenses, startDate, endDate);
            }
            else
            {
                revenues = JsonSerializer.Deserialize<List<ChartData>>(await app.GetHttpContent(new Uri($"{app.baseuri}/transactions/revenues/monthly")));
                expenses = JsonSerializer.Deserialize<List<ChartData>>(await app.GetHttpContent(new Uri($"{app.baseuri}/transactions/expenses/monthly")));

                (int startDate, int endDate) = InitVars(type, limit, revenues, expenses);
                revenuesSource = IncludeMonthsWithNoTransactions(revenues, startDate, endDate);
                expensesSource = IncludeMonthsWithNoTransactions(expenses, startDate, endDate);
            }
            
            if (limit != 0)
            {
                revenuesSource = revenuesSource.TakeLast(limit).ToList();
                expensesSource = expensesSource.TakeLast(limit).ToList();
            }

            if (revenuesSource.Select(r => r.amount).Sum() == 0)
            {
                revenuesColumnChart.Height = 100;
                noRevenues.Visibility = Visibility.Visible;
            }
            else
            {
                revenuesColumnChart.Height = 300;
                noRevenues.Visibility = Visibility.Collapsed;
            }

            if (expensesSource.Select(r => r.amount).Sum() == 0)
            {
                expensesColumnChart.Height = 100;
                noExpenses.Visibility = Visibility.Visible;
            }
            else
            {
                expensesColumnChart.Height = 300;
                noExpenses.Visibility = Visibility.Collapsed;
            }

            (revenuesColumnChart.Series[0] as ColumnSeries).ItemsSource = revenuesSource;
            (expensesColumnChart.Series[0] as ColumnSeries).ItemsSource = expensesSource;
            (revenuesColumnChart.Series[0] as ColumnSeries).DependentRangeAxis = new LinearAxis()
            {
                Minimum = 0,
                Maximum = revenuesSource.Select(r => r.amount).Max(),
                Orientation = AxisOrientation.Y,
                ShowGridLines = true
            };
            (expensesColumnChart.Series[0] as ColumnSeries).DependentRangeAxis = new LinearAxis()
            {
                Minimum = 0,
                Maximum = expensesSource.Select(r => r.amount).Max(),
                Orientation = AxisOrientation.Y,
                ShowGridLines = true
            };
        }

        private (int startDate, int endDate) InitVars(ComboBoxItem type, int limit, List<ChartData> revenues, List<ChartData> expenses)
        {
            int startDate;
            int endDate;
            if (type == yearly)
            {
                int limitMin = Convert.ToInt32(DateTime.Today.Year - limit);
                int revenuesFirst = revenues.Count == 0 ? limitMin : revenues.First().getYear();
                int expensesFirst = expenses.Count == 0 ? limitMin : expenses.First().getYear();
                startDate = Math.Min(Math.Min(revenuesFirst, expensesFirst), limitMin);
                endDate = Convert.ToInt32(DateTime.Today.Year);
            }
            else
            {
                int limitMin = Convert.ToInt32(DateTime.Today.Year * 12 + DateTime.Today.Month - limit);
                int revenuesFirst = revenues.Count == 0 ? limitMin : revenues.First().getMonths();
                int expensesFirst = expenses.Count == 0 ? limitMin : expenses.First().getMonths();
                startDate = Math.Min(Math.Min(revenuesFirst, expensesFirst), limitMin);
                endDate = Convert.ToInt32(DateTime.Today.Year * 12 + DateTime.Today.Month);
            }
            return (startDate, endDate);
        }

        private List<ChartData> IncludeMonthsWithNoTransactions(List<ChartData> transactions, int startDate, int endDate)
        {
            int i = 0;
            List<ChartData> temp = new List<ChartData>();
            for (int months = startDate; months <= endDate; months++)
            {
                if (i < transactions.Count && transactions.ElementAt(i).getMonths() == months)
                {
                    temp.Add(transactions.ElementAt(i));
                    i++;
                }
                else
                {
                    temp.Add(new ChartData() { date = ChartData.getDate(months), amount = 0 });
                }
            }
            return temp;
        }

        private List<ChartData> IncludeYearsWithNoTransactions(List<ChartData> transactions, int startDate, int endDate)
        {
            int i = 0;
            List<ChartData> temp = new List<ChartData>();
            for (int year = startDate; year <= endDate; year++)
            {
                if (i < transactions.Count && transactions.ElementAt(i).getYear() == year)
                {
                    temp.Add(transactions.ElementAt(i));
                    i++;
                }
                else
                {
                    temp.Add(new ChartData() { date = year.ToString(), amount = 0 });
                }
            }
            return temp;
        }

        private void limitNum_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            UpdateCharts(typeComboBox.SelectedItem as ComboBoxItem, limitNum.Value);
        }
    }
}

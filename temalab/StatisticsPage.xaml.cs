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
        ObservableCollection<ChartData> revenues = new ObservableCollection<ChartData>();
        ObservableCollection<ChartData> expenses = new ObservableCollection<ChartData>();

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

        private async void UpdateCharts(ComboBoxItem type, double limit)
        {
            limit = double.IsNaN(limit) ? 0 : limit;
            if (type == yearly)
            {
                revenues = JsonSerializer.Deserialize<ObservableCollection<ChartData>>(await app.GetHttpContent(new Uri($"{app.baseuri}/transactions/revenues/yearly?limit={limit}")));
                expenses = JsonSerializer.Deserialize<ObservableCollection<ChartData>>(await app.GetHttpContent(new Uri($"{app.baseuri}/transactions/expenses/yearly?limit={limit}")));
            }
            else
            {
                revenues = JsonSerializer.Deserialize<ObservableCollection<ChartData>>(await app.GetHttpContent(new Uri($"{app.baseuri}/transactions/revenues/monthly?limit={limit}")));
                expenses = JsonSerializer.Deserialize<ObservableCollection<ChartData>>(await app.GetHttpContent(new Uri($"{app.baseuri}/transactions/expenses/monthly?limit={limit}")));
            }
            
            (revenuesColumnChart.Series[0] as ColumnSeries).ItemsSource = revenues;
            (expensesColumnChart.Series[0] as ColumnSeries).ItemsSource = expenses;
            (revenuesColumnChart.Series[0] as ColumnSeries).DependentRangeAxis = new LinearAxis()
            {
                Minimum = 0,
                Maximum = revenues.Select(r => r.amount).Max(),
                Orientation = AxisOrientation.Y,
                ShowGridLines = true
            };
            (expensesColumnChart.Series[0] as ColumnSeries).DependentRangeAxis = new LinearAxis()
            {
                Minimum = 0,
                Maximum = expenses.Select(r => r.amount).Max(),
                Orientation = AxisOrientation.Y,
                ShowGridLines = true
            };
        }

        private void limitNum_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            UpdateCharts(typeComboBox.SelectedItem as ComboBoxItem, limitNum.Value);
        }
    }
}

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
    public sealed partial class TransactionsPage : Page
    {

        public ObservableCollection<TransactionModel> transactions = new ObservableCollection<TransactionModel>();
        App app = Application.Current as App;

        public TransactionsPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            transactions = JsonSerializer.Deserialize<ObservableCollection<TransactionModel>>(await app.GetHttpContent(new Uri($"{app.baseuri}/transactions")));
            TransactionsGrid.ItemsSource = transactions;
        }

        private void TransactionsGrid_Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
        {
            var column = e.Column.ClipboardContentBinding.Path.Path.ToString();
            var pi = typeof(TransactionModel).GetProperty(column);

            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
            {
                TransactionsGrid.ItemsSource = transactions.OrderBy(t => pi.GetValue(t));
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            }
            else
            {
                TransactionsGrid.ItemsSource = transactions.OrderByDescending(t => pi.GetValue(t));
                e.Column.SortDirection = DataGridSortDirection.Descending;
            }

            foreach (var c in TransactionsGrid.Columns)
            {
                if (c.Header.ToString() != e.Column.Header.ToString())
                {
                    c.SortDirection = null;
                }
            }

        }
    }
}

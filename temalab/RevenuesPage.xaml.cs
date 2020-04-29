using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.UI.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RevenuesPage : Page
    {
        public ObservableCollection<EditableTransactionModel> transactions = new ObservableCollection<EditableTransactionModel>();
        App app = Application.Current as App;

        public RevenuesPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var app = (App)Application.Current;
            transactions = JsonSerializer.Deserialize<ObservableCollection<EditableTransactionModel>>(await app.GetHttpContent(new Uri($"{app.baseuri}/transactions/revenues")));
            revenuesGrid.ItemsSource = transactions;
        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(name.Text) || string.IsNullOrEmpty(amount.Text))
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

            var transaction = new EditableTransactionModel { name = name.Text, amount = amount.Value, description = description.Text, date =  dateTime};
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };

            var json = JsonSerializer.Serialize(transaction, options);
            var res = await app.PostJson(new Uri($"{app.baseuri}/transactions"), json);
            
            if(!string.IsNullOrEmpty(res))
            {
                transaction.id = Convert.ToInt32(res);
                transactions.Add(transaction);
            }
               

            if (app.user.GetIsNew())
                app.user.isNew = false;
        }

        private void revenuesGrid_Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
        {
            var column = e.Column.ClipboardContentBinding.Path.Path.ToString();
            var pi = typeof(EditableTransactionModel).GetProperty(column);

            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
            {
                revenuesGrid.ItemsSource = transactions.OrderBy(t => pi.GetValue(t));
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            }
            else
            {
                revenuesGrid.ItemsSource = transactions.OrderByDescending(t => pi.GetValue(t));
                e.Column.SortDirection = DataGridSortDirection.Descending;
            }

            foreach (var c in revenuesGrid.Columns)
            {
                if (c.Header.ToString() != e.Column.Header.ToString())
                {
                    c.SortDirection = null;
                }
            }
        }

        private async void revenuesGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var transaction = e.Row.DataContext as EditableTransactionModel;
                
                if (!await app.PutJson(new Uri($"{app.baseuri}/transactions/" + transaction.id), JsonSerializer.Serialize(transaction)))
                    e.Cancel = true;
            }
        }
    }
}

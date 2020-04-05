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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace temalab
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RevenuesPage : Page
    {
        public ObservableCollection<TransactionModel> transactions = new ObservableCollection<TransactionModel>();

        public RevenuesPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var app = (App)Application.Current;
            transactions = JsonSerializer.Deserialize<ObservableCollection<TransactionModel>>(await app.GetJson(new Uri("http://localhost:60133/transactions/revenues")));
            revenuesGrid.ItemsSource = transactions;
        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(name.Text) || string.IsNullOrEmpty(amount.Text))
                return;

            var app = (App)Application.Current;
            var dateTime = DateTime.Now;

            if (dueDate.Date.HasValue && time.SelectedTime != null)
                dateTime = dateTime.Date + time.Time;

            var transaction = new TransactionModel { name = name.Text, amount = amount.Value, description = description.Text, date =  dateTime};

            var json = JsonSerializer.Serialize(transaction);
            var res = await app.PostJson(new Uri("http://localhost:60133/transactions"), json);
            
            if(!string.IsNullOrEmpty(res))
                transactions.Add(transaction);

            if (app.user.GetIsNew())
                app.user.isNew = false;
        }
    }
}

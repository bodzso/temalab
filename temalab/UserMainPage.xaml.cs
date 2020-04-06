using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class UserMainPage : Page
    {
        public static int Balance { get; set; } = 5000;
        public string BalanceString { get; set; } = $"{Balance} Ft";
        public ObservableCollection<TransactionModel> latestTransactions = new ObservableCollection<TransactionModel>(); //majd átírni Transaction osztályra
        public ObservableCollection<TransactionModel> upcomingTransactions = new ObservableCollection<TransactionModel>();
        App app = (App)Application.Current;

        //csak hogy látszódjon valami
        public string Price { get; set; } = "5000";
        public string Date { get; set; } = "2020.03.29.";
        public string Time { get; set; } = "16:57";

        public UserMainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            latestTransactions = JsonSerializer.Deserialize<ObservableCollection<TransactionModel>>(await app.GetJson(new Uri("http://localhost:60133/transactions/latest")));
            latestTransactionsDataGrid.ItemsSource = latestTransactions;
            upcomingTransactions = JsonSerializer.Deserialize<ObservableCollection<TransactionModel>>(await app.GetJson(new Uri("http://localhost:60133/transactions/pending")));
            upcomingTransactionsDataGrid.ItemsSource = upcomingTransactions;
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void MinuButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }
    }
}

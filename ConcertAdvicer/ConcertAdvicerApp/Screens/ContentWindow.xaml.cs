using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Logic.Database;

namespace ConcertAdvicerApp.Screens
{
    /// <summary>
    /// Логика взаимодействия для ContentWindow.xaml
    /// </summary>
    public partial class ContentWindow : Window
    {
        public Repository Repository { private get; set; }

        public ContentWindow()
        {
            InitializeComponent();
        }

        private void LogoutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            LoginRegister lr = new LoginRegister();
            lr.Show();
            this.Close();
        }

        // updating database
        private void UpdateDBMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            // updating in the different thread to notify user about start and complition
            Thread back = new Thread(Repository.UpdateDb);
            back.Start();
            MessageBox.Show("Updating database");
            back.Join();
            MessageBox.Show("Database updated!");
        }


        private void CloseMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // fill fields on browse screen with cities and dates
        private void browse_Loaded(object sender, RoutedEventArgs e)
        {
            cityComboBox.ItemsSource = Repository.GetCityShortingDictionary().Keys.ToList();
            cityComboBox.SelectedIndex = 1;
            fromDatePicker.SelectedDate = DateTime.Now;
            toDatePicker.SelectedDate = DateTime.Now.AddMonths(1);
        }

        private void browseShow_OnClick(object sender, RoutedEventArgs e)
        {
            var dictionary = Repository.GetCityShortingDictionary();
            if (!dictionary.ContainsKey(cityComboBox.Text))
                return;
            BrowseDataGrid.ItemsSource = Repository.CityConcerstBetweenDates(
                dictionary[cityComboBox.Text],
                fromDatePicker.SelectedDate.Value,
                toDatePicker.SelectedDate.Value);
            formatGrids();
        }

        private void addToWishList_OnClick(object sender, RoutedEventArgs e)
        {
            if (BrowseDataGrid.SelectedItem != null)
                try
                {
                    Repository.AddToWishlist((BrowseDataGrid.SelectedItem as Concert).ID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }

        // show wishlist after login
        private void wishlistLoaded_OnLoaded(object sender, RoutedEventArgs e)
        {
            WishlistDataGrid.ItemsSource = Repository.UserWishlist();
            formatGrids();
        }

        // hide reundant columns
        private void formatGrids()
        {
            WishlistDataGrid.Columns.ToList().Where(c =>
                c.Header.Equals("ID") ||
                c.Header.Equals("Description") ||
                c.Header.Equals("Location") ||
                c.Header.Equals("URL")).ToList().ForEach(column => column.Visibility = Visibility.Hidden);

            BrowseDataGrid.Columns.ToList().Where(c =>
                c.Header.Equals("ID") ||
                c.Header.Equals("Description") ||
                c.Header.Equals("Location") ||
                c.Header.Equals("URL")).ToList().ForEach(column => column.Visibility = Visibility.Hidden);
        }

        private void removeFromWishlist_OnClick(object sender, RoutedEventArgs e)
        {
            if (WishlistDataGrid.SelectedItem != null)
                Repository.RemoveFromWishlist((WishlistDataGrid.SelectedItem as Concert).ID);

            WishlistDataGrid.ItemsSource = Repository.UserWishlist();
            formatGrids();
        }

        private void ClearWishlistButton_OnClick(object sender, RoutedEventArgs e)
        {
            Repository.UserWishlist().ForEach(conc=>Repository.RemoveFromWishlist(conc.ID));

            WishlistDataGrid.ItemsSource = Repository.UserWishlist();
            formatGrids();
        }

        private void RefreshWishlistButton_OnClick(object sender, RoutedEventArgs e)
        {
            WishlistDataGrid.ItemsSource = Repository.UserWishlist();
            formatGrids();
        }

        private void WishlistMoreInfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (WishlistDataGrid.SelectedItem == null) return;
            Concert concert = WishlistDataGrid.SelectedItem as Concert;
            
            ShowInfo(concert);
        }

        private void BrowseMoreInfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (BrowseDataGrid.SelectedItem == null) return;
            Concert concert = BrowseDataGrid.SelectedItem as Concert;

            ShowInfo(concert);
        }

        private void ShowInfo(Concert concert)
        {
            MoreInfo mi = new MoreInfo
            {
                TitleTextBox = { Text = concert.Title },
                DateTextBox = { Text = concert.Date.Value.ToShortDateString() + " " + concert.Date.Value.ToShortTimeString() },
                PriceTextBox = { Text = concert.Price },
                DescriptionTextBox = { Text = concert.Description },
                URTextBox = { Text = concert.URL }
            };

            mi.Show();
        }
    }
}
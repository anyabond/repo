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

        private void UpdateDBMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WishlistDataGrid.ItemsSource = Repository.UserWishlist();
        }

        private void browse_Loaded(object sender, RoutedEventArgs e)
        {
            cityComboBox.ItemsSource = Repository.GetCityShortingDictionary().Keys.ToList();
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
            BrowseDataGrid.Columns.ToList().Where(c =>
                c.Header.Equals("ID") ||
                c.Header.Equals("Description") ||
                c.Header.Equals("Location") ||
                c.Header.Equals("URL")).ToList().ForEach(column => column.Visibility = Visibility.Hidden);
        }
    }
}
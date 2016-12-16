using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ConcertAdvicerApp.Screens
{
    /// <summary>
    /// Логика взаимодействия для MoreInfo.xaml
    /// </summary>
    public partial class MoreInfo : Window
    {
        public MoreInfo()
        {
            InitializeComponent();
        }

        private void CopyURL_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(URTextBox.Text);
            infoLabel.Content = "Copied to clipboard";
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

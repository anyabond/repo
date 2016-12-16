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
using Logic.Database;

namespace ConcertAdvicerApp.Screens
{
    /// <summary>
    /// Логика взаимодействия для LoginRegister.xaml
    /// </summary>
    public partial class LoginRegister : Window
    {
        private Repository rep { get; }

        public LoginRegister()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            rep = new Repository();
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordBox.Password;
            try
            {
                rep.Login(login, password);
            }
            catch (Exception ex)
            {
                loginErrorLabel.Content = ex.Message;
                return;
            }

            ContentWindow cw = new ContentWindow();
            cw.Repository = rep;
            cw.Show();
            this.Close();
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(newLoginTextBox.Text) ||
                string.IsNullOrEmpty(newPasswordBox.Password) ||
                string.IsNullOrEmpty(nameTextBox.Text) ||
                string.IsNullOrEmpty(surnameTextBox.Text) ||
                string.IsNullOrEmpty(emailTextBox.Text))
            {
                registerErrorLabel.Content = "Some fields are empty!";
                return;
            }

            if (rep.LoginUsed(newLoginTextBox.Text))
            {
                registerErrorLabel.Content = "Login used!";
                return;
            }

            rep.Register(nameTextBox.Text,
                surnameTextBox.Text,
                newLoginTextBox.Text,
                newPasswordBox.Password,
                emailTextBox.Text);
            rep.Login(newLoginTextBox.Text, newPasswordBox.Password);
            ContentWindow cw = new ContentWindow();
            cw.Repository = rep;
            cw.Show();
            this.Close();
        }
    }
}
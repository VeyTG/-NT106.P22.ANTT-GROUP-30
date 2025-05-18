using System;
using System.Text.RegularExpressions;
using System.Windows;
using ThuAo.Models;
using ThuAo.Services;

namespace ThuAo
{
    public partial class SignUp : Window
    {
        private readonly UserService _userService = new UserService();

        public SignUp()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Validate email format
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.");
                return;
            }

            if (await _userService.EmailExists(email))
            {
                MessageBox.Show("Email already exists.");
                return;
            }

            User newUser = new User
            {
                Username = username,
                Email = email,
                Password = password
            };

            await _userService.CreateUser(newUser);
            

            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            // Simple email validation regex
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private void LoginLink_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}

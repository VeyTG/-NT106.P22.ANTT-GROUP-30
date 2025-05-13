using System.Windows;
using ThuAo.Services;
using ThuAo.Models;

namespace ThuAo
{
    public partial class Login : Window
    {
        private readonly UserService _userService = new UserService();

        public Login()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in both fields.");
                return;
            }

            // Gọi UserService để xác thực thông tin người dùng
            var user = await _userService.AuthenticateUser(username, password);

            if (user != null)
            {
                

                //  Lưu thông tin người dùng vào Session
                Session.Username = user.Username;
                Session.Email = user.Email;

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }

        private void CreateAccountLink_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var signUpWindow = new SignUp();
            signUpWindow.Show();
            this.Close();
        }

        private void ForgotPasswordLink_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("Password recovery is not yet implemented.");
        }
    }
}

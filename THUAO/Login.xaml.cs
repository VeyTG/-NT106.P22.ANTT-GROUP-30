using System.Windows;
using ThuAo.Services;
using ThuAo.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth;
using Google.Apis.Util.Store;
using System.Threading;
using System;

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

        private async void GoogleLoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var clientSecrets = new ClientSecrets
                {
                    ClientId = "",
                    ClientSecret = ""  
                };

                string[] scopes = new string[] { "openid", "email", "profile" };

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("GoogleToken", true)
                );

                if (credential.Token.IsStale)
                {
                    await credential.RefreshTokenAsync(CancellationToken.None);
                }


                var payload = await GoogleJsonWebSignature.ValidateAsync(credential.Token.IdToken);

                // Dữ liệu trả về từ Google
                Session.Username = payload.Name;
                Session.Email = payload.Email;

                MessageBox.Show($"Welcome, {payload.Name}!");

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Google login failed: " + ex.Message);
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

            var ForgotPassword = new ForgotPassword();
            ForgotPassword.ShowDialog();

        }

    }


}


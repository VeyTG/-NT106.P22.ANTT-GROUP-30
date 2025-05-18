using System.Windows;
using ThuAo.Services;
using System.Threading.Tasks;
using System;

namespace ThuAo
{
    public partial class ForgotPassword : Window
    {
        private readonly UserService _userService = new UserService();
        private string _generatedOtp;
        private string _userEmail;

        public ForgotPassword()
        {
            InitializeComponent();
        }

        private void SendOtp_Click(object sender, RoutedEventArgs e)
        {
            _userEmail = EmailTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(_userEmail))
            {
                MessageBox.Show("Please enter your email.");
                return;
            }

            // Generate OTP
            _generatedOtp = new Random().Next(100000, 999999).ToString();

            // TODO: Send OTP to email (giả lập bằng MessageBox)
            MessageBox.Show($"[Simulated OTP] Your OTP is: {_generatedOtp}");
        }

        private async void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string enteredOtp = OtpTextBox.Text.Trim();
            string newPassword = NewPasswordBox.Password;

            if (enteredOtp != _generatedOtp)
            {
                MessageBox.Show("Invalid OTP.");
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Please enter a new password.");
                return;
            }

            bool updated = await _userService.UpdatePasswordByEmail(_userEmail, newPassword);

            if (updated)
            {
                MessageBox.Show("Password has been reset successfully.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to reset password.");
            }
        }
    }
}

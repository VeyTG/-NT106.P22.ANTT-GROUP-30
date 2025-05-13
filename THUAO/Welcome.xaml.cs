// Welcome.xaml.cs
using System;
using System.Windows;

namespace ThuAo
{
    public partial class Welcome : Window
    {
        public Welcome()
        {
            InitializeComponent();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the LoginWindow
            Login loginWindow = new Login();
            loginWindow.Show();

            // Close the current Welcome window (optional)
            this.Close();
        }
    }
}

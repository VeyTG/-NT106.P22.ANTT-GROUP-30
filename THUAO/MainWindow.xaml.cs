using System.Windows;

namespace ThuAo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Hiển thị tên người dùng từ Session vào TextBlock
            UsernameTextBlock.Text = $"Welcome, {Session.Username}";
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ chơi mới
            PlayWindow playWindow = new PlayWindow();
            playWindow.Show();

            // Đóng cửa sổ hiện tại (MainWindow)
            this.Close();
        }
    }
}

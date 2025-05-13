using System;
using System.Windows;
using System.Windows.Threading;

namespace ThuAo
{
    public partial class PlayWindow : Window
    {
        private DispatcherTimer statusTimer;
        private double foodLevel = 100;
        private double sleepLevel = 100;
        private double studyLevel = 100;

        public PlayWindow()
        {
            InitializeComponent();
            StartStatusTimer();
        }

        // Bắt đầu giảm dần các thanh trạng thái theo thời gian
        private void StartStatusTimer()
        {
            statusTimer = new DispatcherTimer();
            statusTimer.Interval = TimeSpan.FromSeconds(10); // Giảm dần mỗi 10s
            statusTimer.Tick += (s, e) =>
            {
                foodLevel = Math.Max(0, foodLevel - 1);  // Giảm 1% thức ăn mỗi phút
                sleepLevel = Math.Max(0, sleepLevel - 0.5); // Giảm 0.5% giấc ngủ mỗi phút
                studyLevel = Math.Max(0, studyLevel - 0.2); // Giảm 0.2% học bài mỗi phút

                FoodBar.Value = foodLevel;
                SleepBar.Value = sleepLevel;
                StudyBar.Value = studyLevel;

                // Cảnh báo nếu bất kỳ thanh trạng thái nào dưới 10%
                if (foodLevel < 10 || sleepLevel < 10 || studyLevel < 10)
                {
                    MessageBox.Show("⚠️ Thú cưng đang cần chăm sóc, hãy giúp nó!");
                }
            };
            statusTimer.Start();
        }


        // Khi người dùng nhấn nút "Cho ăn"
        private async void FeedButton_Click(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ chơi mới
            FeedWindow FeedWindow = new FeedWindow();
            FeedWindow.Show();

            // Đóng cửa sổ hiện tại (PlayWindow)
            this.Close();
        }

        // Khi người dùng nhấn nút "Ngủ"
        private void SleepButton_Click(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ chơi mới
            Bedroom Bedroom = new Bedroom();
            Bedroom.Show();

            // Đóng cửa sổ hiện tại (PlayWindow)
            this.Close();
        }

        // Khi người dùng nhấn nút "Học bài"
        private void StudyButton_Click(object sender, RoutedEventArgs e)
        {
            studyLevel = Math.Min(100, studyLevel + 10); // Tăng 10% cho học bài
            StudyBar.Value = studyLevel;

            // Cập nhật lại trạng thái sau khi học bài
            if (studyLevel == 100)
            {
                MessageBox.Show("Tính năng đang được phát triển!");
            }
        }
       

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Giả sử hiển thị một MessageBox. Bạn có thể tạo một SettingsWindow riêng sau.
            MessageBox.Show("🔧 Cài đặt đang được phát triển...", "Cài đặt", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Mở menu ☰
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MenuPopup.IsOpen = true;
        }

        // Chọn "Trang chủ"
        private void HomeMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        // Chọn "Thoát ứng dụng"
        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private bool isSoundOn = true;

        private void SoundButton_Click(object sender, RoutedEventArgs e)
        {
            isSoundOn = !isSoundOn;

            if (isSoundOn)
            {
                SoundButton.Content = "🔊";
                SoundButton.ToolTip = "Tắt âm thanh";
                App.SetMusicVolume(1.0); // Gọi từ App.xaml.cs
            }
            else
            {
                SoundButton.Content = "🔇";
                SoundButton.ToolTip = "Bật âm thanh";
                App.SetMusicVolume(0.0); // Tắt âm thanh
            }
        }

    }
}

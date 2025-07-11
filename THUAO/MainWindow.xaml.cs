using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using THUAO.Properties;

namespace ThuAo
{
    public partial class MainWindow : Window
    {
        /* ==== CÁC BIẾN & KHỞI TẠO ==== */
        private bool isSoundOn = true;            // Nút âm thanh góc phải

        private readonly string soundOnImagePath = "Assets/Button/setting/am_thanh.png";
        private readonly string soundOffImagePath = "Assets/Button/setting/tat_am.png";

        public MainWindow()
        {
            InitializeComponent();

            UsernameTextBlock.Text = $"Hello, {Session.Username}";

            /* Đọc cấu hình đã lưu */
            isSoundOn = Settings.Default.MusicOn;

            /* Cập nhật hình ảnh và âm lượng */
            UpdateSoundImage(isSoundOn);
            App.SetMusicVolume(isSoundOn ? 1.0 : 0.0);
        }

        /* ======= HÀM DÙNG CHUNG ======= */
        private void UpdateSoundImage(bool isOn)
        {
            SoundImage.Source = new BitmapImage(new Uri(isOn ? soundOnImagePath : soundOffImagePath, UriKind.Relative));
            SoundImage.ToolTip = isOn ? "Tắt âm thanh" : "Bật âm thanh";
        }

        private void AnimateClickEffect(UIElement element)
        {
            element.RenderTransform = new ScaleTransform(1.0, 1.0);
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            var sb = (Storyboard)FindResource("ClickEffectStoryboard");
            var clone = sb.Clone();
            Storyboard.SetTarget(clone, element);
            clone.Begin();
        }

        /* ====== NÚT ÂM THANH GÓC TRÊN ====== */
        private void SoundImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(SoundImage);

            isSoundOn = !isSoundOn;
            Settings.Default.MusicOn = isSoundOn;
            Settings.Default.Save();

            UpdateSoundImage(isSoundOn);
            App.SetMusicVolume(isSoundOn ? 1.0 : 0.0);
        }

        /* ===== MỞ / ĐÓNG LỚP PHỦ SETTING ===== */
        private void SettingsImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);

            // Đảo visibility của SettingsGrid trong SettingControl
            var settingsGrid = SettingControl.FindName("SettingsGrid") as Grid;
            if (settingsGrid != null)
            {
                settingsGrid.Visibility = settingsGrid.Visibility == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        /* ===== MENU & PLAY ===== */
        private void MenuImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            Application.Current.Shutdown();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var gameState = new GameState();
            var playWindow = new PlayWindow(gameState);
            playWindow.Show();
            Close();
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using THUAO.Properties;

namespace ThuAo
{
    public partial class SettingOverlay : UserControl
    {
        private bool isMusicOn;
        private bool isSoundOverlayOn;
        private bool isNotifyOn;
        private readonly string onImagePath = "Assets/Button/setting/on.png";
        private readonly string offImagePath = "Assets/Button/setting/off.png";

        public SettingOverlay()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            isMusicOn = Settings.Default.MusicOn;
            isSoundOverlayOn = Settings.Default.SoundOn;
            isNotifyOn = Settings.Default.NotifyOn;

            UpdateImage(MusicImage, isMusicOn);
            UpdateImage(SoundOverlayImage, isSoundOverlayOn);
            UpdateImage(NotifyImage, isNotifyOn);

            App.SetMusicVolume(isMusicOn ? 1.0 : 0.0);
        }

        private void UpdateImage(Image img, bool isOn)
        {
            img.Source = new BitmapImage(new Uri(isOn ? onImagePath : offImagePath, UriKind.Relative));
        }

        private void MusicImage_Click(object sender, MouseButtonEventArgs e)
        {
            isMusicOn = !isMusicOn;
            UpdateImage(MusicImage, isMusicOn);
            App.SetMusicVolume(isMusicOn ? 1.0 : 0.0);
        }

        private void SoundOverlayImage_Click(object sender, MouseButtonEventArgs e)
        {
            isSoundOverlayOn = !isSoundOverlayOn;
            UpdateImage(SoundOverlayImage, isSoundOverlayOn);
            // TODO: xử lý âm thanh hiệu ứng nếu cần
        }

        private void NotifyImage_Click(object sender, MouseButtonEventArgs e)
        {
            isNotifyOn = !isNotifyOn;
            UpdateImage(NotifyImage, isNotifyOn);
            // TODO: xử lý thông báo nếu cần
        }

        private void SaveImage_Click(object sender, MouseButtonEventArgs e)
        {
            Settings.Default.MusicOn = isMusicOn;
            Settings.Default.SoundOn = isSoundOverlayOn;
            Settings.Default.NotifyOn = isNotifyOn;
            Settings.Default.Save();

            SettingsGrid.Visibility = Visibility.Collapsed; // Đóng overlay
        }
    }
}
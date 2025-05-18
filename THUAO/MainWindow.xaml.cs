using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;

namespace ThuAo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Hiển thị tên người dùng từ Session vào TextBlock
            UsernameTextBlock.Text = $"Hello, {Session.Username}";
        }

        private void AnimateClickEffect(UIElement element)
        {
            element.RenderTransform = new ScaleTransform(1.0, 1.0);
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            Storyboard sb = (Storyboard)FindResource("ClickEffectStoryboard");
            Storyboard clone = sb.Clone(); // Đảm bảo hiệu ứng chạy độc lập mỗi lần
            Storyboard.SetTarget(clone, element);
            clone.Begin();
        }

        private bool isSoundOn = true;

        private void SoundImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            isSoundOn = !isSoundOn;

            var image = sender as Image;
            if (image == null) return;

            if (isSoundOn)
            {
                image.Source = new BitmapImage(new Uri("Assets/Button/setting/am_thanh.png", UriKind.Relative));
                image.ToolTip = "Tắt âm thanh";
                App.SetMusicVolume(1.0);
            }
            else
            {
                image.Source = new BitmapImage(new Uri("Assets/Button/setting/tat_am.png", UriKind.Relative));
                image.ToolTip = "Bật âm thanh";
                App.SetMusicVolume(0.0);
            }
        }

        private void SettingsImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            Setting Setting = new Setting();
            Setting.Show();
            this.Close();
        
        }

        private void MenuImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            AnimateClickEffect(sender as UIElement);


            Application.Current.Shutdown();



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

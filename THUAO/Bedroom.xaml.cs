using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using THUAO.Properties;

namespace ThuAo
{
    public partial class Bedroom : Window
    {
        private readonly GameState gameState; // Biến lưu trạng thái game chung
        private DispatcherTimer energyTimer;
        private DispatcherTimer sleepDelayTimer;
        private DispatcherTimer sleepIncreaseTimer;

        public Bedroom(GameState gameState)
        {
            this.gameState = gameState;
            InitializeComponent();
            UpdateFoodEnergyVisual();
            UpdateSleepEnergyVisual();
            UpdateStudyEnergyVisual();
            UpdateCoinDisplay(); // Cập nhật hiển thị số xu

            // Khởi tạo timer giảm năng lượng
            energyTimer = new DispatcherTimer();
            energyTimer.Interval = TimeSpan.FromSeconds(15);
            energyTimer.Tick += EnergyTimer_Tick;
            energyTimer.Start();

            // Khởi tạo và đồng bộ SettingOverlay
            InitializeSettingOverlay();
        }

        private void StartSleepDelay()
        {
            // Nếu có timer cũ thì dừng lại
            sleepDelayTimer?.Stop();
            sleepDelayTimer = new DispatcherTimer();
            sleepDelayTimer.Interval = TimeSpan.FromSeconds(10); // đợi 10s

            sleepDelayTimer.Tick += (s, e) =>
            {
                sleepDelayTimer.Stop();
                StartIncreasingSleepEnergy(); // sau 10s thì bắt đầu tăng năng lượng
            };

            sleepDelayTimer.Start();
        }

        // Tăng dần năng lượng sau 10s delay
        private void StartIncreasingSleepEnergy()
        {
            sleepIncreaseTimer?.Stop();
            sleepIncreaseTimer = new DispatcherTimer();
            sleepIncreaseTimer.Interval = TimeSpan.FromSeconds(1);

            sleepIncreaseTimer.Tick += (s, e) =>
            {
                if (gameState.SleepEnergy >= GameState.MaxEnergy)
                {
                    sleepIncreaseTimer.Stop();
                    return;
                }

                gameState.SleepEnergy += 1; // tăng mỗi giây
                if (gameState.SleepEnergy > GameState.MaxEnergy) gameState.SleepEnergy = GameState.MaxEnergy;

                UpdateSleepEnergyVisual();
            };

            sleepIncreaseTimer.Start();
        }

        private void EnergyTimer_Tick(object sender, EventArgs e)
        {
            // Giảm năng lượng mỗi 15 giây, không dưới 0
            gameState.FoodEnergy = Math.Max(0, gameState.FoodEnergy - 1);
            gameState.SleepEnergy = Math.Max(0, gameState.SleepEnergy - 0.5);
            gameState.StudyEnergy = Math.Max(0, gameState.StudyEnergy - 0.3);

            UpdateFoodEnergyVisual();
            UpdateSleepEnergyVisual();
            UpdateStudyEnergyVisual();
        }

        private void UpdateFoodEnergyVisual()
        {
            double maxWidth = 190;
            double marginLeft = 22;
            double marginRight = 10;
            double totalWidth = maxWidth - marginLeft - marginRight;
            double widthFill = totalWidth * (gameState.FoodEnergy / GameState.MaxEnergy);

            HealthFill1.Margin = new Thickness(marginLeft, 0, marginRight + (totalWidth - widthFill), 0);
        }

        private void UpdateSleepEnergyVisual()
        {
            double maxWidth = 190;
            double marginLeft = 22;
            double marginRight = 10;
            double totalWidth = maxWidth - marginLeft - marginRight;
            double widthFill = totalWidth * (gameState.SleepEnergy / GameState.MaxEnergy);

            HealthFill2.Margin = new Thickness(marginLeft, 0, marginRight + (totalWidth - widthFill), 0);
        }

        private void UpdateStudyEnergyVisual()
        {
            double maxWidth = 190;
            double marginLeft = 22;
            double marginRight = 10;
            double totalWidth = maxWidth - marginLeft - marginRight;
            double widthFill = totalWidth * (gameState.StudyEnergy / GameState.MaxEnergy);

            HealthFill3.Margin = new Thickness(marginLeft, 0, marginRight + (totalWidth - widthFill), 0);
        }

        private void UpdateCoinDisplay()
        {
            CoinTextBlock.Text = gameState.CoinBalance.ToString();
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

        private void FeedImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            FeedWindow feedWindow = new FeedWindow(gameState);
            feedWindow.Show();
            this.Close();
        }

        private void SleepImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement); // Thêm hiệu ứng click
            StartSleepDelay();
        }

        private void StudyImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            Classroom classroom = new Classroom(gameState);
            classroom.Show();
            this.Close();
        }

        private bool isSoundOn = true;

        private void SoundImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);

            // Đảo trạng thái và lưu lại
            Settings.Default.MusicOn = !Settings.Default.MusicOn;
            Settings.Default.Save(); // Lưu xuống file cấu hình

            var image = sender as Image;
            if (image == null) return;

            if (Settings.Default.MusicOn)
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

        private void MenuImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            PlayWindow playWindow = new PlayWindow(gameState);
            playWindow.Show();
            this.Close();
        }

        // Phương thức mới để khởi tạo và đồng bộ SettingOverlay
        private void InitializeSettingOverlay()
        {
            isSoundOn = Settings.Default.MusicOn; // Đồng bộ với Settings.Default
            UpdateSoundImage(isSoundOn);

            // Cập nhật trạng thái ban đầu của SettingOverlay
            var musicImage = (Image)SettingControl.FindName("MusicImage");
            var soundOverlayImage = (Image)SettingControl.FindName("SoundOverlayImage");
            var notifyImage = (Image)SettingControl.FindName("NotifyImage");

            if (musicImage != null) musicImage.Source = new BitmapImage(new Uri(Settings.Default.MusicOn ? "Assets/Button/setting/on.png" : "Assets/Button/setting/off.png", UriKind.Relative));
            if (soundOverlayImage != null) soundOverlayImage.Source = new BitmapImage(new Uri(Settings.Default.SoundOn ? "Assets/Button/setting/on.png" : "Assets/Button/setting/off.png", UriKind.Relative));
            if (notifyImage != null) notifyImage.Source = new BitmapImage(new Uri(Settings.Default.NotifyOn ? "Assets/Button/setting/on.png" : "Assets/Button/setting/off.png", UriKind.Relative));

            App.SetMusicVolume(Settings.Default.MusicOn ? 1.0 : 0.0);
        }

        // Cập nhật hình ảnh âm thanh
        private void UpdateSoundImage(bool isOn)
        {
            if (SoundImage != null)
            {
                SoundImage.Source = new BitmapImage(new Uri(isOn ? "Assets/Button/setting/am_thanh.png" : "Assets/Button/setting/tat_am.png", UriKind.Relative));
                SoundImage.ToolTip = isOn ? "Tắt âm thanh" : "Bật âm thanh";
            }
        }

        // Mở SettingOverlay khi nhấn nút Settings
        private void SettingsImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            var settingsGrid = SettingControl.FindName("SettingsGrid") as Grid;
            if (settingsGrid != null)
            {
                settingsGrid.Visibility = settingsGrid.Visibility == Visibility.Collapsed
                    ? Visibility.Visible
                    : settingsGrid.Visibility; // Không thay đổi nếu đã Visible
            }
        }
    }
}
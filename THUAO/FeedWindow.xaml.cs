using System;
using System.Collections.Generic;
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
    public partial class FeedWindow : Window
    {
        private readonly List<FoodItem> foods = new List<FoodItem>()
        {
            new FoodItem("donut.png", "10 xu"),
            new FoodItem("chese.png", "15 xu"),
            new FoodItem("carrot.png", "20 xu"),
            new FoodItem("meat.png", "12 xu"),
            new FoodItem("sandwich.png", "8 xu"),
            new FoodItem("strawberry.png", "18 xu")
        };

        private int currentFoodIndex = 0;
        private readonly GameState gameState; // Biến lưu trạng thái game chung
        private DispatcherTimer energyTimer;

        public FeedWindow(GameState state)
        {
            InitializeComponent();
            gameState = state; // Gán trạng thái game được truyền vào

            LoadCurrentFood();
            UpdateFoodEnergyVisual();
            UpdateSleepEnergyVisual();
            UpdateStudyEnergyVisual();
            UpdateCoinDisplay(); // Cập nhật hiển thị số xu

            // Khởi tạo timer giảm năng lượng
            energyTimer = new DispatcherTimer();
            energyTimer.Interval = TimeSpan.FromSeconds(5);
            energyTimer.Tick += EnergyTimer_Tick;
            energyTimer.Start();

            // Khởi tạo và đồng bộ SettingOverlay
            InitializeSettingOverlay();
        }

        private void EnergyTimer_Tick(object sender, EventArgs e)
        {
            // Giảm năng lượng mỗi 5 giây, không dưới 0
            gameState.FoodEnergy = Math.Max(0, gameState.FoodEnergy - 1);
            gameState.SleepEnergy = Math.Max(0, gameState.SleepEnergy - 0.5);
            gameState.StudyEnergy = Math.Max(0, gameState.StudyEnergy - 0.3);

            UpdateFoodEnergyVisual();
            UpdateSleepEnergyVisual();
            UpdateStudyEnergyVisual();
        }

        private void LoadCurrentFood()
        {
            var currentFood = foods[currentFoodIndex];
            string basePath = "Assets/Button/Food/";
            FoodImage1.Source = LoadImage(basePath + currentFood.ImageFile);
            FoodPrice.Text = currentFood.Price;
        }

        private BitmapImage LoadImage(string relativePath)
        {
            try
            {
                var uri = new Uri(relativePath, UriKind.Relative);
                return new BitmapImage(uri);
            }
            catch
            {
                return null;
            }
        }

        private void Food_Click(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            IncreaseFoodEnergy(10);
            PlayEatEffect();
        }

        private void IncreaseFoodEnergy(double amount)
        {
            gameState.FoodEnergy += amount;
            if (gameState.FoodEnergy > GameState.MaxEnergy) gameState.FoodEnergy = GameState.MaxEnergy;
            UpdateFoodEnergyVisual();
        }

        /*private void IncreaseSleepEnergy(double amount)
        {
            gameState.SleepEnergy += amount;
            if (gameState.SleepEnergy > GameState.MaxEnergy) gameState.SleepEnergy = GameState.MaxEnergy;
            UpdateSleepEnergyVisual();
        }

        private void IncreaseStudyEnergy(double amount)
        {
            gameState.StudyEnergy += amount;
            if (gameState.StudyEnergy > GameState.MaxEnergy) gameState.StudyEnergy = GameState.MaxEnergy;
            UpdateStudyEnergyVisual();
        }*/

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

        private void PlayEatEffect()
        {
            FoodImage.Source = FoodImage1.Source;
            FoodImage.Visibility = Visibility.Visible;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(600);
            timer.Tick += (s, e) =>
            {
                FoodImage.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
            timer.Start();
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

        private void ArrowImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            var img = sender as System.Windows.Controls.Image;
            if (img == null) return;

            if (img.ToolTip.ToString() == "Tiếp theo")
            {
                currentFoodIndex++;
                if (currentFoodIndex >= foods.Count) currentFoodIndex = 0;
            }
            else if (img.ToolTip.ToString() == "Quay lại")
            {
                currentFoodIndex--;
                if (currentFoodIndex < 0) currentFoodIndex = foods.Count - 1;
            }
            LoadCurrentFood();
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

        // Định nghĩa lớp món ăn
        private class FoodItem
        {
            public string ImageFile { get; set; }
            public string Price { get; set; }

            public FoodItem(string imageFile, string price)
            {
                ImageFile = imageFile;
                Price = price;
            }
        }
    }
}
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
        private MediaPlayer eatPlayer;
        private readonly List<FoodItem> foods = new List<FoodItem>();
        private int currentFoodIndex = 0;
        private readonly GameState gameState;
        private DispatcherTimer energyTimer;
        private bool isSoundOn = true;

        public FeedWindow(GameState state)
        {
            InitializeComponent();
            gameState = state;
            eatPlayer = new MediaPlayer();
            eatPlayer.Open(new Uri("Assets/Am_thanh/eat.mp3", UriKind.Relative));
            eatPlayer.Volume = 1.0;

            InitializeFoods();
            LoadCurrentFood();
            UpdateAllEnergies();
            UpdateCoinDisplay();
            InitializeSettingOverlay();

            // Khởi tạo timer giảm năng lượng
            energyTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            energyTimer.Tick += EnergyTimer_Tick;
            energyTimer.Start();
        }

        private void InitializeFoods()
        {
            if (!Settings.Default.IsFirstTimeFeed)
            {
                gameState.OwnedFoods.Add("donut.png");
                gameState.OwnedFoods.Add("meat.png");
                gameState.OwnedFoods.Add("sandwich.png");

                Settings.Default.IsFirstTimeFeed = true;
                Settings.Default.Save();
            }

            foods.Clear();
            foreach (var foodName in gameState.OwnedFoods)
            {
                foods.Add(new FoodItem(foodName));
            }

            if (foods.Count == 0)
            {
                foods.Add(new FoodItem("donut.png"));
            }
        }

        private void LoadCurrentFood()
        {
            if (foods.Count == 0)
            {
                FoodImage1.Source = null;
                FoodPrice.Text = "Hết đồ ăn";
                return;
            }

            var currentFood = foods[currentFoodIndex];
            string path = $"Assets/Button/Food/{currentFood.ImageFile}";
            FoodImage1.Source = LoadImage(path);
            FoodPrice.Text = "";
        }

        private BitmapImage LoadImage(string relativePath)
        {
            try
            {
                return new BitmapImage(new Uri(relativePath, UriKind.Relative));
            }
            catch
            {
                return null;
            }
        }

        private void Food_Click(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);

            if (foods.Count == 0) return;

            IncreaseFoodEnergy(10);
            PlayEatEffect();

            string justAte = foods[currentFoodIndex].ImageFile;
            gameState.OwnedFoods.Remove(justAte);
            foods.RemoveAt(currentFoodIndex);

            if (foods.Count == 0)
            {
                FoodImage1.Source = null;
                FoodPrice.Text = "Hết đồ ăn";
                return;
            }

            if (currentFoodIndex >= foods.Count)
                currentFoodIndex = 0;

            LoadCurrentFood();
        }

        private void IncreaseFoodEnergy(double amount)
        {
            gameState.FoodEnergy += amount;
            if (gameState.FoodEnergy > GameState.MaxEnergy)
                gameState.FoodEnergy = GameState.MaxEnergy;

            UpdateFoodEnergyVisual();
        }

        private void PlayEatEffect()
        {
            FoodImage.Source = FoodImage1.Source;
            FoodImage.Visibility = Visibility.Visible;

            if (Settings.Default.SoundOn && eatPlayer != null)
            {
                eatPlayer.Position = TimeSpan.Zero; // tua lại từ đầu
                eatPlayer.Play();
            }

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            timer.Tick += (s, e) =>
            {
                FoodImage.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
            timer.Start();
        }


        private void EnergyTimer_Tick(object sender, EventArgs e)
        {
            gameState.FoodEnergy = Math.Max(0, gameState.FoodEnergy - 1);
            gameState.SleepEnergy = Math.Max(0, gameState.SleepEnergy - 0.5);
            gameState.StudyEnergy = Math.Max(0, gameState.StudyEnergy - 0.3);

            UpdateAllEnergies();
        }

        private void UpdateAllEnergies()
        {
            UpdateFoodEnergyVisual();
            UpdateSleepEnergyVisual();
            UpdateStudyEnergyVisual();
        }

        private void UpdateFoodEnergyVisual() => SetEnergyBar(HealthFill1, gameState.FoodEnergy);
        private void UpdateSleepEnergyVisual() => SetEnergyBar(HealthFill2, gameState.SleepEnergy);
        private void UpdateStudyEnergyVisual() => SetEnergyBar(HealthFill3, gameState.StudyEnergy);

        private void SetEnergyBar(Image bar, double energy)
        {
            double maxWidth = 190;
            double left = 22;
            double right = 10;
            double usable = maxWidth - left - right;
            double fill = usable * (energy / GameState.MaxEnergy);
            bar.Margin = new Thickness(left, 0, right + (usable - fill), 0);
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
            var clone = sb.Clone();
            Storyboard.SetTarget(clone, element);
            clone.Begin();
        }

        private void ArrowImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            var img = sender as Image;
            if (img?.ToolTip?.ToString() == "Tiếp theo")
            {
                currentFoodIndex = (currentFoodIndex + 1) % foods.Count;
            }
            else if (img?.ToolTip?.ToString() == "Quay lại")
            {
                currentFoodIndex = (currentFoodIndex - 1 + foods.Count) % foods.Count;
            }

            LoadCurrentFood();
        }

        private void SoundImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            Settings.Default.MusicOn = !Settings.Default.MusicOn;
            Settings.Default.Save();

            App.SetMusicVolume(Settings.Default.MusicOn ? 1.0 : 0.0);
            UpdateSoundImage(Settings.Default.MusicOn);
        }

        private void UpdateSoundImage(bool isOn)
        {
            if (SoundImage != null)
            {
                SoundImage.Source = new BitmapImage(new Uri(
                    isOn ? "Assets/Button/setting/am_thanh.png" : "Assets/Button/setting/tat_am.png",
                    UriKind.Relative));
                SoundImage.ToolTip = isOn ? "Tắt âm thanh" : "Bật âm thanh";
            }
        }

        private void MenuImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            new PlayWindow(gameState).Show();
            this.Close();
        }

        private void SettingsImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            var settingsGrid = SettingControl.FindName("SettingsGrid") as Grid;
            if (settingsGrid != null)
            {
                settingsGrid.Visibility = settingsGrid.Visibility == Visibility.Collapsed
                    ? Visibility.Visible : settingsGrid.Visibility;
            }
        }

        private void InitializeSettingOverlay()
        {
            isSoundOn = Settings.Default.MusicOn;
            UpdateSoundImage(isSoundOn);

            var musicImage = (Image)SettingControl.FindName("MusicImage");
            var soundOverlayImage = (Image)SettingControl.FindName("SoundOverlayImage");
            var notifyImage = (Image)SettingControl.FindName("NotifyImage");

            if (musicImage != null)
                musicImage.Source = new BitmapImage(new Uri(Settings.Default.MusicOn ? "Assets/Button/setting/on.png" : "Assets/Button/setting/off.png", UriKind.Relative));
            if (soundOverlayImage != null)
                soundOverlayImage.Source = new BitmapImage(new Uri(Settings.Default.SoundOn ? "Assets/Button/setting/on.png" : "Assets/Button/setting/off.png", UriKind.Relative));
            if (notifyImage != null)
                notifyImage.Source = new BitmapImage(new Uri(Settings.Default.NotifyOn ? "Assets/Button/setting/on.png" : "Assets/Button/setting/off.png", UriKind.Relative));

            App.SetMusicVolume(Settings.Default.MusicOn ? 1.0 : 0.0);
        }

        private class FoodItem
        {
            public string ImageFile { get; set; }
            public FoodItem(string imageFile)
            {
                ImageFile = imageFile;
            }
        }

        private void SettingControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Không cần xử lý gì thêm
        }
    }
}

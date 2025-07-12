using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;
using THUAO.Properties;

namespace ThuAo
{
    // Lớp lưu trạng thái chung của game (năng lượng và tiền)
    public class GameState
    {
        public double FoodEnergy { get; set; } = 100;
        public double SleepEnergy { get; set; } = 100;
        public double StudyEnergy { get; set; } = 100;
        public int CoinBalance { get; set; } = 100;
        public List<string> OwnedFoods { get; set; } = new List<string>();
        public static readonly double MaxEnergy = 100;
    }

    public partial class PlayWindow : Window
    {
        private readonly GameState gameState;
        private DispatcherTimer energyTimer;
        private bool isSoundOn = true;

        public PlayWindow(GameState state)
        {
            InitializeComponent();
            gameState = state;

            UpdateEnergyVisuals();
            UpdateCoinDisplay();

            energyTimer = new DispatcherTimer();
            energyTimer.Interval = TimeSpan.FromSeconds(15);
            energyTimer.Tick += EnergyTimer_Tick;
            energyTimer.Start();

            InitializeSettingOverlay();
        }

        private void EnergyTimer_Tick(object sender, EventArgs e)
        {
            gameState.FoodEnergy = Math.Max(0, gameState.FoodEnergy - 1);
            gameState.SleepEnergy = Math.Max(0, gameState.SleepEnergy - 0.5);
            gameState.StudyEnergy = Math.Max(0, gameState.StudyEnergy - 0.3);

            UpdateEnergyVisuals();
        }

        private void UpdateEnergyVisuals()
        {
            UpdateBar(HealthFill1, gameState.FoodEnergy);
            UpdateBar(HealthFill2, gameState.SleepEnergy);
            UpdateBar(HealthFill3, gameState.StudyEnergy);
        }

        private void UpdateBar(FrameworkElement fillElement, double value)
        {
            double maxWidth = 190;
            double marginLeft = 22;
            double marginRight = 10;
            double totalWidth = maxWidth - marginLeft - marginRight;
            double widthFill = totalWidth * (value / GameState.MaxEnergy);

            fillElement.Margin = new Thickness(marginLeft, 0, marginRight + (totalWidth - widthFill), 0);
        }

        private void AnimateClickEffect(UIElement element)
        {
            element.RenderTransform = new ScaleTransform(1.0, 1.0);
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            Storyboard sb = (Storyboard)FindResource("ClickEffectStoryboard");
            Storyboard clone = sb.Clone();
            Storyboard.SetTarget(clone, element);
            clone.Begin();
        }

        private void FeedImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            new FeedWindow(gameState).Show();
            Close();
        }

        private void SleepImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            new Bedroom(gameState).Show();
            Close();
        }

        private void StudyImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            new StudyHub(gameState).Show(); //vào màn hình trung gian StudyHub
            Close();
        }


        private void SoundImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);

            Settings.Default.MusicOn = !Settings.Default.MusicOn;
            Settings.Default.Save();

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
            Application.Current.Shutdown();
        }

        private void UpdateCoinDisplay()
        {
            CoinTextBlock.Text = gameState.CoinBalance.ToString();
        }

        private void InitializeSettingOverlay()
        {
            isSoundOn = Settings.Default.MusicOn;
            UpdateSoundImage(isSoundOn);

            var musicImage = (Image)SettingControl.FindName("MusicImage");
            var soundOverlayImage = (Image)SettingControl.FindName("SoundOverlayImage");
            var notifyImage = (Image)SettingControl.FindName("NotifyImage");

            if (musicImage != null) musicImage.Source = new BitmapImage(new Uri(Settings.Default.MusicOn ? "Assets/Button/setting/on.png" : "Assets/Button/setting/off.png", UriKind.Relative));
            if (soundOverlayImage != null) soundOverlayImage.Source = new BitmapImage(new Uri(Settings.Default.SoundOn ? "Assets/Button/setting/on.png" : "Assets/Button/setting/off.png", UriKind.Relative));
            if (notifyImage != null) notifyImage.Source = new BitmapImage(new Uri(Settings.Default.NotifyOn ? "Assets/Button/setting/on.png" : "Assets/Button/setting/off.png", UriKind.Relative));

            App.SetMusicVolume(Settings.Default.MusicOn ? 1.0 : 0.0);
        }

        private void UpdateSoundImage(bool isOn)
        {
            if (SoundImage != null)
            {
                SoundImage.Source = new BitmapImage(new Uri(isOn ? "Assets/Button/setting/am_thanh.png" : "Assets/Button/setting/tat_am.png", UriKind.Relative));
                SoundImage.ToolTip = isOn ? "Tắt âm thanh" : "Bật âm thanh";
            }
        }

        private void SettingsImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            var settingsGrid = SettingControl.FindName("SettingsGrid") as Grid;
            if (settingsGrid != null)
            {
                settingsGrid.Visibility = settingsGrid.Visibility == Visibility.Collapsed
                    ? Visibility.Visible
                    : settingsGrid.Visibility;
            }
        }
    }
}
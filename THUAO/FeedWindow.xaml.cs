using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;

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

        // Thanh năng lượng ăn, ngủ, học
        private double foodEnergy = 75;
        private double sleepEnergy = 75;
        private double studyEnergy = 75;

        private const double MaxEnergy = 100;

        private DispatcherTimer energyTimer;

        public FeedWindow()
        {
            InitializeComponent();

            LoadCurrentFood();
            UpdateFoodEnergyVisual();
            UpdateSleepEnergyVisual();
            UpdateStudyEnergyVisual();

            // Khởi tạo timer giảm năng lượng
            energyTimer = new DispatcherTimer();
            energyTimer.Interval = TimeSpan.FromSeconds(5);
            energyTimer.Tick += EnergyTimer_Tick;
            energyTimer.Start();
        }

        private void EnergyTimer_Tick(object sender, EventArgs e)
        {
            // Giảm năng lượng mỗi 5 giây, không dưới 0
            foodEnergy = Math.Max(0, foodEnergy - 1);
            sleepEnergy = Math.Max(0, sleepEnergy - 0.5);
            studyEnergy = Math.Max(0, studyEnergy - 0.3);

            UpdateFoodEnergyVisual();
            UpdateSleepEnergyVisual();
            UpdateStudyEnergyVisual();
        }

        private void LoadCurrentFood()
        {
            var currentFood = foods[currentFoodIndex];
            string basePath = "Assets/Button/food/";
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
            foodEnergy += amount;
            if (foodEnergy > MaxEnergy) foodEnergy = MaxEnergy;
            UpdateFoodEnergyVisual();
        }

        // Tăng ngủ (ví dụ)
        /*private void IncreaseSleepEnergy(double amount)
        {
            sleepEnergy += amount;
            if (sleepEnergy > MaxEnergy) sleepEnergy = MaxEnergy;
            UpdateSleepEnergyVisual();
        }

        // Tăng học (ví dụ)
        private void IncreaseStudyEnergy(double amount)
        {
            studyEnergy += amount;
            if (studyEnergy > MaxEnergy) studyEnergy = MaxEnergy;
            UpdateStudyEnergyVisual();
        }*/

        private void UpdateFoodEnergyVisual()
        {
            double maxWidth = 190;
            double marginLeft = 22;
            double marginRight = 10;
            double totalWidth = maxWidth - marginLeft - marginRight;
            double widthFill = totalWidth * (foodEnergy / MaxEnergy);

            HealthFill1.Margin = new Thickness(marginLeft, 0, marginRight + (totalWidth - widthFill), 0);
        }

        private void UpdateSleepEnergyVisual()
        {
            double maxWidth = 190;
            double marginLeft = 22;
            double marginRight = 10;
            double totalWidth = maxWidth - marginLeft - marginRight;
            double widthFill = totalWidth * (sleepEnergy / MaxEnergy);

            HealthFill2.Margin = new Thickness(marginLeft, 0, marginRight + (totalWidth - widthFill), 0);
        }

        private void UpdateStudyEnergyVisual()
        {
            double maxWidth = 190;
            double marginLeft = 22;
            double marginRight = 10;
            double totalWidth = maxWidth - marginLeft - marginRight;
            double widthFill = totalWidth * (studyEnergy / MaxEnergy);

            HealthFill3.Margin = new Thickness(marginLeft, 0, marginRight + (totalWidth - widthFill), 0);
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
                image.Source = new BitmapImage(new Uri("Assets/Button/setting/am_thanh.png", UriKind.Relative));
                image.ToolTip = "Bật âm thanh";
                App.SetMusicVolume(0.0);
            }
        }

        private void SettingsImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            
        }

        private void MenuImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            // Ví dụ quay lại PlayWindow
            PlayWindow playWindow = new PlayWindow();
            playWindow.Show();
            this.Close();
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

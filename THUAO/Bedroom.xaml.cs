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
    public partial class Bedroom : Window
    {
       
        // Thanh năng lượng ăn, ngủ, học
        private double foodEnergy = 75;
        private double sleepEnergy = 75;
        private double studyEnergy = 75;

        private const double MaxEnergy = 100;

        private DispatcherTimer energyTimer;
        private DispatcherTimer sleepDelayTimer;        
        private DispatcherTimer sleepIncreaseTimer;


        public Bedroom()
        {
            InitializeComponent();
            UpdateFoodEnergyVisual();
            UpdateSleepEnergyVisual();
            UpdateStudyEnergyVisual();

            // Khởi tạo timer giảm năng lượng
            energyTimer = new DispatcherTimer();
            energyTimer.Interval = TimeSpan.FromSeconds(15);
            energyTimer.Tick += EnergyTimer_Tick;
            energyTimer.Start();

        }
           private void StartSleepDelay()
        {
            // Nếu có timer cũ thì dừng lại
            sleepDelayTimer?.Stop();
            sleepDelayTimer = new DispatcherTimer();
            sleepDelayTimer.Interval = TimeSpan.FromSeconds(10); // đợi 40s

            sleepDelayTimer.Tick += (s, e) =>
            {
                sleepDelayTimer.Stop();
                StartIncreasingSleepEnergy();  // sau 40s thì bắt đầu tăng năng lượng
            };

            sleepDelayTimer.Start();
        }

        // Tăng dần năng lượng sau 40s delay
        private void StartIncreasingSleepEnergy()
        {
            sleepIncreaseTimer?.Stop();
            sleepIncreaseTimer = new DispatcherTimer();
            sleepIncreaseTimer.Interval = TimeSpan.FromSeconds(1);

            sleepIncreaseTimer.Tick += (s, e) =>
            {
                if (sleepEnergy >= MaxEnergy)
                {
                    sleepIncreaseTimer.Stop();
                    return;
                }

                sleepEnergy += 1;  // tăng mỗi giây
                if (sleepEnergy > MaxEnergy) sleepEnergy = MaxEnergy;

                UpdateSleepEnergyVisual();
            };

            sleepIncreaseTimer.Start();
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
            // Mở cửa sổ chơi mới
            FeedWindow FeedWindow = new FeedWindow();
            FeedWindow.Show();
            this.Close();
        }

        // Ngủ
        private void SleepImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StartSleepDelay();
        }

        // Học bài
        private void StudyImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            Classroom Classroom = new Classroom();
            Classroom.Show();
            this.Close();
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

        
    }
}

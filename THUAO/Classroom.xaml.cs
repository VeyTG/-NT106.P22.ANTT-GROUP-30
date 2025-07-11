using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ThuAo.Models;
using THUAO.Properties;

namespace ThuAo
{
    public partial class Classroom : Window
    {
        private List<Question> questions;
        private int currentQuestionIndex = 0;
        private readonly GameState gameState; // Biến lưu trạng thái game được truyền từ PlayWindow
        private DispatcherTimer energyTimer;
        private bool isSoundOn = true;

        public Classroom(GameState gameState)
        {
            this.gameState = gameState; // Nhận GameState từ PlayWindow
            InitializeComponent();
            LoadQuestions();
            DisplayQuestion(currentQuestionIndex);
            UpdateFoodEnergyVisual();
            UpdateSleepEnergyVisual();
            UpdateStudyEnergyVisual();
            UpdateCoinDisplay();

            energyTimer = new DispatcherTimer();
            energyTimer.Interval = TimeSpan.FromSeconds(15);
            energyTimer.Tick += EnergyTimer_Tick;
            energyTimer.Start();

            // Khởi tạo và cập nhật trạng thái SettingOverlay
            InitializeSettingOverlay();
        }

        private void LoadQuestions()
        {
            questions = new List<Question>
            {
                new Question
                {
                    Content = "Trái Đất quay quanh Mặt Trời theo quỹ đạo gì?",
                    Options = new List<string> { "Hình vuông", "Hình tròn", "Hình elip", "Hình tam giác" },
                    CorrectIndex = 2
                },
                new Question
                {
                    Content = "Ai là cha đẻ của máy tính?",
                    Options = new List<string> { "Bill Gates", "Alan Turing", "Steve Jobs", "Charles Babbage" },
                    CorrectIndex = 3
                },
                new Question
                {
                    Content = "Ngôn ngữ lập trình nào được dùng để xây dựng giao diện Web?",
                    Options = new List<string> { "HTML", "Python", "C++", "Java" },
                    CorrectIndex = 0
                },
                new Question
                {
                    Content = "Ai là người sáng lập Microsoft?",
                    Options = new List<string> { "Steve Jobs", "Mark Zuckerberg", "Elon Musk", "Bill Gates" },
                    CorrectIndex = 3
                },
            };
        }

        private void DisplayQuestion(int index)
        {
            if (index < 0 || index >= questions.Count) return;

            var q = questions[index];
            QuestionText.Text = q.Content;
            OptionA.Text = "A. " + q.Options[0];
            OptionB.Text = "B. " + q.Options[1];
            OptionC.Text = "C. " + q.Options[2];
            OptionD.Text = "D. " + q.Options[3];

            ResetOptionColors();
            ResultText.Text = "";
        }

        private void ResetOptionColors()
        {
            foreach (var child in AnswerPanel.Children)
            {
                if (child is TextBlock tb)
                {
                    tb.Background = new SolidColorBrush(Color.FromRgb(0xEE, 0xEE, 0xEE));
                    tb.Foreground = Brushes.Black;
                }
            }
        }

        private void Answer_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock clicked)
            {
                // Kiểm tra đủ xu để trả lời (5 xu mỗi lần)
                if (gameState.CoinBalance < 5)
                {
                    ResultText.Foreground = Brushes.Red;
                    ResultText.Text = "Không đủ xu!";
                    return;
                }

                // Trừ 5 xu khi trả lời câu hỏi
                gameState.CoinBalance -= 5;
                UpdateCoinDisplay();

                int selectedIndex = AnswerPanel.Children.IndexOf(clicked);
                var correctIndex = questions[currentQuestionIndex].CorrectIndex;

                ResetOptionColors();

                if (selectedIndex == correctIndex)
                {
                    clicked.Background = Brushes.LightGreen;
                    ResultText.Foreground = Brushes.Green;
                    ResultText.Text = "Chính xác!";
                    gameState.CoinBalance += 10; // Thưởng 10 xu khi trả lời đúng
                    gameState.StudyEnergy = Math.Min(GameState.MaxEnergy, gameState.StudyEnergy + 5); // Tăng 5 năng lượng học khi đúng
                }
                else
                {
                    clicked.Background = Brushes.IndianRed;
                    ResultText.Foreground = Brushes.Red;
                    ResultText.Text = $"Sai rồi! Đáp án đúng là: {(char)(65 + correctIndex)}";
                    gameState.StudyEnergy = Math.Min(GameState.MaxEnergy, gameState.StudyEnergy + 2); // Tăng 2 năng lượng học khi sai

                    if (correctIndex >= 0 && correctIndex < AnswerPanel.Children.Count)
                    {
                        if (AnswerPanel.Children[correctIndex] is TextBlock correctOption)
                        {
                            correctOption.Background = Brushes.LightGreen;
                        }
                    }
                }

                // Cập nhật giao diện sau khi thay đổi
                UpdateStudyEnergyVisual();
                UpdateCoinDisplay();
            }
        }

        private void EnergyTimer_Tick(object sender, EventArgs e)
        {
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
            Storyboard clone = sb.Clone();
            Storyboard.SetTarget(clone, element);
            clone.Begin();
        }

        private void ArrowImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            var image = sender as Image;
            string source = image?.Source.ToString();

            if (source != null && source.Contains("left"))
            {
                if (currentQuestionIndex > 0)
                    currentQuestionIndex--;
            }
            else if (source != null && source.Contains("right"))
            {
                if (currentQuestionIndex < questions.Count - 1)
                    currentQuestionIndex++;
            }

            DisplayQuestion(currentQuestionIndex);
        }

        private void FeedImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            FeedWindow feedWindow = new FeedWindow(gameState);
            feedWindow.Show();
            this.Close();
        }

        private void SleepImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            Bedroom bedroom = new Bedroom(gameState);
            bedroom.Show();
            this.Close();
        }

        private void StudyImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
        }

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
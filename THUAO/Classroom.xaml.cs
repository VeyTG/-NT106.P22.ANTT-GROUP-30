using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using ThuAo.Models;
using System.Windows.Media.Animation;

namespace ThuAo
{
    public partial class Classroom : Window
    {
        private List<Question> questions;
        private int currentQuestionIndex = 0;

        public Classroom()
        {
            InitializeComponent();
            LoadQuestions();
            DisplayQuestion(currentQuestionIndex);
            // Khởi tạo timer giảm năng lượng
            energyTimer = new DispatcherTimer();
            energyTimer.Interval = TimeSpan.FromSeconds(15);
            energyTimer.Tick += EnergyTimer_Tick;
            energyTimer.Start();
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

            // Reset màu và kết quả
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
                int selectedIndex = AnswerPanel.Children.IndexOf(clicked);
                var correctIndex = questions[currentQuestionIndex].CorrectIndex;

                // Reset lại màu của tất cả đáp án trước khi tô màu mới
                ResetOptionColors();

                if (selectedIndex == correctIndex)
                {
                    clicked.Background = Brushes.LightGreen;
                    ResultText.Foreground = Brushes.Green;
                    ResultText.Text = "Chính xác!";
                }
                else
                {
                    clicked.Background = Brushes.IndianRed; // Đáp án sai tô đỏ
                    ResultText.Foreground = Brushes.Red;
                    ResultText.Text = $"Sai rồi! Đáp án đúng là: {(char)(65 + correctIndex)}";

                    // Tô màu xanh đáp án đúng
                    if (correctIndex >= 0 && correctIndex < AnswerPanel.Children.Count)
                    {
                        if (AnswerPanel.Children[correctIndex] is TextBlock correctOption)
                        {
                            correctOption.Background = Brushes.LightGreen;
                        }
                    }
                }
            }
        }


        // Thanh năng lượng ăn, ngủ, học
        private double foodEnergy = 50;
        private double sleepEnergy = 50;
        private double studyEnergy = 50;

        private const double MaxEnergy = 100;

        private DispatcherTimer energyTimer;

       

        private void EnergyTimer_Tick(object sender, EventArgs e)
        {
            // Giảm năng lượng mỗi 5 giây, không dưới 0
            foodEnergy = Math.Max(0, foodEnergy - 1);
            sleepEnergy = Math.Max(0, sleepEnergy - 0.5);
            studyEnergy = Math.Max(0, studyEnergy - 0.3);

           
            //UpdateSleepEnergyVisual();
            //UpdateStudyEnergyVisual();
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



        /* private void UpdateSleepEnergyVisual()
         {
             double maxWidth = 190;
             double marginLeft = 22;
             double marginRight = 10;
             double totalWidth = maxWidth - marginLeft - marginRight;
             double widthFill = totalWidth * (sleepEnergy / MaxEnergy);

             HealthFill2.Margin = new Thickness(marginLeft, 0, marginRight + (totalWidth - widthFill), 0);
         }*/

        /* private void UpdateStudyEnergyVisual()
         {
             double maxWidth = 190;
             double marginLeft = 22;
             double marginRight = 10;
             double totalWidth = maxWidth - marginLeft - marginRight;
             double widthFill = totalWidth * (studyEnergy / MaxEnergy);

             HealthFill3.Margin = new Thickness(marginLeft, 0, marginRight + (totalWidth - widthFill), 0);
         }*/

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


        private void FeedImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Mở cửa sổ chơi mới
            FeedWindow FeedWindow = new FeedWindow();
            FeedWindow.Show();
            this.Close();
        }

        // Ngủ
        private void SleepImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Mở cửa sổ chơi mới
            Bedroom Bedroom = new Bedroom();
            Bedroom.Show();

            // Đóng cửa sổ hiện tại (PlayWindow)
            this.Close();
        }

        // Học bài
        private void StudyImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Classroom Classroom = new Classroom();
            Classroom.Show();
            this.Close();
        }

        private bool isSoundOn = true;

        private void SoundImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isSoundOn = !isSoundOn;
            AnimateClickEffect(sender as UIElement);
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
            // Ví dụ quay lại PlayWindow
            PlayWindow playWindow = new PlayWindow();
            playWindow.Show();
            this.Close();
        }

       
    }
}

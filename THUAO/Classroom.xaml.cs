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
        private HashSet<int> correctlyAnsweredQuestions = new HashSet<int>();

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
                new Question
            {
                Content = "Trong Vật lý, định luật bảo toàn năng lượng phát biểu điều gì?",
                Options = new List<string> {
                    "Năng lượng có thể được tạo ra và mất đi",
                    "Năng lượng chỉ có thể tăng lên",
                    "Năng lượng không thể tạo ra hoặc mất đi, chỉ chuyển hóa",
                    "Năng lượng có thể bị phá hủy trong điều kiện đặc biệt"
                },
                CorrectIndex = 2
            },
            new Question
            {
                Content = "Tác giả của thuyết tương đối nổi tiếng là ai?",
                Options = new List<string> {
                    "Isaac Newton", "Albert Einstein", "Stephen Hawking", "Galileo Galilei"
                },
                CorrectIndex = 1
            },
            new Question
            {
                Content = "Tổng góc trong của một đa giác 10 cạnh là bao nhiêu độ?",
                Options = new List<string> {
                    "1440", "1620", "1800", "1980"
                },
                CorrectIndex = 2
            },
            new Question
            {
                Content = "Dãy số Fibonacci bắt đầu với 0, 1, tiếp theo là gì?",
                Options = new List<string> {
                    "2", "3", "1", "4"
                },
                CorrectIndex = 2
            },
            new Question
            {
                Content = "Ngôn ngữ lập trình nào thường được dùng để xây dựng hệ điều hành?",
                Options = new List<string> {
                    "Python", "C", "Java", "Ruby"
                },
                CorrectIndex = 1
            },
            new Question
            {
                Content = "Trong Hóa học, ký hiệu hóa học của Natri là gì?",
                Options = new List<string> {
                    "Na", "N", "Ni", "Ns"
                },
                CorrectIndex = 0
            },
            new Question
            {
                Content = "Câu lệnh SQL nào được dùng để xóa bảng khỏi cơ sở dữ liệu?",
                Options = new List<string> {
                    "DROP TABLE", "DELETE TABLE", "REMOVE TABLE", "CLEAR TABLE"
                },
                CorrectIndex = 0
            },
            new Question
            {
                Content = "Đơn vị đo cường độ dòng điện là gì?",
                Options = new List<string> {
                    "Watt", "Volt", "Ampere", "Ohm"
                },
                CorrectIndex = 2
            },
            new Question
            {
                Content = "Sự kiện nào đánh dấu sự bắt đầu của Thế chiến thứ hai?",
                Options = new List<string> {
                    "Nhật tấn công Trân Châu Cảng", "Đức tấn công Ba Lan", "Hiệp định Versailles", "Mỹ tham chiến"
                },
                CorrectIndex = 1
            },
            new Question
            {
                Content = "Kết quả của tích phân ∫₀¹ x² dx là bao nhiêu?",
                Options = new List<string> {
                    "1", "1/3", "1/2", "2/3"
                },
                CorrectIndex = 1
            },
            new Question
            {
                Content = "Số nguyên tố lớn nhất nhỏ hơn 100 là bao nhiêu?",
                Options = new List<string> { "89", "97", "93", "91" },
                CorrectIndex = 1
            },
            new Question
            {
                Content = "Trong cấu trúc dữ liệu, thuật toán tìm kiếm nhị phân áp dụng tốt nhất khi nào?",
                Options = new List<string> {
                    "Dữ liệu không được sắp xếp", "Dữ liệu có số lượng phần tử nhỏ",
                    "Dữ liệu đã được sắp xếp", "Dữ liệu là dạng xâu ký tự"
                },
                CorrectIndex = 2
            },
            new Question
            {
                Content = "Từ 'algorithm' bắt nguồn từ tên của nhà toán học nào?",
                Options = new List<string> {
                    "Al-Khwarizmi", "Euclid", "Archimedes", "Pythagoras"
                },
                CorrectIndex = 0
            },
            new Question
            {
                Content = "Trong hệ điều hành, deadlock xảy ra khi nào?",
                Options = new List<string> {
                    "Các tiến trình chờ tài nguyên không được cấp phát",
                    "Một tiến trình bị lỗi logic",
                    "Tất cả tiến trình bị treo do tranh chấp tài nguyên",
                    "CPU quá tải không xử lý kịp"
                },
                CorrectIndex = 2
            },
            new Question
            {
                Content = "Giá trị của biểu thức logic: (true && false) || (!false && true) là gì?",
                Options = new List<string> {
                    "true", "false", "null", "undefined"
                },
                CorrectIndex = 0
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

            if (correctlyAnsweredQuestions.Contains(index))
            {
                // Tô lại màu đáp án đúng
                int correct = q.CorrectIndex;
                if (AnswerPanel.Children[correct] is TextBlock tb)
                    tb.Background = Brushes.LightGreen;

                ResultText.Text = "Bạn đã trả lời đúng câu này!";
                ResultText.Foreground = Brushes.Green;
            }
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
                // Nếu đã trả lời đúng câu này thì không cho chọn lại nữa
                if (correctlyAnsweredQuestions.Contains(currentQuestionIndex))
                {
                    ResultText.Foreground = Brushes.Gray;
                    ResultText.Text = "Bạn đã trả lời đúng câu này rồi!";
                    return;
                }

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
                int correctIndex = questions[currentQuestionIndex].CorrectIndex;

                ResetOptionColors();

                if (selectedIndex == correctIndex)
                {
                    clicked.Background = Brushes.LightGreen;
                    ResultText.Foreground = Brushes.Green;
                    ResultText.Text = "Chính xác!";
                    gameState.CoinBalance += 30;
                    gameState.StudyEnergy = Math.Min(GameState.MaxEnergy, gameState.StudyEnergy + 5);

                    //Lưu trạng thái đã đúng để khóa lại sau này
                    correctlyAnsweredQuestions.Add(currentQuestionIndex);
                }
                else
                {
                    clicked.Background = Brushes.IndianRed;
                    ResultText.Foreground = Brushes.Red;
                    ResultText.Text = $"Sai rồi! Đáp án đúng là: {(char)(65 + correctIndex)}";
                    gameState.StudyEnergy = Math.Min(GameState.MaxEnergy, gameState.StudyEnergy + 2);

                    // Tô màu đáp án đúng cho người học
                    if (AnswerPanel.Children[correctIndex] is TextBlock correctOption)
                    {
                        correctOption.Background = Brushes.LightGreen;
                    }
                }

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

        //Âm thanh góc trên bên phải
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

        // Mở PlayWindow khi nhấn vào hình ảnh MenuImage
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

        private void SettingControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
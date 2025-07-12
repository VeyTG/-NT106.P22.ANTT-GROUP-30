using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ThuAo.Models;
using THUAO.Properties;

namespace ThuAo
{
    public partial class Classroom : Window
    {
        private List<Question> questions;
        private int currentQuestionIndex = 0;
        private readonly GameState gameState;
        private DispatcherTimer energyTimer;
        private bool isSoundOn = true;

        public Classroom(GameState gameState)
        {
            this.gameState = gameState;
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

            InitializeSettingOverlay();
        }

        private void LoadQuestions()
        {
            questions = new List<Question>
    {
            new Question { Id = 1, Content = "Trái Đất quay quanh Mặt Trời theo quỹ đạo gì?", Options = new List<string> { "Hình vuông", "Hình tròn", "Hình elip", "Hình tam giác" }, CorrectIndex = 2 },
            new Question { Id = 2, Content = "Ai là cha đẻ của máy tính?", Options = new List<string> { "Bill Gates", "Alan Turing", "Steve Jobs", "Charles Babbage" }, CorrectIndex = 3 },
            new Question { Id = 3, Content = "Ngôn ngữ lập trình nào được dùng để xây dựng giao diện Web?", Options = new List<string> { "HTML", "Python", "C++", "Java" }, CorrectIndex = 0 },
            new Question { Id = 4, Content = "Ai là người sáng lập Microsoft?", Options = new List<string> { "Steve Jobs", "Mark Zuckerberg", "Elon Musk", "Bill Gates" }, CorrectIndex = 3 },
            new Question { Id = 5, Content = "Trong Vật lý, định luật bảo toàn năng lượng phát biểu điều gì?", Options = new List<string> { "Năng lượng có thể được tạo ra và mất đi", "Năng lượng chỉ có thể tăng lên", "Năng lượng không thể tạo ra hoặc mất đi, chỉ chuyển hóa", "Năng lượng có thể bị phá hủy trong điều kiện đặc biệt" }, CorrectIndex = 2 },
            new Question { Id = 6, Content = "Tác giả của thuyết tương đối nổi tiếng là ai?", Options = new List<string> { "Isaac Newton", "Albert Einstein", "Stephen Hawking", "Galileo Galilei" }, CorrectIndex = 1 },
            new Question { Id = 7, Content = "Tổng góc trong của một đa giác 10 cạnh là bao nhiêu độ?", Options = new List<string> { "1440", "1620", "1800", "1980" }, CorrectIndex = 2 },
            new Question { Id = 8, Content = "Dãy số Fibonacci bắt đầu với 0, 1, tiếp theo là gì?", Options = new List<string> { "2", "3", "1", "4" }, CorrectIndex = 2 },
            new Question { Id = 9, Content = "Ngôn ngữ lập trình nào thường được dùng để xây dựng hệ điều hành?", Options = new List<string> { "Python", "C", "Java", "Ruby" }, CorrectIndex = 1 },
            new Question { Id = 10, Content = "Trong Hóa học, ký hiệu hóa học của Natri là gì?", Options = new List<string> { "Na", "N", "Ni", "Ns" }, CorrectIndex = 0 },
            new Question { Id = 11, Content = "Câu lệnh SQL nào được dùng để xóa bảng khỏi cơ sở dữ liệu?", Options = new List<string> { "DROP TABLE", "DELETE TABLE", "REMOVE TABLE", "CLEAR TABLE" }, CorrectIndex = 0 },
            new Question { Id = 12, Content = "Đơn vị đo cường độ dòng điện là gì?", Options = new List<string> { "Watt", "Volt", "Ampere", "Ohm" }, CorrectIndex = 2 },
            new Question { Id = 13, Content = "Sự kiện nào đánh dấu sự bắt đầu của Thế chiến thứ hai?", Options = new List<string> { "Nhật tấn công Trân Châu Cảng", "Đức tấn công Ba Lan", "Hiệp định Versailles", "Mỹ tham chiến" }, CorrectIndex = 1 },
            new Question { Id = 14, Content = "Kết quả của tích phân ∫₀¹ x² dx là bao nhiêu?", Options = new List<string> { "1", "1/3", "1/2", "2/3" }, CorrectIndex = 1 },
            new Question { Id = 15, Content = "Số nguyên tố lớn nhất nhỏ hơn 100 là bao nhiêu?", Options = new List<string> { "89", "97", "93", "91" }, CorrectIndex = 1 },
            new Question { Id = 16, Content = "Trong cấu trúc dữ liệu, thuật toán tìm kiếm nhị phân áp dụng tốt nhất khi nào?", Options = new List<string> { "Dữ liệu không được sắp xếp", "Dữ liệu có số lượng phần tử nhỏ", "Dữ liệu đã được sắp xếp", "Dữ liệu là dạng xâu ký tự" }, CorrectIndex = 2 },
            new Question { Id = 17, Content = "Từ 'algorithm' bắt nguồn từ tên của nhà toán học nào?", Options = new List<string> { "Al-Khwarizmi", "Euclid", "Archimedes", "Pythagoras" }, CorrectIndex = 0 },
            new Question { Id = 18, Content = "Trong hệ điều hành, deadlock xảy ra khi nào?", Options = new List<string> { "Các tiến trình chờ tài nguyên không được cấp phát", "Một tiến trình bị lỗi logic", "Tất cả tiến trình bị treo do tranh chấp tài nguyên", "CPU quá tải không xử lý kịp" }, CorrectIndex = 2 },
            new Question { Id = 19, Content = "Giá trị của biểu thức logic: (true && false) || (!false && true) là gì?", Options = new List<string> { "true", "false", "null", "undefined" }, CorrectIndex = 0 },
            new Question { Id = 20, Content = "Độ phức tạp thời gian trung bình của thuật toán Quick Sort là gì?", Options = new List<string> { "O(n)", "O(n log n)", "O(n^2)", "O(log n)" }, CorrectIndex = 1 },
            new Question { Id = 21, Content = "Trong mạng máy tính, giao thức nào đảm bảo độ tin cậy của truyền dữ liệu?", Options = new List<string> { "UDP", "IP", "TCP", "HTTP" }, CorrectIndex = 2 },
            new Question { Id = 22, Content = "Trong C#, từ khóa nào được dùng để khai báo interface?", Options = new List<string> { "abstract", "interface", "class", "struct" }, CorrectIndex = 1 },
            new Question { Id = 23, Content = "Thuật toán Dijkstra được dùng để làm gì?", Options = new List<string> { "Tìm cây khung nhỏ nhất", "Tìm đường đi ngắn nhất", "Sắp xếp mảng", "Tìm đỉnh mạnh liên thông" }, CorrectIndex = 1 },
            new Question { Id = 24, Content = "Câu lệnh nào trong SQL dùng để cấp quyền cho người dùng?", Options = new List<string> { "GRANT", "GIVE", "ALLOW", "PERMIT" }, CorrectIndex = 0 },
            new Question { Id = 25, Content = "Trong hệ điều hành, 'thrashing' là gì?", Options = new List<string> { "Khi CPU bị lỗi", "Khi hệ thống chạy quá nhanh", "Khi hệ thống bị mất dữ liệu", "Khi quá nhiều chuyển đổi trang làm chậm hệ thống" }, CorrectIndex = 3 },
            new Question { Id = 26, Content = "Trong lập trình, 'recursion' là gì?", Options = new List<string> { "Hàm gọi lại chính nó", "Hàm không có tham số", "Hàm có nhiều vòng lặp", "Hàm không trả về giá trị" }, CorrectIndex = 0 },
            new Question { Id = 27, Content = "Cấu trúc dữ liệu nào thường được dùng để triển khai hàng đợi ưu tiên?", Options = new List<string> { "Stack", "Queue", "Heap", "Array" }, CorrectIndex = 2 },
            new Question { Id = 28, Content = "Hàm hash tốt nên có đặc điểm gì?", Options = new List<string> { "Tính ngẫu nhiên thấp", "Dễ đoán đầu ra", "Không sinh ra va chạm", "Phân phối đồng đều kết quả" }, CorrectIndex = 3 },
            new Question { Id = 29, Content = "Kỹ thuật nào được dùng để ngăn chặn tấn công SQL Injection?", Options = new List<string> { "Escaping input", "Use of stored procedures", "Parameterized queries", "Tất cả các phương án trên" }, CorrectIndex = 3 }
        };
            questions = questions.OrderBy(q => Guid.NewGuid()).ToList();
        }

        private void CheckIfAllAnswered()
        {
            if (questions.All(q => gameState.AnsweredQuestionIds.Contains(q.Id)))
            {
                MessageBox.Show("Bạn đã học xong ngày hôm nay rồi!\nHãy quay lại vào ngày mai nhé.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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

            if (gameState.AnsweredQuestionIds.Contains(q.Id))
            {
                if (AnswerPanel.Children[q.CorrectIndex] is TextBlock tb)
                    tb.Background = Brushes.LightGreen;

                ResultText.Text = "Bạn đã trả lời câu này!";
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
            var q = questions[currentQuestionIndex];

            if (gameState.AnsweredQuestionIds.Contains(q.Id))
            {
                ResultText.Foreground = Brushes.Gray;
                ResultText.Text = "Bạn đã trả lời đúng câu này rồi!";
                return;
            }

            if (gameState.CoinBalance < 5)
            {
                ResultText.Foreground = Brushes.Red;
                ResultText.Text = "Không đủ xu!";
                return;
            }

            gameState.CoinBalance -= 5;
            UpdateCoinDisplay();

            if (sender is TextBlock clicked)
            {
                int selectedIndex = AnswerPanel.Children.IndexOf(clicked);
                int correctIndex = q.CorrectIndex;

                ResetOptionColors();

                if (selectedIndex == correctIndex)
                {
                    clicked.Background = Brushes.LightGreen;
                    ResultText.Foreground = Brushes.Green;
                    ResultText.Text = "Chính xác!";
                    gameState.CoinBalance += 30;
                    gameState.StudyEnergy = Math.Min(GameState.MaxEnergy, gameState.StudyEnergy + 5);

                    gameState.AnsweredQuestionIds.Add(q.Id);
                    CheckIfAllAnswered();
                }
                else
                {
                    clicked.Background = Brushes.IndianRed;
                    ResultText.Foreground = Brushes.Red;
                    ResultText.Text = $"Sai rồi! Đáp án đúng là: {(char)(65 + correctIndex)}";
                    gameState.StudyEnergy = Math.Min(GameState.MaxEnergy, gameState.StudyEnergy + 2);

                    if (AnswerPanel.Children[correctIndex] is TextBlock correctOption)
                        correctOption.Background = Brushes.LightGreen;
                }

                UpdateStudyEnergyVisual();
                UpdateCoinDisplay();
            }
        }

        private void ArrowImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            var img = sender as Image;
            if (img?.Source.ToString().Contains("left") == true)
            {
                if (currentQuestionIndex > 0)
                {
                    currentQuestionIndex--;
                    DisplayQuestion(currentQuestionIndex);
                }
                else
                {
                    MessageBox.Show("Không còn câu hỏi phía trước.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (img?.Source.ToString().Contains("right") == true)
            {
                if (currentQuestionIndex < questions.Count - 1)
                {
                    currentQuestionIndex++;
                    DisplayQuestion(currentQuestionIndex);
                }
                else
                {
                    MessageBox.Show("Không còn câu hỏi phía sau.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }


            DisplayQuestion(currentQuestionIndex);
        }

        private void UpdateFoodEnergyVisual() => SetEnergyBar(HealthFill1, gameState.FoodEnergy);
        private void UpdateSleepEnergyVisual() => SetEnergyBar(HealthFill2, gameState.SleepEnergy);
        private void UpdateStudyEnergyVisual() => SetEnergyBar(HealthFill3, gameState.StudyEnergy);

        private void SetEnergyBar(FrameworkElement bar, double energy)
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

        private void EnergyTimer_Tick(object sender, EventArgs e)
        {
            gameState.FoodEnergy = Math.Max(0, gameState.FoodEnergy - 1);
            gameState.SleepEnergy = Math.Max(0, gameState.SleepEnergy - 0.5);
            gameState.StudyEnergy = Math.Max(0, gameState.StudyEnergy - 0.3);
            UpdateFoodEnergyVisual();
            UpdateSleepEnergyVisual();
            UpdateStudyEnergyVisual();
        }

        private void FeedImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            new FeedWindow(gameState).Show();
            this.Close();
        }

        private void SleepImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            new Bedroom(gameState).Show();
            this.Close();
        }

        private void StudyImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
        }

        private void MenuImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            new PlayWindow(gameState).Show();
            this.Close();
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
                SoundImage.Source = new BitmapImage(new Uri(isOn ? "Assets/Button/setting/am_thanh.png" : "Assets/Button/setting/tat_am.png", UriKind.Relative));
                SoundImage.ToolTip = isOn ? "Tắt âm thanh" : "Bật âm thanh";
            }
        }

        private void SettingsImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            var grid = SettingControl.FindName("SettingsGrid") as Grid;
            if (grid != null)
                grid.Visibility = grid.Visibility == Visibility.Collapsed ? Visibility.Visible : grid.Visibility;
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

        private void SettingControl_Loaded(object sender, RoutedEventArgs e) { }
    }
}

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ThuAo
{
    public partial class Setting : Window
    {
        private bool isMusicOn = true;
        private bool isSoundOn = true;

        public Setting()
        {
            InitializeComponent();
        }

        private void FeedImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Tùy vào nút nào đang gọi, bạn có thể kiểm tra qua tên ảnh nếu cần
            // Ở đây giả sử là nhạc và âm thanh

            if (sender is System.Windows.Controls.Image img)
            {
                string imageSource = img.Source.ToString();

                if (imageSource.Contains("music"))
                {
                    // Xử lý bật/tắt nhạc
                    isMusicOn = !isMusicOn;
                    MessageBox.Show(isMusicOn ? "Âm nhạc đã bật" : "Âm nhạc đã tắt");
                    // TODO: thêm xử lý âm nhạc thật
                }
                else if (imageSource.Contains("sound"))
                {
                    // Xử lý bật/tắt âm thanh
                    isSoundOn = !isSoundOn;
                    MessageBox.Show(isSoundOn ? "Âm thanh đã bật" : "Âm thanh đã tắt");
                    // TODO: thêm xử lý âm thanh thật
                }

                // Gọi hiệu ứng click
                AnimateClick(img);
            }
        }

        private void SleepImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.Image img)
            {
                string imageSource = img.Source.ToString();

                if (imageSource.Contains("inbox"))
                {
                    MessageBox.Show("Mở hộp thư đến");
                    // TODO: mở form inbox hoặc điều hướng đến giao diện tương ứng
                }
                else if (imageSource.Contains("help"))
                {
                    MessageBox.Show("Trợ giúp: ...");
                    // TODO: mở cửa sổ hướng dẫn, hoặc file PDF, hoặc overlay
                }

                AnimateClick(img);
            }
        }

        private void MenuImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            // Ví dụ quay lại PlayWindow
            PlayWindow playWindow = new PlayWindow();
            playWindow.Show();
            this.Close();
        }

        private void AnimateClick(System.Windows.Controls.Image image)
        {
            var sb = this.FindResource("ImageClickEffect") as Storyboard;
            if (sb != null)
            {
                image.RenderTransform = new ScaleTransform(1.0, 1.0);
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                sb.Begin(image);
            }
        }
    }
}

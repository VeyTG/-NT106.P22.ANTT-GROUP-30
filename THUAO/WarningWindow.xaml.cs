using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace THUAO
{
    /// <summary>
    /// Interaction logic for WarningWindow.xaml
    /// </summary>
    public partial class WarningWindow : Window
    {
        public WarningWindow()
        {
            InitializeComponent();
        }

        // Giải thích: Constructor nhận message để hiển thị text cảnh báo.
        public WarningWindow(string message)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;

            // Áp dụng hiệu ứng fade-in khi mở cửa sổ.
            var fadeIn = (Storyboard)FindResource("FadeIn");
            fadeIn.Begin(this);
        }

        // Đóng cửa sổ với hiệu ứng fade-out.
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = (Storyboard)FindResource("FadeOut");
            fadeOut.Completed += (s, ev) => this.Close();
            fadeOut.Begin(this);
        }
    }
}

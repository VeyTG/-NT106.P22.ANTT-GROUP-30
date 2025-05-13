using System.Windows;
using System.Windows.Controls;

namespace ThuAo
{
    public partial class Message_Box : Window
    {
        public Message_Box(string message, string title = "Message", MessageType type = MessageType.Information)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
            this.Title = title;

            // Adjust button style based on message type
            SetMessageBoxStyle(type);
        }

        private void SetMessageBoxStyle(MessageType type)
        {
            switch (type)
            {
                case MessageType.Information:
                    OkButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                    break;
                case MessageType.Warning:
                    OkButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
                    break;
                case MessageType.Error:
                    OkButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                    break;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public enum MessageType
    {
        Information,
        Warning,
        Error
    }
}

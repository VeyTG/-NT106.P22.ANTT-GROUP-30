using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using THUAO.Properties;

namespace ThuAo
{
    public partial class StudyHub : Window
    {
        private GameState gameState;

        public StudyHub(GameState state)
        {
            InitializeComponent();
            gameState = state; //Gán giá trị truyền vào
        }

        private void ClassroomButton_Click(object sender, RoutedEventArgs e)
        {
            new Classroom(gameState).Show();
            this.Close();
        }

        private void ShoppingButton_Click(object sender, RoutedEventArgs e)
        {
            new Shopping(gameState).Show();
            this.Close();
        }

        private void ArrowImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new PlayWindow(gameState).Show();
            this.Close();
        }
    }
}

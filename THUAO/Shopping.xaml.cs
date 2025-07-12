using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ThuAo.Models;
using THUAO.Properties;

namespace ThuAo
{
    /// <summary>
    /// Interaction logic for Shopping.xaml
    /// </summary>
    public partial class Shopping : Window
    {
        private GameState gameState;

        public Shopping(GameState state)
        {
            InitializeComponent();
            gameState = state;
            UpdateCoinDisplay();
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


        private void BuyFoodImage_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img && img.Tag != null)
            {
                string[] parts = img.Tag.ToString().Split('|');
                if (parts.Length == 2)
                {
                    string fileName = parts[0];
                    if (int.TryParse(parts[1], out int cost))
                    {
                        if (gameState.CoinBalance >= cost)
                        {
                            gameState.CoinBalance -= cost;
                            gameState.OwnedFoods.Add(fileName);
                            UpdateCoinDisplay();

                            MessageBox.Show($"Bạn đã mua thành công!");
                        }
                        else
                        {
                            MessageBox.Show("Bạn không đủ xu để mua món này!", "Cảnh báo");
                        }
                    }
                }
            }
        }



        private void ArrowImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Quay về màn hình StudyHub và truyền lại gameState
            StudyHub hub = new StudyHub(gameState);
            hub.Show();
            this.Close();
        }

        //Âm thanh nút góc trên bên phải
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

        //Mở PlayWindow
        private void MenuImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AnimateClickEffect(sender as UIElement);
            PlayWindow playWindow = new PlayWindow(gameState);
            playWindow.Show();
            this.Close();
        }

        private void SettingsImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SettingControl.Visibility = Visibility.Visible;
        }

    }
}

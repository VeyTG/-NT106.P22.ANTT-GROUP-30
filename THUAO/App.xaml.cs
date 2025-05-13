using System;
using System.Windows;
using System.Windows.Media;

namespace ThuAo
{
    public partial class App : Application
    {
        private static MediaPlayer _backgroundMusic;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _backgroundMusic = new MediaPlayer();
            _backgroundMusic.Open(new Uri("Assets/backgroundmusic.mp3", UriKind.Relative)); // đường dẫn tương đối
            _backgroundMusic.Volume = 1.0;
            _backgroundMusic.MediaEnded += (s, args) =>
            {
                _backgroundMusic.Position = TimeSpan.Zero;
                _backgroundMusic.Play(); // Lặp lại
            };
            _backgroundMusic.Play(); // Bắt đầu phát nhạc
        }

        public static void SetMusicVolume(double volume)
        {
            if (_backgroundMusic != null)
            {
                _backgroundMusic.Volume = volume;
            }
        }
    }
}

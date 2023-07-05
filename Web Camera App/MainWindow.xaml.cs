using System;
using System.IO;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Web_Camera_App
{
    public partial class MainWindow : Window
    {
        private FilterInfoCollection _videoDevices;
        private VideoCaptureDevice _capture;
        private Bitmap _currentFrame;

        public MainWindow()
        {
            InitializeComponent();
            InitializeVideoCapture();
        }

        private void InitializeVideoCapture()
        {
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (_videoDevices.Count == 0)
            {
                MessageBox.Show("No video devices found.");
                return;
            }

            _capture = new VideoCaptureDevice(_videoDevices[0].MonikerString);
            _capture.NewFrame += Capture_NewFrame;
            _capture.Start();
        }

        private void Capture_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            _currentFrame = (Bitmap)eventArgs.Frame.Clone();
            var bitmapImage = Convert(_currentFrame);

            CameraImage.Dispatcher.Invoke(new Action(() => { CameraImage.Source = bitmapImage; }));
        }

        private void TakePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentFrame != null)
            {
                SavePhotoButton.IsEnabled = true;
            }
        }

        private void SavePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentFrame != null)
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    _currentFrame.Save(saveFileDialog.FileName
                        , ImageFormat.Jpeg);
                    MessageBox.Show("Photo saved successfully!", "Success", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    SavePhotoButton.IsEnabled = false;
                }
            }
        }

        private BitmapImage Convert(System.Drawing.Bitmap src)
        {
            using (var ms = new MemoryStream())
            {
                src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // Allows the bitmap to be used across threads
                return bitmapImage;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_capture != null)
            {
                if (_capture.IsRunning)
                {
                    _capture.Stop();
                }

                _capture.NewFrame -= Capture_NewFrame;
                _capture = null;
            }
        }
    }
}
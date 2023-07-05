using System.Windows;
using AForge.Video.DirectShow;

namespace Web_Camera_App
{
    public partial class CameraSelectionWindow : Window
    {
        public FilterInfo SelectedCamera { get; private set; }

        public CameraSelectionWindow(FilterInfoCollection cameras)
        {
            InitializeComponent();
            cameraListBox.ItemsSource = cameras;
            cameraListBox.SelectedIndex = 0;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedCamera = cameraListBox.SelectedItem as FilterInfo;
            DialogResult = true;
            Close();
        }
    }
}
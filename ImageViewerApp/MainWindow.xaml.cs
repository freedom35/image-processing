using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageViewerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_OpenImage_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select an image to open",
                FileName = "",
                DefaultExt = ".bmp",
                Filter = "Image files (*.bmp/jpg/png/tif)|*.bmp;*.jpg;*.png;*.tif",
                Multiselect = false,
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                tbImageName.Text = openFileDialog.FileName;

                UpdateImage(openFileDialog.FileName);
            }
        }

        private void UpdateImage(string filename)
        {
            UpdateImage(Image.FromFile(filename));
        }

        private void UpdateImage(Image image)
        {
            if (pbImage.CheckAccess())
            {
                // Convert to WPF image
                ImageSource imageSource = ImageConverter.ConvertImageToBitmap(image);

                // Update GUI with image
                pbImage.Source = imageSource;
            }
            else
            {
                pbImage.Dispatcher.BeginInvoke((Action)delegate () { UpdateImage(image); } );
            }
        }
    }
}

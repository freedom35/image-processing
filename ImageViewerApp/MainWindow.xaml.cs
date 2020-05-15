using Freedom35.ImageProcessing;
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

        #region Members

        private Image originalImage = null;
        private Image currentImage = null;
        private Image previousImage= null;

        #endregion

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
                OpenImage(openFileDialog.FileName);
            }
        }

        private void OpenImage(string filename)
        {
            // Dispose of previously loaded image
            originalImage?.Dispose();

            try
            {
                // Keep reference to original image for restore
                originalImage = Image.FromFile(filename);

                // Clone as do not want original modified
                DisplayImage((Image)originalImage.Clone());

                // Update filename
                tbImageName.Text = filename;

                // Update thumbnail
                Image thumbnailImage = ImageThumbnail.CreateWithSameAspect(originalImage, (int)pbThumbnail.ActualWidth, (int)pbThumbnail.ActualHeight);
                pbThumbnail.Source = ImageConverter.ConvertImageToBitmapSource(thumbnailImage);
            }
            catch (Exception ex)
            {
                tbImageName.Text = $"{filename}\n{ex.Message}";
            }

            // Update buttons
            btRestoreImage.IsEnabled = originalImage != null;
        }

        private void DisplayImage(Image image)
        {
            if (pbImage.CheckAccess())
            {
                // Check if displaying new image
                if (image != previousImage)
                {
                    // No longer needed
                    previousImage?.Dispose();

                    // Keep reference for undo
                    previousImage = currentImage;
                }
                else
                {
                    // Undo, no longer need current image
                    currentImage?.Dispose();

                    // Previous no longer available
                    previousImage = null;
                }

                // Keep reference for image processing
                currentImage = image;

                // Convert to WPF image for GUI
                pbImage.Source = ImageConverter.ConvertImageToBitmapSource(image);

                // Enable if previous available
                btUndoImageChange.IsEnabled = previousImage != null;
            }
            else
            {
                pbImage.Dispatcher.BeginInvoke((Action)delegate () { DisplayImage(image); } );
            }
        }

        private void Button_RestoreImage_Click(object sender, RoutedEventArgs e)
        {
            // Dispose of existing images
            previousImage?.Dispose();
            previousImage = null;

            currentImage?.Dispose();
            currentImage = null;

            // Restore original
            if (originalImage != null)
            {
                DisplayImage((Image)originalImage.Clone());
            }
        }

        private void Button_UndoImageChange_Click(object sender, RoutedEventArgs e)
        {
            if (previousImage != null)
            {
                DisplayImage(previousImage);   
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage != null)
            {
                DisplayImage(ImageConvolution.ApplyKernel(currentImage, ConvolutionType.Edge));
                //DisplayImage(ImageResize.ResizeAsNew((Bitmap)currentImage, 0.8));
            }
        }
    }
}

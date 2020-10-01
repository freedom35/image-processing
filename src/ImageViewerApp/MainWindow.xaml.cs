using Freedom35.ImageProcessing;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Linq;

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

        private const string SaveFileFilter = "Bitmap|*.bmp|JPEG|*.jpg|PNG|*.png|TIFF|*.tif";
        
        #endregion

        private void DisposeOfImages()
        {
            originalImage?.Dispose();
            originalImage = null;

            currentImage?.Dispose();
            currentImage = null;

            previousImage?.Dispose();
            previousImage = null;
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            // Check command line args for any 'Open With...' images.
            // (First arg will be app path)
            string imageName = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();

            string[] SupportedImageTypes = { "bmp", "jpg", "png", "tif" };

            // Open image if valid file type
            if (!string.IsNullOrEmpty(imageName) && SupportedImageTypes.Any(t => imageName.EndsWith(t, StringComparison.OrdinalIgnoreCase)))
            {
                OpenImage(imageName);
            }
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
                OpenImage(openFileDialog.FileName);
            }
        }

        private void Button_SaveImage_Click(object sender, RoutedEventArgs e)
        {
            string fileName = tbImageName.Text;
            string ext = System.IO.Path.GetExtension(fileName);

            // Configure file dialog box
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save image as...",
                FileName = fileName,
                DefaultExt = ext,
                Filter = SaveFileFilter,
                FilterIndex = GetFilterIndex(SaveFileFilter, ext),
                AddExtension = true,
                CheckFileExists = false
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // Update filename
                tbImageName.Text = saveFileDialog.FileName;

                SaveCurrentImage(saveFileDialog.FileName);
            }
        }

        private int GetFilterIndex(string filter, string searchExt)
        {
            // Extract ext types
            char[] types = filter.Split('|').Where(t => t.StartsWith('*')).Select(t => t.TrimStart('*', '.').First()).ToArray();

            // Get first char of ext
            char search = searchExt.ToLower().TrimStart('.').FirstOrDefault();

            // Default to zero if not found in array
            // +1 as index for filter is 1-based
            return Math.Max(0, Array.IndexOf(types, search)) + 1;
        }

        private void SaveCurrentImage(string fileName)
        {
            if (currentImage != null)
            {
                try
                {
                    string ext = System.IO.Path.GetExtension(fileName).Trim('.').ToLower();
                    ImageFormat saveFormat = ext switch
                    {
                        "jpg" => ImageFormat.Jpeg,
                        "png" => ImageFormat.Png,
                        "tif" => ImageFormat.Tiff,
                        _ => ImageFormat.Bmp,
                    };

                    // Save in target image format
                    currentImage.Save(fileName, saveFormat);
                }
                catch (Exception ex)
                {
                    ReportException(ex);
                }
            }
        }

        private void OpenImage(string filename)
        {
            // Dispose of previously loaded images
            DisposeOfImages();

            try
            {
                // Keep reference to original image for restore
                originalImage = Image.FromFile(filename);

                // Clone as do not want original modified
                Image clone = (Image)originalImage.Clone();

                DisplayImage(clone);

                // Update filename
                tbImageName.Text = filename;

                // Update thumbnail
                Image thumbnailImage = ImageThumbnail.CreateWithSameAspect(clone, (int)pbThumbnail.DesiredSize.Width, (int)pbThumbnail.DesiredSize.Height);
                pbThumbnail.Source = ImageConverter.ConvertImageToBitmapSource(thumbnailImage);

                // Create histogram of original image
                DisplayHistogram(clone, pbHistogramOrig);
            }
            catch (Exception ex)
            {
                tbImageName.Text = $"{filename}\n{ex.Message}";
            }

            // Check loaded
            bool enable = originalImage != null;
            
            // Update buttons
            btSaveImage.IsEnabled = enable;
            btRestoreImage.IsEnabled = enable;
        }

        private void DisplayHistogram(Image sourceImage, System.Windows.Controls.Image targetPictureBox)
        {
            // Create histogram of image
            Image histogram = ImageHistogram.Create(sourceImage,
                new System.Drawing.Size((int)targetPictureBox.DesiredSize.Width - 20, (int)targetPictureBox.DesiredSize.Height),
                System.Drawing.Color.DodgerBlue,
                System.Drawing.Color.White);

            // Display histogram
            targetPictureBox.Source = ImageConverter.ConvertImageToBitmapSource(histogram);
        }

        private void DisplayImage(Image image)
        {
            if (pbImage.CheckAccess())
            {
                // Check if displaying previous image (undo)
                if (image == previousImage)
                {
                    // No longer need current image
                    currentImage?.Dispose();

                    // Previous no longer available
                    previousImage = null;
                }
                else
                {
                    // No longer needed
                    previousImage?.Dispose();

                    // Keep reference for undo
                    previousImage = currentImage;
                }

                // Keep reference for image processing
                currentImage = image;

                // Convert to WPF image for GUI
                pbImage.Source = ImageConverter.ConvertImageToBitmapSource(image);

                // Display latest histogram
                DisplayHistogram(image, pbHistogramCurrent);

                // Enable if previous available
                btUndoImageChange.IsEnabled = previousImage != null;

                // Clear any previous error
                ClearError();
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

        private void Button_ApplyConvolution_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage != null && cmbConvolution.SelectedIndex > -1)
            {
                string strValue = cmbConvolution.SelectedItem as string;

                ConvolutionType type = EnumConverter.GetValueFromDescription<ConvolutionType>(strValue);

                try
                {
                    DisplayImage(ImageConvolution.ApplyKernel(currentImage, type));
                }
                catch (Exception ex)
                {
                    ReportException(ex);
                }
            }
        }

        private void Button_ApplyThreshold_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage != null)
            {
                try
                {
                    DisplayImage(ImageThreshold.Apply(currentImage));
                }
                catch (Exception ex)
                {
                    ReportException(ex);
                }
            }
        }

        private void Button_EnhanceContrast_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage != null)
            {
                try
                {
                    DisplayImage(ImageContrast.Enhance(currentImage));
                }
                catch (Exception ex)
                {
                    ReportException(ex);
                }
            }
        }

        private void Button_ApplyRedFilter_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage != null)
            {
                try
                {
                    DisplayImage(ImageColor.ToRed(currentImage));
                }
                catch (Exception ex)
                {
                    ReportException(ex);
                }
            }
        }

        private void Button_ToNegative_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage != null)
            {
                try
                {
                    DisplayImage(ImageColor.ToNegative(currentImage));
                }
                catch (Exception ex)
                {
                    ReportException(ex);
                }
            }
        }

        private void ReportException(Exception ex)
        {
            DisplayError(ex.Message);
        }

        private void ClearError()
        {
            DisplayError("");
        }

        private void DisplayError(string errorText)
        {
            if (tbErrorInfo.CheckAccess())
            {
                tbErrorInfo.Text = errorText;
            }
            else
            {
                tbErrorInfo.Dispatcher.BeginInvoke((Action)delegate () { DisplayError(errorText); });
            }
        }
    }
}

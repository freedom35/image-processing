using Freedom35.ImageProcessing;
using System;
using System.Drawing;
using System.Windows;
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

        private const string SaveFileFilter = "Bitmap|*.bmp|JPEG|*.jpg|PNG|*.png|TIFF|*.tif";

        private Image? originalImage = null;
        private Image? currentImage = null;
        private Image? previousImage= null;

        private System.Windows.Point zoomStartPoint = new(-1, -1);
        
        private System.Windows.Input.Cursor? defaultCursor = null;

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
            string? imageName = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();

            if (!string.IsNullOrEmpty(imageName))
            {
                string[] SupportedImageTypes = ["bmp", "jpg", "png", "tif"];

                // Open image if valid file type
                if (SupportedImageTypes.Any(t => imageName.EndsWith(t, StringComparison.OrdinalIgnoreCase)))
                {
                    OpenImage(imageName);
                }
            }
        }

        private void Button_OpenImage_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
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
            Microsoft.Win32.SaveFileDialog saveFileDialog = new()
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

        private static int GetFilterIndex(string filter, string searchExt)
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
                // Load image into memory
                Image imageFromFile = Image.FromFile(filename);

                // Adjust image orientation based on EXIF (if present)
                Image orientatedImage = ImageEXIF.ApplyOrientation(imageFromFile);

                // Keep reference to original image for restore
                originalImage = orientatedImage;

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

        private static void DisplayHistogram(Image sourceImage, System.Windows.Controls.Image targetPictureBox)
        {
            // Create histogram of image
            Image histogram = ImageHistogram.Create(sourceImage,
                new System.Drawing.Size((int)targetPictureBox.DesiredSize.Width - 20, (int)targetPictureBox.DesiredSize.Height),
                Color.DodgerBlue,
                Color.White);

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

        private void Button_UndoImageChange_Click(object? sender, RoutedEventArgs? e)
        {
            if (previousImage != null)
            {
                DisplayImage(previousImage);   
            }
        }

        private void Button_ApplyConvolution_Click(object sender, RoutedEventArgs e)
        {
            // Check an image is loaded
            if (currentImage == null)
            {
                return;
            }

            // Check a valid filter is selected
            if (cmbConvolution.SelectedIndex > -1 && cmbConvolution.SelectedItem is string strValue)
            {
                if (EnumConverter.TryGetValueFromDescription(strValue, out ConvolutionType type))
                {
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

        private void Button_ToGrayscale_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage != null)
            {
                try
                {
                    DisplayImage(ImageColor.ToGrayscale(currentImage));
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

        private void Button_ToSepia_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage != null)
            {
                try
                {
                    DisplayImage(ImageColor.ToSepia(currentImage));
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

        private void Image_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Shortcut to undo - useful when zooming
            Button_UndoImageChange_Click(null, null);
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (currentImage != null && sender is IInputElement element)
            {
                zoomStartPoint = e.GetPosition(element);

                // Change cursor whilst selecting area
                defaultCursor = Cursor;
                Cursor = System.Windows.Input.Cursors.Cross;

                // Show rectangle over zoom area
                // (Get position relative to control area so as to account for any border offset)
                System.Windows.Point zoomCurrentPoint = e.GetPosition(borderImage);
                
                rectZoomArea.Visibility = Visibility.Visible;
                rectZoomArea.Margin = new(zoomCurrentPoint.X, zoomCurrentPoint.Y, 0, 0);
                rectZoomArea.Width = 0;
                rectZoomArea.Height = 0;
                rectZoomArea.Tag = zoomCurrentPoint;
            }
        }

        private void Image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (zoomStartPoint.X > -1 && rectZoomArea.Tag is System.Windows.Point startPoint)
            {
                System.Windows.Point zoomCurrentPoint = e.GetPosition(borderImage);

                double newWidth = zoomCurrentPoint.X - startPoint.X;
                double newHeight = zoomCurrentPoint.Y - startPoint.Y;

                // Shift margin if mouse moved in negative direction
                if (newWidth < 0 || newHeight < 0)
                {
                    double newX = newWidth < 0 ? startPoint.X + newWidth : startPoint.X;
                    double newY = newHeight < 0 ? startPoint.Y + newHeight : startPoint.Y;
                    rectZoomArea.Margin = new(newX, newY, 0, 0);
                }

                rectZoomArea.Width = Math.Abs(newWidth);
                rectZoomArea.Height = Math.Abs(newHeight);
            }
        }

        private void Image_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (currentImage != null)
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left && e.LeftButton == System.Windows.Input.MouseButtonState.Released)
                {
                    try
                    {
                        // Check mouse started down within image
                        if (zoomStartPoint.X > -1 && sender is System.Windows.Controls.Image imageControl)
                        {
                            // This will return area used to display image
                            // (Regardless of image orientation and borders within control)
                            System.Windows.Size controlDisplaySize = new(imageControl.ActualWidth, imageControl.ActualHeight);

                            // Check control visible
                            if (controlDisplaySize.Width > 0 && controlDisplaySize.Height > 0)
                            {
                                System.Windows.Point zoomEndPoint = e.GetPosition(imageControl);

                                // Zoom area could be drawn in any direction
                                double x = Math.Min(zoomStartPoint.X, zoomEndPoint.X);
                                double y = Math.Min(zoomStartPoint.Y, zoomEndPoint.Y);
                                double width = Math.Abs(zoomEndPoint.X - zoomStartPoint.X);
                                double height = Math.Abs(zoomEndPoint.Y - zoomStartPoint.Y);

                                // Validate size of zoom area
                                if (width > 0 && height > 0)
                                {
                                    // Start/End points are relative to control size,
                                    // need to convert values relative to image size
                                    double ratioX = currentImage.Width / controlDisplaySize.Width;
                                    double ratioY = currentImage.Height / controlDisplaySize.Height;

                                    Rectangle zoomArea = new()
                                    {
                                        X = (int)Math.Floor(x * ratioX),
                                        Y = (int)Math.Floor(y * ratioY),
                                        Width = (int)Math.Ceiling(width * ratioX),
                                        Height = (int)Math.Ceiling(height * ratioY)
                                    };

                                    DisplayImage(ImageCrop.ByRegion(currentImage, zoomArea));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ReportException(ex);
                    }

                    ResetZoom();
                }
            }
        }

        private void Image_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ResetZoom();
        }

        private void ResetZoom()
        {
            // Invalidate area
            zoomStartPoint = new(-1, -1);

            // Hide rectangle
            rectZoomArea.Visibility = Visibility.Hidden;

            // Restore origninal cursor
            Cursor = defaultCursor ?? System.Windows.Input.Cursors.Arrow;
        }
    }
}

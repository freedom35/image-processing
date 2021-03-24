using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageViewerApp
{
    /// <summary>
    /// Converts System.Drawing.Image to System.Windows.Media.ImageSource for WPF controls.
    /// </summary>
    [ValueConversion(typeof(Image), typeof(ImageSource))]
    internal sealed class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Image img)
            {
                return ConvertImageToBitmapSource(img);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-way converter
            throw new NotSupportedException();
        }

        public static BitmapImage ConvertImageToBitmapSource(Image image)
        {
            // First convert to stream
            using MemoryStream stream = new();
            image.Save(stream, ImageFormat.Bmp);

            return ConvertStreamToBitmapSource(stream);
        }

        public static BitmapImage ConvertStreamToBitmapSource(MemoryStream stream)
        {
            BitmapImage bmp = null;

            if (stream != null && stream.CanSeek && stream.CanRead)
            {
                // Ensure stream reset
                stream.Seek(0, SeekOrigin.Begin);

                bmp = new BitmapImage();

                // Cache on load so image is retained once memory stream is closed
                bmp.BeginInit();
                bmp.StreamSource = stream;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();

                // Close stream before freezing
                stream.Close();

                // Save on resources.
                if (bmp.CanFreeze)
                {
                    bmp.Freeze();
                }
            }

            return bmp;
        }
    }
}

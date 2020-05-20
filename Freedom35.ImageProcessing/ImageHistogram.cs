using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Xml.Schema;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class for handling histogram methods.
    /// </summary>
    public static class ImageHistogram
    {
        /// <summary>
        /// Gets (256) array of histogram values for image.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <returns>256 array of histogram values</returns>
        public static int[] GetHistogramValues(Image image)
        {
            if (image is Bitmap bmp)
            {
                return GetHistogramValues(bmp);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    return GetHistogramValues(bitmap);
                }
            }
        }

        /// <summary>
        /// Gets (256) array of histogram values for image.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <returns>256 array of histogram values</returns>
        public static int[] GetHistogramValues(Bitmap bitmap)
        {
            byte[] imageBytes = ImageBytes.FromBitmap(bitmap, out BitmapData bmpData);

            int pixelDepth = bmpData.GetPixelDepth();

            return GetHistogramValues(imageBytes, pixelDepth);
        }

        /// <summary>
        /// Gets (256) array of histogram values for image.
        /// </summary>
        /// <param name="imageBytes">Image bytes</param>
        /// <param name="pixelDepth">Pixel depth of image</param>
        /// <returns>256 array of histogram values</returns>
        public static int[] GetHistogramValues(byte[] imageBytes, int pixelDepth)
        {
            bool isColor = BitmapDataExt.IsColorPixelDepth(pixelDepth);

            // 0-255
            int[] histogram = new int[byte.MaxValue + 1];

            int limit = (pixelDepth > 1 ? imageBytes.Length - (pixelDepth - 1) : imageBytes.Length);
            byte avg;

            // Find number of pixels at each level.
            for (int i = 0; i < limit; i += pixelDepth)
            {
                if (isColor)
                {
                    avg = (byte)((imageBytes[i] + imageBytes[i + 1] + imageBytes[i + 2]) / 3);
                }
                else
                {
                    avg = imageBytes[i];
                }

                // double-check within range.
                if (avg >= 0 && avg < histogram.Length)
                {
                    histogram[avg]++;
                }
            }

            return histogram;
        }

        /// <summary>
        /// Histogram Equalization will enhance general contrast 
        /// by distributing grey levels wider and more evenly.
        /// </summary>
        /// <param name="image">Image to equalize</param>
        /// <returns>Equalized image</returns>
        public static Image HistogramEqualization(Image image)
        {
            if (image is Bitmap bmp)
            {
                return HistogramEqualization(bmp);
            }
            else
            {
                Bitmap bitmap = ImageFormatting.ToBitmap(image);
                
                HistogramEqualizationDirect(ref bitmap);
                    
                // Restore original image format
                Image equalizedImage = ImageFormatting.ToFormat(bitmap, image.RawFormat);

                // Dispose of temp bitmap
                bitmap.Dispose();

                return equalizedImage;
            }
        }

        /// <summary>
        /// Histogram Equalization will enhance general contrast 
        /// by distributing grey levels wider and more evenly.
        /// </summary>
        /// <param name="bitmap">Image to equalize</param>
        /// <returns>Equalized image</returns>
        public static Bitmap HistogramEqualization(Bitmap bitmap)
        {
            // Return new image
            Bitmap clone = (Bitmap)bitmap.Clone();

            HistogramEqualizationDirect(ref clone);

            return clone;
        }

        /// <summary>
        /// Histogram Equalization will enhance general contrast 
        /// by distributing grey levels wider and more evenly.
        /// </summary>
        /// <param name="bitmap">Image to equalize</param>
        public static void HistogramEqualizationDirect(ref Bitmap bitmap)
        {
            // Get image bytes
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            int pixelDepth = bmpData.GetPixelDepth();

            HistogramEqualizationDirect(imageBytes, pixelDepth, bitmap.Width, bitmap.Height);

            ImageEdit.End(bitmap, bmpData, imageBytes);
        }

        /// <summary>
        /// Histogram Equalization will enhance general contrast 
        /// by distributing grey levels wider and more evenly.
        /// </summary>
        /// <param name="imageBytes">Image bytes</param>
        /// <param name="pixelDepth">Pixel depth of image</param>
        /// <param name="imageWidth">Image width</param>
        /// <param name="imageHeight">Image height</param>
        public static void HistogramEqualizationDirect(byte[] imageBytes, int pixelDepth, int imageWidth, int imageHeight)
        {
            int[] histogram = GetHistogramValues(imageBytes, pixelDepth);

            // HE Frequencies
            int idealFrequency = (imageWidth * imageHeight) / histogram.Length;
            int cumulativeFrequency = 0;
            int equalizedValue;

            // Perform histogram equalization for each level
            for (int i = 0; i < histogram.Length; i++)
            {
                cumulativeFrequency += histogram[i];

                equalizedValue = (int)Math.Round((double)((cumulativeFrequency / idealFrequency) - 1));

                // Ensure +ve
                histogram[i] = Math.Max(equalizedValue, 0);
            }

            bool isColor = BitmapDataExt.IsColorPixelDepth(pixelDepth);
            byte avg;

            int limit = (pixelDepth > 1 ? imageBytes.Length - (pixelDepth - 1) : imageBytes.Length);

            // Equalize image
            for (int i = 0; i < limit; i += pixelDepth)
            {
                if (isColor)
                {
                    avg = (byte)((imageBytes[i] + imageBytes[i + 1] + imageBytes[i + 2]) / 3);

                    imageBytes[i] = (byte)histogram[avg];
                    imageBytes[i + 1] = (byte)histogram[avg];
                    imageBytes[i + 2] = (byte)histogram[avg];
                }
                else
                {
                    imageBytes[i] = (byte)histogram[imageBytes[i]];
                }
            }
        }

        /// <summary>
        /// Creates a black & white histogram.
        /// </summary>
        /// <param name="histogramSource">Image histogram is based on</param>
        /// <param name="histogramSize">Size of histogram to create</param>
        /// <returns>Bitmap containing histogram</returns>
        public static Bitmap Create(Image histogramSource, Size histogramSize)
        {
            if (histogramSource is Bitmap bmp)
            {
                return Create(bmp, histogramSize, Color.Black, Color.White);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(histogramSource))
                {
                    return Create(bitmap, histogramSize, Color.Black, Color.White);
                }
            }
        }

        /// <summary>
        /// Creates a black & white histogram.
        /// </summary>
        /// <param name="histogramSource">Bitmap histogram is based on</param>
        /// <param name="histogramSize">Size of histogram to create</param>
        /// <returns>Bitmap containing histogram</returns>
        public static Bitmap Create(Bitmap histogramSource, Size histogramSize)
        {
            return Create(histogramSource, histogramSize, Color.Black, Color.White);
        }

        /// <summary>
        /// Creates a histogram image.
        /// </summary>
        /// <param name="histogramSource">Image histogram is based on</param>
        /// <param name="histogramSize">Size of histogram to create</param>
        /// <param name="histogramBackground">Background color of histogram to create</param>
        /// <param name="histogramForeground">Foreground color of histogram to create</param>
        /// <returns>Bitmap containing histogram</returns>
        public static Bitmap Create(Image histogramSource, Size histogramSize, Color histogramBackground, Color histogramForeground)
        {
            if (histogramSource is Bitmap bmp)
            {
                return Create(bmp, histogramSize, histogramBackground, histogramForeground);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(histogramSource))
                {
                    return Create(bitmap, histogramSize, histogramBackground, histogramForeground);
                }
            }
        }

        /// <summary>
        /// Creates a histogram image.
        /// </summary>
        /// <param name="histogramSource">Bitmap histogram is based on</param>
        /// <param name="histogramSize">Size of histogram to create</param>
        /// <param name="histogramBackground">Background color of histogram to create</param>
        /// <param name="histogramForeground">Foreground color of histogram to create</param>
        /// <returns>Bitmap containing histogram</returns>
        public static Bitmap Create(Bitmap histogramSource, Size histogramSize, Color histogramBackground, Color histogramForeground)
        {
            // Get histogram values for source bitmap
            int[] histogramValues = GetHistogramValues(histogramSource);

            int maxValue = histogramValues.Length > 0 ? histogramValues.Max() : 0;

            int histogramWidth = histogramSize.Width;
            int histogramHeight = histogramSize.Height;

            float scaleX = (float)histogramWidth / histogramValues.Length;
            float scaleY = (float)histogramHeight / maxValue;

            // Create new bitmap to contain histogram
            Bitmap bitmapHistogram = new Bitmap(histogramWidth, histogramHeight);

            using (Graphics g = Graphics.FromImage(bitmapHistogram))
            {
                // Initialize background color for bitmap
                g.FillRectangle(new SolidBrush(histogramBackground), 0, 0, histogramWidth, histogramHeight);

                // Create brush for foreground
                SolidBrush histogramBrush = new SolidBrush(histogramForeground);

                // Each value should be same width
                float valueWidth = Math.Max(1.0F, (float)Math.Round(scaleX));

                float x, y, valueHeight;

                // Draw vertical bar for each histogram value
                for (int i = 0; i < histogramValues.Length; i++)
                {
                    // Vertical bar representing number of pixels at value
                    valueHeight = histogramValues[i] * scaleY;

                    if (valueHeight > 0)
                    {
                        // X position of vertical bar
                        x = (int)(i * scaleX);

                        // Draw each bar from bottom-up
                        y = histogramHeight - valueHeight;

                        // Draw vertical bar
                        g.FillRectangle(histogramBrush, x, y, valueWidth, valueHeight);
                    }
                }
            }

            return bitmapHistogram;
        }
    }
}

//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

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
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <returns>256 array of histogram values</returns>
        public static int[] GetHistogramValues<T>(T image) where T : Image
        {
            byte[] imageBytes = ImageBytes.FromImage(image, out BitmapData bmpData);

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
        /// Creates a black & white histogram.
        /// </summary>
        /// <param name="histogramSource">Image histogram is based on</param>
        /// <param name="histogramSize">Size of histogram to create</param>
        /// <returns>Bitmap containing histogram</returns>
        public static Bitmap Create<T>(T histogramSource, Size histogramSize) where T : Image
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
        public static Bitmap Create<T>(T histogramSource, Size histogramSize, Color histogramBackground, Color histogramForeground) where T : Image
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
                    // (Round up to ensure at least 1 pixel represented)
                    valueHeight = (float)Math.Ceiling(histogramValues[i] * scaleY);

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

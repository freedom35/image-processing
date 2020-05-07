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
        /// <param name="image">Image to process</param>
        /// <returns>256 array of histogram values</returns>
        public static int[] GetHistogramValues(Image image)
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            return GetHistogramValues(bitmap);
        }

        /// <summary>
        /// Gets (256) array of histogram values for image.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <returns>256 array of histogram values</returns>
        public static int[] GetHistogramValues(Bitmap bitmap)
        {
            byte[] rgbValues = ImageBytes.FromBitmap(bitmap, out BitmapData bmpData);

            int pixelDepth = bmpData.GetPixelDepth();
            bool isColor = bmpData.IsColor();

            // 0-255
            int[] histogram = new int[byte.MaxValue + 1];

            int limit = (pixelDepth > 1 ? rgbValues.Length - (pixelDepth - 1) : rgbValues.Length);
            byte avg;

            // Find number of pixels at each level.
            for (int i = 0; i < limit; i += pixelDepth)
            {
                if (isColor)
                {
                    avg = (byte)((rgbValues[i] + rgbValues[i + 1] + rgbValues[i + 2]) / 3);
                }
                else
                {
                    avg = rgbValues[i];
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
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            Bitmap equalizedBitmap = HistogramEqualization(bitmap);

            // Restore original image format
            return ImageFormatting.ToFormat(equalizedBitmap, image.RawFormat);
        }

        /// <summary>
        /// Histogram Equalization will enhance general contrast 
        /// by distributing grey levels wider and more evenly.
        /// </summary>
        /// <param name="bitmap">Image to equalize</param>
        /// <returns>Equalized image</returns>
        public static Bitmap HistogramEqualization(Bitmap bitmap)
        {
            int[] histogram = GetHistogramValues(bitmap);

            // HE Frequencies
            int idealFrequency = (bitmap.Width * bitmap.Height) / histogram.Length;
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

            // Return new image
            Bitmap clone = (Bitmap)bitmap.Clone();

            byte[] rgbValues = ImageEdit.Begin(clone, out BitmapData bmpData);

            int pixelDepth = bmpData.GetPixelDepth();
            bool isColor = bmpData.IsColor();
            byte avg;

            int limit = (pixelDepth > 1 ? rgbValues.Length - (pixelDepth - 1) : rgbValues.Length);

            // Apply to image
            for (int i = 0; i < limit; i += pixelDepth)
            {
                if (isColor)
                {
                    avg = (byte)((rgbValues[i] + rgbValues[i + 1] + rgbValues[i + 2]) / 3);

                    rgbValues[i] = (byte)histogram[avg];
                    rgbValues[i + 1] = (byte)histogram[avg];
                    rgbValues[i + 2] = (byte)histogram[avg];
                }
                else
                {
                    rgbValues[i] = (byte)histogram[rgbValues[i]];
                }
            }

            ImageEdit.End(clone, bmpData, rgbValues);

            return clone;
        }

        /// <summary>
        /// Creates a black & white histogram.
        /// </summary>
        /// <param name="histogramSource">Image histogram is based on</param>
        /// <param name="histogramSize">Size of histogram to create</param>
        /// <returns>Bitmap containing histogram</returns>
        public static Bitmap Create(Image histogramSource, Size histogramSize)
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(histogramSource);

            return Create(bitmap, histogramSize, Color.Black, Color.White);
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
            Bitmap bitmap = ImageFormatting.ToBitmap(histogramSource);

            return Create(bitmap, histogramSize, histogramBackground, histogramForeground);
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
                float valueWidth = scaleX;

                // Draw each bar from bottom-up
                float y = histogramHeight;

                float x, valueHeight;

                // Draw vertical bar for each histgram value
                for (int i = 0; i < histogramValues.Length; i++)
                {
                    // X position of vertical bar
                    x = i * scaleX;

                    // Vertical bar representing number of pixels at value
                    valueHeight = histogramValues[i] * scaleY;

                    // Draw vertical bar
                    g.FillRectangle(histogramBrush, x, y, valueWidth, valueHeight);
                }
            }

            return bitmapHistogram;
        }
    }
}

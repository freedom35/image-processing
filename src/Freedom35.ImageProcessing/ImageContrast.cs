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
    /// Methods relating to image contrast.
    /// </summary>
    public static class ImageContrast
    {
        /// <summary>
        /// Stretches image contrast to get maximum contrast.
        /// If image has unused lower/upper range, values can be stretched to use all available range, improving image contrast.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <returns>Contrast-stretched image</returns>
        public static T Stretch<T>(T image) where T: Image
        {
            return Stretch(image, byte.MinValue, byte.MaxValue);
        }

        /// <summary>
        /// Stretches image contrast to specified min/max values.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <param name="min">Minimum contrast value</param>
        /// <param name="max">Maximum contrast value</param>
        /// <returns>Contrast-stretched image</returns>
        public static T Stretch<T>(T image, byte min, byte max) where T : Image
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            StretchDirect(ref bitmap, min, max);

            return (T)ImageFormatting.Convert(bitmap, image.RawFormat);
        }

        /// <summary>
        /// Stretches image contrast to specified min/max values.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="min">Minimum contrast value</param>
        /// <param name="max">Maximum contrast value</param>
        public static void StretchDirect(ref Bitmap bitmap, byte min, byte max)
        {
            // Lock image for processing
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            StretchDirect(imageBytes, bmpData, min, max);

            // Copy modified array back to image, and release lock
            ImageEdit.End(bitmap, bmpData, imageBytes);
        }

        /// <summary>
        /// Stretches image contrast to specified min/max values.
        /// </summary>
        /// <param name="imageBytes">Image bytes (Grayscale/RGB)</param>
        /// <param name="bmpData">Info on image properties</param>
        /// <param name="min">Minimum contrast value</param>
        /// <param name="max">Maximum contrast value</param>
        public static void StretchDirect(byte[] imageBytes, BitmapData bmpData, byte min, byte max)
        {
            int pixelDepth = bmpData.GetPixelDepth();
            bool isColor = bmpData.IsColor();

            // Initialize with opposite values
            byte highest = byte.MinValue;
            byte lowest = byte.MaxValue;
            byte val;

            // Bitmap converted from jpeg can potentially can potentially have array with extra odd byte.
            int limit = (isColor ? imageBytes.Length - (pixelDepth - 1) : imageBytes.Length);

            //////////////////////////////////////
            // First find current contrast range
            //////////////////////////////////////
            for (int i = 0; i < limit; i += pixelDepth)
            {
                if (isColor)
                {
                    val = (byte)((imageBytes[i] + imageBytes[i + 1] + imageBytes[i + 2]) / 3);
                }
                else
                {
                    val = imageBytes[i];
                }

                // Check contrast ranges
                if (val < lowest)
                {
                    lowest = val;
                }
                
                if (val > highest)
                {
                    highest = val;
                }
            }

            // Constant for loop
            double maxMinusMin = max - min;

            //////////////////////////////////////
            // Now contrast stretch image
            //////////////////////////////////////
            for (int i = 0; i < limit; i += pixelDepth)
            {
                for (int j = 0; j < pixelDepth; j++)
                {
                    // Contrast-stretch value
                    val = (byte)(((imageBytes[i + j] - lowest) * (maxMinusMin / (highest - lowest))) + min);

                    // Check limits
                    if (val < lowest)
                    {
                        val = min;
                    }
                    else if (val > highest)
                    {
                        val = max;
                    }

                    imageBytes[i + j] = val;
                }
            }
        }

        /// <summary>
        /// Histogram Equalization will enhance general contrast 
        /// by distributing grey levels wider and more evenly.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to equalize</param>
        /// <returns>Equalized image</returns>
        public static T HistogramEqualization<T>(T image) where T : Image
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            HistogramEqualizationDirect(ref bitmap);

            return (T)ImageFormatting.Convert(bitmap, image.RawFormat);
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
            int[] histogram = ImageHistogram.GetHistogramValues(imageBytes, pixelDepth);

            double numberOfPixels = (imageWidth * imageHeight);
            double numberOfLevels = histogram.Length;
            int cumulativeFrequency = 0;
            int equalizedValue;

            // Calculate cumulative distribution for histogram
            for (int i = 0; i < histogram.Length; i++)
            {
                cumulativeFrequency += histogram[i];

                // Calculate equalized value
                equalizedValue = (int)Math.Round((numberOfLevels * cumulativeFrequency) / numberOfPixels) - 1;

                // Ensure +ve value
                histogram[i] = Math.Max(0, equalizedValue);
            }

            bool isColor = BitmapDataExt.IsColorPixelDepth(pixelDepth);
            int limit = (pixelDepth > 1 ? imageBytes.Length - (pixelDepth - 1) : imageBytes.Length);

            // Apply distribution to image to equalize
            if (isColor)
            {
                for (int i = 0, j; i < limit; i += pixelDepth)
                {
                    for (j = 0; j < pixelDepth; j++)
                    {
                        imageBytes[i + j] = (byte)histogram[imageBytes[i + j]];
                    }
                }
            }
            else
            {
                for (int i = 0; i < limit; i++)
                {
                    imageBytes[i] = (byte)histogram[imageBytes[i]];
                }
            }
        }

        /// <summary>
        /// Improves contrast by stretching or equalization (decision based on current image values).
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to enhance</param>
        /// <returns>Image with enhanced contrast</returns>
        public static T Enhance<T>(T image) where T : Image
        {
            Tuple<byte, byte> minMax = ImageBytes.GetMinMaxValue(image);

            double percentRoomToStretch = (double)(minMax.Item1 + (byte.MaxValue - minMax.Item2)) / byte.MaxValue;

            // If space in light/dark areas, then stretch to fill
            // (Retains contrast levels)
            if (percentRoomToStretch > 0.2F)
            {
                return Stretch(image);
            }
            else
            {
                // Otherwise equalize whole image
                return HistogramEqualization(image);
            }
        }
    }
}

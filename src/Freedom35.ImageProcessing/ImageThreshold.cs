﻿using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class to handle applying thresholds to images.
    /// </summary>
    public static class ImageThreshold
    {
        /// <summary>
        /// Applies thresholding to image using Otsu's Method.
        /// Returned image will consist of black (0) and white (255) values only.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <returns>New image with threshold applied</returns>
        public static Image ApplyOtsuMethod(Image image)
        {
            if (image is Bitmap bmp)
            {
                return ApplyOtsuMethod(bmp);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    using (Bitmap thresholdBitmap = ApplyOtsuMethod(bitmap))
                    {
                        // Restore original image format
                        return ImageFormatting.ToFormat(thresholdBitmap, image.RawFormat);
                    }
                }
            }
        }

        /// <summary>
        /// Applies thresholding to image using Otsu's Method.
        /// Returned image will consist of black (0) and white (255) values only.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <returns>New image with threshold applied</returns>
        public static Bitmap ApplyOtsuMethod(Bitmap bitmap)
        {
            Bitmap clone = (Bitmap)bitmap.Clone();

            ApplyOtsuMethodDirect(ref clone);

            return clone;
        }

        /// <summary>
        /// Applies thresholding directly to image using Otsu's Method.
        /// Returned image will consist of black (0) and white (255) values only.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        public static void ApplyOtsuMethodDirect(ref Bitmap bitmap)
        {
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            // Determine whether color
            int pixelDepth = bmpData.GetPixelDepth();

            // Apply threshold value to image.
            ApplyOtsuMethodDirect(imageBytes, pixelDepth);

            ImageEdit.End(bitmap, bmpData, imageBytes);
        }

        /// <summary>
        /// Any pixel values below threshold will be changed to 0 (black).
        /// Any pixel values above (or equal to) threshold will be changed to 255 (white).
        /// </summary>
        /// <param name="imageBytes">Image bytes</param>
        /// <param name="pixelDepth">Pixel depth</param>
        public static void ApplyOtsuMethodDirect(byte[] imageBytes, int pixelDepth)
        {
            int[] histValues = ImageHistogram.GetHistogramValues(imageBytes, pixelDepth);

            int histLen = histValues.Length;

            int weightBackground = 0;
            int weightForeground;

            float sumBackground = 0.0F;

            float meanBackground;
            float meanForeground;
            float meanDiff;

            float varianceBetween;
            float varianceMax = 0.0F;

            // Otsu's threshold value
            int threshold = 0;

            float sumHistValues = 0.0F;

            // Total variance
            for (int i = 0; i < histLen; i++)
            {
                sumHistValues += i * histValues[i];
            }

            // Image may be color
            int pixelCount = imageBytes.Length / pixelDepth;

            // Calculate 'between class variance' to separate foreground from background.
            // (Same result as intra-class variance, but quicker to calculate)
            // i.e. Find position with largest variance
            for (int i = 0; i < histLen; i++)
            {
                // Keep track of weights up to this point
                weightBackground += histValues[i];

                // No point calculating until reached pixel values in image
                if (weightBackground > 0)
                {
                    weightForeground = pixelCount - weightBackground;

                    if (weightForeground == 0)
                    {
                        // Quit loop if no more pixels
                        break;
                    }

                    // Variance up to this point
                    sumBackground += i * histValues[i];

                    // Calculate mean values
                    meanBackground = sumBackground / weightBackground;
                    meanForeground = (sumHistValues - sumBackground) / weightForeground;
                    meanDiff = meanBackground - meanForeground;

                    // Calculate between class variance
                    varianceBetween = meanDiff * meanDiff * weightBackground * weightForeground;
                    
                    // Check for new max variance
                    if (varianceBetween > varianceMax)
                    {
                        varianceMax = varianceBetween;

                        // Histogram position has highest variance
                        threshold = i;
                    }
                }
            }

            // Apply threshold to image using Otsu's value
            ApplyDirect(imageBytes, pixelDepth, (byte)threshold);
        }

        /// <summary>
        /// Any pixel values below threshold will be changed to 0 (black).
        /// Any pixel values above (or equal to) threshold will be changed to 255 (white).
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <param name="threshold">Threshold value</param>
        /// <returns>New image with threshold applied</returns>
        public static Image Apply(Image image, byte threshold)
        {
            if (image is Bitmap bmp)
            {
                return Apply(bmp, threshold);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    using (Bitmap thresholdBitmap = Apply(bitmap, threshold))
                    {
                        // Restore original image format
                        return ImageFormatting.ToFormat(thresholdBitmap, image.RawFormat);
                    }
                }
            }
        }

        /// <summary>
        /// Any pixel values below threshold will be changed to 0 (black).
        /// Any pixel values above (or equal to) threshold will be changed to 255 (white).
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="threshold">Threshold value</param>
        /// <returns>New image with threshold applied</returns>
        public static Bitmap Apply(Bitmap bitmap, byte threshold)
        {
            Bitmap clone = (Bitmap)bitmap.Clone();

            ApplyDirect(ref clone, threshold);

            return clone;
        }

        /// <summary>
        /// Any pixel values below threshold will be changed to 0 (black).
        /// Any pixel values above (or equal to) threshold will be changed to 255 (white).
        /// </summary>
        /// <param name="imageBytes">Image bytes</param>
        /// <param name="threshold">Threshold value</param>
        public static void ApplyDirect(ref Bitmap bitmap, byte threshold)
        {
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            // Determine whether color
            int pixelDepth = bmpData.GetPixelDepth();

            // Apply threshold value to image.
            ApplyDirect(imageBytes, pixelDepth, threshold);

            ImageEdit.End(bitmap, bmpData, imageBytes);
        }

        /// <summary>
        /// Any pixel values below threshold will be changed to 0 (black).
        /// Any pixel values above (or equal to) threshold will be changed to 255 (white).
        /// </summary>
        /// <param name="imageBytes">Image bytes</param>
        /// <param name="pixelDepth">Pixel depth</param>
        /// <param name="threshold">Threshold value</param>
        public static void ApplyDirect(byte[] imageBytes, int pixelDepth, byte threshold)
        {
            // Check if color
            if (pixelDepth > 1)
            {
                int pixelSum;
                bool belowThreshold;

                // Loop each pixel
                for (int i = 0; i < imageBytes.Length; i += pixelDepth)
                {
                    pixelSum = 0;

                    // Sum each pixel component
                    for (int j = 0; j < pixelDepth && i + j < imageBytes.Length; j++)
                    {
                        pixelSum += imageBytes[i + j];
                    }

                    // Compare average to threshold
                    belowThreshold = (pixelSum / pixelDepth) < threshold;

                    // Apply threshold
                    for (int j = 0; j < pixelDepth && i + j < imageBytes.Length; j++)
                    {
                        imageBytes[i + j] = belowThreshold ? byte.MinValue : byte.MaxValue;
                    }
                }
            }
            else
            {
                // Apply threshold value to image
                for (int i = 0; i < imageBytes.Length; i++)
                {
                    imageBytes[i] = imageBytes[i] < threshold ? byte.MinValue : byte.MaxValue;
                }
            }
        }

        /// <summary>
        /// Any pixel values below threshold will be changed to min value.
        /// (High-pass filter)
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="minValue">Min value to retain</param>
        /// <returns>New image with threshold applied</returns>
        public static Image ApplyMin(Image image, byte minValue)
        {
            if (image is Bitmap bmp)
            {
                return ApplyMin(bmp, minValue);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    using (Bitmap thresholdBitmap = ApplyMin(bitmap, minValue))
                    {
                        // Restore original image format
                        return ImageFormatting.ToFormat(thresholdBitmap, image.RawFormat);
                    }
                }
            }
        }

        /// <summary>
        /// Any pixel values below threshold will be changed to min value.
        /// (High-pass filter)
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="minValue">Min value to retain</param>
        /// <returns>New image with threshold applied</returns>
        public static Bitmap ApplyMin(Bitmap bitmap, byte minValue)
        {
            Bitmap clone = (Bitmap)bitmap.Clone();

            ApplyMinDirect(ref bitmap, minValue);

            return clone;
        }

        /// <summary>
        /// Any pixel values below threshold will be changed to min value.
        /// (High-pass filter)
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="minValue">Min value to retain</param>
        public static void ApplyMinDirect(ref Bitmap bitmap, byte minValue)
        {
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            // Determine whether color
            int pixelDepth = bmpData.GetPixelDepth();

            ApplyMinDirect(imageBytes, pixelDepth, minValue);

            ImageEdit.End(bitmap, bmpData, imageBytes);   
        }

        /// <summary>
        /// Any pixel values below threshold will be changed to min value.
        /// (High-pass filter)
        /// </summary>
        /// <param name="imageBytes">Image bytes (Grayscale/RGB)</param>
        /// <param name="pixelDepth">Pixel depth. i.e. 1 for grayscale, 3 for color</param>
        /// <param name="minValue">Min value to retain</param>
        public static void ApplyMinDirect(byte[] imageBytes, int pixelDepth, byte minValue)
        {
            // Apply threshold value to image.
            for (int i = 0; i < imageBytes.Length; i += pixelDepth)
            {
                if (imageBytes[i] < minValue)
                {
                    imageBytes[i] = minValue;
                }

                if (pixelDepth == 3 && i < imageBytes.Length - 2)
                {
                    if (imageBytes[i + 1] < minValue)
                    {
                        imageBytes[i + 1] = minValue;
                    }

                    if (imageBytes[i + 2] < minValue)
                    {
                        imageBytes[i + 2] = minValue;
                    }
                }
            }
        }

        /// <summary>
        /// Any pixel values above threshold will be changed to max value.
        /// (Low-pass filter)
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="maxValue">Max value to retain</param>
        /// <returns>New image with threshold applied</returns>
        public static Image ApplyMax(Image image, byte maxValue)
        {
            if (image is Bitmap bmp)
            {
                return ApplyMax(bmp, maxValue);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    using (Bitmap thresholdBitmap = ApplyMax(bitmap, maxValue))
                    {
                        // Restore original image format
                        return ImageFormatting.ToFormat(thresholdBitmap, image.RawFormat);
                    }
                }
            }
        }

        /// <summary>
        /// Any pixel values above threshold will be changed to max value.
        /// (Low-pass filter)
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="maxValue">Max value to retain</param>
        /// <returns>New image with threshold applied</returns>
        public static Bitmap ApplyMax(Bitmap bitmap, byte maxValue)
        {
            Bitmap clone = (Bitmap)bitmap.Clone();

            ApplyMaxDirect(ref clone, maxValue);

            return clone;
        }

        /// <summary>
        /// Any pixel values above threshold will be changed to max value.
        /// (Low-pass filter)
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="maxValue">Max value to retain</param>
        public static void ApplyMaxDirect(ref Bitmap bitmap, byte maxValue)
        {
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            // Determine whether color
            int pixelDepth = bmpData.GetPixelDepth();

            ApplyMaxDirect(imageBytes, pixelDepth, maxValue);

            ImageEdit.End(bitmap, bmpData, imageBytes);
        }

        /// <summary>
        /// Any pixel values above threshold will be changed to max value.
        /// (Low-pass filter)
        /// </summary>
        /// <param name="imageBytes">Image bytes (Grayscale/RGB)</param>
        /// <param name="pixelDepth">Pixel depth. i.e. 1 for grayscale, 3 for color</param>
        /// <param name="maxValue">Max value to retain</param>
        public static void ApplyMaxDirect(byte[] imageBytes, int pixelDepth, byte maxValue)
        {
            // Apply threshold value to image.
            for (int i = 0; i < imageBytes.Length; i += pixelDepth)
            {
                if (imageBytes[i] > maxValue)
                {
                    imageBytes[i] = maxValue;
                }

                if (pixelDepth == 3 && i < imageBytes.Length - 2)
                {
                    if (imageBytes[i + 1] > maxValue)
                    {
                        imageBytes[i + 1] = maxValue;
                    }

                    if (imageBytes[i + 1] > maxValue)
                    {
                        imageBytes[i + 2] = maxValue;
                    }
                }
            }
        }

        /// <summary>
        /// Any pixel values outside the threshold will be changed to min/max.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <param name="minValue">Minimum threshold value</param>
        /// <param name="maxValue">Maximum threshold value</param>
        /// <returns>New image with threshold applied</returns>
        public static Image ApplyMinMax(Image image, byte minValue, byte maxValue)
        {
            if (image is Bitmap bmp)
            {
                return ApplyMinMax(bmp, minValue, maxValue);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    using (Bitmap thresholdBitmap = ApplyMinMax(bitmap, minValue, maxValue))
                    {
                        // Restore original image format
                        return ImageFormatting.ToFormat(thresholdBitmap, image.RawFormat);
                    }
                }
            }
        }

        /// <summary>
        /// Any pixel values outside the threshold will be changed to min/max.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="minValue">Minimum threshold value</param>
        /// <param name="maxValue">Maximum threshold value</param>
        /// <returns>New image with threshold applied</returns>
        public static Bitmap ApplyMinMax(Bitmap bitmap, byte minValue, byte maxValue)
        {
            // Return new image
            Bitmap clone = (Bitmap)bitmap.Clone();

            ApplyMinMaxDirect(ref bitmap, minValue, maxValue);

            return clone;
        }

        /// <summary>
        /// Any pixel values outside the threshold will be changed to min/max.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="minValue">Minimum threshold value</param>
        /// <param name="maxValue">Maximum threshold value</param>
        public static void ApplyMinMaxDirect(ref Bitmap bitmap, byte minValue, byte maxValue)
        {
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            ApplyMinMaxDirect(imageBytes, bmpData, minValue, maxValue);

            ImageEdit.End(bitmap, bmpData, imageBytes);
        }

        /// <summary>
        /// Any pixel values outside the threshold will be changed to min/max.
        /// </summary>
        /// <param name="imageBytes">Image bytes (Grayscale/RGB)</param>
        /// <param name="bmpData">Info on image properties</param>
        /// <param name="minValue">Minimum threshold value</param>
        /// <param name="maxValue">Maximum threshold value</param>
        public static void ApplyMinMaxDirect(byte[] imageBytes, BitmapData bmpData, byte minValue, byte maxValue)
        {
            int pixelDepth = bmpData.GetPixelDepth();

            // Adjust image to within min/max.
            for (int i = 0; i < imageBytes.Length; i += pixelDepth)
            {
                // Change values outside threshold to extremes
                if (imageBytes[i] < minValue)
                {
                    imageBytes[i] = minValue;
                }
                else if (imageBytes[i] > maxValue)
                {
                    imageBytes[i] = maxValue;
                }

                // Extra bytes for color images (RGB)
                if (pixelDepth == 3 && i < imageBytes.Length - 2)
                {
                    // G
                    if (imageBytes[i + 1] < minValue)
                    {
                        imageBytes[i + 1] = minValue;
                    }
                    else if (imageBytes[i + 1] > maxValue)
                    {
                        imageBytes[i + 1] = maxValue;
                    }

                    // B
                    if (imageBytes[i + 2] < minValue)
                    {
                        imageBytes[i + 2] = minValue;
                    }
                    else if (imageBytes[i + 2] > maxValue)
                    {
                        imageBytes[i + 2] = maxValue;
                    }
                }
            }
        }
    }
}
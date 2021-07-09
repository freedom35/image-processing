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
    /// Class to handle applying thresholds to images.
    /// </summary>
    public static class ImageThreshold
    {
        /// <summary>
        /// Applies thresholding to image using the default method.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <returns>New image with threshold applied</returns>
        public static T Apply<T>(T image) where T : Image
        {
            // Otsu: optimum balance between quality and performance
            return ApplyOtsuMethod(image);
        }

        /// <summary>
        /// Applies thresholding to image using Otsu's Method.
        /// Returned image will consist of black (0) and white (255) values only.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <returns>New image with threshold applied</returns>
        public static T ApplyOtsuMethod<T>(T image) where T : Image
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            ApplyOtsuMethodDirect(ref bitmap);

            return (T)ImageFormatting.Convert(bitmap, image.RawFormat);
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
        /// Applies thresholding directly to image using Otsu's Method.
        /// Returned image will consist of black (0) and white (255) values only.
        /// </summary>
        /// <param name="imageBytes">Image bytes</param>
        /// <param name="pixelDepth">Pixel depth</param>
        public static void ApplyOtsuMethodDirect(byte[] imageBytes, int pixelDepth)
        {
            byte threshold = GetByOtsuMethod(imageBytes, pixelDepth);

            // Apply threshold to image using Otsu's value
            ApplyDirect(imageBytes, pixelDepth, threshold);
        }

        /// <summary>
        /// Obtains threshold value for image using Otsu's Method.
        /// </summary>
        /// <param name="imageBytes">Image bytes</param>
        /// <param name="pixelDepth">Pixel depth</param>
        /// <returns>Threshold value</returns>
        public static byte GetByOtsuMethod(byte[] imageBytes, int pixelDepth)
        {
            int[] histValues = ImageHistogram.GetHistogramValues(imageBytes, pixelDepth);

            return GetByOtsuMethod(histValues);
        }

        /// <summary>
        /// Obtains threshold value for histogram using Otsu's Method.
        /// </summary>
        /// <param name="histogramValues">Histogram of image</param>
        /// <returns>Threshold value</returns>
        public static byte GetByOtsuMethod(int[] histogramValues)
        {
            int histLen = histogramValues.Length;

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
                sumHistValues += i * histogramValues[i];
            }

            // Get total number of pixels
            int pixelCount = histogramValues.Sum();

            // Calculate 'between class variance' to separate foreground from background.
            // (Same result as intra-class variance, but quicker to calculate)
            // i.e. Find position with largest variance
            for (int i = 0; i < histLen; i++)
            {
                weightForeground = pixelCount - weightBackground;

                if (weightForeground == 0)
                {
                    // Quit loop if no more pixels
                    break;
                }

                // Variance up to this point
                sumBackground += i * histogramValues[i];

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

                // Keep sum of weights up to this point
                weightBackground += histogramValues[i];
            }

            return (byte)threshold;
        }

        /// <summary>
        /// Any pixel values below threshold will be changed to 0 (black).
        /// Any pixel values above (or equal to) threshold will be changed to 255 (white).
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <param name="threshold">Threshold value</param>
        /// <returns>New image with threshold applied</returns>
        public static T Apply<T>(T image, byte threshold) where T : Image
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            ApplyDirect(ref bitmap, threshold);

            return (T)ImageFormatting.Convert(bitmap, image.RawFormat);
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
                // Exclude alpha/transparency byte when averaging
                int thresholdByteLength = Math.Min(pixelDepth, Constants.PixelDepthRGB);

                int pixelSum;
                bool belowThreshold;

                // Loop each pixel (include transparency byte)
                for (int i = 0; i < imageBytes.Length; i += pixelDepth)
                {
                    pixelSum = 0;

                    // Sum each pixel component
                    for (int j = 0; j < thresholdByteLength && i + j < imageBytes.Length; j++)
                    {
                        pixelSum += imageBytes[i + j];
                    }

                    // Compare average to threshold
                    belowThreshold = (pixelSum / thresholdByteLength) < threshold;

                    // Apply threshold
                    for (int j = 0; j < thresholdByteLength && i + j < imageBytes.Length; j++)
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
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <param name="minValue">Min value to retain</param>
        /// <returns>New image with threshold applied</returns>
        public static T ApplyMin<T>(T image, byte minValue) where T : Image
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            ApplyMinDirect(ref bitmap, minValue);

            return (T)ImageFormatting.Convert(bitmap, image.RawFormat);
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
            bool isColor = BitmapDataExt.IsColorPixelDepth(pixelDepth);

            // Apply threshold value to image.
            for (int i = 0; i < imageBytes.Length; i += pixelDepth)
            {
                if (imageBytes[i] < minValue)
                {
                    imageBytes[i] = minValue;
                }

                if (isColor && i < imageBytes.Length - 2)
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
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <param name="maxValue">Max value to retain</param>
        /// <returns>New image with threshold applied</returns>
        public static T ApplyMax<T>(T image, byte maxValue) where T : Image
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            ApplyMaxDirect(ref bitmap, maxValue);

            return (T)ImageFormatting.Convert(bitmap, image.RawFormat);
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
            bool isColor = BitmapDataExt.IsColorPixelDepth(pixelDepth);

            // Apply threshold value to image.
            for (int i = 0; i < imageBytes.Length; i += pixelDepth)
            {
                if (imageBytes[i] > maxValue)
                {
                    imageBytes[i] = maxValue;
                }

                if (isColor && i < imageBytes.Length - 2)
                {
                    // G
                    if (imageBytes[i + 1] > maxValue)
                    {
                        imageBytes[i + 1] = maxValue;
                    }

                    // R
                    if (imageBytes[i + 2] > maxValue)
                    {
                        imageBytes[i + 2] = maxValue;
                    }
                }
            }
        }

        /// <summary>
        /// Any pixel values outside the threshold will be changed to min/max.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <param name="minValue">Minimum threshold value</param>
        /// <param name="maxValue">Maximum threshold value</param>
        /// <returns>New image with threshold applied</returns>
        public static T ApplyMinMax<T>(T image, byte minValue, byte maxValue) where T : Image
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            ApplyMinMaxDirect(ref bitmap, minValue, maxValue);

            return (T)ImageFormatting.Convert(bitmap, image.RawFormat);
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

            ApplyMinMaxDirect(imageBytes, bmpData.GetPixelDepth(), minValue, maxValue);

            ImageEdit.End(bitmap, bmpData, imageBytes);
        }

        /// <summary>
        /// Any pixel values outside the threshold will be changed to min/max.
        /// </summary>
        /// <param name="imageBytes">Image bytes (Grayscale/RGB)</param>
        /// <param name="pixelDepth">Depth of pixels (typically 1 for grayscale, 3 for color)</param>
        /// <param name="minValue">Minimum threshold value</param>
        /// <param name="maxValue">Maximum threshold value</param>
        public static void ApplyMinMaxDirect(byte[] imageBytes, int pixelDepth, byte minValue, byte maxValue)
        {
            bool isColor = BitmapDataExt.IsColorPixelDepth(pixelDepth);

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
                if (isColor && i < imageBytes.Length - 2)
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

                    // R
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

        /// <summary>
        /// Applies localized/adaptive region thresholding to image using Chow & Kaneko method.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <returns>New image with localized threshold applied</returns>
        public static T ApplyChowKanekoMethod<T>(T image) where T : Image
        {
            return ApplyChowKanekoMethod(image, 3, 3);
        }

        /// <summary>
        /// Applies localized/adaptive region thresholding to image using Chow & Kaneko method.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <param name="horizontalRegions">Number of horizonal regions to apply</param>
        /// <param name="verticalRegions">Number of veritical regions to apply</param>
        /// <returns>New image with localized threshold applied</returns>
        public static T ApplyChowKanekoMethod<T>(T image, int horizontalRegions, int verticalRegions) where T : Image
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            ApplyChowKanekoMethodDirect(ref bitmap, horizontalRegions, verticalRegions);

            return (T)ImageFormatting.Convert(bitmap, image.RawFormat);
        }

        /// <summary>
        /// Applies localized/adaptive region thresholding to image using Chow & Kaneko method.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="horizontalRegions">Number of horizonal regions to apply</param>
        /// <param name="verticalRegions">Number of vertical regions to apply</param>
        public static void ApplyChowKanekoMethodDirect(ref Bitmap bitmap, int horizontalRegions, int verticalRegions)
        {
            // Ensure at least 1 region
            if (horizontalRegions < 1 || verticalRegions < 1)
            {
                throw new ArgumentOutOfRangeException("Chow & Kaneko requires at least one region.");
            }

            // We will use Otsu's method to determine threshold for each region
            ThresholdRegionData[] regionThresholds = new ThresholdRegionData[horizontalRegions * verticalRegions];

            // Get image bytes and info
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            // Use stride to ensure correct row length
            int stride = Math.Abs(bmpData.Stride);
            int imageHeight = bmpData.Height;
            int imageWidth = bmpData.Width;

            // Get pixels per region
            // (Round up to ensure thresholding applied to every pixel)
            int horizontalPPZ = (int)Math.Ceiling((double)imageWidth / horizontalRegions);
            int verticalPPZ = (int)Math.Ceiling((double)imageHeight / verticalRegions);

            // Determine whether color
            int pixelDepth = bmpData.GetPixelDepth();

            // Variables for region loop
            byte[] regionBytes;
            int regionY1, regionY2, regionHeight;
            int regionX1, regionX2, regionWidth;
            int yOffset;
            int imageOffset, regionOffset;
            int y, x, i;
            ThresholdRegionData rd;

            // Apply Otsu to obtain localized threshold for each region
            for (y = 0; y < verticalRegions; y++)
            {
                // Get y positions
                regionY1 = y * verticalPPZ;
                regionY2 = Math.Min(imageHeight, regionY1 + verticalPPZ);
                regionHeight = regionY2 - regionY1;

                yOffset = stride * regionY1;

                for (x = 0; x < horizontalRegions; x++)
                {
                    // Get current region position, limit to edge of image
                    regionX1 = x * horizontalPPZ * pixelDepth;
                    regionX2 = Math.Min(imageWidth, regionX1 + horizontalPPZ) * pixelDepth;

                    regionWidth = regionX2 - regionX1;

                    // Allocate bytes
                    regionBytes = new byte[regionWidth * regionHeight];

                    // Copy bytes from image to region array
                    // Copy row by row (region bytes not consecutive)
                    for (i = 0; i < regionHeight; i++)
                    {
                        imageOffset = yOffset + (stride * i) + regionX1;
                        regionOffset = regionWidth * i;
                        
                        Buffer.BlockCopy(imageBytes, imageOffset, regionBytes, regionOffset, regionWidth);
                    }

                    // Create struct with threshold data
                    rd = new ThresholdRegionData()
                    {
                        X = x,
                        Y = y,
                        CenterX = regionX1 + (regionWidth / 2),
                        CenterY = (y * regionHeight) + (regionHeight / 2)
                    };
                                        
                    // Apply Otsu to localized region
                    rd.Threshold = GetByOtsuMethod(regionBytes, pixelDepth);

                    // Assign threshold data to region
                    regionThresholds[(y * horizontalRegions) + x] = rd;
                }
            }

            // Variables for pixel loop
            int pixelSum;
            bool belowThreshold;
            byte threshold;
            double totalDistance;
            ThresholdRegionData[] nearestRegions;
            int py, px, n, j;

            const int NumberOfRegions = 4;

            // Go through each pixel calculating threshold
            for (py = 0; py < imageHeight; py++)
            {
                for (px = 0; px < imageWidth; px++)
                {
                    // Calculate distances from pixel to each region
                    for (n = 0; n < regionThresholds.Length; n++)
                    {
                        regionThresholds[n].CalculateDistance(px, py);
                    }

                    // Find the nearest 4 regions to pixel, then get their thresholds
                    nearestRegions = regionThresholds.OrderBy(r => r.Distance).Take(NumberOfRegions).ToArray();
                    
                    // Sum all distances so we have reference for each relative distance
                    totalDistance = nearestRegions.Sum(r => r.Distance);

                    // Weight bias the threshold of each nearest region based on proximity to pixel
                    // (Shorter distance has higher weight bias)
                    // Sum of each weight bias should add up to 1.
                    if (nearestRegions.Length > 1)
                    {
                        threshold = (byte)nearestRegions.Sum(r => ((1 - (r.Distance / totalDistance)) / (nearestRegions.Length - 1)) * r.Threshold);
                    }
                    else
                    {
                        threshold = nearestRegions.First().Threshold;
                    }

                    // Get pixel index within image bytes
                    i = (px * pixelDepth) + (py * stride);
                    pixelSum = 0;

                    // Sum each pixel component for color images
                    for (j = 0; j < pixelDepth && i + j < imageBytes.Length; j++)
                    {
                        pixelSum += imageBytes[i + j];
                    }

                    // Compare average to threshold
                    belowThreshold = (pixelSum / pixelDepth) < threshold;

                    // Apply threshold
                    for (j = 0; j < pixelDepth && i + j < imageBytes.Length; j++)
                    {
                        imageBytes[i + j] = belowThreshold ? byte.MinValue : byte.MaxValue;
                    }
                }
            }

            // End edit, write bytes back to image
            ImageEdit.End(bitmap, bmpData, imageBytes);
        }
    }
}

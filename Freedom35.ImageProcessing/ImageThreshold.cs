using System;
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
        /// Any pixel values below (or equal to) threshold will be changed to 0 (black).
        /// Any pixel values above threshold will be changed to 255 (white).
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="threshold">Threshold value</param>
        /// <returns>New image with threshold applied</returns>
        public static Image Apply(Image image, byte threshold)
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            Bitmap thresholdBitmap = Apply(bitmap, threshold);

            // Restore original image format
            return ImageFormatting.ToFormat(thresholdBitmap, image.RawFormat);
        }

        /// <summary>
        /// Any pixel values below (or equal to) threshold will be changed to 0 (black).
        /// Any pixel values above threshold will be changed to 255 (white).
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="threshold">Threshold value</param>
        /// <returns>New image with threshold applied</returns>
        public static Bitmap Apply(Bitmap bitmap, byte threshold)
        {
            Bitmap clone = (Bitmap)bitmap.Clone();

            byte[] rgbValues = ImageEdit.BeginWrite(clone, out BitmapData bmpData);

            // Determine whether color
            int pixelDepth = (bmpData.Stride / bmpData.Width);
            
            // Apply threshold value to image.
            for (int i = 0; i < rgbValues.Length; i += pixelDepth)
            {
                rgbValues[i] = rgbValues[i] > threshold ? byte.MaxValue : byte.MinValue;

                if (pixelDepth == 3 && i < rgbValues.Length - 2)
                {
                    rgbValues[i + 1] = rgbValues[i + 1] > threshold ? byte.MaxValue : byte.MinValue;
                    rgbValues[i + 2] = rgbValues[i + 2] > threshold ? byte.MaxValue : byte.MinValue;
                }
            }

            ImageEdit.EndWrite(clone, bmpData, rgbValues);

            return clone;
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
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            Bitmap thresholdBitmap = ApplyMin(bitmap, minValue);

            // Restore original image format
            return ImageFormatting.ToFormat(thresholdBitmap, image.RawFormat);
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

            byte[] rgbValues = ImageEdit.BeginWrite(clone, out BitmapData bmpData);

            // Determine whether color
            int pixelDepth = (bmpData.Stride / bmpData.Width);

            // Apply threshold value to image.
            for (int i = 0; i < rgbValues.Length; i += pixelDepth)
            {
                if (rgbValues[i] < minValue)
                {
                    rgbValues[i] = minValue;
                }

                if (pixelDepth == 3 && i < rgbValues.Length - 2)
                {
                    if (rgbValues[i + 1] < minValue)
                    {
                        rgbValues[i + 1] = minValue;
                    }

                    if (rgbValues[i + 2] < minValue)
                    {
                        rgbValues[i + 2] = minValue;
                    }
                }
            }

            ImageEdit.EndWrite(clone, bmpData, rgbValues);

            return clone;
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
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            Bitmap thresholdBitmap = ApplyMax(bitmap, maxValue);

            // Restore original image format
            return ImageFormatting.ToFormat(thresholdBitmap, image.RawFormat);
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

            byte[] rgbValues = ImageEdit.BeginWrite(clone, out BitmapData bmpData);

            // Determine whether color
            int pixelDepth = (bmpData.Stride / bmpData.Width);

            // Apply threshold value to image.
            for (int i = 0; i < rgbValues.Length; i += pixelDepth)
            {
                if (rgbValues[i] > maxValue)
                {
                    rgbValues[i] = maxValue;
                }

                if (pixelDepth == 3 && i < rgbValues.Length - 2)
                {
                    if (rgbValues[i+ 1] > maxValue)
                    {
                        rgbValues[i + 1] = maxValue;
                    }

                    if (rgbValues[i + 1] > maxValue)
                    {
                        rgbValues[i + 2] = maxValue;
                    }
                }
            }

            ImageEdit.EndWrite(clone, bmpData, rgbValues);

            return clone;
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
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            Bitmap thresholdBitmap = ApplyMinMax(bitmap, minValue, maxValue);

            // Restore original image format
            return ImageFormatting.ToFormat(thresholdBitmap, image.RawFormat);
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
            Bitmap clone = (Bitmap)bitmap.Clone();

            byte[] rgbValues = ImageEdit.BeginWrite(clone, out BitmapData bmpData);

            int pixelDepth = (bmpData.Stride / bmpData.Width);

            // Adjust image to within min/max.
            for (int i = 0; i < rgbValues.Length; i += pixelDepth)
            {
                // Change values outside threshold to extremes
                if (rgbValues[i] < minValue)
                {
                    rgbValues[i] = minValue;
                }
                else if (rgbValues[i] > maxValue)
                {
                    rgbValues[i] = maxValue;
                }

                // Extra bytes for color images (RGB)
                if (pixelDepth == 3 && i < rgbValues.Length - 2)
                {
                    // G
                    if (rgbValues[i + 1] < minValue)
                    {
                        rgbValues[i + 1] = minValue;
                    }
                    else if (rgbValues[i + 1] > maxValue)
                    {
                        rgbValues[i + 1] = maxValue;
                    }

                    // B
                    if (rgbValues[i + 2] < minValue)
                    {
                        rgbValues[i + 2] = minValue;
                    }
                    else if (rgbValues[i + 2] > maxValue)
                    {
                        rgbValues[i + 2] = maxValue;
                    }
                }
            }

            ImageEdit.EndWrite(clone, bmpData, rgbValues);

            return clone;
        }
    }
}

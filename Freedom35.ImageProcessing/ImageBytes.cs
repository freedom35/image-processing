using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class with methods relating to image bytes.
    /// </summary>
    public static class ImageBytes
    {
        /// <summary>
        /// Converts the image to a bitmap, and gets the bytes.
        /// </summary>
        /// <param name="image">Image to get bytes from</param>
        /// <returns>Image bytes</returns>
        public static byte[] FromImage(Image image)
        {
            return FromBitmap(ImageFormatting.ToBitmap(image));
        }

        /// <summary>
        /// Gets the image bytes from a bitmap image.
        /// </summary>
        /// <param name="bitmap">Bitmap to get bytes from</param>
        /// <returns>Image bytes</returns>
        public static byte[] FromBitmap(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Converts the image to a bitmap, and gets the bytes.
        /// </summary>
        /// <param name="image">Image to get bytes from</param>
        /// <param name="bmpData">Data relating to bitmap</param>
        /// <returns>Image bytes</returns>
        public static byte[] FromImage(Image image, out BitmapData bmpData)
        {
            return FromBitmap(ImageFormatting.ToBitmap(image), out bmpData);
        }

        /// <summary>
        /// Gets the bytes from a bitmap image.
        /// </summary>
        /// <param name="bitmap">Bitmap to get bytes from</param>
        /// <param name="bmpData">Data relating to bitmap</param>
        /// <returns>Image bytes</returns>
        public static byte[] FromBitmap(Bitmap bitmap, out BitmapData bmpData)
        {
            // Lock full image
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Lock the bits while we read them.  
            bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            // Create length with number of bytes in image
            byte[] rgbValues = new byte[bmpData.GetImageLength()];

            // Copy the RGB values into the array.
            Marshal.Copy(bmpData.Scan0, rgbValues, 0, rgbValues.Length);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);

            return rgbValues;
        }

        /// <summary>
        /// Gets the image bytes from an image file.
        /// </summary>
        /// <param name="path">Path to image</param>
        /// <returns>Image bytes</returns>
        public static byte[] FromFile(string path)
        {
            using (Image image = Image.FromFile(path))
            {
                return FromImage(image);
            }
        }

        /// <summary>
        /// Gets the image bytes from an image stream.
        /// </summary>
        /// <param name="stream">Image stream</param>
        /// <returns>Image bytes</returns>
        public static byte[] FromStream(Stream stream)
        {
            using (Image image = Image.FromStream(stream))
            {
                return FromImage(image);
            }
        }

        /// <summary>
        /// Gets the minimum pixel value within an image.
        /// (For color images, min pixel avg returned)
        /// </summary>
        /// <param name="image">Image to search</param>
        /// <returns>Minimum pixel value</returns>
        public static byte GetMinValue(Image image)
        {
            return GetMinValue(ImageFormatting.ToBitmap(image));
        }

        /// <summary>
        /// Gets the minimum pixel value within an image.
        /// (For color images, min pixel avg returned)
        /// </summary>
        /// <param name="bitmap">Image to search</param>
        /// <returns>Minimum pixel value</returns>
        public static byte GetMinValue(Bitmap bitmap)
        {
            // Start with highest value
            byte min = byte.MaxValue;

            byte[] rgbValues = FromBitmap(bitmap, out BitmapData bmpData);

            int pixelDepth = bmpData.GetPixelDepth();
            bool isColor = bmpData.IsColor();
            byte avg;

            // Find minimum value
            // (Zero is lowest possible, so stop looking once found)
            for (int i = 0; i < rgbValues.Length && min > byte.MinValue; i += pixelDepth)
            {
                if (isColor)
                {
                    avg = (byte)((rgbValues[i] + rgbValues[i + 1] + rgbValues[i + 2]) / 3);

                    if (avg < min)
                    {
                        min = avg;
                    }
                }
                else if (rgbValues[i] < min)
                {
                    min = rgbValues[i];
                }
            }

            return min;
        }

        /// <summary>
        /// Gets the maximum pixel value within an image.
        /// (For color images, max pixel avg is returned)
        /// </summary>
        /// <param name="image">Image to search</param>
        /// <returns>Maximum pixel value</returns>
        public static byte GetMaxValue(Image image)
        {
            return GetMaxValue(ImageFormatting.ToBitmap(image));
        }

        /// <summary>
        /// Gets the maximum pixel value within an image.
        /// (For color images, max pixel avg is returned)
        /// </summary>
        /// <param name="bitmap">Image to search</param>
        /// <returns>Maximum pixel value</returns>
        public static byte GetMaxValue(Bitmap bitmap)
        {
            // Start with lowest value
            byte max = byte.MinValue;

            byte[] rgbValues = FromBitmap(bitmap, out BitmapData bmpData);

            int pixelDepth = bmpData.GetPixelDepth();
            bool isColor = bmpData.IsColor();
            byte avg;

            // Find maximum value
            // (255 is highest possible, so stop looking once found)
            for (int i = 0; i < rgbValues.Length && max < byte.MaxValue; i += pixelDepth)
            {
                if (isColor)
                {
                    avg = (byte)((rgbValues[i] + rgbValues[i + 1] + rgbValues[i + 2]) / 3);

                    if (avg > max)
                    {
                        max = avg;
                    }
                }
                else if (rgbValues[i] > max)
                {
                    max = rgbValues[i];
                }
            }

            return max;
        }

        /// <summary>
        /// Get the average pixel value between 0-255.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <returns>Average pixel intensity</returns>
        public static byte GetAverageValue(Image image)
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            return GetAverageValue(bitmap);
        }

        /// <summary>
        /// Get the average pixel value between 0-255.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <returns>Average pixel intensity</returns>
        public static byte GetAverageValue(Bitmap bitmap)
        {
            return GetAverageValue(bitmap, byte.MinValue, byte.MaxValue);
        }

        /// <summary>
        /// Get the average pixel value between min and max.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <param name="min">Minimum byte value</param>
        /// <param name="max">Maximum byte value</param>
        /// <returns>Average pixel intensity</returns>
        public static byte GetAverageValue(Image image, byte min, byte max)
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            return GetAverageValue(bitmap, min, max);
        }

        /// <summary>
        /// Get the average pixel value between min and max level.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="min">Minimum byte value</param>
        /// <param name="max">Maximum byte value</param>
        /// <returns>Average pixel intensity</returns>
        public static byte GetAverageValue(Bitmap bitmap, byte min, byte max)
        {
            // Get range of values
            int valueRange = (max - min) + 1;

            // Check range is valid
            if (valueRange < 1)
            {
                throw new ArgumentException("max cannot be less than min.");
            }

            // Get image bytes
            byte[] rgbValues = FromBitmap(bitmap, out BitmapData bmpData);

            int pixelDepth = bmpData.GetPixelDepth();
            bool isColor = bmpData.IsColor();
            byte avg;

            // Create array for each value in range
            byte[] histogram = new byte[valueRange];

            // Find distribution of pixels at each level.
            for (int i = 0; i < rgbValues.Length; i += pixelDepth)
            {
                if (isColor)
                {
                    // Find average value of RGB
                    avg = (byte)((rgbValues[i] + rgbValues[i + 1] + rgbValues[i + 2]) / 3);
                }
                else
                {
                    avg = rgbValues[i];
                }

                // Check value within range
                if (avg >= min && avg <= max)
                {
                    // Offset index for array
                    histogram[avg - min]++;
                }
            }

            // Return average value within range
            return (byte)histogram.Average(b => b);
        }
    }
}

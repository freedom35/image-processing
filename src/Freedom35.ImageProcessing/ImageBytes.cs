//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
using System.Collections.Generic;
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
        /// Gets the image bytes including header info.
        /// </summary>
        /// <param name="image">Image to get bytes from</param>
        /// <returns>Image bytes (including header info)</returns>
        public static byte[] FromImageWithHeaderInfo(Image image)
        {
            return FromImageWithHeaderInfo(image, image.RawFormat);
        }

        /// <summary>
        /// Gets the image bytes including header info.
        /// </summary>
        /// <param name="image">Image to get bytes from</param>
        /// <param name="format">Format of bytes</param>
        /// <returns>Image bytes (including header info)</returns>
        public static byte[] FromImageWithHeaderInfo(Image image, ImageFormat format)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, format);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Converts the image to a bitmap, and gets the bytes.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to get bytes from</param>
        /// <returns>Image bytes (without header info)</returns>
        public static byte[] FromImage<T>(T image) where T : Image
        {
            return FromImage(image, out BitmapData _);
        }

        /// <summary>
        /// Converts the image to a bitmap, and gets the bytes.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to get bytes from</param>
        /// <param name="bitmapData">Data relating to bitmap</param>
        /// <returns>Image bytes (without header info)</returns>
        public static byte[] FromImage<T>(T image, out BitmapData bitmapData) where T : Image
        {
            if (image is Bitmap bmp)
            {
                return GetBitmapBytes(bmp, out bitmapData);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    return GetBitmapBytes(bitmap, out bitmapData);
                }
            }
        }

        /// <summary>
        /// Gets the bytes from a bitmap image (image portion only).
        /// </summary>
        /// <param name="bitmap">Bitmap to get bytes from</param>
        /// <param name="bmpData">Data relating to bitmap</param>
        /// <returns>Image bytes</returns>
        private static byte[] GetBitmapBytes(Bitmap bitmap, out BitmapData bmpData)
        {
            // Lock full image
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Lock the bits while we read them.  
            bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            // Create length with number of bytes in image
            byte[] rgbValues = new byte[bmpData.GetByteCount()];

            // Copy the RGB values into the array.
            Marshal.Copy(bmpData.Scan0, rgbValues, 0, rgbValues.Length);

            // Check if monochrome
            if (bitmap.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                // Adjust stride
                bmpData.Stride *= Constants.BitsPerByte;

                // Convert each bit to a separate byte
                rgbValues = BytesToBits(rgbValues).Select(b => b > 0 ? byte.MaxValue : byte.MinValue).ToArray();
            }

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
        /// Gets the image bytes from an embedded resource.
        /// </summary>
        /// <param name="resourcePath">Path to resource</param>
        /// <returns>Image bytes</returns>
        public static byte[] FromResource(string resourcePath)
        {
            using (Stream resourceStream = System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream(resourcePath))
            {
                return FromStream(resourceStream);
            }
        }

        /// <summary>
        /// Gets the minimum pixel value within an image.
        /// (For color images, min pixel avg returned)
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to search</param>
        /// <returns>Minimum pixel value</returns>
        public static byte GetMinValue<T>(T image) where T : Image
        {
            if (image is Bitmap bmp)
            {
                return GetMinBitmapValue(bmp);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    return GetMinBitmapValue(bitmap);
                }
            }
        }

        /// <summary>
        /// Gets the minimum pixel value within an image.
        /// (For color images, min pixel avg returned)
        /// </summary>
        /// <param name="bitmap">Image to search</param>
        /// <returns>Minimum pixel value</returns>
        private static byte GetMinBitmapValue(Bitmap bitmap)
        {
            // Start with highest value
            byte min = byte.MaxValue;

            byte[] rgbValues = GetBitmapBytes(bitmap, out BitmapData bmpData);

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
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to search</param>
        /// <returns>Maximum pixel value</returns>
        public static byte GetMaxValue<T>(T image) where T : Image
        {
            if (image is Bitmap bmp)
            {
                return GetMaxBitmapValue(bmp);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    return GetMaxBitmapValue(bitmap);
                }
            }
        }

        /// <summary>
        /// Gets the maximum pixel value within an image.
        /// (For color images, max pixel avg is returned)
        /// </summary>
        /// <param name="bitmap">Image to search</param>
        /// <returns>Maximum pixel value</returns>
        private static byte GetMaxBitmapValue(Bitmap bitmap)
        {
            // Start with lowest value
            byte max = byte.MinValue;

            byte[] rgbValues = GetBitmapBytes(bitmap, out BitmapData bmpData);

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
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <returns>Average pixel intensity</returns>
        public static byte GetAverageValue<T>(T image) where T : Image
        {
            return GetAverageValue(image, byte.MinValue, byte.MaxValue);
        }

        /// <summary>
        /// Get the average pixel value between min and max.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <param name="min">Minimum byte value</param>
        /// <param name="max">Maximum byte value</param>
        /// <returns>Average pixel intensity</returns>
        public static byte GetAverageValue<T>(T image, byte min, byte max) where T : Image
        {
            if (image is Bitmap bmp)
            {
                return GetAverageBitmapValue(bmp, min, max);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    return GetAverageBitmapValue(bitmap, min, max);
                }
            }
        }

        /// <summary>
        /// Get the average pixel value between min and max level.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="min">Minimum byte value</param>
        /// <param name="max">Maximum byte value</param>
        /// <returns>Average pixel intensity</returns>
        private static byte GetAverageBitmapValue(Bitmap bitmap, byte min, byte max)
        {
            // Get range of values
            int valueRange = (max - min) + 1;

            // Check range is valid
            if (valueRange < 1)
            {
                throw new ArgumentException("max cannot be less than min.");
            }

            // Get image bytes
            byte[] rgbValues = GetBitmapBytes(bitmap, out BitmapData bmpData);

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

        /// <summary>
        /// Converts array of bytes to an array of bits.
        /// </summary>
        /// <param name="byteValues">Array of bytes values</param>
        /// <returns>Array of 0/1 bit values</returns>
        public static byte[] BytesToBits(byte[] byteValues)
        {
            byte[] bitValues = new byte[byteValues.Length * Constants.BitsPerByte];

            byte val;
            int bitIndex, j;

            for (int i = 0; i < byteValues.Length; i++)
            {
                val = byteValues[i];

                bitIndex = i * Constants.BitsPerByte;

                for (j = 0; j < Constants.BitsPerByte; j++)
                {
                    // Get most significant bit
                    bitValues[bitIndex + j] = (byte)((val & 0x80) >> 7);

                    // Shift bits up
                    val <<= 1;
                }
            }

            return bitValues;
        }

        /// <summary>
        /// Converts array of bits to an array of bytes.
        /// </summary>
        /// <param name="bitValues">Array of bit values</param>
        /// <returns>Array of bytes</returns>
        public static byte[] BitsToBytes(byte[] bitValues)
        {
            byte[] byteValues = new byte[bitValues.Length / Constants.BitsPerByte];

            byte val;
            int bitIndex, j;

            for (int i = 0; i < byteValues.Length; i++)
            {
                val = 0;

                bitIndex = i * Constants.BitsPerByte;

                for (j = 0; j < Constants.BitsPerByte; j++)
                {
                    // Shift existing bits up
                    val <<= 1;

                    // Add byte value
                    val += bitValues[bitIndex + j] > 0 ? Constants.One : Constants.Zero;
                }

                byteValues[i] = val;
            }

            return byteValues;
        }

        /// <summary>
        /// Determines image type based on header bytes.
        /// </summary>
        /// <param name="imageBytes">Image bytes</param>
        /// <param name="imageType">Type of image determined</param>
        /// <returns>True if image type could be determined</returns>
        public static bool TryGetImageType(byte[] imageBytes, out ImageType imageType)
        {
            imageType = ImageType.Unknown;

            // Get image types to compare
            IEnumerable<ImageType> imageTypes = Enum.GetValues(typeof(ImageType)).Cast<ImageType>().Where(t => t != ImageType.Unknown);

            // Decode image type in buffer
            foreach (ImageType type in imageTypes)
            {
                byte[] encodingBytes = type.GetEncodingBytes();

                if (IsImageType(imageBytes, encodingBytes))
                {
                    imageType = type;
                    break;
                }
            }

            return imageType != ImageType.Unknown;
        }

        /// <summary>
        /// Determines if image matches the encoding type.
        /// </summary>
        /// <param name="imageBytes">Image bytes</param>
        /// <param name="encodingBytes">Encoding bytes for comparison</param>
        /// <returns>True if image header matches encoding bytes</returns>
        public static bool IsImageType(byte[] imageBytes, byte[] encodingBytes)
        {
            int i;

            // Compare image bytes to encoding
            for (i = 0; i < encodingBytes.Length && i < imageBytes.Length; i++)
            {
                if (imageBytes[i] != encodingBytes[i])
                {
                    break;
                }
            }

            // Check if all encoding bytes matched buffer
            return (i == encodingBytes.Length);
        }
    }
}

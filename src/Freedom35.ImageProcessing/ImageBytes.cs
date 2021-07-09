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
        /// <param name="image">Image to get bytes from</param>
        /// <returns>Image bytes (without header info)</returns>
        public static byte[] FromImage(Image image)
        {
            return FromImage(image, out BitmapData _);
        }

        /// <summary>
        /// Converts the image to a bitmap, and gets the bytes.
        /// </summary>
        /// <param name="image">Image to get bytes from</param>
        /// <param name="bitmapData">Data relating to bitmap</param>
        /// <returns>Image bytes (without header info)</returns>
        public static byte[] FromImage(Image image, out BitmapData bitmapData)
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
        /// For color images, min pixel avg returned.
        /// </summary>
        /// <param name="image">Image to search</param>
        /// <returns>Minimum pixel value</returns>
        public static byte GetMinValue(Image image)
        {
            return GetMinMaxValue(image, true, false).Item1;
        }

        /// <summary>
        /// Gets the maximum pixel value within an image.
        /// (For color images, max pixel avg is returned)
        /// </summary>
        /// <param name="image">Image to search</param>
        /// <returns>Maximum pixel value</returns>
        public static byte GetMaxValue(Image image)
        {
            return GetMinMaxValue(image, false, true).Item2;
        }

        /// <summary>
        /// Gets the minimum & maximum pixel value within an image.
        /// For color images, min/max pixel avg is returned.
        /// </summary>
        /// <param name="image">Image to search</param>
        /// <returns>min, max bytes</returns>
        public static Tuple<byte, byte> GetMinMaxValue(Image image)
        {
            return GetMinMaxValue(image, true, true);
        }

        /// <summary>
        /// Gets the minimum & maximum pixel value within an image.
        /// For color images, min/max pixel avg is returned.
        /// </summary>
        /// <param name="image">Image to search</param>
        /// <param name="findMin">Determines whether to return min value</param>
        /// <param name="findMax">Determines whether to return max value</param>
        /// <returns>min, max bytes</returns>
        private static Tuple<byte, byte> GetMinMaxValue(Image image, bool findMin, bool findMax)
        {
            if (image is Bitmap bmp)
            {
                return GetMinMaxBitmapValue(bmp, findMin, findMax);
            }
            else
            {
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    return GetMinMaxBitmapValue(bitmap, findMin, findMax);
                }
            }
        }

        /// <summary>
        /// Gets the minimum & maximum pixel value within a bitmap.
        /// For color images, min/max pixel avg is returned.
        /// </summary>
        /// <param name="bitmap">Image to search</param>
        /// <param name="findMin">Determines whether to return min value</param>
        /// <param name="findMax">Determines whether to return max value</param>
        /// <returns>min, max bytes</returns>
        private static Tuple<byte, byte> GetMinMaxBitmapValue(Bitmap bitmap, bool findMin, bool findMax)
        {
            // Start with opposite value
            byte min = findMin ? byte.MaxValue : byte.MinValue;
            byte max = findMax ? byte.MinValue : byte.MaxValue;

            // Get bytes for image
            byte[] rgbValues = GetBitmapBytes(bitmap, out BitmapData bmpData);

            int pixelDepth = bmpData.GetPixelDepth();
            int stride = bmpData.Stride;
            int width = bmpData.GetStrideWithoutPadding();
            int height = bmpData.Height;
            byte avg;

            int limit = bmpData.GetSafeArrayLimitForImage(rgbValues);

            if (bmpData.IsColor())
            {
                // Find minimum/maximum value
                // (Zero is lowest min, 255 is highest max - stop looking once both found)
                for (int y = 0; y < height && (min > byte.MinValue || max < byte.MaxValue); y++)
                {
                    // Images may have extra bytes per row to pad for CPU addressing.
                    // so need to ensure we traverse to the correct byte when moving between rows.
                    int offset = y * stride;

                    for (int x = 0; x < width; x += pixelDepth)
                    {
                        int i = offset + x;

                        if (i < limit)
                        {
                            // Avg of RGB values only (not transparency layer)
                            avg = (byte)((rgbValues[i] + rgbValues[i + 1] + rgbValues[i + 2]) / 3);

                            // Check for min
                            if (avg < min)
                            {
                                min = avg;
                            }

                            // Check for max
                            if (avg > max)
                            {
                                max = avg;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                // Find minimum/maximum value
                for (int i = 0; i < limit && (min > byte.MinValue || max < byte.MaxValue); i += pixelDepth)
                {
                    // Check for new min
                    if (rgbValues[i] < min)
                    {
                        min = rgbValues[i];
                    }

                    // Check for new max
                    if (rgbValues[i] > max)
                    {
                        max = rgbValues[i];
                    }
                }
            }
            
            return Tuple.Create(min, max);
        }

        /// <summary>
        /// Get the average pixel value between 0-255.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <returns>Average pixel intensity</returns>
        public static byte GetAverageValue(Image image)
        {
            return GetAverageValue(image, byte.MinValue, byte.MaxValue);
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
            // Check range is valid
            if (max < min)
            {
                throw new ArgumentException("max cannot be less than min.");
            }

            // Get image bytes
            byte[] rgbValues = GetBitmapBytes(bitmap, out BitmapData bmpData);

            int pixelDepth = bmpData.GetPixelDepth();
            int stride = bmpData.Stride;
            int width = bmpData.GetStrideWithoutPadding();
            int height = bmpData.Height;

            int limit = bmpData.GetSafeArrayLimitForImage(rgbValues);

            int sum = 0;
            int count = 0;
            byte val;

            if (bmpData.IsColor())
            {
                // Find distribution of pixels at each level
                for (int y = 0; y < height; y++)
                {
                    // Images may have extra bytes per row to pad for CPU addressing.
                    // so need to ensure we traverse to the correct byte when moving between rows.
                    int offset = y * stride;

                    for (int x = 0; x < width; x += pixelDepth)
                    {
                        int i = offset + x;

                        if (i < limit)
                        {
                            // Find average value of RGB
                            val = (byte)((rgbValues[i] + rgbValues[i + 1] + rgbValues[i + 2]) / 3);

                            // Check value within range
                            if (val >= min && val <= max)
                            {
                                sum += val;
                                count++;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                // Find distribution of pixels at each level
                for (int i = 0; i < limit; i += pixelDepth)
                {
                    val = rgbValues[i];

                    // Check value within range
                    if (val >= min && val <= max)
                    {
                        sum += val;
                        count++;
                    }
                }
            }

            // Return average value within range
            return count > 0 ? (byte)Math.Round(Convert.ToDouble(sum) / count) : byte.MinValue;
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
            // Get image types to compare
            IEnumerable<ImageType> imageTypes = Enum.GetValues(typeof(ImageType)).Cast<ImageType>().Where(t => t != ImageType.Unknown);

            // Compare types (default = 0, Unknown)
            imageType = imageTypes.FirstOrDefault(t => IsImageType(imageBytes, t));

            return imageType != ImageType.Unknown;
        }

        /// <summary>
        /// Determines if image matches the specified type.
        /// </summary>
        /// <param name="imageBytes">Image bytes (including header bytes)</param>
        /// <param name="type">Type of image to test against</param>
        /// <returns>True if image header matches type</returns>
        public static bool IsImageType(byte[] imageBytes, ImageType type)
        {
            if (type == ImageType.Unknown)
            {
                // If unknown, check it doesn't match any valid type
                return !TryGetImageType(imageBytes, out _);
            }
            else
            {
                // Get header bytes for image type
                byte[] encodingBytes = type.GetHeaderBytes();

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
}

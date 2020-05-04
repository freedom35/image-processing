using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            byte[] rgbValues = new byte[bmpData.Stride * bmpData.Height];

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

            byte[] rgbValues = ImageEdit.BeginRead(bitmap, out BitmapData bmpData);

            int pixelDepth = (bmpData.Stride / bmpData.Width);
            byte avg;

            // Find minimum value
            // (Zero is lowest possible, so stop looking once found)
            for (int i = 0; i < rgbValues.Length && min > byte.MinValue; i += pixelDepth)
            {
                if (pixelDepth == 3)
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

            ImageEdit.EndRead(bitmap, bmpData);

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

            byte[] rgbValues = ImageEdit.BeginRead(bitmap, out BitmapData bmpData);

            int pixelDepth = (bmpData.Stride / bmpData.Width);
            byte avg;

            // Find maximum value
            // (255 is highest possible, so stop looking once found)
            for (int i = 0; i < rgbValues.Length && max < byte.MaxValue; i += pixelDepth)
            {
                if (pixelDepth == 3)
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

            ImageEdit.EndRead(bitmap, bmpData);

            return max;
        }
    }
}

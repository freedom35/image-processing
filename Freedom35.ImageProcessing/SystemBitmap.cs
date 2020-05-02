using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class to handle System.Drawing.Bitmap related methods.
    /// </summary>
    internal static class SystemBitmap
    {
        /// <summary>
        /// Converts system image to bitmap.
        /// (Image processing performed on bytes in bitmap format)
        /// </summary>
        public static Bitmap ConvertImageToBitmap(Image image)
        {
            return (Bitmap)ConvertImageFormat(image, ImageFormat.Bmp);
        }

        /// <summary>
        /// Converts system image format.
        /// </summary>
        public static Image ConvertImageFormat(Image image, ImageFormat targetFormat)
        {
            // Returning new image
            Image clone = (Image)image.Clone();

            // Check not already a Bitmap
            if (!image.RawFormat.Equals(targetFormat))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Save to stream in Bitmap format
                    image.Save(ms, targetFormat);

                    // Dispose of current image
                    //image.Dispose();

                    // Create new image
                    clone = Image.FromStream(ms);
                }
            }

            return clone;
        }

        /// <summary>
        /// Get the bytes from a bitmap image.
        /// </summary>
        public static byte[] GetBytes(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                return stream.ToArray();
            }

            // Lock full image
            //Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            //// Lock the bits while we read them.  
            //bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            // Get number of bytes in image
            //int byteCount = (bmpData.Stride * bmpData.Height);

            //byte[] rgbValues = new byte[byteCount];

            //// Copy the RGB values into the array.
            //Marshal.Copy(bmpData.Scan0, rgbValues, 0, byteCount);

            //// Unlock the bits.
            //bitmap.UnlockBits(bmpData);

            //return rgbValues;
        }

        /// <summary>
        /// Begin editing an image.
        /// </summary>
        public static byte[] BeginEdit(Bitmap bitmap, out BitmapData bmpData)
        {
            return BeginEdit(bitmap, ImageLockMode.ReadWrite, out bmpData);
        }

        /// <summary>
        /// Begin editing an image.
        /// </summary>
        public static byte[] BeginEdit(Bitmap bitmap, ImageLockMode lockMode, out BitmapData bmpData)
        {
            // Lock full image
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Lock the bitmap's bits while we change them.  
            bmpData = bitmap.LockBits(rect, lockMode, bitmap.PixelFormat);
            
            // Get number of bytes in image
            int byteCount = (bmpData.Stride * bmpData.Height);

            byte[] rgbValues = new byte[byteCount];

            // Copy the RGB values into the array.
            Marshal.Copy(bmpData.Scan0, rgbValues, 0, byteCount);

            return rgbValues;
        }

        /// <summary>
        /// Finished editing an image.
        /// </summary>
        public static void EndEdit(Bitmap bitmap, BitmapData bmpData, byte[] rgbValues)
        {
            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbValues, 0, bmpData.Scan0, rgbValues.Length);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);
        }
    }
}

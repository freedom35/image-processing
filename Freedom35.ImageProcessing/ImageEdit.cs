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
    internal static class ImageEdit
    {
        /// <summary>
        /// Begin editing an image.
        /// </summary>
        public static byte[] Begin(Bitmap bitmap, out BitmapData bmpData)
        {
            return Begin(bitmap, ImageLockMode.ReadWrite, out bmpData);
        }

        /// <summary>
        /// Begin editing an image.
        /// </summary>
        public static byte[] Begin(Bitmap bitmap, ImageLockMode lockMode, out BitmapData bmpData)
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
        public static void End(Bitmap bitmap, BitmapData bmpData, byte[] rgbValues)
        {
            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbValues, 0, bmpData.Scan0, rgbValues.Length);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);
        }
    }
}

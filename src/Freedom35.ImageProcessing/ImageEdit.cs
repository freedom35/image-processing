//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class to handle editing System.Drawing.Bitmap.
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
                return ImageBytes.BytesToBits(rgbValues);
            }
            else
            {
                return rgbValues;
            }
        }

        /// <summary>
        /// Finished editing an image.
        /// (Copies new byte values to image and unlocks bitmap)
        /// </summary>
        public static void End(Bitmap bitmap, BitmapData bmpData, byte[] rgbValues)
        {
            // Convert monochrome back to byte array
            if (bitmap.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                // Restore stride
                bmpData.Stride /= Constants.BitsPerByte;

                // Consolidate to bytes
                rgbValues = ImageBytes.BitsToBytes(rgbValues);
            }

            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbValues, 0, bmpData.Scan0, rgbValues.Length);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);
        }
    }
}

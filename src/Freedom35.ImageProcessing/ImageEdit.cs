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
        public static byte[] Begin(Bitmap bitmap, out BitmapData bitmapData)
        {
            return Begin(bitmap, ImageLockMode.ReadWrite, out bitmapData);
        }

        /// <summary>
        /// Begin editing an image.
        /// </summary>
        public static byte[] Begin(Bitmap bitmap, ImageLockMode lockMode, out BitmapData bitmapData)
        {
            // Lock full image
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Lock the bitmap's bits while we change them.  
            bitmapData = bitmap.LockBits(rect, lockMode, bitmap.PixelFormat);

            // Create length with number of bytes in image
            byte[] rgbValues = new byte[bitmapData.GetByteCount()];

            // Copy the RGB values into the array.
            Marshal.Copy(bitmapData.Scan0, rgbValues, 0, rgbValues.Length);

            // Check if monochrome
            if (bitmap.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                // Adjust stride
                bitmapData.Stride *= Constants.BitsPerByte;

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
        public static void End(Bitmap bitmap, BitmapData bitmapData, byte[] imageBytes)
        {
            // Convert monochrome back to byte array
            if (bitmap.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                // Restore stride
                bitmapData.Stride /= Constants.BitsPerByte;

                // Consolidate to bytes
                imageBytes = ImageBytes.BitsToBytes(imageBytes);
            }

            // Copy the RGB values back to the bitmap
            Marshal.Copy(imageBytes, 0, bitmapData.Scan0, imageBytes.Length);

            // Unlock the bits.
            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// Finished editing an image.
        /// (Just unlocks bitmap)
        /// </summary>
        public static void End(Bitmap bitmap, BitmapData bitmapData)
        {
            // Unlock the bits.
            bitmap.UnlockBits(bitmapData);
        }
    }
}

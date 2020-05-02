using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

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

            // Create length with number of bytes in image
            byte[] rgbValues = new byte[bmpData.Stride * bmpData.Height];

            // Copy the RGB values into the array.
            Marshal.Copy(bmpData.Scan0, rgbValues, 0, rgbValues.Length);

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

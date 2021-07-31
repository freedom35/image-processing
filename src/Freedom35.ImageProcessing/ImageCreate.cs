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
    /// Helper class for creating images.
    /// </summary>
    internal static class ImageCreate
    {
        /// <summary>
        /// Creates a grayscale bitmap.
        /// </summary>
        public static Bitmap CreateGrayscaleBitmap(int width, int height, byte[] imageBytes)
        {
            // Create new image with same dimensions in grayscale
            Bitmap grayscaleBitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            // Create suitable color palette
            ImageColorPalette.ApplyGrayscale8bit(grayscaleBitmap);

            // Lock full image
            Rectangle rect = new Rectangle(0, 0, grayscaleBitmap.Width, grayscaleBitmap.Height);

            // Lock the bitmap's bits while we change them.  
            BitmapData bitmapData = grayscaleBitmap.LockBits(rect, ImageLockMode.WriteOnly, grayscaleBitmap.PixelFormat);

            try
            {
                // Copy the binary values to the bitmap
                Marshal.Copy(imageBytes, 0, bitmapData.Scan0, imageBytes.Length);
            }
            finally
            {
                // Release/unlock image
                grayscaleBitmap.UnlockBits(bitmapData);
            }

            return grayscaleBitmap;
        }
    }
}

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
    /// Methods for copying images.
    /// </summary>
    public static class ImageCopy
    {
        /// <summary>
        /// Copies pixels from source image to destination.
        /// </summary>
        public static void FromSourceToDestination(Bitmap imageSource, Bitmap imageDestination)
        {
            // Lock destination during copy/write
            byte[] rgbValuesDest = ImageEdit.Begin(imageDestination, out BitmapData bmpDataDest);

            // Copy entire image
            Rectangle rect = new Rectangle(0, 0, imageSource.Width, imageSource.Height);

            // Lock source during copy/read
            BitmapData bmpDataSource = imageSource.LockBits(rect, ImageLockMode.ReadOnly, imageSource.PixelFormat);

            // Copy the RGB values into the array.
            Marshal.Copy(bmpDataSource.Scan0, rgbValuesDest, 0, rgbValuesDest.Length);

            // Release source image
            imageSource.UnlockBits(bmpDataSource);

            // Release lock on destination
            ImageEdit.End(imageDestination, bmpDataDest, rgbValuesDest);
        }
    }
}

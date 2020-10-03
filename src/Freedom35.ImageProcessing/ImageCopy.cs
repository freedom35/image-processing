//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Imaging;

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
            // Get bytes to copy
            byte[] sourceBytes = ImageBytes.FromImage(imageSource);

            // Lock destination during copy/write
            byte[] destinationBytes = ImageEdit.Begin(imageDestination, out BitmapData bmpDataDest);

            // Copy as many bytes of the image as possible
            int limit = Math.Min(sourceBytes.Length, destinationBytes.Length);

            Array.Copy(sourceBytes, destinationBytes, limit);

            // Release lock on destination
            ImageEdit.End(imageDestination, bmpDataDest, destinationBytes);
        }
    }
}

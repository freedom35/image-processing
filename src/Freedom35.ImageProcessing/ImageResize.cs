//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
using System.Drawing;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Methods for resizing an image.
    /// </summary>
    public static class ImageResize
    {
        /// <summary>
        /// Resizes an image to the new width/height.
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="width">New width of image</param>
        /// <param name="height">New height of image</param>
        /// <returns>New image with new size</returns>
        public static Bitmap ResizeAsNew(Bitmap image, int width, int height)
        {
            return new Bitmap(image, width, height);
        }

        /// <summary>
        /// Resizes an image to the new width/height.
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="width">New width of image</param>
        /// <param name="height">New height of image</param>
        public static void ResizeOriginal(ref Bitmap image, int width, int height)
        {
            // Retain pointer to original for disposal
            Bitmap resizedBitmap = new Bitmap(image, width, height);

            // Dispose of original image
            image.Dispose();

            // Assign to original ref
            image = resizedBitmap;
        }

        /// <summary>
        /// Resizes an image and maintains aspect ratio.
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="sizeRatio">Size ratio of new image</param>
        /// <returns>New image with new size</returns>
        public static Bitmap ResizeAsNew(Bitmap image, double sizeRatio)
        {
            // Maintain aspect ratio
            int newWidth = (int)Math.Round(image.Width * sizeRatio);
            int newHeight = (int)Math.Round(image.Height * sizeRatio);

            return ResizeAsNew(image, newWidth, newHeight);
        }

        /// <summary>
        /// Resizes an image and maintains aspect ratio.
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="sizeRatio">Size ratio of new image</param>
        public static void ResizeOriginal(ref Bitmap image, double sizeRatio)
        {
            // Maintain aspect ratio
            int newWidth = (int)Math.Round(image.Width * sizeRatio);
            int newHeight = (int)Math.Round(image.Height * sizeRatio);

            ResizeOriginal(ref image, newWidth, newHeight);
        }
    }
}

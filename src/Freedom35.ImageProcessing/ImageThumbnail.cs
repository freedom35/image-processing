//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
using System.Drawing;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class for creating an image thumbnail.
    /// </summary>
    public static class ImageThumbnail
    {
        /// <summary>
        /// Creates a thumbnail image based on the original image.
        /// Note: For larger thumbnail images, resize methods will produce a higher quality image.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to base thumbnail on</param>
        /// <param name="thumbnailSize">Size of thumbnail image</param>
        /// <returns>Thumbnail image</returns>
        public static T Create<T>(T image, Size thumbnailSize) where T : Image
        {
            return Create(image, thumbnailSize.Width, thumbnailSize.Height);
        }

        /// <summary>
        /// Creates a thumbnail image based on the original image.
        /// Note: For larger thumbnail images, resize methods will produce a higher quality image.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to base thumbnail on</param>
        /// <param name="thumbnailWidth">Width of thumbnail image</param>
        /// <param name="thumbnailHeight">Height of thumbnail image</param>
        /// <returns>Thumbnail image</returns>
        public static T Create<T>(T image, int thumbnailWidth, int thumbnailHeight) where T : Image
        {
            // Get aspect ratios for image
            double widthAspect = (double)image.Width / thumbnailWidth;
            double heightAspect = (double)image.Height / thumbnailHeight;

            // Do nothing if aspect same, else adjust target size to maintain aspect ratio
            if (widthAspect > heightAspect)
            {
                thumbnailHeight = (int)Math.Round(image.Height / widthAspect);
            }
            else if (widthAspect < heightAspect)
            {
                thumbnailWidth = (int)Math.Round(image.Width / heightAspect);
            }

            // Create callback for thumbnail method
            var thumbCallback = new Image.GetThumbnailImageAbort(AbortThumbnailCallback);

            return (T)image.GetThumbnailImage(thumbnailWidth, thumbnailHeight, thumbCallback, IntPtr.Zero);
        }

        /// <summary>
        /// Creates a thumbnail image based on the original image.
        /// Note: For larger thumbnail images, resize methods will produce a higher quality image.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to base thumbnail on</param>
        /// <param name="maxThumbnailWidth">Max width of thumbnail image</param>
        /// <param name="maxThumbnailHeight">Max height of thumbnail image</param>
        /// <returns>Thumbnail image</returns>
        public static T CreateWithSameAspect<T>(T image, int maxThumbnailWidth, int maxThumbnailHeight) where T : Image
        {
            // Get aspect ratios for image
            double widthAspect = (double)image.Width / maxThumbnailWidth;
            double heightAspect = (double)image.Height / maxThumbnailHeight;

            // Do nothing if aspect same, else adjust target size to maintain aspect ratio
            if (widthAspect > heightAspect)
            {
                maxThumbnailHeight = (int)Math.Round(image.Height / widthAspect);
            }
            else if (widthAspect < heightAspect)
            {
                maxThumbnailWidth = (int)Math.Round(image.Width / heightAspect);
            }

            return Create(image, maxThumbnailWidth, maxThumbnailHeight);
        }

        /// <summary>
        /// Never called - valid callback required for creating thumbnail.
        /// </summary>
        private static bool AbortThumbnailCallback()
        {
            return false;
        }
    }
}

﻿//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
using System.Drawing;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class containing methods for cropping an image.
    /// </summary>
    public static class ImageCrop
    {
        /// <summary>
        /// Crops an image based on the crop region.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to crop</param>
        /// <param name="cropRegion">Region within image to crop</param>
        /// <returns>Cropped image</returns>
        public static T ByRegion<T>(T image, Rectangle cropRegion) where T : Image
        {
            // Check if already a bitmap
            if (image is Bitmap bmp)
            {
                return (T)(Image)CropBitmap(bmp, cropRegion);
            }
            else
            {
                // Convert to bitmap
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    // Crop as bitmap
                    using (Image croppedBitmap = CropBitmap(bitmap, cropRegion))
                    {
                        // Restore original image format
                        return (T)ImageFormatting.ToFormat(croppedBitmap, image.RawFormat);
                    }
                }
            }
        }

        /// <summary>
        /// Crops bitmap based on the crop region.
        /// </summary>
        /// <param name="bitmap">Image to crop</param>
        /// <param name="cropRegion">Region within image to crop</param>
        /// <returns>Cropped image</returns>
        private static Bitmap CropBitmap(Bitmap bitmap, Rectangle cropRegion)
        {
            //////////////////////////////////////
            // Trim region if overlaps edges
            //////////////////////////////////////
            
            // X outside image
            if (cropRegion.X < 0)
            {
                cropRegion.Width += cropRegion.X;
                cropRegion.X = 0;
            }

            // Y outside image
            if (cropRegion.Y < 0)
            {
                cropRegion.Height += cropRegion.Y;
                cropRegion.Y = 0;
            }

            // Width spans outside of image
            if (cropRegion.Right > bitmap.Width)
            {
                cropRegion.Width -= bitmap.Width - cropRegion.X;
            }

            // Height spans outside of image
            if (cropRegion.Bottom > bitmap.Height)
            {
                cropRegion.Height -= bitmap.Height - cropRegion.Y;
            }

            // Check region still within image
            if (cropRegion.X > bitmap.Width || cropRegion.Y > bitmap.Height || cropRegion.Width <= 0 || cropRegion.Height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cropRegion), "Region provided outside of image area.");
            }

            // Return new cropped image
            return bitmap.Clone(cropRegion, bitmap.PixelFormat);
        }
    }
}

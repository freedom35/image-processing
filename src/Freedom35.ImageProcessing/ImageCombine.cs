//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class for combining images.
    /// </summary>
    public static class ImageCombine
    {
        /// <summary>
        /// Combines multiple images together.
        /// (Pixel values combined via bitwise or, not averaged)
        /// </summary>
        /// <param name="images">Images to combine</param>
        /// <returns>New combined image as bitmap</returns>
        public static Bitmap All<T>(IEnumerable<T> images) where T : Image
        {
            // Check have at least 1 image
            if (!images.Any())
            {
                return null;
            }

            // Use first image as starting point
            Bitmap combinedImage = ImageFormatting.ToBitmap(images.ElementAt(0));

            // Get bytes for image
            byte[] rgbValues1 = ImageEdit.Begin(combinedImage, out BitmapData bmpData1);

            try
            {
                bool firstImageIsColor = bmpData1.IsColor();

                // Add additional images to first
                foreach (Image image in images.Skip(1))
                {
                    // Only reading this image
                    byte[] rgbValues2 = ImageBytes.FromImage(image, out BitmapData bmpData2);

                    // Check both images are color or B&W
                    if (firstImageIsColor == bmpData2.IsColor())
                    {
                        // Protect against different sized images
                        int limit = Math.Min(rgbValues1.Length, rgbValues2.Length);

                        // Combine images
                        for (int i = 0; i < limit; i++)
                        {
                            // Max value for pixel is 255
                            rgbValues1[i] |= rgbValues2[i];
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Not all images have the same color depth");
                    }    
                }
            }
            finally
            {
                // Release combined image
                ImageEdit.End(combinedImage, bmpData1, rgbValues1);
            }

            return combinedImage;
        }
    }
}

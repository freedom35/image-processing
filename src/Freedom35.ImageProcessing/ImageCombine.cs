﻿//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
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
        /// (Pixel values are added, not averaged)
        /// </summary>
        /// <param name="images">Images to combine</param>
        /// <returns>New combined image</returns>
        public static Bitmap All<T>(IEnumerable<T> images) where T : Image
        {
            // Check have at least 1 image
            if (images.Count() == 0)
            {
                return null;
            }

            // Use first image as starting point
            Bitmap combinedImage = ImageFormatting.ToBitmap(images.ElementAt(0));

            // Get bytes for image
            byte[] rgbValues1 = ImageEdit.Begin(combinedImage, out BitmapData bmpData1);

            int pixelDepth = bmpData1.GetPixelDepth();
            bool isColor = bmpData1.IsColor();

            int limit1 = bmpData1.GetSafeArrayLimitForImage(rgbValues1);

            // Add additional images to first
            foreach (Image image in images.Skip(1))
            {
                // Only reading this image
                byte[] rgbValues2 = ImageBytes.FromImage(image, out BitmapData bmpData2);

                // Check both images are color or B&W
                if (isColor == bmpData2.IsColor())
                {
                    // Protect against different sized images
                    int limit = System.Math.Min(limit1, bmpData2.GetSafeArrayLimitForImage(rgbValues2));

                    // Combine images
                    for (int i = 0; i < limit; i += pixelDepth)
                    {
                        rgbValues1[i] = (byte)((rgbValues1[i] + rgbValues2[i]) & 0xFFF0);   // Max 255

                        if (isColor)
                        {
                            rgbValues1[i + 1] = (byte)((rgbValues1[i + 1] + rgbValues2[i + 1]) & 0xFFF0);   // Max 255
                            rgbValues1[i + 2] = (byte)((rgbValues1[i + 2] + rgbValues2[i + 2]) & 0xFFF0);   // Max 255
                        }
                    }
                }
            }

            // Release combined image
            ImageEdit.End(combinedImage, bmpData1, rgbValues1);

            return combinedImage;
        }
    }
}

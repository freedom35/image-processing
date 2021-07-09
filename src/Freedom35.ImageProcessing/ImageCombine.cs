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
                // Get image 1 data
                int pixelDepth1 = bmpData1.GetPixelDepth();
                int pixelDepthWithoutAlpha = Math.Min(pixelDepth1, Constants.PixelDepthRGB);
                int stride1 = bmpData1.Stride;
                int width1 = bmpData1.GetStrideWithoutPadding();
                int height1 = bmpData1.Height;

                // Add additional images to first
                foreach (Image image in images.Skip(1))
                {
                    // Only reading this image
                    byte[] rgbValues2 = ImageBytes.FromImage(image, out BitmapData bmpData2);

                    // Check both images are color or B&W
                    if (pixelDepth1 == bmpData2.GetPixelDepth())
                    {
                        // Protect against different sized images
                        int limit = Math.Min(bmpData1.GetSafeArrayLimitForImage(rgbValues1), bmpData2.GetSafeArrayLimitForImage(rgbValues2));
                        int minHeight = Math.Min(height1, bmpData2.Height);
                        int minStride = Math.Min(stride1, bmpData2.Stride);
                        int minWidth = Math.Min(width1, bmpData2.GetStrideWithoutPadding());

                        for (int y = 0; y < minHeight; y++)
                        {
                            // Images may have extra bytes per row to pad for CPU addressing.
                            // so need to ensure we traverse to the correct byte when moving between rows.
                            int offset = y * minStride;

                            for (int x = 0; x < minWidth; x += pixelDepth1)
                            {
                                int i = offset + x;

                                if (i < limit)
                                {
                                    for (int j = 0; j < pixelDepthWithoutAlpha; j++)
                                    {
                                        // Combine images
                                        rgbValues1[i + j] |= rgbValues2[i + j];
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
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

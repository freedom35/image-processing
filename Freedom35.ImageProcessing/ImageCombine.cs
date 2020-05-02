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
        public static Bitmap All(IEnumerable<Bitmap> images)
        {
            // Check have at least 1 image
            if (images.Count() == 0)
            {
                return null;
            }

            // Use first image as starting point
            Bitmap combinedImage = (Bitmap)images.ElementAt(0).Clone();

            // Get bytes for image
            byte[] rgbValues1 = ImageEdit.Begin(combinedImage, ImageLockMode.ReadWrite, out BitmapData bmpData1);

            int pixelDepth = (bmpData1.Stride / bmpData1.Width);

            // Add additional images to first
            foreach (Bitmap bitmap in images.Skip(1))
            {
                // Only reading this image
                byte[] rgbValues2 = ImageBytes.Get(bitmap);

                // Combine images
                for (int i = 0; i < rgbValues1.Length; i += pixelDepth)
                {
                    rgbValues1[i] = (byte)((rgbValues1[i] + rgbValues2[i]) & 0xFFF0);   // Max 255

                    if (pixelDepth == 3)
                    {
                        rgbValues1[i + 1] = (byte)((rgbValues1[i + 1] + rgbValues2[i + 1]) & 0xFFF0);   // Max 255
                        rgbValues1[i + 2] = (byte)((rgbValues1[i + 2] + rgbValues2[i + 2]) & 0xFFF0);   // Max 255
                    }
                }
            }

            // Release combined image
            ImageEdit.End(combinedImage, bmpData1, rgbValues1);

            return combinedImage;
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Methods for converting color.
    /// </summary>
    public static class ImageConvert
    {
        /// <summary>
        /// Converts system image to bitmap.
        /// (Image processing performed on bytes in bitmap format)
        /// </summary>
        public static Bitmap ImageToBitmap(Image image)
        {
            return (Bitmap)ImageToFormat(image, ImageFormat.Bmp);
        }

        /// <summary>
        /// Converts system image format.
        /// </summary>
        public static Image ImageToFormat(Image image, ImageFormat targetFormat)
        {
            // Returning new image
            Image clone = (Image)image.Clone();

            // Check not already a Bitmap
            if (!image.RawFormat.Equals(targetFormat))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Save to stream in Bitmap format
                    image.Save(ms, targetFormat);

                    // Dispose of current image
                    //image.Dispose();

                    // Create new image
                    clone = Image.FromStream(ms);
                }
            }

            return clone;
        }

        /// <summary>
        /// Converts color image bytes to grayscale.
        /// </summary>
        /// <returns>New image as grayscale</returns>
        /// <param name="rgbBytes">bytes for color image</param>
        public static byte[] ColorImageToGrayscale(byte[] rgbBytes)
        {
            // Check image bytes non-null
            int length = rgbBytes?.Length ?? 0;

            // Check length is valid, otherwise unlikely color
            if (length % 3 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rgbBytes), "Array length is not divisible by 3.");
            }

            // Create new array for converted bytes
            byte[] grayscaleBytes = new byte[length / 3];

            // Original array contains 3 bytes per pixel - color/RGB
            // Converted array will only contain one byte per pixel
            for (int i = 0, j = 0; i < length - 2; i += 3, j++)
            {
                // Get average value for each RGB pixel
                grayscaleBytes[j] = (byte)((rgbBytes[i] + rgbBytes[i + 1] + rgbBytes[i + 2]) / 3);
            }

            return grayscaleBytes;
        }

        /// <summary>
        /// Converts grayscale image bytes to black and white.
        /// </summary>
        /// <returns>New image as black and white</returns>
        /// <param name="grayscaleBytes">bytes for grayscale image</param>
        public static byte[] GrayscaleImageToBlackAndWhite(byte[] grayscaleBytes)
        {
            // Use mid-threshold value for each pixel
            return GrayscaleImageToBlackAndWhite(grayscaleBytes, 128);
        }

        /// <summary>
        /// Converts grayscale image bytes to black and white using a specific threshold value.
        /// </summary>
        /// <returns>New image as black and white</returns>
        /// <param name="grayscaleBytes">bytes for grayscale image</param>
        /// <param name="whiteThreshold">Threshold value for determining a white value</param>
        public static byte[] GrayscaleImageToBlackAndWhite(byte[] grayscaleBytes, byte whiteThreshold)
        {
            // Check image bytes non-null
            int length = grayscaleBytes?.Length ?? 0;

            // Create new array for converted bytes
            byte[] bwBytes = new byte[length];

            // Convert each pixel
            for (int i = 0; i < length; i++)
            {
                // Use threshold value to determine black (0) or white (255)
                bwBytes[i] = (grayscaleBytes[i] < whiteThreshold ? byte.MinValue : byte.MaxValue);
            }

            return bwBytes;
        }

        /// <summary>
        /// Inverts image to negative.
        /// </summary>
        /// <param name="image">Image to convert</param>
        /// <returns>Negative image</returns>
        public static Image ToNegative(Image image)
        {
            Bitmap bitmap = ImageToBitmap(image);

            Bitmap negativeBitmap = ToNegative(bitmap);

            // Restore original image format
            return ImageToFormat(negativeBitmap, image.RawFormat);
        }

        /// <summary>
        /// Inverts image to negative.
        /// </summary>
        /// <param name="bitmap">Image to convert</param>
        /// <returns>Negative image</returns>
        public static Bitmap ToNegative(Bitmap bitmap)
        {
            // Return new image
            Bitmap clone = (Bitmap)bitmap.Clone();

            byte[] rgbValues = ImageEdit.Begin(clone, out BitmapData bmpData);

            int pixelDepth = (bmpData.Stride / bmpData.Width);

            int limit = (pixelDepth > 1 ? rgbValues.Length - (pixelDepth - 1) : rgbValues.Length);

            for (int i = 0; i < limit; i += pixelDepth)
            {
                rgbValues[i] = (byte)~rgbValues[i];

                if (pixelDepth == 3)
                {
                    rgbValues[i + 1] = (byte)~rgbValues[i + 1];
                    rgbValues[i + 2] = (byte)~rgbValues[i + 2];
                }
            }

            ImageEdit.End(clone, bmpData, rgbValues);

            return clone;
        }
    }
}

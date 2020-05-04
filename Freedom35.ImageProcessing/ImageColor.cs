using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Methods for converting color.
    /// </summary>
    public static class ImageColor
    {
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
            return GrayscaleImageToBlackAndWhite(grayscaleBytes, 0x80);
        }

        /// <summary>
        /// Converts grayscale image bytes to black and white using a specific threshold value.
        /// </summary>
        /// <returns>New image as black and white</returns>
        /// <param name="grayscaleBytes">bytes for grayscale image</param>
        /// <param name="whiteThreshold">Threshold value for determining a white value (lower will be black)</param>
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
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            Bitmap negativeBitmap = ToNegative(bitmap);

            // Restore original image format
            return ImageFormatting.ToFormat(negativeBitmap, image.RawFormat);
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

            // Edit clone
            byte[] imageBytes = ImageEdit.BeginWrite(clone, out BitmapData bmpData);

            ToNegative(imageBytes);

            ImageEdit.EndWrite(clone, bmpData, imageBytes);

            return clone;
        }

        /// <summary>
        /// Inverts image to negative.
        /// </summary>
        /// <param name="imageBytes">Image bytes to convert</param>
        public static void ToNegative(byte[] imageBytes)
        {
            // Invert bits on each byte
            for (int i = 0; i < imageBytes.Length; i++)
            {
                imageBytes[i] = (byte)~imageBytes[i];
            }
        }
    }
}

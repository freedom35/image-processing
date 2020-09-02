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
            byte[] imageBytes = ImageEdit.Begin(clone, out BitmapData bmpData);

            ToNegative(imageBytes);

            ImageEdit.End(clone, bmpData, imageBytes);

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

        /// <summary>
        /// Applies red color filter to image.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <returns>New image with filter applied</returns>
        public static T ToRed<T>(T image) where T : Image
        {
            return ApplyFilterRGB(image, 0xFF, 0x00, 0x00);
        }

        /// <summary>
        /// Applies green color filter to image.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <returns>New image with filter applied</returns>
        public static T ToGreen<T>(T image) where T : Image
        {
            return ApplyFilterRGB(image, 0x00, 0xFF, 0x00);
        }

        /// <summary>
        /// Applies blue color filter to image.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <returns>New image with filter applied</returns>
        public static T ToBlue<T>(T image) where T : Image
        {
            return ApplyFilterRGB(image, 0x00, 0x00, 0xFF);
        }

        /// <summary>
        /// Applies color filter to image.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <param name="r">Red component to apply</param>
        /// <param name="g">Green component to apply</param>
        /// <param name="b">Blue component to apply</param>
        /// <returns>New image with filter applied</returns>
        public static T ApplyFilterRGB<T>(T image, byte r, byte g, byte b) where T : Image
        {
            if (image is Bitmap bmp)
            {
                // Return new image
                Bitmap clone = (Bitmap)bmp.Clone();

                ApplyFilterDirectRGB(ref clone, r, g, b);

                return (T)(Image)clone;
            }
            else
            {
                // Create new bitmap from image
                Bitmap bitmap = ImageFormatting.ToBitmap(image);

                ApplyFilterDirectRGB(ref bitmap, r, g, b);

                // Restore original image format
                Image thresholdImage = ImageFormatting.ToFormat(bitmap, image.RawFormat);

                // Dispose of temp image
                bitmap.Dispose();

                return (T)thresholdImage;
            }
        }

        /// <summary>
        /// Applies color filter to image.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="r">Red component to apply</param>
        /// <param name="g">Green component to apply</param>
        /// <param name="b">Blue component to apply</param>
        public static void ApplyFilterDirectRGB(ref Bitmap bitmap, byte r, byte g, byte b)
        {
            // Get image bytes and info
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            try
            {
                // Can only apply color filter to a color image
                if (bmpData.IsColor())
                {
                    ApplyFilterDirectRGB(imageBytes, r, g, b);
                }
                else
                {
                    throw new ArgumentException("Image is not color, RGB filter cannot be applied.");
                }
            }
            finally
            {
                ImageEdit.End(bitmap, bmpData, imageBytes);
            }
        }

        /// <summary>
        /// Applies color filter to image.
        /// </summary>
        /// <param name="imageBytes">Color image bytes to process</param>
        /// <param name="r">Red component to apply</param>
        /// <param name="g">Green component to apply</param>
        /// <param name="b">Blue component to apply</param>
        public static void ApplyFilterDirectRGB(byte[] imageBytes, byte r, byte g, byte b)
        {
            // Apply mask for each color
            for (int i = 0; i < imageBytes.Length - 2; i += 3)
            {
                // Red (LSB)
                imageBytes[i + 2] &= r;

                // Green
                imageBytes[i + 1] &= g;

                // Blue (MSB)
                imageBytes[i] &= b;
            }
        }
    }
}

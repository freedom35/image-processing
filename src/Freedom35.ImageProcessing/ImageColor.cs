//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Methods for converting color.
    /// </summary>
    public static class ImageColor
    {
        /// <summary>
        /// Converts color image to grayscale.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to convert</param>
        /// <returns>Grayscale image</returns>
        public static T ToGrayscale<T>(T image) where T : Image
        {
            Bitmap colorBitmap = ImageFormatting.ToBitmap(image);

            // Read bytes for image
            byte[] originalBytes = ImageBytes.FromImage(colorBitmap, out BitmapData bitmapData);

            if (!bitmapData.IsColor())
            {
                throw new ArgumentException("Image is not color, cannot convert to grayscale.");
            }

            // Convert bytes
            byte[] convertedBytes = ToGrayscale(originalBytes, bitmapData);

            // Create new image with same dimensions in grayscale
            Bitmap grayscaleBitmap = CreateGrayscaleBitmap(bitmapData.Width, bitmapData.Height, convertedBytes);

            // Convert to original image format
            return (T)ImageFormatting.Convert(grayscaleBitmap, image.RawFormat);
        }

        /// <summary>
        /// Creates a grayscale bitmap.
        /// </summary>
        private static Bitmap CreateGrayscaleBitmap(int width, int height, byte[] imageBytes)
        {
            // Create new image with same dimensions in grayscale
            Bitmap grayscaleBitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            // Create suitable color palette
            ImageColorPalette.ApplyGrayscale8bit(grayscaleBitmap);

            // Lock full image
            Rectangle rect = new Rectangle(0, 0, grayscaleBitmap.Width, grayscaleBitmap.Height);

            // Lock the bitmap's bits while we change them.  
            BitmapData bitmapData = grayscaleBitmap.LockBits(rect, ImageLockMode.WriteOnly, grayscaleBitmap.PixelFormat);

            try
            {
                // Ensure we stay within image
                //int limit = Math.Min(imageBytes.Length, rect.Width * rect.Height);

                // Copy the binary values to the bitmap
                Marshal.Copy(imageBytes, 0, bitmapData.Scan0, imageBytes.Length);
            }
            finally
            {
                // Release/unlock image
                grayscaleBitmap.UnlockBits(bitmapData);
            }

            return grayscaleBitmap;
        }

        /// <summary>
        /// Converts color image bytes to grayscale.
        /// </summary>
        /// <returns>New image as grayscale</returns>
        /// <param name="imageBytes">bytes for color image</param>
        /// <param name="bitmapData">Image dimension info</param>
        /// <returns>Grayscale image bytes</returns>
        public static byte[] ToGrayscale(byte[] imageBytes, BitmapData bitmapData)
        {
            int stride = bitmapData.Stride;
            int stridePadding = bitmapData.GetStridePaddingLength();
            int width = stride - stridePadding;
            int height = bitmapData.Height;
            int limit = bitmapData.GetSafeArrayLimitForImage(imageBytes);

            // May also have alpha byte
            int pixelDepth = bitmapData.GetPixelDepth();

            // Determine any required padding
            int newStridePadding = (4 - (bitmapData.Width * sizeof(byte)) % 4) % 4;

            // Create new array for converted bytes
            byte[] grayscaleBytes = new byte[(bitmapData.Width + newStridePadding) * height];

            int grayscaleIndex = 0;

            // Apply mask for each color pixel
            for (int y = 0; y < height; y++)
            {
                // Images may have extra bytes per row to pad for CPU addressing.
                // so need to ensure we traverse to the correct byte when moving between rows.
                // I.e. not divisible by 3
                int offset = y * stride;

                for (int x = 0; x < width; x += pixelDepth)
                {
                    int i = offset + x;

                    if (i < limit && grayscaleIndex < grayscaleBytes.Length)
                    {
                        // Get average value for each RGB pixel
                        grayscaleBytes[grayscaleIndex++] = (byte)((imageBytes[i] + imageBytes[i + 1] + imageBytes[i + 2]) / 3);
                    }
                }

                // Add padding before moving to next row
                grayscaleIndex += newStridePadding;
            }

            return grayscaleBytes;
        }

        /// <summary>
        /// Converts color image bytes to grayscale.
        /// </summary>
        /// <returns>New image as grayscale</returns>
        /// <param name="rgbBytes">bytes for color image</param>
        [Obsolete("Method is deprecated, use ToGrayscale method instead.")]
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
        /// Converts image to black & white.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to convert</param>
        /// <returns>Black & white image</returns>
        public static T ToBlackAndWhite<T>(T image) where T : Image
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);
            
            // Read bytes for image
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bitmapData);

            // If color, convert to grayscale first (will convert to single byte per pixel)
            if (bitmapData.IsColor())
            {
                byte[] grayscaleBytes = ToGrayscale(imageBytes, bitmapData);

                // Release original bitmap
                ImageEdit.End(bitmap, bitmapData);

                // Create new image with same dimensions in grayscale
                bitmap = CreateGrayscaleBitmap(bitmapData.Width, bitmapData.Height, grayscaleBytes);

                // Re-aquire image bytes
                imageBytes = ImageEdit.Begin(bitmap, out bitmapData);
            }

            try
            {
                // Convert bytes
                imageBytes = GrayscaleImageToBlackAndWhite(imageBytes);
            }
            finally
            {
                // Release image
                ImageEdit.End(bitmap, bitmapData, imageBytes);
            }

            // Convert to original image format
            return (T)ImageFormatting.Convert(bitmap, image.RawFormat);
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
                bwBytes[i] = grayscaleBytes[i] < whiteThreshold ? byte.MinValue : byte.MaxValue;
            }

            return bwBytes;
        }

        /// <summary>
        /// Inverts image to negative.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to convert</param>
        /// <returns>Negative image</returns>
        public static T ToNegative<T>(T image) where T : Image
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            ToNegativeDirect(ref bitmap);

            return (T)ImageFormatting.Convert(bitmap, image.RawFormat);
        }

        /// <summary>
        /// Inverts image to negative directly.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        public static void ToNegativeDirect(ref Bitmap bitmap)
        {
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bitmapData);

            try
            {
                // Check if monochrome
                if (bitmapData.PixelFormat == PixelFormat.Format1bppIndexed)
                {
                    MonochromeToNegative(imageBytes);
                }
                else
                {
                    ToNegative(imageBytes);
                }
            }
            finally
            {
                ImageEdit.End(bitmap, bitmapData, imageBytes);
            }
        }

        /// <summary>
        /// Inverts image bytes to negative.
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
        /// Inverts monochrome image bytes (0's and 1's) to negative.
        /// </summary>
        /// <param name="monochromeBytes">Monochrome image bytes to convert</param>
        public static void MonochromeToNegative(byte[] monochromeBytes)
        {
            // Invert least significant bit on each byte
            for (int i = 0; i < monochromeBytes.Length; i++)
            {
                monochromeBytes[i] ^= 1;
            }
        }

        /// <summary>
        /// Applies red color filter to image.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <returns>New image with filter applied</returns>
        public static T ToRed<T>(T image) where T : Image
        {
            return ApplyFilterRGB(image, 0xFF, 0x00, 0x00);
        }

        /// <summary>
        /// Applies green color filter to image.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <returns>New image with filter applied</returns>
        public static T ToGreen<T>(T image) where T : Image
        {
            return ApplyFilterRGB(image, 0x00, 0xFF, 0x00);
        }

        /// <summary>
        /// Applies blue color filter to image.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <returns>New image with filter applied</returns>
        public static T ToBlue<T>(T image) where T : Image
        {
            return ApplyFilterRGB(image, 0x00, 0x00, 0xFF);
        }

        /// <summary>
        /// Applies color filter to image.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <param name="r">Red component to apply</param>
        /// <param name="g">Green component to apply</param>
        /// <param name="b">Blue component to apply</param>
        /// <returns>New image with filter applied</returns>
        public static T ApplyFilterRGB<T>(T image, byte r, byte g, byte b) where T : Image
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            ApplyFilterDirectRGB(ref bitmap, r, g, b);

            return (T)ImageFormatting.Convert(bitmap, image.RawFormat);
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
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bitmapData);

            try
            {
                // Can only apply color filter to a color image
                if (bitmapData.IsColor())
                {
                    ApplyFilterDirectRGB(imageBytes, r, g, b, bitmapData);
                }
                else
                {
                    throw new ArgumentException("Image is not color, RGB filter cannot be applied.");
                }
            }
            finally
            {
                ImageEdit.End(bitmap, bitmapData, imageBytes);
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

        /// <summary>
        /// Applies color filter to image.
        /// (Safer if source image was compressed, may be additional bytes per row)
        /// </summary>
        /// <param name="imageBytes">Color image bytes to process</param>
        /// <param name="r">Red component to apply</param>
        /// <param name="g">Green component to apply</param>
        /// <param name="b">Blue component to apply</param>
        /// <param name="bitmapData">Image dimension info</param>
        public static void ApplyFilterDirectRGB(byte[] imageBytes, byte r, byte g, byte b, BitmapData bitmapData)
        {
            if (!bitmapData.IsColor())
            {
                throw new ArgumentException("Image is not color, RGB filter cannot be applied.");
            }

            int stride = bitmapData.Stride;
            int stridePadding = bitmapData.GetStridePaddingLength();
            int width = stride - stridePadding;
            int height = bitmapData.Height;
            int limit = bitmapData.GetSafeArrayLimitForImage(imageBytes);

            // May also have alpha byte
            int pixelDepth = bitmapData.GetPixelDepth();

            // Apply mask for each color pixel
            for (int y = 0; y < height; y++)
            {
                // Images may have extra bytes per row to pad for CPU addressing.
                // so need to ensure we traverse to the correct byte when moving between rows.
                // I.e. not divisible by 3
                int offset = y * stride;

                for (int x = 0; x < width; x += pixelDepth)
                {
                    int i = offset + x;

                    if (i < limit)
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

        /// <summary>
        /// Applies sepia filter to image.
        /// (Reddish-brown color associated with old photographs)
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to convert</param>
        /// <returns>Sepia image</returns>
        public static T ToSepia<T>(T image) where T : Image
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            ToSepiaDirect(ref bitmap);

            return (T)ImageFormatting.Convert(bitmap, image.RawFormat);
        }

        /// <summary>
        /// Applies sepia filter to image.
        /// (Reddish-brown color associated with old photographs)
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        public static void ToSepiaDirect(ref Bitmap bitmap)
        {
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bitmapData);

            try
            {
                if (!bitmapData.IsColor())
                {
                    throw new ArgumentException("Image is not color, sepia filter cannot be applied.");
                }

                int stride = bitmapData.Stride;
                int stridePadding = bitmapData.GetStridePaddingLength();
                int width = stride - stridePadding;
                int height = bitmapData.Height;
                int limit = bitmapData.GetSafeArrayLimitForImage(imageBytes);

                // May also have alpha byte
                int pixelDepth = bitmapData.GetPixelDepth();

                byte originalRed, originalGreen, originalBlue;

                // Apply mask for each color pixel
                for (int y = 0; y < height; y++)
                {
                    // Images may have extra bytes per row to pad for CPU addressing.
                    // so need to ensure we traverse to the correct byte when moving between rows.
                    // I.e. not divisible by 3
                    int offset = y * stride;

                    for (int x = 0; x < width; x += pixelDepth)
                    {
                        int i = offset + x;

                        // Set RGB to sepia values (Source: Microsoft)
                        if (i < limit)
                        {
                            // Get original RGB pixel values
                            originalRed = imageBytes[i + 2];
                            originalGreen = imageBytes[i + 1];
                            originalBlue = imageBytes[i];

                            // Red (LSB)
                            imageBytes[i + 2] = ConvertToByte((0.393 * originalRed) + (0.769 * originalGreen) + (0.189 * originalBlue));

                            // Green
                            imageBytes[i + 1] = ConvertToByte((0.349 * originalRed) + (0.686 * originalGreen) + (0.168 * originalBlue));

                            // Blue (MSB)
                            imageBytes[i] = ConvertToByte((0.272 * originalRed) + (0.534 * originalGreen) + (0.131 * originalBlue));
                        }
                    }
                }
            }
            finally
            {
                ImageEdit.End(bitmap, bitmapData, imageBytes);
            }
        }

        /// <summary>
        /// Helper method to convert a double value to a byte.
        /// </summary>
        private static byte ConvertToByte(double f)
        {
            // Ensure no bigger than 255
            return f < byte.MaxValue ? (byte)Math.Round(f) : byte.MaxValue;
        }
    }
}

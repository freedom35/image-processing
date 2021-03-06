﻿//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
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
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            // Check if monochrome
            if (bmpData.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                MonochromeToNegative(imageBytes);
            }
            else
            {
                ToNegative(imageBytes);
            }

            ImageEdit.End(bitmap, bmpData, imageBytes);
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
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            try
            {
                // Can only apply color filter to a color image
                if (bmpData.IsColor())
                {
                    ApplyFilterDirectRGB(imageBytes, r, g, b, bmpData);
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

        /// <summary>
        /// Applies color filter to image.
        /// (Safer if source image was compressed, may be additional bytes per row)
        /// </summary>
        /// <param name="imageBytes">Color image bytes to process</param>
        /// <param name="r">Red component to apply</param>
        /// <param name="g">Green component to apply</param>
        /// <param name="b">Blue component to apply</param>
        /// <param name="bmpData">Image dimension info</param>
        public static void ApplyFilterDirectRGB(byte[] imageBytes, byte r, byte g, byte b, BitmapData bmpData)
        {
            if (!bmpData.IsColor())
            {
                throw new ArgumentException("Image is not color, RGB filter cannot be applied.");
            }

            int stride = bmpData.Stride;
            int stridePadding = bmpData.GetStridePaddingLength();
            int width = stride - stridePadding;
            int height = bmpData.Height;
            int limit = bmpData.GetSafeArrayLimitForImage(imageBytes);

            // May also have alpha byte
            int pixelDepth = bmpData.GetPixelDepth();

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
    }
}

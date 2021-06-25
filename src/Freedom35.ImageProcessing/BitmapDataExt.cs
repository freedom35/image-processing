//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
using System.Drawing.Imaging;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class to extend BitmapData class.
    /// (BitmapData is sealed - cannot inherit)
    /// </summary>
    internal static class BitmapDataExt
    {
        /// <summary>
        /// Gets the number of bytes in an image.
        /// (Excluding encoding bytes)
        /// </summary>
        public static int GetByteCount(this BitmapData bitmapData)
        {
            return Math.Abs(bitmapData.Stride) * bitmapData.Height;
        }

        /// <summary>
        /// Gets pixel depth for image.
        /// </summary>
        public static int GetPixelDepth(this BitmapData bitmapData)
        {
            return Math.Max(1, Math.Abs(bitmapData.Stride) / bitmapData.Width);
        }

        /// <summary>
        /// Determines whether image is color.
        /// </summary>
        public static bool IsColor(this BitmapData bitmapData)
        {
            return IsColorPixelDepth(GetPixelDepth(bitmapData));
        }

        /// <summary>
        /// Determines whether pixel depth represents color.
        /// </summary>
        public static bool IsColorPixelDepth(int pixelDepth)
        {
            // RGB[A]
            return pixelDepth >= 3;
        }

        /// <summary>
        /// Determines a safe array limit for when looping image bytes.
        /// (Takes into account pixel depth)
        /// </summary>
        public static int GetSafeArrayLimitForImage(this BitmapData bitmapData, byte[] imageBytes)
        {
            bool isColor = bitmapData.IsColor();
            int pixelDepth = bitmapData.GetPixelDepth();

            return GetSafeArrayLimitForImage(isColor, pixelDepth, imageBytes);
        }

        /// <summary>
        /// Determines a safe array limit for when looping image bytes.
        /// (Takes into account pixel depth)
        /// </summary>
        public static int GetSafeArrayLimitForImage(bool isColor, int pixelDepth, byte[] imageBytes)
        {
            // A bitmap converted from a jpeg can potentially have an array with extra odd byte.
            return isColor ? imageBytes.Length - (pixelDepth - 1) : imageBytes.Length;
        }
    }
}

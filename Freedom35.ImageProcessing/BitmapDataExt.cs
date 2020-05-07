﻿using System.Drawing.Imaging;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class to extend BitmapData class.
    /// (BitmapData is sealed - cannot inherit)
    /// </summary>
    internal static class BitmapDataExt
    {
        /// <summary>
        /// Gets pixel depth for image.
        /// </summary>
        public static int GetPixelDepth(this BitmapData bitmapData)
        {
            return bitmapData.Stride / bitmapData.Width;
        }

        /// <summary>
        /// Gets the number of bytes in an image.
        /// (Excluding encoding bytes)
        /// </summary>
        public static int GetImageLength(this BitmapData bitmapData)
        {
            return bitmapData.Stride * bitmapData.Height;
        }

        /// <summary>
        /// Determines whether image is color.
        /// </summary>
        public static bool IsColor(this BitmapData bitmapData)
        {
            return GetPixelDepth(bitmapData) == 3;
        }
    }
}
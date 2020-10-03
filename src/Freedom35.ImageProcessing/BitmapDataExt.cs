﻿//------------------------------------------------
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
    }
}

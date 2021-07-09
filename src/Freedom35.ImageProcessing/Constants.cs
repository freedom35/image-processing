//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Internal image constants
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Number of bits in a byte.
        /// </summary>
        public const int BitsPerByte = 8;

        /// <summary>
        /// Binary value one.
        /// </summary>
        public const byte One = 0x01;

        /// <summary>
        /// Binary value zero.
        /// </summary>
        public const byte Zero = 0x00;

        /// <summary>
        /// Bytes for RGB color pixels.
        /// </summary>
        public const int PixelDepthRGB = 3;

        /// <summary>
        /// Bytes for RGBA - color pixels with a transparency layer.
        /// </summary>
        public const int PixelDepthRGBA = 4;
    }
}

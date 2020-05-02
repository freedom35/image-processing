using System;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Types of single static images.
    /// </summary>
    public enum ImageType
    {
        Unknown,

        /// <summary>
        /// Bit-mapped array of pixels
        /// (Uncompressed)
        /// </summary>
        Bitmap,

        /// <summary>
        /// Tagged Image File Format
        /// (Uncompressed)
        /// </summary>
        TIFF,

        /// <summary>
        /// Joint Photographic Experts Group
        /// (Compressed image - lossy)
        /// </summary>
        JPEG,

        /// <summary>
        /// Portable Network Graphics
        /// (Compressed image - loss-less)
        /// </summary>
        PNG
    }

    /// <summary>
    /// Extention methods for ImageType enum.
    /// </summary>
    public static class ImageTypeEnumExt
    {
        #region Encoding Definitions

        /// <summary>
        /// BMP - BM
        /// </summary>
        private static byte[] BitmapEncoding => new byte[] { 0x42, 0x4d };

        /// <summary>
        /// TIFF - II*
        /// </summary>
        private static byte[] TiffEncoding => new byte[] { 0x49, 0x49, 0x2a };

        /// <summary>
        /// JPEG - ......JFIF
        /// </summary>
        private static byte[] JpegEncoding => new byte[] { 0xff, 0xd8, 0xff, 0xf4, 0x00, 0x10, 0x4a, 0x46, 0x49, 0x46 };

        /// <summary>
        /// PNG - .PNG
        /// </summary>
        private static byte[] PngEncoding => new byte[] { 0x89, 0x50, 0x4e, 0x47 };

        #endregion

        /// <summary>
        /// Gets array of bytes to identify type of encoding method.
        /// </summary>
        /// <param name="type">Image type</param>
        /// <returns>Encoding bytes</returns>
        public static byte[] GetEncodingBytes(this ImageType type)
        {
            switch (type)
            {
                case ImageType.Bitmap:
                    return BitmapEncoding;

                case ImageType.TIFF:
                    return TiffEncoding;
                
                case ImageType.JPEG:
                    return JpegEncoding;

                case ImageType.PNG:
                    return PngEncoding;

                case ImageType.Unknown:
                default:
                    throw new NotImplementedException($"Encoding not implemented for '{type}'.");
            }
        }
    }
}

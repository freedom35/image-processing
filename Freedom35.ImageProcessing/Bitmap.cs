namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class representing a bitmap image.
    /// </summary>
    public sealed class Bitmap
    {
        /// <summary>
        /// Image width.
        /// </summary>
        public int Width
        {
            get;
            set;
        } = 0;

        /// <summary>
        /// Image height.
        /// </summary>
        public int Height
        {
            get;
            set;
        } = 0;

        /// <summary>
        /// Raw image bytes.
        /// </summary>
        public byte[] RawBytes
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Stride width (aka scan width).
        /// </summary>
        public int Stride
        {
            get;
            set;
        } = 0;

        /// <summary>
        /// Pixel depth of the image.
        /// (bytes used per pixel: 1 indicates grayscale, 3 indicates color)
        /// </summary>
        public int PixelDepth
        {
            get => Width > 0 ? (Stride / Width) : 0;
        }
    }
}

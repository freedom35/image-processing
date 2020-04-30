namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Interface for images that can be processed by this image processing lib.
    /// </summary>
    public interface IImage
    {
        /// <summary>
        /// Image width.
        /// </summary>
        int Width
        {
            get;
        }

        /// <summary>
        /// Image height.
        /// </summary>
        int Height
        {
            get;
        }

        /// <summary>
        /// Raw image bytes.
        /// </summary>
        byte[] ImageBytes
        {
            get;
        }
    }
}

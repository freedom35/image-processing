using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class for changing format of image.
    /// </summary>
    public sealed class ImageFormatting
    {
        /// <summary>
        /// Converts system image to bitmap.
        /// (Image processing performed on bytes in bitmap format)
        /// </summary>
        public static Bitmap ToBitmap(Image image)
        {
            return (Bitmap)ToFormat(image, ImageFormat.Bmp);
        }

        /// <summary>
        /// Converts system image format.
        /// </summary>
        public static Image ToFormat(Image image, ImageFormat targetFormat)
        {
            // Returning new image
            Image clone = (Image)image.Clone();

            // Check not already a Bitmap
            if (!image.RawFormat.Equals(targetFormat))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Save to stream in Bitmap format
                    image.Save(ms, targetFormat);

                    // Dispose of current image
                    //image.Dispose();

                    // Create new image
                    clone = Image.FromStream(ms);
                }
            }

            return clone;
        }
    }
}

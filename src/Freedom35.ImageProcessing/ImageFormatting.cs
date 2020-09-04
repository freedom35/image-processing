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
        /// </summary>
        /// <param name="image">Image to convert</param>
        /// <returns>New/Cloned bitmap</returns>
        public static Bitmap ToBitmap(Image image)
        {
            // Check if already a bitmap
            if (image is Bitmap bmp)
            {
                return (Bitmap)bmp.Clone();
            }
            else
            {
                return (Bitmap)ToFormat(image, ImageFormat.Bmp);
            }
        }

        /// <summary>
        /// Converts system image format.
        /// </summary>
        /// <param name="image">Image to convert</param>
        /// <param name="targetFormat">Target image format</param>
        /// <returns>New image in target format</returns>
        public static Image ToFormat(Image image, ImageFormat targetFormat)
        {
            // Returning new image
            Image clone;

            // Check not already a correct format
            if (!image.RawFormat.Equals(targetFormat))
            {
                // ** Do not dispose of stream **
                // (Or will cause issues for future image processing calls)
                MemoryStream ms = new MemoryStream();
                image.Save(ms, targetFormat);

                clone = Image.FromStream(ms);
            }
            else
            {
                clone = (Image)image.Clone();
            }

            return clone;
        }

        ///// <summary>
        ///// Compares image formats.
        ///// </summary>
        //private static bool AreSameFormat(ImageFormat format1, ImageFormat format2)
        //{
        //    return format1.Equals(format2)
        //        || format1.Equals(ImageFormat.Bmp) && format2.Equals(ImageFormat.MemoryBmp)
        //        || format1.Equals(ImageFormat.MemoryBmp) && format2.Equals(ImageFormat.Bmp);
        //}
    }
}

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class with methods relating to image bytes.
    /// </summary>
    public static class ImageBytes
    {
        /// <summary>
        /// Converts the image to a bitmap, and gets the bytes.
        /// </summary>
        /// <param name="image">Image to get bytes from</param>
        /// <returns>Image bytes</returns>
        public static byte[] Get(Image image)
        {
            return Get(ImageConvert.ImageToBitmap(image));
        }

        /// <summary>
        /// Gets the bytes from a bitmap image.
        /// </summary>
        /// <param name="bitmap">Bitmap to get bytes from</param>
        /// <returns>Image bytes</returns>
        public static byte[] Get(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Converts the image to a bitmap, and gets the bytes.
        /// </summary>
        /// <param name="image">Image to get bytes from</param>
        /// <param name="bmpData">Data relating to bitmap</param>
        /// <returns>Image bytes</returns>
        public static byte[] Get(Image image, out BitmapData bmpData)
        {
            return Get(ImageConvert.ImageToBitmap(image), out bmpData);
        }

        /// <summary>
        /// Gets the bytes from a bitmap image.
        /// </summary>
        /// <param name="bitmap">Bitmap to get bytes from</param>
        /// <param name="bmpData">Data relating to bitmap</param>
        /// <returns>Image bytes</returns>
        public static byte[] Get(Bitmap bitmap, out BitmapData bmpData)
        {
            // Lock full image
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Lock the bits while we read them.  
            bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            // Create length with number of bytes in image
            byte[] rgbValues = new byte[bmpData.Stride * bmpData.Height];

            // Copy the RGB values into the array.
            Marshal.Copy(bmpData.Scan0, rgbValues, 0, rgbValues.Length);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);

            return rgbValues;
        }
    }
}

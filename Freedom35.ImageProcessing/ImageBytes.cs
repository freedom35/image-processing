using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Freedom35.ImageProcessing
{
    public static class ImageBytes
    {
        /// <summary>
        /// Get the bytes from a bitmap image.
        /// </summary>
        public static byte[] Get(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                return stream.ToArray();
            }

            // Lock full image
            //Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            //// Lock the bits while we read them.  
            //bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            // Get number of bytes in image
            //int byteCount = (bmpData.Stride * bmpData.Height);

            //byte[] rgbValues = new byte[byteCount];

            //// Copy the RGB values into the array.
            //Marshal.Copy(bmpData.Scan0, rgbValues, 0, byteCount);

            //// Unlock the bits.
            //bitmap.UnlockBits(bmpData);

            //return rgbValues;
        }
    }
}

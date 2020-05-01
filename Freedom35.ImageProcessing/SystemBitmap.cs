using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class to handle System.Drawing.Bitmap related methods.
    /// </summary>
    internal static class SystemBitmap
    {
        /// <summary>
        /// Converts system image to bitmap.
        /// (Image processing performed on bytes in bitmap format)
        /// </summary>
        public static System.Drawing.Bitmap ConvertImageToBitmap(System.Drawing.Image image)
        {
            return (System.Drawing.Bitmap)ConvertImageFormat(image, ImageFormat.Bmp);
        }

        /// <summary>
        /// Converts system image format.
        /// </summary>
        public static System.Drawing.Image ConvertImageFormat(System.Drawing.Image image, ImageFormat targetFormat)
        {
            // Check not already a Bitmap
            if (!image.RawFormat.Equals(targetFormat))
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    // Save to stream in Bitmap format
                    image.Save(ms, targetFormat);

                    // Dispose of current image
                    image.Dispose();

                    // Create new image
                    image = System.Drawing.Image.FromStream(ms);
                }
            }

            return image;
        }

        /// <summary>
        /// Get the bytes from a bitmap image.
        /// </summary>
        public static byte[] GetBytes(System.Drawing.Bitmap bitmap)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
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

        /// <summary>
        /// Begin editing an image.
        /// </summary>
        public static byte[] BeginEdit(System.Drawing.Bitmap bitmap, out BitmapData bmpData)
        {
            // Lock full image
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Lock the bitmap's bits while we change them.  
            bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            
            // Get number of bytes in image
            int byteCount = (bmpData.Stride * bmpData.Height);

            byte[] rgbValues = new byte[byteCount];

            // Copy the RGB values into the array.
            Marshal.Copy(bmpData.Scan0, rgbValues, 0, byteCount);

            return rgbValues;
        }

        /// <summary>
        /// Finished editing an image.
        /// </summary>
        public static void EndEdit(System.Drawing.Bitmap bitmap, BitmapData bmpData, byte[] rgbValues)
        {
            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbValues, 0, bmpData.Scan0, rgbValues.Length);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);
        }
    }
}

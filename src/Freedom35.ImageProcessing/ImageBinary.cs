//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Converts image to binary/monochrome, 0's and 1's.
    /// </summary>
    public static class ImageBinary
    {
        /// <summary>
        /// Mid-point in threshold range.
        /// </summary>
        private const byte MidThreshold = 0x80;

        /// <summary>
        /// Image will be converted to binary, 0's and 1's.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <returns>New image as binary</returns>
        public static T AsImage<T>(T image) where T : Image
        {
            return AsImage(image, MidThreshold);
        }

        /// <summary>
        /// Any pixel values below threshold will be changed to 0.
        /// Any pixel values above (or equal to) threshold will be changed to 1.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to process</param>
        /// <param name="threshold">Binary threshold</param>
        /// <returns>New image as binary</returns>
        public static T AsImage<T>(T image, byte threshold) where T : Image
        {
            Bitmap binaryBitmap = AsBitmap(image, threshold);

            // Convert to original format
            return (T)ImageFormatting.ToFormat(binaryBitmap, image.RawFormat);
        }

        /// <summary>
        /// Image will be converted to binary, 0's and 1's.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <returns>New bitmap as binary</returns>
        public static Bitmap AsBitmap(Image image)
        {
            return AsBitmap(image, MidThreshold);
        }

        /// <summary>
        /// Any pixel values below threshold will be changed to 0.
        /// Any pixel values above (or equal to) threshold will be changed to 1.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <param name="threshold">Binary threshold</param>
        /// <returns>New bitmap as binary</returns>
        public static Bitmap AsBitmap(Image image, byte threshold)
        {
            byte[] binaryBytes = AsBytes(image, threshold);

            // Retain original size
            Bitmap binaryBitmap = new Bitmap(image.Width, image.Height);

            // Retain original size
            Rectangle rect = new Rectangle(0, 0, binaryBitmap.Width, binaryBitmap.Height);

            // Lock the bitmap's bits while we change them.  
            BitmapData bmpData = binaryBitmap.LockBits(rect, ImageLockMode.WriteOnly, binaryBitmap.PixelFormat);

            // Set 1 bit per pixel
            bmpData.PixelFormat = PixelFormat.Format1bppIndexed;
            bmpData.Stride = 1;

            // Copy the binary values to the bitmap
            Marshal.Copy(binaryBytes, 0, bmpData.Scan0, binaryBytes.Length);

            // Unlock the bits.
            binaryBitmap.UnlockBits(bmpData);

            return binaryBitmap;
        }

        /// <summary>
        /// Converts image to byte array of 0's and 1's.
        /// </summary>
        /// <param name="image">Image to convert</param>
        /// <param name="threshold">Binary threshold</param>
        /// <returns>byte array of 0's and 1's</returns>
        public static byte[] AsBytes(Image image, byte threshold)
        {
            byte[] imageBytes = ImageBytes.FromImage(image, out BitmapData bmpData);

            // If original image color, now only require 1 byte per image
            byte[] binaryBytes;

            if (bmpData.IsColor())
            {
                // If original image color, now only require 1 byte per image
                binaryBytes = new byte[bmpData.Width * bmpData.Height];

                int pixelDepth = bmpData.GetPixelDepth();

                int stride = bmpData.Stride;
                int width = bmpData.GetStrideWithoutPadding();
                int height = bmpData.Height;
                int limit = bmpData.GetSafeArrayLimitForImage(imageBytes);
                int index = 0;
                
                // Get 0 or 1 for each pixel
                for (int y = 0; y < height; y++)
                {
                    // Images may have extra bytes per row to pad for CPU addressing.
                    // so need to ensure we traverse to the correct byte when moving between rows.
                    // I.e. row width not divisible by 3
                    int offset = y * stride;
                    
                    for (int x = 0; x < width; x += pixelDepth)
                    {
                        int i = offset + x;

                        if (i < limit && index < binaryBytes.Length)
                        {
                            int sum = 0;

                            // Check if any component has value
                            for (int j = 0; j < Constants.PixelDepthRGB && i + j < imageBytes.Length; j++)
                            {
                                sum += imageBytes[i + j];
                            }

                            // Set binary value
                            binaryBytes[index++] = (sum / Constants.PixelDepthRGB) < threshold ? Constants.Zero : Constants.One;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                // Grayscale
                binaryBytes = new byte[imageBytes.Length];

                // Get 0 or 1 for each pixel
                for (int i = 0; i < binaryBytes.Length; i++)
                {
                    // Set binary value
                    binaryBytes[i] = imageBytes[i] < threshold ? Constants.Zero : Constants.One;
                }
            }

            return binaryBytes;
        }
    }
}

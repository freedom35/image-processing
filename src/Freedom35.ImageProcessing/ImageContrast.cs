//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System.Drawing;
using System.Drawing.Imaging;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Methods relating to image contrast.
    /// </summary>
    public static class ImageContrast
    {
        /// <summary>
        /// Stretches image contrast to get maximum contrast.
        /// If image has unused lower/upper range, values can be stretched to use all available range, improving image contrast.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <returns>Contrast-stretched image</returns>
        public static T Stretch<T>(T image) where T: Image
        {
            return Stretch(image, byte.MinValue, byte.MaxValue);
        }

        /// <summary>
        /// Stretches image contrast to specified min/max values.
        /// </summary>
        /// <param name="image">Image to process</param>
        /// <param name="min">Minimum contrast value</param>
        /// <param name="max">Maximum contrast value</param>
        /// <returns>Contrast-stretched image</returns>
        public static T Stretch<T>(T image, byte min, byte max) where T : Image
        {
            if (image is Bitmap bmp)
            {
                // Return new image
                Bitmap clone = (Bitmap)bmp.Clone();

                StretchDirect(ref clone, min, max);

                return (T)(Image)clone;
            }
            else
            {
                // Convert to bitmap format to perform stretch
                Bitmap bitmap = ImageFormatting.ToBitmap(image);
                
                StretchDirect(ref bitmap, min, max);

                // Restore original image format
                Image stretchedImage = ImageFormatting.ToFormat(bitmap, image.RawFormat);

                // Dispose of temp bitmap
                bitmap.Dispose();

                return (T)stretchedImage;
            }
        }

        /// <summary>
        /// Stretches image contrast to specified min/max values.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="min">Minimum contrast value</param>
        /// <param name="max">Maximum contrast value</param>
        public static void StretchDirect(ref Bitmap bitmap, byte min, byte max)
        {
            // Lock image for processing
            byte[] imageBytes = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            StretchDirect(imageBytes, bmpData, min, max);

            // Copy modified array back to image, and release lock
            ImageEdit.End(bitmap, bmpData, imageBytes);
        }

        /// <summary>
        /// Stretches image contrast to specified min/max values.
        /// </summary>
        /// <param name="imageBytes">Image bytes (Grayscale/RGB)</param>
        /// <param name="bmpData">Info on image properties</param>
        /// <param name="min">Minimum contrast value</param>
        /// <param name="max">Maximum contrast value</param>
        public static void StretchDirect(byte[] imageBytes, BitmapData bmpData, byte min, byte max)
        {
            int pixelDepth = bmpData.GetPixelDepth();
            bool isColor = bmpData.IsColor();

            // Initialize with opposite values
            byte highest = byte.MinValue;
            byte lowest = byte.MaxValue;
            byte val;

            // Bitmap converted from jpeg can potentially can potentially have array with extra odd byte.
            int limit = (isColor ? imageBytes.Length - (pixelDepth - 1) : imageBytes.Length);

            //////////////////////////////////////
            // First find current contrast range
            //////////////////////////////////////
            for (int i = 0; i < limit; i += pixelDepth)
            {
                if (isColor)
                {
                    val = (byte)((imageBytes[i] + imageBytes[i + 1] + imageBytes[i + 2]) / 3);
                }
                else
                {
                    val = imageBytes[i];
                }

                // Check contrast ranges
                if (val < lowest)
                {
                    lowest = val;
                }
                else if (val > highest)
                {
                    highest = val;
                }
            }

            //////////////////////////////////////
            // Now contrast stretch image
            //////////////////////////////////////
            for (int i = 0; i < limit; i += pixelDepth)
            {
                for (int j = 0; j < pixelDepth; j++)
                {
                    // Contrast-stretch value
                    val = (byte)((imageBytes[i + j] - lowest) * (max / (highest - lowest)));

                    // Check limits
                    if (val < lowest)
                    {
                        val = min;
                    }
                    else if (val > highest)
                    {
                        val = max;
                    }

                    imageBytes[i + j] = val;
                }
            }
        }
    }
}

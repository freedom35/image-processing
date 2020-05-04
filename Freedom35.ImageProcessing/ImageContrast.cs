using System;
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
        public static Image Stretch(Image image)
        {
            return Stretch(image, byte.MinValue, byte.MaxValue);
        }

        /// <summary>
        /// Stretches image contrast to get maximum contrast.
        /// If image has unused lower/upper range, values can be stretched to use all available range, improving image contrast.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <returns>Contrast-stretched image</returns>
        public static Bitmap Stretch(Bitmap bitmap)
        {
            return Stretch(bitmap, byte.MinValue, byte.MaxValue);
        }

        /// <summary>
        /// Stretches image contrast to specified min/max values.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="min">Minimum contrast value</param>
        /// <param name="max">Maximum contrast value</param>
        /// <returns>Contrast-stretched image</returns>
        public static Image Stretch(Image image, byte min, byte max)
        {
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            Bitmap stretchedBitmap = Stretch(bitmap, min, max);

            // Restore original image format
            return ImageFormatting.ToFormat(stretchedBitmap, image.RawFormat);
        }

        /// <summary>
        /// Stretches image contrast to specified min/max values.
        /// </summary>
        /// <param name="bitmap">Image to process</param>
        /// <param name="min">Minimum contrast value</param>
        /// <param name="max">Maximum contrast value</param>
        /// <returns>Contrast-stretched image</returns>
        public static Bitmap Stretch(Bitmap bitmap, byte min, byte max)
        {
            // Return new image
            Bitmap clone = (Bitmap)bitmap.Clone();

            byte[] rgbValues = ImageEdit.Begin(clone, out BitmapData bmpData);

            int pixelDepth = bmpData.GetPixelDepth();
            bool isColor = bmpData.IsColor();

            // Initialize with opposite values
            byte highest = byte.MinValue;
            byte lowest = byte.MaxValue;
            byte val;

            // Bitmap converted from jpeg can potentially can potentially have array with extra odd byte.
            int limit = (isColor ? rgbValues.Length - (pixelDepth - 1) : rgbValues.Length);

            //////////////////////////////////////
            // First find current contrast range
            //////////////////////////////////////
            for (int i = 0; i < limit; i += pixelDepth)
            {
                if (isColor)
                {
                    val = (byte)((rgbValues[i] + rgbValues[i + 1] + rgbValues[i + 2]) / 3);
                }
                else
                {
                    val = rgbValues[i];
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
                    val = (byte)((rgbValues[i + j] - lowest) * (max / (highest - lowest)));

                    // Check limits
                    if (val < lowest)
                    {
                        val = min;
                    }
                    else if (val > highest)
                    {
                        val = max;
                    }

                    rgbValues[i + j] = val;
                }
            }

            // Overwrite cloned image with stretched contrast
            ImageEdit.End(clone, bmpData, rgbValues);

            return clone;
        }
    }
}

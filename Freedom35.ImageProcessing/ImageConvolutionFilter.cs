using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace Freedom35.ImageProcessing
{
    public static class ImageConvolutionFilter
    {
        public static Image ApplyMultiple(Image image, params ConvolutionFilterType[] filterTypes)
        {
            // Remember original image type
            ImageFormat originalFormat = image.RawFormat;

            // Convert to bitmap for image processing
            Bitmap bitmap = ImageConvert.ImageToBitmap(image);

            Bitmap combinedBitmap = ApplyMultiple(bitmap, filterTypes);

            // Return processed image to original format
            return ImageConvert.ImageToFormat(combinedBitmap, originalFormat);
        }

        public static Bitmap ApplyMultiple(Bitmap bitmap, params ConvolutionFilterType[] filterTypes)
        {
            List<Bitmap> bitmaps = new List<Bitmap>();
            
            // Create new image for each filter
            foreach (ConvolutionFilterType filterType in filterTypes)
            {
                Bitmap bmp = Apply(bitmap, filterType);
                bitmaps.Add(bmp);
            }

            // Combine to create a new image
            bitmap = ImageCombine.All(bitmaps);

            // No longer needed
            foreach (Bitmap bmp in bitmaps)
            {
                bmp.Dispose();
            }

            return bitmap;
        }

        public static Image Apply(Image image, ConvolutionFilterType filterType)
        {
            // Remember original image type
            ImageFormat originalFormat = image.RawFormat;

            // Convert to bitmap for image processing
            Bitmap bitmap = ImageConvert.ImageToBitmap(image);

            // Apply filter to bitmap
            Bitmap bitmapWithFilter = Apply(bitmap, filterType);

            // Covert processed image to original format
            return ImageConvert.ImageToFormat(bitmapWithFilter, originalFormat);
        }

        /// <summary>
        /// Applies convolution filter to image.
        /// i.e. edge detection.
        /// </summary>
        /// <param name="bitmap">Image to apply filter to.</param>
        /// <param name="filterType">Type of filter.</param>
        public static Bitmap Apply(Bitmap bitmap, ConvolutionFilterType filterType)
        {
            // Returning new image
            Bitmap clone = (Bitmap)bitmap.Clone();

            // Lock image for processing
            byte[] rgbValues = ImageEdit.Begin(clone, out BitmapData bmpData);

            // Get convolution filter to apply
            int[,] template = filterType.GetTemplate();

            int templateLen = template.GetLength(0);
            int pixelDepth = (bmpData.Stride / bmpData.Width);
            
            // Move through image
            for (int bmpX = 0; bmpX < bmpData.Width; bmpX++)
            {
                for (int bmpY = 0; bmpY < bmpData.Height; bmpY++)
                {
                    int tmp = 0;

                    for (int templateX = 0; templateX < templateLen; templateX++)
                    {
                        for (int templateY = 0; templateY < templateLen; templateY++)
                        {
                            // Image coordinates with respect to template.
                            int iX = bmpX + templateX;
                            int iY = bmpY + templateY;

                            if (iX < bmpData.Width && iY < bmpData.Height)
                            {
                                // Apply filter
                                tmp += template[templateX, templateY] * rgbValues[(iX * pixelDepth) + (iY * bmpData.Stride)];
                            }
                        }
                    }

                    // Check value within range
                    if (tmp < 0)
                    {
                        tmp = 0;
                    }
                    else if (tmp > 255)
                    {
                        tmp = 255;
                    }

                    // Get current pixel
                    int pixelIndex = (bmpX * pixelDepth) + (bmpY * bmpData.Stride);

                    // Update pixel value
                    rgbValues[pixelIndex] = (byte)tmp;

                    // 3 bits per pixel on colour image (RGB)
                    if (pixelDepth == 3)
                    {
                        rgbValues[pixelIndex + 1] = rgbValues[pixelIndex];
                        rgbValues[pixelIndex + 2] = rgbValues[pixelIndex];
                    }
                }
            }

            // Copy modified array back to image, and release lock
            ImageEdit.End(clone, bmpData, rgbValues);

            // Return new instance
            return clone;
        }
    }
}

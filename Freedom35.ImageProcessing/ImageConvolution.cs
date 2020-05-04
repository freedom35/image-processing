using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class to apply convolution kernels to an image.
    /// </summary>
    public static class ImageConvolution
    {
        /// <summary>
        /// Applies multiple convolution kernels to bitmap.
        /// </summary>
        /// <param name="bitmap">Image to apply kernel to</param>
        /// <param name="convolutionTypes">Convolutions to apply</param>
        /// <returns>Image with all kernels applied</returns>
        public static Image ApplyKernels(Image image, params ConvolutionType[] convolutionTypes)
        {
            // Remember original image type
            ImageFormat originalFormat = image.RawFormat;

            // Convert to bitmap for image processing
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            Bitmap combinedBitmap = ApplyKernels(bitmap, convolutionTypes);

            // Return processed image to original format
            return ImageFormatting.ToFormat(combinedBitmap, originalFormat);
        }

        /// <summary>
        /// Applies multiple convolution kernels to bitmap.
        /// </summary>
        /// <param name="bitmap">Bitmap to apply kernel to</param>
        /// <param name="convolutionTypes">Convolutions to apply</param>
        /// <returns>Bitmap with all kernels applied</returns>
        public static Bitmap ApplyKernels(Bitmap bitmap, params ConvolutionType[] convolutionTypes)
        {
            List<Bitmap> bitmaps = new List<Bitmap>();

            // Create new image for each filter
            foreach (ConvolutionType type in convolutionTypes)
            {
                Bitmap bmp = ApplyKernel(bitmap, type);
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

        /// <summary>
        /// Applies convolution matrix/kernel to image.
        /// </summary>
        /// <param name="image">Image to apply kernel to</param>
        /// <param name="convolutionType">Type of convolution</param>
        /// <returns>Image with kernel applied</returns>
        public static Image ApplyKernel(Image image, ConvolutionType convolutionType)
        {
            return ApplyKernel(image, convolutionType.GetConvolutionMatrix());
        }

        /// <summary>
        /// Applies convolution matrix/kernel to image.
        /// </summary>
        /// <param name="image">Image to apply kernel to</param>
        /// <param name="kernelMatrix">Kernel matrix</param>
        /// <returns>Image with kernel applied</returns>
        public static Image ApplyKernel(Image image, int[,] kernelMatrix)
        {
            // Remember original image type
            ImageFormat originalFormat = image.RawFormat;

            // Convert to bitmap for image processing
            Bitmap bitmap = ImageFormatting.ToBitmap(image);

            // Apply filter to bitmap
            Bitmap bitmapWithFilter = ApplyKernel(bitmap, kernelMatrix);

            // Covert processed image to original format
            return ImageFormatting.ToFormat(bitmapWithFilter, originalFormat);
        }

        /// <summary>
        /// Applies convolution matrix/kernel to image.
        /// i.e. edge detection.
        /// </summary>
        /// <param name="bitmap">Image to apply kernel to</param>
        /// <param name="convolutionType">Type of convolution</param>
        /// <returns>Bitmap with kernel applied</returns>
        public static Bitmap ApplyKernel(Bitmap bitmap, ConvolutionType convolutionType)
        {
            // Get convolution kernel to apply
            int[,] matrix = convolutionType.GetConvolutionMatrix();

            return ApplyKernel(bitmap, matrix);
        }

        /// <summary>
        /// Applies convolution matrix/kernel to image.
        /// i.e. edge detection.
        /// </summary>
        /// <param name="bitmap">Image to apply kernel to</param>
        /// <param name="kernelMatrix">Kernel matrix</param>
        /// <returns>Bitmap with kernel applied</returns>
        public static Bitmap ApplyKernel(Bitmap bitmap, int[,] kernelMatrix)
        {
            // Returning new image
            Bitmap clone = (Bitmap)bitmap.Clone();

            // Lock image for processing
            byte[] rgbValues = ImageEdit.BeginWrite(clone, out BitmapData bmpData);

            // Matrix is typically square, but may not be
            int matrixLenX = kernelMatrix.GetLength(0);
            int matrixLenY = kernelMatrix.GetLength(1);

            // Get bitmap info
            int bmpWidth = bmpData.Width;
            int bmpHeight = bmpData.Height;
            int bmpStride = bmpData.Stride;
            int pixelDepth = (bmpStride / bmpWidth);

            // Re-used often
            int bmpX, bmpY, mX, mY, iX, iY;
            int newValue;
            int pixelIndex;

            // Move through image
            for (bmpX = 0; bmpX < bmpWidth; bmpX++)
            {
                for (bmpY = 0; bmpY < bmpHeight; bmpY++)
                {
                    newValue = 0;

                    // Move through matrix to calculate new value
                    for (mX = 0; mX < matrixLenX; mX++)
                    {
                        // Image X coordinate with respect to matrix.
                        iX = bmpX + mX;

                        // Check within image
                        if (iX < bmpWidth)
                        {
                            for (mY = 0; mY < matrixLenY; mY++)
                            {
                                // Image Y coordinate with respect to matrix.
                                iY = bmpY + mY;

                                // Check within image
                                if (iY < bmpHeight)
                                {
                                    // Add convolution value
                                    newValue += kernelMatrix[mX, mY] * rgbValues[(iX * pixelDepth) + (iY * bmpStride)];
                                }
                                else
                                {
                                    // Outside of image bounds
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // Outside of image bounds
                            break;
                        }
                    }

                    // Check value within range
                    // (Some filter values are negative)
                    if (newValue < 0)
                    {
                        newValue = 0;
                    }
                    else if (newValue > byte.MaxValue)
                    {
                        // Value oversaturated
                        newValue = byte.MaxValue;
                    }

                    // Get current pixel
                    pixelIndex = (bmpX * pixelDepth) + (bmpY * bmpStride);

                    // Update pixel value
                    rgbValues[pixelIndex] = (byte)newValue;

                    // 3 bits per pixel on color image (RGB)
                    if (pixelDepth == 3)
                    {
                        rgbValues[pixelIndex + 1] = rgbValues[pixelIndex];
                        rgbValues[pixelIndex + 2] = rgbValues[pixelIndex];
                    }
                }
            }

            // Copy modified array back to image, and release lock
            ImageEdit.EndWrite(clone, bmpData, rgbValues);

            // Return new instance
            return clone;
        }
    }
}

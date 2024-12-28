//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
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
        /// Applies multiple convolution kernels in sequence to image.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to apply kernel to</param>
        /// <param name="convolutionTypes">Convolutions to apply</param>
        /// <returns>Image with all kernels applied in sequence</returns>
        public static T ApplyKernelsInSequence<T>(T image, params ConvolutionType[] convolutionTypes) where T : Image
        {
            if (image is Bitmap bmp)
            {
                return (T)(Image)ApplyKernelsInSequence(bmp, convolutionTypes);
            }
            else
            {
                // Convert to bitmap for image processing
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    using (Bitmap combinedBitmap = ApplyKernelsInSequence(bitmap, convolutionTypes))
                    {
                        // Return processed image to original format
                        return (T)ImageFormatting.ToFormat(combinedBitmap, image.RawFormat);
                    }
                }
            }
        }

        /// <summary>
        /// Applies multiple convolution kernels in sequence to bitmap.
        /// </summary>
        /// <param name="bitmap">Bitmap to apply kernel to</param>
        /// <param name="convolutionTypes">Convolutions to apply</param>
        /// <returns>Bitmap with all kernels applied in sequence</returns>
        public static Bitmap ApplyKernelsInSequence(Bitmap bitmap, params ConvolutionType[] convolutionTypes)
        {
            Bitmap kernelBitmap = bitmap;

            // Apply kernels in sequence
            for (int i = 0; i < convolutionTypes.Length; i++)
            {
                Bitmap bmpTmp = ApplyKernel(kernelBitmap, convolutionTypes[i]);
                
                // Don't dispose of original image
                if (i > 0)
                {
                    // No longer needed
                    kernelBitmap.Dispose();
                }

                // Apply next kernel to new image
                kernelBitmap = bmpTmp;
            }

            // Return bitmap with kernels applied
            return kernelBitmap;
        }

        /// <summary>
        /// Applies multiple convolution kernels to source image, then combines the results.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to apply kernel to</param>
        /// <param name="convolutionTypes">Convolutions to apply</param>
        /// <returns>Image with all kernels combined</returns>
        public static T ApplyKernelsThenCombine<T>(T image, params ConvolutionType[] convolutionTypes) where T : Image
        {
            if (image is Bitmap bmp)
            {
                return (T)(Image)ApplyKernelsThenCombine(bmp, convolutionTypes);
            }
            else
            {
                // Convert to bitmap for image processing
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    using (Bitmap combinedBitmap = ApplyKernelsThenCombine(bitmap, convolutionTypes))
                    {
                        // Return processed image to original format
                        return (T)ImageFormatting.ToFormat(combinedBitmap, image.RawFormat);
                    }
                }
            }
        }

        /// <summary>
        /// Applies multiple convolution kernels to source bitmap, then combines the results.
        /// </summary>
        /// <param name="bitmap">Bitmap to apply kernel to</param>
        /// <param name="convolutionTypes">Convolutions to apply</param>
        /// <returns>Bitmap with all kernels combined</returns>
        public static Bitmap ApplyKernelsThenCombine(Bitmap bitmap, params ConvolutionType[] convolutionTypes)
        {
            List<Bitmap> bitmaps = new List<Bitmap>();

            // Appky each kernel to the original image
            foreach (ConvolutionType type in convolutionTypes)
            {
                bitmaps.Add(ApplyKernel(bitmap, type));
            }

            // Combine to create a new image
            Bitmap combinedBitmap = ImageCombine.All(bitmaps);

            // No longer needed
            foreach (Bitmap bmp in bitmaps)
            {
                bmp.Dispose();
            }

            return combinedBitmap;
        }

        /// <summary>
        /// Applies convolution matrix/kernel to image.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to apply kernel to</param>
        /// <param name="convolutionType">Type of convolution</param>
        /// <returns>Image with kernel applied</returns>
        public static T ApplyKernel<T>(T image, ConvolutionType convolutionType) where T : Image
        {
            return ApplyKernel(image, convolutionType.GetConvolutionMatrix());
        }

        /// <summary>
        /// Applies convolution matrix/kernel to image.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to apply kernel to</param>
        /// <param name="kernelMatrix">Kernel matrix</param>
        /// <returns>Image with kernel applied</returns>
        public static T ApplyKernel<T>(T image, int[,] kernelMatrix) where T : Image
        {
            // Skip conversion
            if (image is Bitmap bmp)
            {
                return (T)(Image)ApplyKernel(bmp, kernelMatrix);
            }
            else
            {
                // Convert to bitmap for image processing
                using (Bitmap bitmap = ImageFormatting.ToBitmap(image))
                {
                    // Apply filter to bitmap
                    using (Bitmap bitmapWithFilter = ApplyKernel(bitmap, kernelMatrix))
                    {
                        // Covert processed image to original format
                        return (T)ImageFormatting.ToFormat(bitmapWithFilter, image.RawFormat);
                    }
                }
            }
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
            // Returning new instance
            Bitmap clone = (Bitmap)bitmap.Clone();

            // Apply kernel to clone
            ApplyKernelDirect(ref clone, kernelMatrix);

            return clone;
        }

        /// <summary>
        /// Applies convolution matrix/kernel to image.
        /// i.e. edge detection.
        /// </summary>
        /// <param name="bitmap">Image to apply kernel to</param>
        /// <param name="kernelMatrix">Kernel matrix</param>
        public static void ApplyKernelDirect(ref Bitmap bitmap, int[,] kernelMatrix)
        {
            // Lock image for processing
            byte[] rgbValues = ImageEdit.Begin(bitmap, out BitmapData bmpData);

            rgbValues = ApplyKernel(rgbValues, bmpData, kernelMatrix);

            // Copy modified array back to image, and release lock
            ImageEdit.End(bitmap, bmpData, rgbValues);
        }

        /// <summary>
        /// Applies convolution matrix/kernel to image.
        /// i.e. edge detection.
        /// </summary>
        /// <param name="imageBytes">Image bytes (Grayscale/RGB)</param>
        /// <param name="bitmapData">Info on image properties</param>
        /// <param name="kernelMatrix">Kernel matrix</param>
        /// <returns>byte array with kernel applied</returns>
        public static byte[] ApplyKernel(byte[] imageBytes, BitmapData bitmapData, int[,] kernelMatrix)
        {
            // Matrix is typically square, but may not be
            int matrixLenX = kernelMatrix.GetLength(0);
            int matrixLenY = kernelMatrix.GetLength(1);

            // Position in matrix where convolution value assigned
            // (Use center for 3x3 etc.)
            int matrixValX = (matrixLenX - 1) / 2;
            int matrixValY = (matrixLenY - 1) / 2;

            // Get bitmap info
            int bmpWidth = bitmapData.Width;
            int bmpHeight = bitmapData.Height;
            int bmpStride = Math.Abs(bitmapData.Stride);
            int pixelDepth = bitmapData.GetPixelDepth();
            bool isColor = bitmapData.IsColor();

            // Reused often
            int bmpX, bmpY, mX, mY, iX, iY;
            int newValue;
            int pixelIndex;
            int iYbyStride;
            
            // 3 bits per pixel on color image (RGB)
            int maxChannels = isColor ? 3 : 1;

            // Create separate array so previous convolution values
            // don't interfere with later values as traverse image
            byte[] results = new byte[imageBytes.Length];

            // Perform convolution for each color channel
            for (int colorChannel = 0; colorChannel < maxChannels; colorChannel++)
            {
                // Move through rows
                for (bmpY = 0; bmpY < bmpHeight; bmpY++)
                {
                    // Move through columns
                    for (bmpX = 0; bmpX < bmpWidth; bmpX++)
                    {
                        // Check if edge case
                        if ((bmpY + matrixLenY > bmpHeight) || (bmpX + matrixLenX > bmpWidth))
                        {
                            // Assign non-value, black
                            newValue = 0;

                            // Get target pixel for value
                            pixelIndex = (bmpX * pixelDepth) + (bmpY * bmpStride);
                        }
                        else
                        {
                            // Reset
                            newValue = 0;

                            // Move through matrix to calculate new value
                            for (mY = 0; mY < matrixLenY; mY++)
                            {
                                // Image Y coordinate with respect to matrix.
                                iY = bmpY + mY;

                                iYbyStride = iY * bmpStride;

                                for (mX = 0; mX < matrixLenX; mX++)
                                {
                                    // Image X coordinate with respect to matrix.
                                    iX = bmpX + mX;

                                    // Get R, G, or B index
                                    pixelIndex = (iX * pixelDepth) + iYbyStride + colorChannel;

                                    // Add convolution value for pixel
                                    newValue += kernelMatrix[mX, mY] * imageBytes[pixelIndex];
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

                            // Get target pixel for value
                            pixelIndex = (bmpX + matrixValX) * pixelDepth;
                            pixelIndex += (bmpY + matrixValY) * bmpStride;
                        }

                        // Adjust for color channel
                        pixelIndex += colorChannel;

                        // Update pixel value
                        results[pixelIndex] = (byte)newValue;
                    }
                }
            }

            // Return new values
            return results;
        }
    }
}

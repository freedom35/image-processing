//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class for changing format of image.
    /// </summary>
    public static class ImageFormatting
    {
        /// <summary>
        /// Returns copy of source image as a bitmap.
        /// </summary>
        /// <param name="sourceImage">Source image to copy</param>
        /// <returns>New bitmap</returns>
        public static Bitmap ToBitmap(Image sourceImage)
        {
            // Check if already a bitmap
            if (sourceImage is Bitmap bmp)
            {
                return (Bitmap)bmp.Clone();
            }
            else
            {
                return (Bitmap)ToFormat(sourceImage, ImageFormat.Bmp);
            }
        }

        /// <summary>
        /// Returns copy of source image in the target format.
        /// </summary>
        /// <param name="sourceImage">Source image to copy</param>
        /// <param name="targetFormat">Target image format</param>
        /// <returns>New image in target format</returns>
        public static Image ToFormat(Image sourceImage, ImageFormat targetFormat)
        {
            // Returning new image
            Image clone;
            
            // Check not already a correct format
            if (!sourceImage.RawFormat.Equals(targetFormat))
            {
                // ** Do not dispose of stream **
                // (Or will cause issues for future image processing calls)
                MemoryStream ms = new MemoryStream();
                sourceImage.Save(ms, targetFormat);

                clone = Image.FromStream(ms);
            }
            else
            {
                clone = (Image)sourceImage.Clone();
            }

            return clone;
        }

        /// <summary>
        /// Converts source image to target format.
        /// Source image will be disposed of.
        /// </summary>
        /// <param name="sourceImage">Source image to convert</param>
        /// <param name="targetFormat">Target image format</param>
        /// <returns>Image in target format</returns>
        public static Image Convert(Image sourceImage, ImageFormat targetFormat)
        {
            // Check if already in target format
            if (!sourceImage.RawFormat.Equals(targetFormat))
            {
                // Create image in new format
                Image targetFormatImage = ToFormat(sourceImage, targetFormat);

                // Release source image
                sourceImage.Dispose();

                return targetFormatImage;
            }
            else
            {
                // Correct format, just return
                return sourceImage;
            }
        }

        /// <summary>
        /// Converts image to a JPEG.
        /// </summary>
        /// <param name="image">Image to convert</param>
        /// <param name="compressionLevel">Level of compression: 0-100 (0 - max compression, lowest quality, 100 - minimal compression, max quality)</param>
        /// <returns>Compressed image (JPEG)</returns>
        public static Image ToJPEG(Image image, long compressionLevel)
        {
            // Check compression limits
            if (compressionLevel < 0 || compressionLevel > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(compressionLevel), $"Invalid JPEG compression level ({compressionLevel}), value should be within 0 to 100.");
            }

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            // Find JPEG codec
            ImageCodecInfo jpegEncoder = codecs.FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid) ?? throw new Exception("JPEG image encoder not found.");

            // Create compression parameters
            EncoderParameters encParams = new EncoderParameters(1);
            encParams.Param[0] = new EncoderParameter(Encoder.Quality, compressionLevel);

            // Leave stream open so image can be saved
            MemoryStream ms = new MemoryStream();
            
            // Create JPEG in stream
            image.Save(ms, jpegEncoder, encParams);

            return Image.FromStream(ms);
        }
    }
}

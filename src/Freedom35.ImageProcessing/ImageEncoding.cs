//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class containing definitions for image encoding.
    /// </summary>
    public static class ImageEncoding
    {
        /// <summary>
        /// Determines image type based on header bytes.
        /// </summary>
        /// <param name="imageBytes">Image bytes</param>
        /// <param name="imageType">Type of image determined</param>
        /// <returns>True if image type could be determined</returns>
        public static bool TryGetImageType(byte[] imageBytes, out ImageType imageType)
        {
            imageType = ImageType.Unknown;

            // Get image types to compare
            IEnumerable<ImageType> imageTypes = Enum.GetValues(typeof(ImageType)).Cast<ImageType>().Where(t => t != ImageType.Unknown);

            // Decode image type in buffer
            foreach (ImageType type in imageTypes)
            {
                byte[] encodingBytes = type.GetEncodingBytes();

                if (IsImageType(imageBytes, encodingBytes))
                {
                    imageType = type;
                    break;
                }
            }

            return imageType != ImageType.Unknown;
        }

        /// <summary>
        /// Determines if image matches the encoding type.
        /// </summary>
        /// <param name="imageBytes">Image bytes</param>
        /// <param name="encodingBytes">Encoding bytes for comparison</param>
        /// <returns>True if image header matches encoding bytes</returns>
        public static bool IsImageType(byte[] imageBytes, byte[] encodingBytes)
        {
            int i;

            // Compare image bytes to encoding
            for (i = 0; i < encodingBytes.Length && i < imageBytes.Length; i++)
            {
                if (imageBytes[i] != encodingBytes[i])
                {
                    break;
                }
            }

            // Check if all encoding bytes matched buffer
            return (i == encodingBytes.Length);
        }
    }
}

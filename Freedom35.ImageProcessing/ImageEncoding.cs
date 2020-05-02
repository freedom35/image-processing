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
        //private static ImageType[] GetEncodingDefinitions()
        //{
        //    return Enum.GetValues(typeof(ImageType)).OfType<ImageType>().Where(t => t != ImageType.Unknown).ToArray();
        //}

        //private static readonly ImageType[] imageTypes = Enum.GetValues(typeof(ImageType)).Cast<ImageType>().Where(t => t != ImageType.Unknown).ToArray();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="imageType"></param>
        /// <returns></returns>
        public static bool TryGetImageType(byte[] buffer, out ImageType imageType)
        {
            imageType = ImageType.Unknown;

            // Get image types to compare
            IEnumerable<ImageType> imageTypes = Enum.GetValues(typeof(ImageType)).Cast<ImageType>().Where(t => t != ImageType.Unknown);

            // Decode image type in buffer
            foreach (ImageType type in imageTypes)
            {
                byte[] encodingBytes = type.GetEncodingBytes();

                if (IsImageType(buffer, encodingBytes))
                {
                    imageType = type;
                    break;
                }
            }

            return imageType != ImageType.Unknown;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageBuffer"></param>
        /// <param name="encodingBytes"></param>
        /// <returns></returns>
        public static bool IsImageType(byte[] imageBuffer, byte[] encodingBytes)
        {
            int i;

            // Compare image bytes to encoding
            for (i = 0; i < encodingBytes.Length && i < imageBuffer.Length; i++)
            {
                if (imageBuffer[i] != encodingBytes[i])
                {
                    break;
                }
            }

            // Check if all encoding bytes matched buffer
            return (i == encodingBytes.Length);
        }
    }
}

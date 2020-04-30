using System;
using System.Collections.Generic;

namespace Freedom35.ImageProcessing
{
	/// <summary>
	/// Class containing definitions for image encoding.
	/// </summary>
    public static class ImageEncoding
    {
        #region Encoding Definitions

        /// <summary>
        /// BMP - BM
        /// </summary>
        private static byte[] BitmapEncoding => new byte[] { 0x42, 0x4d };

        /// <summary>
        /// TIFF - II*
        /// </summary>
        private static byte[] TiffEncoding => new byte[] { 0x49, 0x49, 0x2a };

        /// <summary>
        /// JPEG - ......JFIF
        /// </summary>
        private static byte[] JpegEncoding => new byte[] { 0xff, 0xd8, 0xff, 0xf4, 0x00, 0x10, 0x4a, 0x46, 0x49, 0x46 }; 

        /// <summary>
        /// PNG - .PNG
        /// </summary>
        private static byte[] PngEncoding => new byte[] { 0x89, 0x50, 0x4e, 0x47 };
                
        #endregion

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private static List<Tuple<ImageType, byte[]>> GetEncodingDefinitions()
		{
			return new List<Tuple<ImageType, byte[]>>()
			{
				new Tuple<ImageType, byte[]>(ImageType.Bitmap, BitmapEncoding),
				new Tuple<ImageType, byte[]>(ImageType.TIFF, TiffEncoding),
				new Tuple<ImageType, byte[]>(ImageType.JPEG, JpegEncoding),
				new Tuple<ImageType, byte[]>(ImageType.PNG, PngEncoding)
			};
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="imageType"></param>
		/// <returns></returns>
		public static bool TryGetImageType(byte[] buffer, out ImageType imageType)
        {
			imageType = ImageType.Unknown;

			// Encoding definitions
			List<Tuple<ImageType, byte[]>> encodings = GetEncodingDefinitions();

			// Decode image type in buffer
			foreach (Tuple<ImageType, byte[]> encoding in encodings)
			{
				if (IsImageType(buffer, encoding.Item2))
				{
					imageType = encoding.Item1;
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

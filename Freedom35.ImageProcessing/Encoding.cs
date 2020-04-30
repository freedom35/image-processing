using System;
using System.Collections.Generic;
using System.Text;

namespace Freedom35.ImageProcessing
{
    public static class Encoding
    {
        #region Image Encoding

        /// <summary>
        /// BMP - BM
        /// </summary>
        private static byte[] BitmapEncoding => new byte[] { (byte)'B', (byte)'M' };

        /// <summary>
        /// TIFF - II*
        /// </summary>
        private static byte[] TiffEncoding => new byte[] { (byte)'I', (byte)'I', (byte)'*' };

        /// <summary>
        /// JPEG - ......JFIF
        /// </summary>
        private static byte[] JpegEncoding => new byte[] { 0xff, 0xd8, 0xff, 0xf4, 0x00, 0x10, (byte)'J', (byte)'F', (byte)'I', (byte)'F' }; 

        /// <summary>
        /// PNG - .PNG
        /// </summary>
        private static byte[] PngEncoding => new byte[] { 0x89, (byte)'P', (byte)'N', (byte)'G' };
                
        #endregion


        public static bool TryGetImageType(byte[] buffer, out ImageType imageType)
        {
			imageType = ImageType.Unknown;

			// Encoding definitions
			List<Tuple<ImageType, byte[]>> encodings = new List<Tuple<ImageType, byte[]>>()
			{
				new Tuple<ImageType, byte[]>(ImageType.Bitmap, BitmapEncoding),
				new Tuple<ImageType, byte[]>(ImageType.TIFF, TiffEncoding),
				new Tuple<ImageType, byte[]>(ImageType.JPEG, JpegEncoding),
				new Tuple<ImageType, byte[]>(ImageType.PNG, PngEncoding)
			};

			// Decode image type in buffer
			foreach (Tuple<ImageType, byte[]> encoding in encodings)
			{
				byte[] encodingBytes = encoding.Item2;
				int i;

				// Compare bytes
				for (i = 0; i < encodingBytes.Length && i < buffer.Length; i++)
				{
					if (encodingBytes[i] != buffer[i])
					{
						break;
					}
				}

				// Check if encoding matches
				if (i == encodingBytes.Length)
				{
					imageType = encoding.Item1;
					break;
				}
			}

			return imageType != ImageType.Unknown;
		}
    }
}

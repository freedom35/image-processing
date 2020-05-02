using System;
using System.IO;
using System.Linq;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class representing a bitmap image.
    /// </summary>
    public sealed class BitmapTemp : IImage
    {
        /// <summary>
        /// Create Bitmap using FromStream / FromFile / FromBytes
        /// </summary>
        private BitmapTemp()
        {
        }

        #region Properties

        /// <summary>
        /// Image width.
        /// </summary>
        public int Width
        {
            get;
            private set;
        } = 0;

        /// <summary>
        /// Image height.
        /// </summary>
        public int Height
        {
            get;
            private set;
        } = 0;

        /// <summary>
        /// Raw image bytes.
        /// </summary>
        public byte[] ImageBytes
        {
            get;
            private set;
        } = null;

        /// <summary>
        /// Stride width (aka scan width).
        /// </summary>
        public int Stride
        {
            get;
            private set;
        } = 0;

        /// <summary>
        /// Pixel depth of the image.
        /// (bytes used per pixel: 1 indicates grayscale, 3 indicates color)
        /// </summary>
        public int PixelDepth
        {
            get => Width > 0 ? (Stride / Width) : 0;
        }

        /// <summary>
        /// Original path of image (if created from file)
        /// </summary>
        public string OriginalPath
        {
            get;
            private set;
        } = "";

        #endregion

        public static BitmapTemp FromFile(string path)
        {
            // Only need to read file
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                // Create image
                BitmapTemp bitmap = FromStream(file);

                // Path may be used to save image back to file later.
                bitmap.OriginalPath = path;
                
                return bitmap;
            }
        }

        public static BitmapTemp FromStream(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];

            // Copy stream to an array
            stream.Read(buffer, 0, buffer.Length);

            return FromBytes(buffer);
        }

        public static BitmapTemp FromBytes(byte[] buffer)
        {
            // Verify bitmap in buffer - other file types not supported
            if (!ImageEncoding.TryGetImageType(buffer, out ImageType type) || type != ImageType.Bitmap)
            {
                throw new NotSupportedException($"Unsupported image type: {type}");
            }

            // Query header for image info...

            int dataOffset = 0;

            // Create bitmap object
            BitmapTemp bitmap = new BitmapTemp()
            {
                ImageBytes = buffer.Skip(dataOffset).ToArray()
            };

            return bitmap;
        }
    }
}

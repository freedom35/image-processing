//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System.Drawing;
using System.Linq;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Methods relating to exchangeable image file format (EXIF) metadata.
    /// </summary>
    public static class ImageEXIF
    {
        /// <summary>
        /// Applies orientation to image based on EXIF metadata (if present).
        /// If image originates from phone or handheld camera, metadata may contain 'intended orientation' based on tilt sensors.
        /// </summary>
        /// <typeparam name="T">Image type to process and return</typeparam>
        /// <param name="image">Image to orientate</param>
        /// <returns>Orientated image based on EXIF</returns>
        public static T ApplyOrientation<T>(T image) where T : Image
        {
            // Clone image, method will return a new image whether orientation changed or not
            Image imageClone = (Image)image.Clone();

            // EXIF Tag ID for image orientation (274 decimal, 0x0112 hex)
            const int TagIdForOrientation = 274;

            // Check whether image contains embedded EXIF metadata (JPEG/TIFF)
            if (imageClone.PropertyIdList.Contains(TagIdForOrientation))
            {
                // Get orientation value
                int orientation = imageClone.GetPropertyItem(TagIdForOrientation).Value.ElementAtOrDefault(0);

                switch (orientation)
                {
                    ///////////////////////////////////////////////////////////
                    // Normal:
                    // The 0th row is at the visual top of the image,
                    // and the 0th column is the visual left-hand side.
                    ///////////////////////////////////////////////////////////
                    case 1:
                        // No rotation needed - same orientation as raw image
                        break;

                    ///////////////////////////////////////////////////////////
                    // Mirror horizontal:
                    // The 0th row is at the visual top of the image,
                    // and the 0th column is the visual right-hand side.
                    ///////////////////////////////////////////////////////////
                    case 2:
                        imageClone.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;

                    ///////////////////////////////////////////////////////////
                    // Rotate 180:
                    // The 0th row is at the visual bottom of the image,
                    // and the 0th column is the visual right-hand side.
                    ///////////////////////////////////////////////////////////
                    case 3:
                        imageClone.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;

                    ///////////////////////////////////////////////////////////
                    // Mirror vertical:
                    // The 0th row is at the visual bottom of the image,
                    // and the 0th column is the visual left-hand side.
                    ///////////////////////////////////////////////////////////
                    case 4:
                        imageClone.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        break;

                    ///////////////////////////////////////////////////////////
                    // Rotate 90 clockwise and mirror horizontal:
                    // The 0th row is the visual left-hand side of the image,
                    // and the 0th column is the visual top. 
                    ///////////////////////////////////////////////////////////
                    case 5:
                        imageClone.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;

                    ///////////////////////////////////////////////////////////
                    // Rotate 90 clockwise:
                    // The 0th row is the visual right-hand side of the image,
                    // and the 0th column is the visual top. 
                    ///////////////////////////////////////////////////////////
                    case 6:
                        imageClone.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;

                    ///////////////////////////////////////////////////////////
                    // Rotate 90 clockwise and mirror vertical:
                    // The 0th row is the visual right-hand side of the image,
                    // and the 0th column is the visual bottom.
                    ///////////////////////////////////////////////////////////
                    case 7:
                        imageClone.RotateFlip(RotateFlipType.Rotate90FlipY);
                        break;

                    ///////////////////////////////////////////////////////////
                    // Rotate 270 clockwise:
                    // The 0th row is the visual left-hand side of the image,
                    // and the 0th column is the visual bottom.
                    ///////////////////////////////////////////////////////////
                    case 8:
                        imageClone.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }

                // EXIF data no longer valid (as we've rotated image), remove property
                // (Also remove property if not rotated for consistency in returned image)
                imageClone.RemovePropertyItem(TagIdForOrientation);
            }

            return (T)imageClone;
        }
    }
}

//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Class for assigning color palettes.
    /// </summary>
    internal static class ImageColorPalette
    {
        /// <summary>
        /// Applies an 8-bit color palette (256 shades) to bitmap.
        /// </summary>
        /// <param name="bitmap">Bitmap to apply palette to</param>
        public static void ApplyGrayscale8bit(Bitmap bitmap)
        {
            // Copy of palette as basis (no constructor for ColorPalette)
            ColorPalette palette = bitmap.Palette;

            ApplyGrayscale8bit(palette);

            // Re-assign back to bitmap palette
            bitmap.Palette = palette;
        }

        /// <summary>
        /// Applies an 8-bit color palette (256 shades).
        /// </summary>
        /// <param name="palette">ColorPalette to apply palette to</param>
        public static void ApplyGrayscale8bit(ColorPalette palette)
        {
            // 8-bit palette, check array large enough
            int limit = Math.Min(palette.Entries.Length, 256);

            // Create shades of gray
            for (int i = 0; i < limit; i++)
            {
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }
        }
    }
}

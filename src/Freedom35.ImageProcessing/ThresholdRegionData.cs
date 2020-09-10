//------------------------------------------------
// GitHub:  freedom35
// License: MIT
//------------------------------------------------
using System;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Region data for Chow & Kaneko thresholding.
    /// </summary>
    internal struct ThresholdRegionData
    {
        /// <summary>
        /// Distance calculated from region.
        /// </summary>
        public double Distance
        {
            get;
            private set;
        }

        /// <summary>
        /// Threshold value of region.
        /// </summary>
        public byte Threshold
        {
            get;
            set;
        }

        /// <summary>
        /// X position of region.
        /// </summary>
        public int X
        {
            get;
            set;
        }

        /// <summary>
        /// Y position of region.
        /// </summary>
        public int Y
        {
            get;
            set;
        }

        /// <summary>
        /// Horizontal center of region.
        /// </summary>
        public int CenterX
        {
            get;
            set;
        }

        /// <summary>
        /// Vertical center of image.
        /// </summary>
        public int CenterY
        {
            get;
            set;
        }

        /// <summary>
        /// Calculates distance of x,y from region.
        /// </summary>
        /// <param name="x">X position of pixel</param>
        /// <param name="y">Y position of pixel</param>
        public void CalculateDistance(int x, int y)
        {
            Distance = Math.Sqrt(Math.Pow(Math.Abs(x - CenterX), 2) + Math.Pow(Math.Abs(y - CenterY), 2));
        }
    }
}

using System;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Definition of different image filter types.
    /// </summary>
    public enum ConvolutionFilter
    {
        /// <summary>
        /// Default edge detection filter.
        /// </summary>
        Edge,

        /// <summary>
        /// Default smoothing filter.
        /// </summary>
        Smoothing,

        /// <summary>
        /// Sharpens edges of image.
        /// </summary>
        Sharpen,

        /// <summary>
        /// Reduces image noise (by smoothing).
        /// </summary>
        NoiseReduction,

        /// <summary>
        /// Creates an embossing effect.
        /// </summary>
        Emboss,

        /// <summary>
        /// Laplacian A edge/high-pass filter.
        /// </summary>
        LaplacianA,

        /// <summary>
        /// Laplacian B edge/high-pass filter.
        /// </summary>
        LaplacianB,

        /// <summary>
        /// Smoothing/Low-pass filter.
        /// </summary>
        MexicanHat,

        /// <summary>
        /// Sobel vertical edge/high-pass filter.
        /// </summary>
        SobelVertical,

        /// <summary>
        /// Sobel horizontal edge/high-pass filter.
        /// </summary>
        SobelHorizontal
    }

    /// <summary>
    /// Extends ImageFilter enum.
    /// </summary>
    public static class ConvolutionFilterEnumExt
    {
        /// <summary>
        /// Gets template matrix for filter.
        /// </summary>
        /// <param name="filterType">Type of filter</param>
        /// <returns>2D matrix of values</returns>
        public static int[,] GetTemplateMatrix(this ConvolutionFilter filterType)
        {
            switch (filterType)
            {
                // Use Laplacian as default edge detection/sharpen filter
                case ConvolutionFilter.Edge:
                case ConvolutionFilter.Sharpen:
                case ConvolutionFilter.LaplacianA:
                    return new int[3, 3]
                    {
                        {  0, -1,  0 },
                        { -1,  4, -1 },
                        {  0, -1,  0 }
                    };

                case ConvolutionFilter.LaplacianB:
                    return new int[3, 3]
                    {
                        { -1, -1, -1 },
                        { -1,  8, -1 },
                        { -1, -1, -1 }
                    };

                case ConvolutionFilter.SobelHorizontal:
                    return new int[3, 3]
                    {
                        { -1,  0,  1 },
                        { -2,  0,  2 },
                        { -1,  0,  1 }
                    };

                case ConvolutionFilter.SobelVertical:
                    return new int[3, 3]
                    {
                        {  1,  2,  1 },
                        {  0,  0,  0 },
                        { -1, -2, -1 }
                    };

                case ConvolutionFilter.Smoothing:
                case ConvolutionFilter.NoiseReduction:
                    return new int[3, 3]
                    {
                        { 1,  1,  1 },
                        { 1,  1,  1 },
                        { 1,  1,  1 }
                    };

                case ConvolutionFilter.MexicanHat:
                    return new int[3, 3]
                    {
                        { 1,  2,  1 },
                        { 2,  4,  2 },
                        { 1,  2,  1 }
                    };

                case ConvolutionFilter.Emboss:
                    return new int[3, 3]
                    {
                        { -2, -1,  0 },
                        { -1,  1,  1 },
                        {  0,  1,  2 }
                    };

                default:
                    throw new NotImplementedException($"Template not implemented for {filterType}");
            }
        }
    }
}

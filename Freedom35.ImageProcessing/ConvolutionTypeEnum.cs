using System;

namespace Freedom35.ImageProcessing
{
    /// <summary>
    /// Definition of different 2D image convolution types.
    /// </summary>
    public enum ConvolutionType
    {
        /// <summary>
        /// Default edge detection filter.
        /// </summary>
        Edge,

        /// <summary>
        /// Default smoothing/blur filter.
        /// </summary>
        SmoothingA,

        /// <summary>
        /// Alternate smoothing filter.
        /// </summary>
        SmoothingB,

        /// <summary>
        /// Reduces image noise (by smoothing).
        /// </summary>
        NoiseReduction,

        /// <summary>
        /// Smoothing/Low-pass filter.
        /// (Less blurring)
        /// </summary>
        SmoothingMexicanHat,

        /// <summary>
        /// Sharpens edges of image.
        /// </summary>
        Sharpen,

        /// <summary>
        /// Laplacian A edge/high-pass filter.
        /// (Matrix with peak of 4)
        /// </summary>
        EdgeLaplacianA,

        /// <summary>
        /// Laplacian B edge/high-pass filter.
        /// (Matrix with diagonals and peak of 8)
        /// </summary>
        EdgeLaplacianB,

        /// <summary>
        /// Applies Gaussian smoothing and Laplacian edge.
        /// (Less sensitive to noise, Gaussian σ = 1.4)
        /// </summary>
        EdgeLaplacianOfGaussian,

        /// <summary>
        /// Sobel vertical edge/high-pass filter.
        /// </summary>
        EdgeSobelVertical,

        /// <summary>
        /// Sobel horizontal edge/high-pass filter.
        /// </summary>
        EdgeSobelHorizontal,

        /// <summary>
        /// Creates an embossing effect.
        /// </summary>
        Emboss
    }

    /// <summary>
    /// Extends ConvolutionFilter enum.
    /// </summary>
    public static class ConvolutionTypeEnumExt
    {
        /// <summary>
        /// Gets matrix/kernel for convolution.
        /// </summary>
        /// <param name="convolutionType">Type of convolution</param>
        /// <returns>2D matrix of kernel values</returns>
        public static int[,] GetConvolutionMatrix(this ConvolutionType convolutionType)
        {
            switch (convolutionType)
            {
                // Use Laplacian as default edge detection/sharpen filter
                case ConvolutionType.EdgeLaplacianA:
                case ConvolutionType.Edge:
                case ConvolutionType.Sharpen:
                    return new int[3, 3]
                    {
                        {  0, -1,  0 },
                        { -1,  4, -1 },
                        {  0, -1,  0 }
                    };

                case ConvolutionType.EdgeLaplacianB:
                    return new int[3, 3]
                    {
                        { -1, -1, -1 },
                        { -1,  8, -1 },
                        { -1, -1, -1 }
                    };

                case ConvolutionType.EdgeSobelHorizontal:
                    return new int[3, 3]
                    {
                        {  1,  2,  1 },
                        {  0,  0,  0 },
                        { -1, -2, -1 }
                    };

                case ConvolutionType.EdgeSobelVertical:
                    return new int[3, 3]
                    {
                        { -1,  0,  1 },
                        { -2,  0,  2 },
                        { -1,  0,  1 }
                    };

                case ConvolutionType.SmoothingA:
                case ConvolutionType.NoiseReduction:
                    return new int[3, 3]
                    {
                        { 1,  1,  1 },
                        { 1,  1,  1 },
                        { 1,  1,  1 }
                    };

                case ConvolutionType.SmoothingB:
                    return new int[3, 3]
                    {
                        { 1,   3,  1 },
                        { 3,  16,  3 },
                        { 1,   3,  1 }
                    };

                case ConvolutionType.SmoothingMexicanHat:
                    return new int[3, 3]
                    {
                        { 1,  2,  1 },
                        { 2,  4,  2 },
                        { 1,  2,  1 }
                    };

                case ConvolutionType.Emboss:
                    return new int[3, 3]
                    {
                        { -2, -1,  0 },
                        { -1,  1,  1 },
                        {  0,  1,  2 }
                    };

                case ConvolutionType.EdgeLaplacianOfGaussian:
                    return new int[9, 9]
                    {
                        { 0, 1, 1,   2,   2,   2, 1, 1, 0 },
                        { 1, 2, 4,   5,   5,   5, 4, 2, 1 },
                        { 1, 4, 5,   3,   0,   3, 5, 4, 1 },
                        { 2, 5, 3, -12, -24, -12, 3, 5, 2 },
                        { 2, 5, 0, -24, -40, -24, 0, 5, 2 },
                        { 2, 5, 3, -12, -24, -12, 3, 5, 2 },
                        { 1, 4, 5,   3,   0,   3, 5, 4, 1 },
                        { 1, 2, 4,   5,   5,   5, 4, 2, 1 },
                        { 0, 1, 1,   2,   2,   2, 1, 1, 0 }
                    };

                default:
                    throw new NotImplementedException($"Matrix not implemented for {convolutionType}");
            }
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Freedom35.ImageProcessing.Tests
{
    [TestClass]
    public class TestConvolutionTypeEnum
    {
        [DataRow(ConvolutionType.Edge, "Edge")]
        [DataRow(ConvolutionType.EdgeSobelVertical, "Sobel Vertical Edge")]
        [DataTestMethod]
        public void TestGetDescription(ConvolutionType type, string description)
        {
            Assert.AreEqual(type.GetDescription(), description);
        }

        [TestMethod]
        public void TestGetConvolutionMatrix()
        {
            ConvolutionType type = ConvolutionType.Smoothing;

            // 2D matrix
            int[,] matrix = type.GetConvolutionMatrix();

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    // Smoothing filter contains all 1's
                    Assert.AreEqual(1, matrix.GetValue(i, j));
                }
            }
        }
    }
}

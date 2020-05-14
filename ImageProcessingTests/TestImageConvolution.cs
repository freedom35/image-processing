using Freedom35.ImageProcessing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Drawing.Imaging;

namespace ImageProcessingTests
{
    [TestClass]
    public class TestImageConvolution
    {
        [TestMethod]
        public void TestApplyKernel()
        {
            // 4x5 image
            byte[] imageBytes = {
                1, 1, 3, 3, 4,
                1, 1, 4, 4, 3,
                2, 1, 3, 3, 3,
                1, 1, 1, 4, 4
            };

            // Grayscale image
            BitmapData bmpData = new BitmapData()
            {
                Height = 4,
                Width = 5,
                Stride = 5
            };

            int[,] kernelMatrix = new int[,]
            {
                {  1, 0 },
                {  0, 1 }
            };

            // Apply kernel to bytes
            ImageConvolution.ApplyKernel(imageBytes, bmpData, kernelMatrix);

            // Edge cases retain original values
            byte[] expectedBytes = {
                2, 5, 7, 6, 4,
                2, 4, 7, 7, 3,
                3, 2, 7, 7, 3,
                1, 1, 1, 4, 4
            };

            // Check arrays are same length
            Assert.AreEqual(expectedBytes.Length, imageBytes.Length);

            // Check each byte
            for (int i = 0; i < expectedBytes.Length; i++)
            {
                Assert.AreEqual(expectedBytes[i], imageBytes[i]);
            }
        }
    }
}

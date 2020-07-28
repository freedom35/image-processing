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
        public void TestApplyKernelGrayscale()
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
            byte[] resultBytes = ImageConvolution.ApplyKernel(imageBytes, bmpData, kernelMatrix);

            // Edge cases retain original values
            //byte[] expectedBytes = {
            //    2, 5, 7, 6, 4,
            //    2, 4, 7, 7, 3,
            //    3, 2, 7, 7, 3,
            //    1, 1, 1, 4, 4
            //};

            // Edge cases assigned black value
            byte[] expectedBytes = {
                2, 5, 7, 6, 0,
                2, 4, 7, 7, 0,
                3, 2, 7, 7, 0,
                0, 0, 0, 0, 0
            };

            // Check arrays are same length
            Assert.AreEqual(expectedBytes.Length, resultBytes.Length);

            // Check each byte
            for (int i = 0; i < expectedBytes.Length; i++)
            {
                Assert.AreEqual(expectedBytes[i], resultBytes[i]);
            }
        }

        [TestMethod]
        public void TestApplyKernelColor()
        {
            // 4x5 image (RGB)
            byte[] imageBytes = {
                1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 3, 3, 4, 4, 4,
                1, 1, 1, 1, 1, 1, 4, 4, 4, 4, 4, 4, 3, 3, 3,
                2, 2, 2, 1, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 4, 4, 4, 4
            };

            // Color image
            BitmapData bmpData = new BitmapData()
            {
                Height = 4,
                Width = 5,
                Stride = 15
            };

            int[,] kernelMatrix = new int[,]
            {
                {  1, 0 },
                {  0, 1 }
            };

            // Apply kernel to bytes
            byte[] resultBytes = ImageConvolution.ApplyKernel(imageBytes, bmpData, kernelMatrix);

            // Edge cases assigned black value
            byte[] expectedBytes = {
                2, 2, 2, 5, 5, 5, 7, 7, 7, 6, 6, 6, 0, 0, 0,
                2, 2, 2, 4, 4, 4, 7, 7, 7, 7, 7, 7, 0, 0, 0,
                3, 3, 3, 2, 2, 2, 7, 7, 7, 7, 7, 7, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };

            // Check arrays are same length
            Assert.AreEqual(expectedBytes.Length, resultBytes.Length);

            // Check each byte
            for (int i = 0; i < expectedBytes.Length; i++)
            {
                Assert.AreEqual(expectedBytes[i], resultBytes[i]);
            }
        }
    }
}

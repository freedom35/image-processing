using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Drawing;

namespace Freedom35.ImageProcessing.Tests
{
    [TestClass]
    public class TestImageThreshold
    {
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp", "Freedom35.ImageProcessing.Tests.Resources.clock-otsu.bmp")]
        [DataTestMethod]
        public void TestApplyThreshold(string sourceResourcePath, string resultResourcePath)
        {
            // Load source image
            using Image sourceImage = TestImage.FromResource(sourceResourcePath);

            // Apply Thresholding - currently uses Otsu as default
            using Image thresholdImage = ImageThreshold.Apply(sourceImage);

            // Load correct result image
            using Image resultImage = TestImage.FromResource(resultResourcePath);

            // Compare images
            Assert.IsTrue(TestImage.Compare(thresholdImage, resultImage));
        }

        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp", "Freedom35.ImageProcessing.Tests.Resources.clock-otsu.bmp")]
        [DataTestMethod]
        public void TestOtsuThreshold(string sourceResourcePath, string resultResourcePath)
        {
            // Load source image
            using Image sourceImage = TestImage.FromResource(sourceResourcePath);
            
            // Apply Thresholding
            using Image thresholdImage = ImageThreshold.ApplyOtsuMethod(sourceImage);

            // Load correct result image
            using Image resultImage = TestImage.FromResource(resultResourcePath);
            
            // Compare images
            Assert.IsTrue(TestImage.Compare(thresholdImage, resultImage));
        }

        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.jpg")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.png")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.tif")]
        [DataTestMethod]
        public void TestApplyThresholdValue(string sourceResourcePath)
        {
            // Load source image
            using Image sourceImage = TestImage.FromResource(sourceResourcePath);

            // Apply threshold
            using Image thresholdImage = ImageThreshold.Apply(sourceImage, 0x40);
            
            byte[] withoutAlphaBytes = RemoveAlphaLayerBytes(ImageBytes.FromImage(thresholdImage), sourceImage.PixelFormat);

            // Check bytes thresholded
            Assert.IsTrue(withoutAlphaBytes.Take(sourceImage.Width).All(b => b == byte.MinValue || b == byte.MaxValue));
        }

        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.jpg")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.png")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.tif")]
        [DataTestMethod]
        public void TestApplyMin(string sourceResourcePath)
        {
            // Load source image
            using Image sourceImage = TestImage.FromResource(sourceResourcePath);

            // Apply min value
            using Image minImage = ImageThreshold.ApplyMin(sourceImage, 0x20);

            byte[] withoutAlphaBytes = RemoveAlphaLayerBytes(ImageBytes.FromImage(minImage), sourceImage.PixelFormat);

            // Check all greater than value
            Assert.IsTrue(withoutAlphaBytes.All(b => b >= 0x20));
        }

        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.jpg")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.png")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.tif")]
        [DataTestMethod]
        public void TestApplyMax(string sourceResourcePath)
        {
            // Load source image
            using Image sourceImage = TestImage.FromResource(sourceResourcePath);

            // Apply max value
            using Image maxImage = ImageThreshold.ApplyMax(sourceImage, 0xD0);

            byte[] withoutAlphaBytes = RemoveAlphaLayerBytes(ImageBytes.FromImage(maxImage), sourceImage.PixelFormat);

            // Check all less than value
            Assert.IsTrue(withoutAlphaBytes.All(b => b <= 0xD0));
        }

        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.jpg")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.png")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.tif")]
        [DataTestMethod]
        public void TestApplyMinMax(string sourceResourcePath)
        {
            // Load source image
            using Image sourceImage = TestImage.FromResource(sourceResourcePath);

            // Apply min/max value
            using Image minMaxImage = ImageThreshold.ApplyMinMax(sourceImage, 0x25, 0xc3);

            byte[] withoutAlphaBytes = RemoveAlphaLayerBytes(ImageBytes.FromImage(minMaxImage), sourceImage.PixelFormat);

            // Check all within range
            Assert.IsTrue(withoutAlphaBytes.All(b => b >= 0x25 && b <= 0xc3));
        }

        private static byte[] RemoveAlphaLayerBytes(byte[] imageBytes, System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            // Check if has a transparency layer
            if (pixelFormat.ToString().ToLower().EndsWith("argb"))
            {
                // Skip last byte (padding) - contents may vary per platform
                return imageBytes.Where((b, i) => i % 4 != 3).ToArray();
            }

            return imageBytes;
        }
    }
}

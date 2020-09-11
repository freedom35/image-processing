using Freedom35.ImageProcessing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Drawing;
using Freedom35.ImageProcessing.Tests;

namespace ImageProcessingTests
{
    [TestClass]
    public class TestImageThreshold
    {
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp", "Freedom35.ImageProcessing.Tests.Resources.clock-otsu.bmp")]
        [DataTestMethod]
        public void TestOtsuThreshold(string sourceResourcePath, string resultResourcePath)
        {
            // Load source image
            using Image sourceImage = TestImage.FromResource(sourceResourcePath);
            
            Assert.IsNotNull(sourceImage);

            // Apply Thresholding
            using Image thresholdImage = ImageThreshold.ApplyOtsuMethod(sourceImage);

            Assert.IsNotNull(thresholdImage);

            // Load correct result image
            using Image resultImage = TestImage.FromResource(resultResourcePath);
            
            Assert.IsNotNull(resultImage);

            // Compare images
            Assert.IsTrue(TestImage.Compare(thresholdImage, resultImage));
        }
    }
}

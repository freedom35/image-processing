using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Freedom35.ImageProcessing.Tests
{
    [TestClass]
    public class TestImageCrop
    {
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.jpg")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.png")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.tif")]
        [DataTestMethod]
        public void TestByRegion(string resourcePath)
        {
            // Load source image
            using Image sourceImage = TestImage.FromResource(resourcePath);

            Rectangle region = new(0, 0, sourceImage.Width / 2, sourceImage.Height / 2);

            // Crop region of image
            using Image croppedImage = ImageCrop.ByRegion(sourceImage, region);

            // Check size
            Assert.AreEqual(region.Width, croppedImage.Width);
            Assert.AreEqual(region.Height, croppedImage.Height);

            // Get bytes for each image
            byte[] sourceBytes = ImageBytes.FromImage(sourceImage);
            byte[] croppedBytes = ImageBytes.FromImage(sourceImage);

            // Compare to ensure correct region cropped
            int limit = croppedImage.Width * croppedImage.Height;

            for (int i = 0; i < limit; i++)
            {
                Assert.AreEqual(sourceBytes[i], croppedBytes[i]);
            }
        }
    }
}

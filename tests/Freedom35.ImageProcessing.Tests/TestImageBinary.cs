using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Drawing;

namespace Freedom35.ImageProcessing.Tests
{
    [TestClass]
    public class TestImageBinary
    {
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.jpg")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.png")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.tif")]
        [DataTestMethod]
        public void TestAsBytes(string sourceResourcePath)
        {
            // Load source image
            using Image sourceImage = TestImage.FromResource(sourceResourcePath);

            Assert.IsNotNull(sourceImage);

            const byte Threshold = 0x7F;

            // Get bytes for image
            byte[] imageBytes = ImageBinary.AsBytes(sourceImage, Threshold);

            // Check we have some bytes
            Assert.IsNotNull(imageBytes);

            // Check all converted to binary (0 or 1)
            Assert.IsTrue(imageBytes.All(b => b < 0x02));

            // Check correct number of bytes after conversion
            // (Same per source image)
            Assert.AreEqual(118287, imageBytes.Length);

            // Count 1's and 0's
            int countZero = imageBytes.Count(b => b == 0x00);
            int countOne = imageBytes.Count(b => b == 0x01);

            // May be slight variances due to compression
            // but should be similar.
            Assert.IsTrue(countZero > 52000 && countZero < 52100);
            Assert.IsTrue(countOne > 66200 && countOne < 66300);
        }
    }
}

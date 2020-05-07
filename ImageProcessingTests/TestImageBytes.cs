using Freedom35.ImageProcessing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ImageProcessingTests
{
    [TestClass]
    public class TestImageBytes
    {
        [DataRow("ImageProcessingTests.Resources.clock.bmp")]
        [DataRow("ImageProcessingTests.Resources.clock.jpg")]
        [DataRow("ImageProcessingTests.Resources.clock.png")]
        [DataRow("ImageProcessingTests.Resources.clock.tif")]
        [DataTestMethod]
        public void TestFromStream(string resourcePath)
        {
            byte[] imageBytes = ImageBytes.FromResource(resourcePath);

            Assert.IsNotNull(imageBytes);

            // Check not an empty array
            Assert.IsTrue(imageBytes.Length > 0);

            // Check for variation in values
            Assert.IsTrue(imageBytes.Any(b => b > byte.MinValue && b < byte.MaxValue));
        }
    }
}

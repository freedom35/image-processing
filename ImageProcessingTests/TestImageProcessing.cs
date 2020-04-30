using Freedom35.ImageProcessing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ImageProcessingTests
{
    [TestClass]
    public class TestImageProcessing
    {
        [TestMethod]
        public void TestConvertToGrayscale()
        {
            byte[] imageBytes = {
                50,
                100,
                150
            };

            byte[] convertedImage = ImageProcessing.ConvertColorImageToGrayscale(imageBytes);

            Assert.IsNotNull(convertedImage);
            Assert.AreEqual(convertedImage.Length, imageBytes.Length / 3);
            Assert.AreEqual(convertedImage[0], imageBytes.Average(b => b));
        }


        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void TestConvertInvalidImageToGrayscale()
        {
            byte[] imageBytes = {
                50
            };

            ImageProcessing.ConvertColorImageToGrayscale(imageBytes);
        }


        [TestMethod]
        public void TestConvertNoImageToGrayscale()
        {
            byte[] imageBytes = { };

            byte[] convertedImage = ImageProcessing.ConvertColorImageToGrayscale(imageBytes);

            Assert.AreEqual(convertedImage.Length, 0);
        }


        [TestMethod]
        public void TestConvertToBackAndWhite()
        {
            byte[] imageBytes = {
                50,
                100,
                150
            };

            byte[] convertedImage = ImageProcessing.ConvertGrayscaleImageToBlackAndWhite(imageBytes);

            Assert.IsTrue(convertedImage.All(b => b == 0 || b == 255));
        }
    }
}

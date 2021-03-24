using Freedom35.ImageProcessing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ImageProcessingTests
{
    [TestClass]
    public class TestImageColor
    {
        [TestMethod]
        public void TestConvertToGrayscale()
        {
            byte[] imageBytes = {
                50,
                100,
                150
            };

            byte[] convertedImage = ImageColor.ColorImageToGrayscale(imageBytes);

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

            ImageColor.ColorImageToGrayscale(imageBytes);
        }


        [TestMethod]
        public void TestConvertNoImageToGrayscale()
        {
            byte[] imageBytes = System.Array.Empty<byte>();

            byte[] convertedImage = ImageColor.ColorImageToGrayscale(imageBytes);

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

            byte[] convertedImage = ImageColor.GrayscaleImageToBlackAndWhite(imageBytes);

            Assert.IsTrue(convertedImage.All(b => b == 0 || b == 255));
        }

        [TestMethod]
        public void TestConvertToNegative()
        {
            byte[] imageBytes = {
                0x01,
                0xf0,
                0x3c
            };

            ImageColor.ToNegative(imageBytes);

            Assert.AreEqual(0xfe, imageBytes[0]);
            Assert.AreEqual(0x0f, imageBytes[1]);
            Assert.AreEqual(0xc3, imageBytes[2]);
        }

        [TestMethod]
        public void TestConvertMonochromeToNegative()
        {
            byte[] imageBytes = {
                1,
                0
            };

            ImageColor.MonochromeToNegative(imageBytes);

            Assert.AreEqual(0, imageBytes[0]);
            Assert.AreEqual(1, imageBytes[1]);
        }
    }
}

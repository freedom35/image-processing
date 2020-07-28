using Freedom35.ImageProcessing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ImageProcessingTests
{
    [TestClass]
    public class TestImageBytes
    {
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.jpg")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.png")]
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.tif")]
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

        [TestMethod]
        public void TestBytesToBits()
        {
            byte[] byteValues = new byte[] 
            { 
                0xD5, 
                0x3C 
            };

            byte[] bitValues = ImageBytes.BytesToBits(byteValues);

            byte[] expectedBitValues = new byte[]
            {
                1, 1, 0, 1, 0, 1, 0, 1,
                0, 0, 1, 1, 1, 1, 0, 0,
            };

            // Check expected number in array
            Assert.AreEqual(expectedBitValues.Length, bitValues.Length);

            // Check each bit
            for (int i = 0; i < expectedBitValues.Length; i++)
            {
                Assert.AreEqual(expectedBitValues[i], bitValues[i]);
            }
        }

        [TestMethod]
        public void TestBitsToBytes()
        {
            byte[] bitValues = new byte[]
            {
                1, 1, 0, 1, 0, 1, 0, 1,
                0, 0, 1, 1, 1, 1, 0, 0,
            };

            byte[] byteValues = ImageBytes.BitsToBytes(bitValues);

            byte[] expectedByteValues = new byte[] 
            { 
                0xD5, 
                0x3C 
            };

            // Check expected number in array
            Assert.AreEqual(expectedByteValues.Length, byteValues.Length);

            // Check each bit
            for (int i = 0; i < expectedByteValues.Length; i++)
            {
                Assert.AreEqual(expectedByteValues[i], byteValues[i]);
            }
        }
    }
}

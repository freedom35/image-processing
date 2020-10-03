using Freedom35.ImageProcessing;
using Freedom35.ImageProcessing.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
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
        public void TestFromResource(string resourcePath)
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

        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataTestMethod]
        public void TestBytesGetMinValue(string resourcePath)
        {
            using Image image = TestImage.FromResource(resourcePath);

            byte min = ImageBytes.GetMinValue(image);

            Assert.AreEqual(0x00, min);
        }

        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataTestMethod]
        public void TestBytesGetMaxValue(string resourcePath)
        {
            using Image image = TestImage.FromResource(resourcePath);
            
            byte max = ImageBytes.GetMaxValue(image);

            Assert.AreEqual(0xff, max);
        }

        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataTestMethod]
        public void TestBytesGetMinMaxValue(string resourcePath)
        {
            using Image image = TestImage.FromResource(resourcePath);

            Tuple<byte, byte> minMax = ImageBytes.GetMinMaxValue(image);

            Assert.AreEqual(0x00, minMax.Item1);
            Assert.AreEqual(0xff, minMax.Item2);
        }

        [DataRow(new byte[0], ImageType.Unknown)]
        [DataRow(new byte[] { 0x00, 0x00 }, ImageType.Unknown)]
        [DataRow(new byte[] { 0xFF, 0xFF, 0x42, 0x42 }, ImageType.Unknown)]
        [DataRow(new byte[] { 0x42, 0x4d, 0x42, 0x42 }, ImageType.Bitmap)]
        [DataRow(new byte[] { 0x49, 0x49, 0x2a, 0x42, 0x42 }, ImageType.TIFF)]
        [DataRow(new byte[] { 0xff, 0xd8, 0xff, 0xf4, 0x00, 0x10, 0x4a, 0x46, 0x49, 0x46, 0x42, 0x42 }, ImageType.JPEG)]
        [DataRow(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x42, 0x42 }, ImageType.PNG)]
        [DataTestMethod]
        public void TestTryGetImageType(byte[] imageBytes, ImageType expectedType)
        {
            ImageType type;

            if (expectedType == ImageType.Unknown)
            {
                // Should fail - unable to determine type
                Assert.IsFalse(ImageBytes.TryGetImageType(imageBytes, out type));
            }
            else
            {
                Assert.IsTrue(ImageBytes.TryGetImageType(imageBytes, out type));
            }

            Assert.AreEqual(expectedType, type);
        }

        [DataRow(new byte[0], ImageType.Unknown)]
        [DataRow(new byte[] { 0x00, 0x00, 0x42, 0x42 }, ImageType.Unknown)]
        [DataRow(new byte[] { 0xFF, 0xFF, 0x42, 0x42 }, ImageType.Unknown)]
        [DataRow(new byte[] { 0x42, 0x4d, 0x42, 0x42 }, ImageType.Bitmap)]
        [DataRow(new byte[] { 0x49, 0x49, 0x2a, 0x42, 0x42 }, ImageType.TIFF)]
        [DataRow(new byte[] { 0xff, 0xd8, 0xff, 0xf4, 0x00, 0x10, 0x4a, 0x46, 0x49, 0x46, 0x42, 0x42 }, ImageType.JPEG)]
        [DataRow(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x42, 0x42 }, ImageType.PNG)]
        [DataTestMethod]
        public void TestIsImageType(byte[] imageBytes, ImageType expectedType)
        {
            Assert.IsTrue(ImageBytes.IsImageType(imageBytes, expectedType));
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace Freedom35.ImageProcessing.Tests
{
    [TestClass]
    public class TestImageColor
    {
        /// <summary>
        /// Helper method for test methods.
        /// </summary>
        private static byte[] CreateColorImage4x4(out BitmapData bitmapData)
        {
            bitmapData = new()
            {
                Height = 4,
                Width = 4,
                Stride = 12,
                PixelFormat = PixelFormat.Format24bppRgb
            };

            List<byte> listBytes = new();

            for (int i = 0; i < bitmapData.Width * bitmapData.Height; i++)
            {
                // RGB
                listBytes.Add(50);
                listBytes.Add(100);
                listBytes.Add(150);
            }

            return listBytes.ToArray();
        }

        [TestMethod]
        public void TestConvertToGrayscale()
        {
            byte[] imageBytes = CreateColorImage4x4(out BitmapData bitmapData);

            byte[] convertedImage = ImageColor.ToGrayscale(imageBytes, bitmapData);

            Assert.IsNotNull(convertedImage);
            Assert.AreEqual(convertedImage.Length, imageBytes.Length / 3);
            Assert.AreEqual(convertedImage[0], imageBytes.Average(b => b));
        }

        [TestMethod]
        public void TestConvertInvalidImageToGrayscale()
        {
            byte[] imageBytes = CreateColorImage4x4(out BitmapData bitmapData);

            // Set to non-color
            bitmapData.Stride = bitmapData.Width;
            bitmapData.PixelFormat = PixelFormat.Format8bppIndexed;

            Assert.ThrowsException<ArgumentException>(() => ImageColor.ToGrayscale(imageBytes, bitmapData));
        }

        [TestMethod]
        public void TestConvertNoImageToGrayscale()
        {
            byte[] imageBytes = Array.Empty<byte>();

            BitmapData bitmapData = new()
            {
                Height = 0,
                Width = 0,
                Stride = 0,
                PixelFormat = PixelFormat.Format24bppRgb
            };

            Assert.ThrowsException<ArgumentException>(() => ImageColor.ToGrayscale(imageBytes, bitmapData));
        }


        [TestMethod]
        public void TestConvertToBackAndWhite()
        {
            byte[] imageBytes = {
                50,
                100,
                150
            };

            byte[] convertedImage = ImageColor.ToBlackAndWhite(imageBytes);

            Assert.IsTrue(convertedImage.All(b => b == 0 || b == 255));
        }

        [TestMethod]
        public void TestConvertToBackAndWhiteWithThreshold()
        {
            byte[] imageBytes = CreateColorImage4x4(out BitmapData bitmapData);

            byte[] convertedImage = ImageColor.ToBlackAndWhite(imageBytes, 0x42);

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

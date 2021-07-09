using Freedom35.ImageProcessing;
using Freedom35.ImageProcessing.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace ImageProcessingTests
{
    [TestClass]
    public class TestImageCombine
    {
        [TestMethod]
        public void TestCombineAllNone()
        {
            Bitmap combinedImage = ImageCombine.All(Array.Empty<Image>());
            Assert.IsNull(combinedImage);
        }

        [TestMethod]
        public void TestCombineAllMixedColor()
        {
            using Image colorImage = TestImage.FromResource("Freedom35.ImageProcessing.Tests.Resources.clock.bmp");
            Assert.IsNotNull(colorImage);

            using Image bwImage = TestImage.FromResource("Freedom35.ImageProcessing.Tests.Resources.clock-bw.bmp");
            Assert.IsNotNull(bwImage);

            Image[] imagesToCombine = new Image[]
            {
                colorImage,
                bwImage
            };

            Assert.ThrowsException<ArgumentException>(() => ImageCombine.All(imagesToCombine));
        }

        [TestMethod]
        public void TestCombineAllOne()
        {
            using Image sourceImage = TestImage.FromResource("Freedom35.ImageProcessing.Tests.Resources.clock.bmp");
            Assert.IsNotNull(sourceImage);

            Image[] imagesToCombine = new Image[]
            {
                sourceImage
            };

            Bitmap combinedImage = ImageCombine.All(imagesToCombine);

            // Get bytes for images
            byte[] sourceBytes = ImageBytes.FromImage(sourceImage);
            byte[] combinedBytes = ImageBytes.FromImage(combinedImage);

            // Should return the same image
            for (int i = 0; i < sourceBytes.Length; i++)
            {
                Assert.AreEqual(sourceBytes[i], combinedBytes[i]);
            }
        }

        [TestMethod]
        public void TestCombineAllSame()
        {
            string resourcePath = "Freedom35.ImageProcessing.Tests.Resources.clock.bmp";

            // Load source image
            using Image sourceImage = TestImage.FromResource(resourcePath);
            Assert.IsNotNull(sourceImage);

            using Image sourceImageCopy = TestImage.FromResource(resourcePath);
            Assert.IsNotNull(sourceImageCopy);

            Image[] imagesToCombine = new Image[]
            {
                sourceImage,
                sourceImageCopy
            };

            Bitmap combinedImage = ImageCombine.All(imagesToCombine);

            // Get bytes for images
            byte[] sourceBytes = ImageBytes.FromImage(sourceImage);
            byte[] combinedBytes = ImageBytes.FromImage(combinedImage);

            // Compare bytes - should be same, bytes OR'd
            for (int i = 0; i < sourceBytes.Length; i++)
            {
                Assert.AreEqual(sourceBytes[i], combinedBytes[i]);
            }
        }

        [TestMethod]
        public void TestCombineAllWithNegative()
        {
            string resourcePath = "Freedom35.ImageProcessing.Tests.Resources.clock.bmp";

            // Load source image
            using Image sourceImage = TestImage.FromResource(resourcePath);
            Assert.IsNotNull(sourceImage);

            using Image sourceImageCopy = TestImage.FromResource(resourcePath);
            Assert.IsNotNull(sourceImageCopy);

            using Image negativeCopy = ImageColor.ToNegative(sourceImageCopy);
            Assert.IsNotNull(negativeCopy);

            Image[] imagesToCombine = new Image[]
            {
                sourceImage,
                negativeCopy
            };

            Bitmap combinedImage = ImageCombine.All(imagesToCombine);

            // Get bytes for images
            byte[] sourceBytes = ImageBytes.FromImage(sourceImage);
            byte[] combinedBytes = ImageBytes.FromImage(combinedImage);

            // Compare bytes - should combine to max out each pixel
            for (int i = 0; i < sourceBytes.Length; i++)
            {
                Assert.AreEqual(byte.MaxValue, combinedBytes[i]);
            }
        }
    }
}

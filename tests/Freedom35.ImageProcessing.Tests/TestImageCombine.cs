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

            // Combine
            Bitmap combinedBitmap = ImageCombine.All(imagesToCombine);

            // Convert for byte comparison
            Bitmap sourceBitmap = ImageFormatting.ToBitmap(sourceImage);

            // Should return the same image when only combining one
            Assert.IsTrue(TestImage.Compare(sourceBitmap, combinedBitmap));
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

            // Combine
            Bitmap combinedImage = ImageCombine.All(imagesToCombine);

            // Convert for byte comparison
            Bitmap sourceBitmap = ImageFormatting.ToBitmap(sourceImage);

            // Compare images
            Assert.IsTrue(TestImage.Compare(sourceBitmap, combinedImage));
        }

        [TestMethod]
        public void TestCombineAllWithNegative()
        {
            string resourcePath = "Freedom35.ImageProcessing.Tests.Resources.clock.bmp";

            // Load source image
            using Image sourceImage = TestImage.FromResource(resourcePath);
            Assert.IsNotNull(sourceImage);

            using Image negativeCopy = ImageColor.ToNegative(sourceImage);
            Assert.IsNotNull(negativeCopy);

            Image[] imagesToCombine = new Image[]
            {
                sourceImage,
                negativeCopy
            };

            Bitmap combinedImage = ImageCombine.All(imagesToCombine);

            // Get bytes for images
            byte[] combinedBytes = ImageBytes.FromImage(combinedImage);

            // Just check first row of bytes has been combined
            for (int i = 0; i < combinedImage.Width; i++)
            {
                Assert.AreEqual(byte.MaxValue, combinedBytes[i]);
            }

            int pixelDepth = 3;
            int stride = 1056;
            int width = 1053;
            int height = combinedImage.Height;

            int limit = combinedBytes.Length - 4;

            // Compare combined bytes excluding stride padding
            for (int y = 0; y < height; y++)
            {
                int offset = y * stride;

                for (int x = 0; x < width; x += pixelDepth)
                {
                    int i = offset + x;

                    if (i < limit)
                    {
                        Assert.AreEqual(byte.MaxValue, combinedBytes[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace Freedom35.ImageProcessing.Tests
{
    [TestClass]
    public class TestImageCombine
    {
        [TestMethod]
        public void TestCombineAllNone()
        {
            Bitmap? combinedImage = ImageCombine.All(Array.Empty<Image>());
            Assert.IsNull(combinedImage);
        }

        [TestMethod]
        public void TestCombineAllMixedColor()
        {
            using Image colorImage = TestImage.FromResource("Freedom35.ImageProcessing.Tests.Resources.clock.bmp");
            
            using Image bwImage = TestImage.FromResource("Freedom35.ImageProcessing.Tests.Resources.clock-bw.bmp");
            
            Image[] imagesToCombine =
            [
                colorImage,
                bwImage
            ];

            Assert.ThrowsException<ArgumentException>(() => ImageCombine.All(imagesToCombine));
        }

        [TestMethod]
        public void TestCombineAllOne()
        {
            using Image sourceImage = TestImage.FromResource("Freedom35.ImageProcessing.Tests.Resources.clock.bmp");
            
            Image[] imagesToCombine =
            [
                sourceImage
            ];

            // Combine
            Bitmap? combinedBitmap = ImageCombine.All(imagesToCombine);
            Assert.IsNotNull(combinedBitmap);

            // Convert for byte comparison
            byte[] sourceBytes = ImageBytes.FromImage(sourceImage);
            byte[] combinedBytes = ImageBytes.FromImage(combinedBitmap);

            // Should return the same image when only combining one
            for (int i = 0; i < sourceImage.Width; i++)
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
            
            using Image sourceImageCopy = TestImage.FromResource(resourcePath);
            
            Image[] imagesToCombine =
            [
                sourceImage,
                sourceImageCopy
            ];

            // Combine
            Bitmap? combinedImage = ImageCombine.All(imagesToCombine);
            Assert.IsNotNull(combinedImage);

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
            
            using Image negativeCopy = ImageColor.ToNegative(sourceImage);
            
            Image[] imagesToCombine =
            [
                sourceImage,
                negativeCopy
            ];

            Bitmap? combinedImage = ImageCombine.All(imagesToCombine);
            Assert.IsNotNull(combinedImage);

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

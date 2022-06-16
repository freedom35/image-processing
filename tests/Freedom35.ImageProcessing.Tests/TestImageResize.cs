using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Freedom35.ImageProcessing.Tests
{
    [TestClass]
    public class TestImageResize
    {
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataTestMethod]
        public void TestImageResizeAsNew(string resourcePath)
        {
            // Load source image
            using Image sourceImage = TestImage.FromResource(resourcePath);

            Assert.IsNotNull(sourceImage);

            Bitmap bitmap = sourceImage as Bitmap;

            Assert.IsNotNull(bitmap);

            // Change original size
            int width = bitmap.Width / 2;
            int height = bitmap.Height / 2;

            using Image resizedImage = ImageResize.ResizeAsNew(bitmap, width, height);

            Assert.IsNotNull(resizedImage);
            Assert.AreEqual(width, resizedImage.Width);
            Assert.AreEqual(height, resizedImage.Height);
        }

        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataTestMethod]
        public void TestImageResizeAsNewByRatio(string resourcePath)
        {
            // Load source image
            using Image sourceImage = TestImage.FromResource(resourcePath);

            Assert.IsNotNull(sourceImage);

            Bitmap bitmap = sourceImage as Bitmap;

            Assert.IsNotNull(bitmap);

            using Image resizedImage = ImageResize.ResizeAsNew(bitmap, 2.0);

            Assert.IsNotNull(resizedImage);
            Assert.AreEqual(bitmap.Width * 2, resizedImage.Width);
            Assert.AreEqual(bitmap.Height * 2, resizedImage.Height);
        }

        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataTestMethod]
        public void TestImageResizeOriginal(string resourcePath)
        {
            // Load source image
            Image sourceImage = TestImage.FromResource(resourcePath);

            Assert.IsNotNull(sourceImage);

            Bitmap bitmap = sourceImage as Bitmap;

            Assert.IsNotNull(bitmap);

            // Change original size
            int width = bitmap.Width / 2;
            int height = bitmap.Height / 2;

            ImageResize.ResizeOriginal(ref bitmap, width, height);

            Assert.IsNotNull(bitmap);
            Assert.AreEqual(width, bitmap.Width);
            Assert.AreEqual(height, bitmap.Height);

            bitmap.Dispose();
        }

        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataTestMethod]
        public void TestImageResizeOriginalByRatio(string resourcePath)
        {
            // Load source image
            Image sourceImage = TestImage.FromResource(resourcePath);

            Assert.IsNotNull(sourceImage);

            Bitmap bitmap = sourceImage as Bitmap;

            Assert.IsNotNull(bitmap);

            // Get original size
            int width = bitmap.Width;
            int height = bitmap.Height;

            ImageResize.ResizeOriginal(ref bitmap, 3.0);

            Assert.IsNotNull(bitmap);
            Assert.AreEqual(width * 3, bitmap.Width);
            Assert.AreEqual(height * 3, bitmap.Height);

            bitmap.Dispose();
        }
    }
}

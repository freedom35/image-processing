using Freedom35.ImageProcessing;
using Freedom35.ImageProcessing.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace ImageProcessingTests
{
    [TestClass]
    public class TestImageCopy
    {
        [DataRow("Freedom35.ImageProcessing.Tests.Resources.clock.bmp")]
        [DataTestMethod]
        public void TestFromSourceToDestination(string resourcePath)
        {
            // Load source image
            using Bitmap sourceBitmap = TestImage.FromResource(resourcePath) as Bitmap;

            Assert.IsNotNull(sourceBitmap);

            // Create blank bitmap
            using Bitmap copyBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.PixelFormat);
            
            // Crop region of image
            ImageCopy.FromSourceToDestination(sourceBitmap, copyBitmap);

            // Check size
            Assert.AreEqual(sourceBitmap.Width, copyBitmap.Width);
            Assert.AreEqual(sourceBitmap.Height, copyBitmap.Height);

            // Get bytes for each image
            byte[] sourceBytes = ImageBytes.FromImage(sourceBitmap);
            byte[] copiedBytes = ImageBytes.FromImage(copyBitmap);

            // Compare to ensure correct copy matches
            int limit = System.Math.Min(sourceBytes.Length, copiedBytes.Length);

            for (int i = 0; i < limit; i++)
            {
                Assert.AreEqual(sourceBytes[i], copiedBytes[i]);
            }
        }
    }
}

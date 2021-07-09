using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace Freedom35.ImageProcessing.Tests
{
    /// <summary>
    /// Helper methods to assist testing
    /// </summary>
    static class TestImage
    {
        public static Image FromResource(string resourcePath)
        {
            // Keep stream open for processing
            Stream resourceStream = System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream(resourcePath);
            
            return Image.FromStream(resourceStream);
        }

        public static bool Compare(Image image1, Image image2)
        {
            byte[] imageBytes1 = ImageBytes.FromImage(image1);
            byte[] imageBytes2 = ImageBytes.FromImage(image2);

            bool match = imageBytes1.Length == imageBytes2.Length;

            for (int i = 0; match && i < imageBytes1.Length; i++)
            {
                match = imageBytes1[i] == imageBytes2[i];
            }

            return match;
        }
    }
}

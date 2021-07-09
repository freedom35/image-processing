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

        public static bool Compare(Image image1, Image image2, bool compareLastByte = true)
        {
            byte[] imageBytes1 = ImageBytes.FromImage(image1);
            byte[] imageBytes2 = ImageBytes.FromImage(image2);

            bool match = imageBytes1.Length == imageBytes2.Length;

            int limit = compareLastByte ? imageBytes1.Length : imageBytes1.Length - 3;

            for (int i = 0; match && i < limit; i++)
            {
                match = imageBytes1[i] == imageBytes2[i];
            }

            return match;
        }
    }
}

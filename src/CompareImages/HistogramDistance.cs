using System;

namespace CompareImages
{
    public class HistogramDistance
    {
        private const int ColorQuantization = 8;

        public static double Calculate(byte[] image1, byte[] image2, bool normalize)
        {
            var histogram1 = GetHistogram(image1, normalize);
            var histogram2 = GetHistogram(image2, normalize);

            var distance = 0d;
            for (int i = 0; i < histogram1.Length; i++)
            {
                distance += Math.Abs(histogram1[i] - histogram2[i]);
            }

            return distance;
        }

        private static double[] GetHistogram(byte[] image, bool normalize)
        {
            var k = 256 / ColorQuantization;
            var size = image.Length;

            var histogram = new double[ColorQuantization];

            for (int i = 0; i < size; i++)
            {
                int quantColor = image[i] / k;

                ++histogram[quantColor];
            }

            if (normalize)
            {
                for (int i = 0; i < ColorQuantization; i++)
                {
                    histogram[i] = histogram[i] / size;
                }
            }

            return histogram;
        }
    }
}
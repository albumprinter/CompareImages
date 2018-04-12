using System;
using System.Runtime.InteropServices;

namespace CompareImages
{
    public static class HistogramDistance
    {
        private const int ColorQuantization = 8;

        public static double Calculate(byte[] image1, byte[] image2, bool normalize)
        {
            var histogram1 = GetHistogram(image1, normalize);
            var histogram2 = GetHistogram(image2, normalize);

            return Calculate(histogram1, histogram2);
        }

        public static double Calculate(IntPtr scan01, int size1, IntPtr scan02, int size2, bool normalize)
        {
            var histogram1 = GetHistogram(scan01, size1, normalize);
            var histogram2 = GetHistogram(scan02, size2, normalize);

            return Calculate(histogram1, histogram2);
        }

        public static double Calculate(IntPtr scan01, int width1, int height1, int bpp1, IntPtr scan02, int width2, int height2, int bpp2, bool normalize)
        {
            var histogram1 = GetHistogram(scan01, width1, height1, bpp1, normalize);
            var histogram2 = GetHistogram(scan02, width2, height2, bpp2, normalize);

            return Calculate(histogram1, histogram2);
        }

        private static double Calculate(double[] histogram1, double[] histogram2)
        {
            var distance = 0d;
            for (int i = 0; i < histogram1.Length; i++)
            {
                distance += Math.Abs(histogram1[i] - histogram2[i]);
            }

            return distance;
        }

        private static double[] GetHistogram(IntPtr scan0, int width, int height, int bpp, bool normalize)
        {
            var size = height * width * bpp;

            return GetHistogram(scan0, size, normalize);
        }

        private static double[] GetHistogram(byte[] image, bool normalize)
        {
            GCHandle pinnedArray;
            try
            {
                pinnedArray = GCHandle.Alloc(image, GCHandleType.Pinned);

                var pointer = pinnedArray.AddrOfPinnedObject();

                return GetHistogram(pointer, image.Length, normalize);
            }
            finally
            {
                pinnedArray.Free();
            }
        }

        private static double[] GetHistogram(IntPtr scan0, int size, bool normalize)
        {
            var k = 256 / ColorQuantization;
            
            var histogram = new double[ColorQuantization];

            unsafe
            {
                byte* scan0Ptr = (byte*) scan0.ToPointer();
                
                for (int i = 0; i < size; i++)
                {
                    int quantColor = scan0Ptr[i] / k;

                    ++histogram[quantColor];
                }
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
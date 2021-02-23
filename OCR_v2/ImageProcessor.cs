using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace OCR_v2
{
    class ImageProcessor
    {
        private Bitmap bitmap;
        private BitmapData bmpData;
        private int bytesPerPixel;
        private byte[] pixels;
        private IntPtr ptrFirstPixel;
        public ImageProcessor(Bitmap bmp)
        {
            bitmap =  bmp;
            bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            var byteCount = bmpData.Stride * bitmap.Height;
            pixels = new byte[byteCount];
            ptrFirstPixel = bmpData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
        }

        public void UpdateBitmap()
        {
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            bitmap.UnlockBits(bmpData);
        }

        public void Threshold(byte threshold, Rectangle area)
        {
            for (var y = area.Top; y < area.Bottom; y++)
            {
                var currentLine = y * bmpData.Stride;
                for (var x = area.Left*bytesPerPixel; x < area.Right*bytesPerPixel; x+=bytesPerPixel)
                {
                    var sum = 0;
                    for (var i = 0; i < 3; i++)
                        sum += pixels[currentLine + x + i];
                    if (sum / 3 > threshold)
                    {
                        for (var i = 0; i < 3; i++)
                            pixels[currentLine + x + i] = 255;
                    }
                    else
                    {
                        for (var i = 0; i < 3; i++)
                            pixels[currentLine + x + i] = 0;
                    }
                }
            }
        }   

        private void SetPixel(int x, int y, ByteColor color)
        {
            var currentLine = y * bmpData.Stride;
            x *= bytesPerPixel;
            pixels[currentLine + x] = color.B;
            pixels[currentLine + x + 1] = color.G;
            pixels[currentLine + x + 2] = color.R;
            pixels[currentLine + x + 3] = color.A;
        }
    }
}

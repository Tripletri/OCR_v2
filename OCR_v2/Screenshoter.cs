using System.Drawing;
using System.Windows.Forms;

namespace OCR_v2
{
    class Screenshoter
    {
        public static int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
        public static int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;

        public Bitmap MakeScreenshot()
        {
            var bitmap = new Bitmap(ScreenWidth, ScreenHeight);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0,
                    bitmap.Size, CopyPixelOperation.SourceCopy);
            }
            return bitmap;
        }
    }
}

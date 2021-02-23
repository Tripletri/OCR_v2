namespace OCR_v2
{
    class ByteColor
    {
        public byte A;
        public byte R;
        public byte G;
        public byte B;

        public ByteColor(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}
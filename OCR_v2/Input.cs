using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OCR_v2
{
    class Input
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        private readonly int modifier;
        private readonly int key;
        private readonly IntPtr hWnd;
        private readonly int id;

        public Input(int modifier, Keys key, Form form)
        {
            this.modifier = modifier;
            this.key = (int)key;
            hWnd = form.Handle;
            id = GetHashCode();
        }

        public override int GetHashCode() => modifier ^ key ^ hWnd.ToInt32();

        public bool Register() => RegisterHotKey(hWnd, id, modifier, key);
        public bool Unregister() => UnregisterHotKey(hWnd, id);
    }

    enum KeysModifiers
    {
        NOMOD = 0x0000,
        ALT = 0x0001,
        CTRL = 0x0002,
        SHIFT = 0x0004,
        WIN = 0x0008
    }
}
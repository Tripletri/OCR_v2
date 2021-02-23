using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OCR_v2
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        ///
        ///
#if DEBUG
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
#endif


        [STAThread]
        static void Main()
        {
#if DEBUG
            AllocConsole();
            var consoleWindow = GetConsoleWindow();
            SetWindowPos(consoleWindow, -1, 50, 50, 400, 200, 0);
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StartForm());
        }
    }
}

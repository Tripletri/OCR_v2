using System;
using System.Collections.Generic;

namespace OCR_v2
{
    static class ApplicationHandler
    {
        private static readonly List<Input> keyBinds = new List<Input>();

        public static void AddKeyBind(Input keyBind)
        {
            keyBinds.Add(keyBind);
            keyBind.Register();
        }

        public static void Exit(int exitCode)
        {
            foreach (var keyBind in keyBinds)
                keyBind.Unregister();
            Environment.Exit(exitCode);
        }
    }
}

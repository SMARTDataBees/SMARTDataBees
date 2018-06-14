using System;
using System.Runtime.InteropServices;

namespace SDBees.Core.Utils
{
    public static class User32
    {
        [DllImport("User32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        internal static readonly IntPtr InvalidHandleValue = IntPtr.Zero;
        internal const int SW_MAXIMIZE = 3;
    }
}

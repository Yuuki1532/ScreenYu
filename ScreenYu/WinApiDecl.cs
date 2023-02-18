using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ScreenYu {
    public static partial class WinAPI {
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("Gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("Gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);
        [DllImport("Gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);
        [DllImport("Gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdc, int x, int y,
            int cx, int cy, IntPtr hdcSrc, int x1, int y1, uint rop);
        [DllImport("Gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("Gdi32.dll")]
        public static extern bool DeleteObject(IntPtr ho);
        [DllImport("User32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("User32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        
        public const int WM_HOTKEY = 0x0312;
        public enum ModifierKeys {
            MOD_ALT = 0x0001,
            MOD_CONTROL = 0x0002,
            MOD_NOREPEAT = 0x4000,
            MOD_SHIFT = 0x0004,
            MOD_WIN = 0x0008
        }
    }
}

using System.Runtime.InteropServices;

namespace ScreenYu.Pkg.Win {
    internal static class WinApi {
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("Gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("Gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);
        [DllImport("Gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);
        [DllImport("Gdi32.dll")]
        public static extern int BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, uint rop);
        [DllImport("Gdi32.dll")]
        public static extern int DeleteDC(IntPtr hdc);
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("Gdi32.dll")]
        public static extern int DeleteObject(IntPtr ho);
        [DllImport("User32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("User32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("User32.dll")]
        public static extern int UnregisterHotKey(IntPtr hWnd, int id);

        public const int WM_HOTKEY = 0x0312;
        public enum ModifierKeys : uint {
            Alt = 0x0001,
            Control = 0x0002,
            NoRepeat = 0x4000,
            Shift = 0x0004,
            Win = 0x0008,
        }
    }
}

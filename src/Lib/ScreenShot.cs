using ScreenYu.Pkg.Win;

namespace ScreenYu.Lib
{
    internal static class ScreenShot {
        // Get and return the screenshot of primary screen
        public static Bitmap GetFullscreenBmp() {

            // Get the screen size the main form is on
            Utils.ThrowIfNull(Screen.PrimaryScreen);

            int cx = Screen.PrimaryScreen!.Bounds.Width;
            int cy = Screen.PrimaryScreen!.Bounds.Height;

            // get desktop dc (0)
            IntPtr hdcSrc = WinApi.GetDC(IntPtr.Zero);
            Utils.ThrowIfEqual(IntPtr.Zero, hdcSrc);

            IntPtr hdcDest = WinApi.CreateCompatibleDC(hdcSrc);
            Utils.ThrowIfEqual(IntPtr.Zero, hdcDest);

            IntPtr hBitmap = WinApi.CreateCompatibleBitmap(hdcSrc, cx, cy);
            Utils.ThrowIfEqual(IntPtr.Zero, hBitmap);

            IntPtr hOld = WinApi.SelectObject(hdcDest, hBitmap);
            Utils.ThrowIfEqual(IntPtr.Zero, hOld);

            Utils.ThrowIfEqual(0,
                WinApi.BitBlt(hdcDest, 0, 0, cx, cy, hdcSrc, 0, 0, (uint)CopyPixelOperation.SourceCopy)
            );
            Utils.ThrowIfEqual(IntPtr.Zero,
                WinApi.SelectObject(hdcDest, hOld)
            );
            Utils.ThrowIfEqual(0,
                WinApi.DeleteDC(hdcDest)
            );
            Utils.ThrowIfEqual(0,
                WinApi.ReleaseDC(IntPtr.Zero, hdcSrc)
            );

            var bmp = Image.FromHbitmap(hBitmap);

            Utils.ThrowIfEqual(0,
                WinApi.DeleteObject(hBitmap)
            );

            return bmp;
        }
    }
}

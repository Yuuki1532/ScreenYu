using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenYu {
    public static partial class Utils {
        public static void getScreenShot(ref Bitmap memoryImage) {
            // Get current screenshot

            // Get the screen the main form is on
            int cx, cy;
            cx = Screen.PrimaryScreen.Bounds.Width;
            cy = Screen.PrimaryScreen.Bounds.Height;

            // get desktop dc (0)
            IntPtr hdcSrc = GetDC(IntPtr.Zero);
            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, cx, cy);
            IntPtr hOld = SelectObject(hdcDest, hBitmap);
            BitBlt(hdcDest, 0, 0, cx, cy, hdcSrc, 0, 0, (UInt32)CopyPixelOperation.SourceCopy);
            SelectObject(hdcDest, hOld);
            DeleteDC(hdcDest);
            ReleaseDC(IntPtr.Zero, hdcSrc);
            memoryImage = Image.FromHbitmap(hBitmap);
            DeleteObject(hBitmap);

        }

        public static void startCapture(ref Bitmap memoryImage, Capture captureForm, IWin32Window owner) {
            // save handle of foreground window
            IntPtr fg_hWnd = GetForegroundWindow();
            getScreenShot(ref memoryImage);

            // show capture form
            captureForm.Cursor = Cursors.Cross;
            captureForm.showSelectForm(ref memoryImage, fg_hWnd, owner);
            SetForegroundWindow(captureForm.Handle);
        }

    }
}

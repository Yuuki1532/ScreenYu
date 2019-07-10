using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ScreenYu {
    public partial class main : Form {

        [DllImport("User32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);
        [DllImport("Gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);
        [DllImport("Gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdc, int x, int y,
            int cx, int cy, IntPtr hdcSrc, int x1, int y1, UInt32 rop);
        [DllImport("Gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);
        [DllImport("User32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("Gdi32.dll")]
        private static extern bool DeleteObject(IntPtr ho);

        [DllImport("User32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vk);
        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        private const int WM_HOTKEY = 0x0312;
        private new enum ModifierKeys {
            MOD_ALT = 0x0001,
            MOD_CONTROL = 0x0002,
            MOD_NOREPEAT = 0x4000,
            MOD_SHIFT = 0x0004,
            MOD_WIN = 0x0008
        }

        private capture captureForm;
        private Bitmap memoryImage;

        public main() {
            InitializeComponent();
            
            if (!RegisterHotKey(this.Handle, 0,
                (UInt32)ModifierKeys.MOD_CONTROL | (UInt32)ModifierKeys.MOD_ALT, (UInt32)Keys.A)) {
                MessageBox.Show("Unable to register for hotkey: Ctrl-Alt-A !", "Error");
                Environment.Exit(-1);
            }
            memoryImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            captureForm = new capture();

            notifyIcon1.Visible = true;
            
        }

        private void main_FormClosed(object sender, FormClosedEventArgs e) {
            UnregisterHotKey(this.Handle, 0);
            Application.Exit();
        }

        private void getScreenShot() {

            int cx, cy;
            cx = Screen.PrimaryScreen.Bounds.Width;
            cy = Screen.PrimaryScreen.Bounds.Height;

            IntPtr hdcSrc = GetDC(IntPtr.Zero);
            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, cx, cy);
            IntPtr hOld = SelectObject(hdcDest, hBitmap);
            BitBlt(hdcDest, 0, 0, cx, cy, hdcSrc, 0, 0, (UInt32)System.Drawing.CopyPixelOperation.SourceCopy);
            SelectObject(hdcDest, hOld);
            DeleteDC(hdcDest);
            ReleaseDC(IntPtr.Zero, hdcSrc);
            memoryImage = Image.FromHbitmap(hBitmap);
            DeleteObject(hBitmap);


        }

        protected override void WndProc(ref Message m) {

            switch (m.Msg) {

                case WM_HOTKEY:

                    if (((int)m.LParam & 0x0000FFFF) ==
                        ((int)ModifierKeys.MOD_CONTROL | (int)ModifierKeys.MOD_ALT)) {
                        if ((int)m.LParam >> 16 == (int)Keys.A) {
                            IntPtr fg_hWnd = GetForegroundWindow();
                            getScreenShot();
                            
                            captureForm.Cursor = Cursors.Cross;
                            captureForm.showSelectForm(memoryImage, fg_hWnd, this);
                            SetForegroundWindow(captureForm.Handle);
                            
                        }
                    }

                    break;
            }
            base.WndProc(ref m);
        }

        protected override bool ShowWithoutActivation {
            get {
                return true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }
        

    }
    
}

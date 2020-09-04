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


        private capture captureForm;
        private Bitmap memoryImage;

        public main() {
            InitializeComponent();
            
            if (!WinAPI.RegisterHotKey(this.Handle, 0,
                (UInt32)WinAPI.ModifierKeys.MOD_CONTROL | (UInt32)WinAPI.ModifierKeys.MOD_ALT, (UInt32)Keys.A)) {
                MessageBox.Show("Unable to register for hotkey: Ctrl-Alt-A !", "Error");
                Environment.Exit(-1);
            }
            memoryImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            captureForm = new capture();

            notifyIcon1.Visible = true;
            
        }

        private void main_FormClosed(object sender, FormClosedEventArgs e) {
            WinAPI.UnregisterHotKey(this.Handle, 0);
            Application.Exit();
        }

        private void getScreenShot() {

            int cx, cy;
            cx = Screen.PrimaryScreen.Bounds.Width;
            cy = Screen.PrimaryScreen.Bounds.Height;

            IntPtr hdcSrc = WinAPI.GetDC(IntPtr.Zero);
            IntPtr hdcDest = WinAPI.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = WinAPI.CreateCompatibleBitmap(hdcSrc, cx, cy);
            IntPtr hOld = WinAPI.SelectObject(hdcDest, hBitmap);
            WinAPI.BitBlt(hdcDest, 0, 0, cx, cy, hdcSrc, 0, 0, (UInt32)System.Drawing.CopyPixelOperation.SourceCopy);
            WinAPI.SelectObject(hdcDest, hOld);
            WinAPI.DeleteDC(hdcDest);
            WinAPI.ReleaseDC(IntPtr.Zero, hdcSrc);
            memoryImage = Image.FromHbitmap(hBitmap);
            WinAPI.DeleteObject(hBitmap);


        }

        protected override void WndProc(ref Message m) {

            switch (m.Msg) {

                case WinAPI.WM_HOTKEY:

                    if (((int)m.LParam & 0x0000FFFF) ==
                        ((int)WinAPI.ModifierKeys.MOD_CONTROL | (int)WinAPI.ModifierKeys.MOD_ALT)) {
                        if ((int)m.LParam >> 16 == (int)Keys.A) {
                            IntPtr fg_hWnd = WinAPI.GetForegroundWindow();
                            getScreenShot();
                            
                            captureForm.Cursor = Cursors.Cross;
                            captureForm.showSelectForm(memoryImage, fg_hWnd, this);
                            WinAPI.SetForegroundWindow(captureForm.Handle);
                            
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

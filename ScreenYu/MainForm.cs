using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenYu {
    public partial class MainForm : Form {

        // pre-defined hotkey
        private const uint hotKeyModifiers = (uint) WinAPI.ModifierKeys.MOD_CONTROL | (uint) WinAPI.ModifierKeys.MOD_ALT;
        private const uint hotKeyKey = (uint) Keys.A;
        private const int hotKeyId = 0;

        private CaptureForm captureForm;
        private Bitmap fullscreenBmp; // reuse memory

        public MainForm() {
            InitializeComponent();

            // register hotkey to Windows
            if (!WinAPI.RegisterHotKey(Handle, hotKeyId, hotKeyModifiers, hotKeyKey)) {
                MessageBox.Show("Unable to register for hotkey: Ctrl-Alt-A !", "Error");
                Environment.Exit(-1);
            }

            // create Bitmap of the screen size the main form is on
            fullscreenBmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            captureForm = new CaptureForm();
            
            mainNotifyIcon.Visible = true;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            WinAPI.UnregisterHotKey(Handle, hotKeyId);
            Application.Exit();
        }

        private void StartCapture() {
            captureForm.StartCapture(ref fullscreenBmp, this);
        }

        protected override void WndProc(ref Message m) {
            // callback function to receive windows hotkey message

            switch (m.Msg) {

                case WinAPI.WM_HOTKEY:

                    if (((int)m.LParam & 0x0000FFFF) ==
                        ((int)WinAPI.ModifierKeys.MOD_CONTROL | (int)WinAPI.ModifierKeys.MOD_ALT)) {
                        if ((int)m.LParam >> 16 == (int)Keys.A) {
                            captureForm.StartCapture(ref fullscreenBmp, this);

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

        private void mainNotifyIcon_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                captureForm.StartCapture(ref fullscreenBmp, this);
            }
        }


    }
    
}

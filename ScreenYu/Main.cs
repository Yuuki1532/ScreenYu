using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenYu {
    public partial class Main : Form {


        private Capture captureForm;
        private Bitmap memoryImage; // reuse memory

        public Main() {
            InitializeComponent();

            // Register hotkey to Windows
            if (!Utils.RegisterHotKey(this.Handle, 0,
                    (UInt32)Utils.WinAPI_ModifierKeys.MOD_CONTROL | (UInt32)Utils.WinAPI_ModifierKeys.MOD_ALT, (UInt32)Keys.A)) {
                MessageBox.Show("Unable to register for hotkey: Ctrl-Alt-A !", "Error");
                Environment.Exit(-1);
            }

            memoryImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            captureForm = new Capture();

            notifyIcon1.Visible = true;
            
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e) {
            Utils.UnregisterHotKey(this.Handle, 0);
            Application.Exit();
        }

        private void startCapture() {
            Utils.startCapture(ref memoryImage, captureForm, this);
        }

        protected override void WndProc(ref Message m) {
            // callback function to receive windows hotkey message

            switch (m.Msg) {

                case Utils.WM_HOTKEY:

                    if (((int)m.LParam & 0x0000FFFF) ==
                        ((int)Utils.WinAPI_ModifierKeys.MOD_CONTROL | (int)Utils.WinAPI_ModifierKeys.MOD_ALT)) {
                        if ((int)m.LParam >> 16 == (int)Keys.A) {
                            startCapture();

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

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                startCapture();
            }

        }
    }
    
}

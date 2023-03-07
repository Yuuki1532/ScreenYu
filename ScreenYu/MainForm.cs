namespace ScreenYu {
    public partial class MainForm : Form {

        private const int hotKeyId = 0;

        private CaptureForm captureForm;
        private Bitmap fullscreenBmp; // reuse memory

        public MainForm() {
            InitializeComponent();

            uint hotKeyModifiers = (uint)WinAPI.ModifierKeys.MOD_NOREPEAT;
            string hotKeyInString = "";
            if (Config.HotKey.WinKey) {
                hotKeyModifiers |= (uint)WinAPI.ModifierKeys.MOD_WIN;
                hotKeyInString += "Win-";
            }
            if (Config.HotKey.Control) {
                hotKeyModifiers |= (uint)WinAPI.ModifierKeys.MOD_CONTROL;
                hotKeyInString += "Ctrl-";
            }
            if (Config.HotKey.Alt) {
                hotKeyModifiers |= (uint)WinAPI.ModifierKeys.MOD_ALT;
                hotKeyInString += "Alt-";
            }
            if (Config.HotKey.Shift) {
                hotKeyModifiers |= (uint)WinAPI.ModifierKeys.MOD_SHIFT;
                hotKeyInString += "Shift-";
            }
            hotKeyInString += Enum.GetName(typeof(Keys), Config.HotKey.Key);


            // register hotkey to Windows
            if (!WinAPI.RegisterHotKey(Handle, hotKeyId, hotKeyModifiers, Config.HotKey.Key)) {
                MessageBox.Show($"Unable to register for hotkey: {hotKeyInString}!", "Error");
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

        protected override void WndProc(ref Message m) {
            // callback function to receive windows hotkey message

            switch (m.Msg) {

                case WinAPI.WM_HOTKEY:

                    //if (((int)m.LParam & 0x0000FFFF) ==
                    //    ((int)WinAPI.ModifierKeys.MOD_CONTROL | (int)WinAPI.ModifierKeys.MOD_ALT)) {
                    //    if ((int)m.LParam >> 16 == (int)Keys.A) {
                    //        captureForm.StartCapture(ref fullscreenBmp, this);

                    //    }
                    //}

                    // since only one hotkey is registered, no need to check

                    captureForm.StartCapture(ref fullscreenBmp, this);
                    break;
            }
            base.WndProc(ref m);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }

        private void mainNotifyIcon_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                captureForm.StartCapture(ref fullscreenBmp, this);
            }
        }

        protected override bool ShowWithoutActivation {
            get {
                return true;
            }
        }


    }
}

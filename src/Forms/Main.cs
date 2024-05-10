namespace ScreenYu.Forms {
    delegate void WindowsMessageHandler(ref Message msg);

    internal partial class Main : Form {
        private WindowsMessageHandler _wndProc;

        public Main(WindowsMessageHandler wndProc) {
            _wndProc = wndProc;
            InitializeComponent();
        }

        // callback function to receive windows hotkey message
        protected override void WndProc(ref Message msg) {
            _wndProc(ref msg);
            base.WndProc(ref msg);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        protected override bool ShowWithoutActivation {
            get {
                return true;
            }
        }
    }
}

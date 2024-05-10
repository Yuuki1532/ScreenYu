using ScreenYu.Pkg.Win;

namespace ScreenYu.App {
    internal partial class App {
        private IntPtr PreviousFgHwnd;

        private void StartScreenShot() {
            if (MainForm.Visible) { // already in process
                return;
            }

            // save handle of (old) foreground window
            PreviousFgHwnd = WinApi.GetForegroundWindow();
            InitialScreenshot = Lib.ScreenShot.GetFullscreenBmp();

            // new selection rect
            SelectionRect = new(new SolidBrush(Color.FromArgb(Convert.ToInt32(Config.SelectionConfig.DimOutsideSelection * 255), 0, 0, 0)));

            // raise start event
            ScreenShotStarted?.Invoke(this, new() {
                ScreenShot = InitialScreenshot,
                SelectionRect = SelectionRect,
                Form = MainForm,
                DrawingManager = DrawingManager,
            });

            MainForm.Size = InitialScreenshot.Size;
            MainForm.Cursor = Cursors.Cross;

            // show form
            MainForm.Show();
            _ = WinApi.SetForegroundWindow(MainForm.Handle);
        }

        private void CommitScreenShot() {
            if (InitialScreenshot == null || SelectionRect == null) {
                return;
            }

            using (Graphics g = Graphics.FromImage(InitialScreenshot)) {
                DrawingManager.PaintAll(g);
            }

            if (SelectionRect.IsEmpty()) { // ignore empty selection
                return;
            }

            SelectionRect.OrderXY();

            Clipboard.SetImage(InitialScreenshot.Clone(
                new Rectangle(
                    SelectionRect.X1, SelectionRect.Y1,
                    SelectionRect.X2 - SelectionRect.X1 + 1, SelectionRect.Y2 - SelectionRect.Y1 + 1
                ), InitialScreenshot.PixelFormat
            ));
        }

        private void EndScreenShot() {
            // hide form
            MainForm.Hide();

            // raise end event
            ScreenShotEnded?.Invoke(this, new());

            if (PreviousFgHwnd != IntPtr.Zero) {
                _ = WinApi.SetForegroundWindow(PreviousFgHwnd);
            }
            PreviousFgHwnd = IntPtr.Zero;
        }

    }
}

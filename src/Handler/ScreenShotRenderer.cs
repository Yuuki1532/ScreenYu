using ScreenYu.App;

namespace ScreenYu.Handler {
    internal class ScreenShotRenderer : EmptyHandler, IScreenShotEventHandler {
        private Bitmap? _screenShot;
        public Bitmap? ScreenShot { get => _screenShot; }

        public ScreenShotRenderer() {
            _screenShot = null;
        }

        public override void Paint(Context.I ctx, PaintEventArgs e) {
            if (_screenShot == null) {
                return;
            }
            
            Graphics g = e.Graphics;
            g.DrawImage(_screenShot, 0, 0);
            ctx.SharedContext().SelectionRect.PaintTo(g, _screenShot.Width, _screenShot.Height);
        }

        public void OnScreenShotStarted(object? sender, ScreenShotEventArgs e) {
            _screenShot = e.ScreenShot!;
        }

        public void OnScreenShotEnded(object? sender, ScreenShotEventArgs e) {
            _screenShot = null;
        }
    }
}

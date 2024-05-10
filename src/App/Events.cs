using ScreenYu.Handler.Common;

namespace ScreenYu.App {
    internal class ScreenShotEventArgs : EventArgs {
        public Bitmap? ScreenShot;
        public SelectionRect? SelectionRect;
        public Form? Form;
        public Drawing.Manager.I? DrawingManager;

        public ScreenShotEventArgs() { }
    }

    internal partial class App {
        private event EventHandler<ScreenShotEventArgs>? ScreenShotStarted;
        private event EventHandler<ScreenShotEventArgs>? ScreenShotEnded;
    }
    internal interface IScreenShotEventHandler {
        public void OnScreenShotStarted(object? sender, ScreenShotEventArgs e);
        public void OnScreenShotEnded(object? sender, ScreenShotEventArgs e);
    }
}

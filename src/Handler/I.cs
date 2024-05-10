using ScreenYu.Handler.Common;

namespace ScreenYu.Handler {
    internal interface I {
        void OnNewSharedContext(SharedContext sharedCtx);
        void MouseDown(Context.I ctx, MouseEventArgs e);
        void MouseUp(Context.I ctx, MouseEventArgs e);
        void MouseMove(Context.I ctx, MouseEventArgs e);
        void MouseDoubleClick(Context.I ctx, MouseEventArgs e);
        void MouseWheel(Context.I ctx, MouseEventArgs e);
        void KeyDown(Context.I ctx, KeyEventArgs e);
        void KeyUp(Context.I ctx, KeyEventArgs e);
        void Paint(Context.I ctx, PaintEventArgs e);
    }
}

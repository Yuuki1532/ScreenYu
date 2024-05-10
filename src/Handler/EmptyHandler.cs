using ScreenYu.Handler.Common;

namespace ScreenYu.Handler {
    internal class EmptyHandler : I {
        public virtual void OnNewSharedContext(SharedContext sharedCtx) { }
        public virtual void KeyDown(Context.I ctx, KeyEventArgs e) { }
        public virtual void KeyUp(Context.I ctx, KeyEventArgs e) { }
        public virtual void MouseDown(Context.I ctx, MouseEventArgs e) { }
        public virtual void MouseMove(Context.I ctx, MouseEventArgs e) { }
        public virtual void MouseDoubleClick(Context.I ctx, MouseEventArgs e) { }
        public virtual void MouseUp(Context.I ctx, MouseEventArgs e) { }
        public virtual void MouseWheel(Context.I ctx, MouseEventArgs e) { }
        public virtual void Paint(Context.I ctx, PaintEventArgs e) { }
    }
}

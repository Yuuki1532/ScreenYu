namespace ScreenYu.Handler {
    internal class DrawingManagerRenderer : EmptyHandler {
        private Drawing.Manager.I DrawingManager;

        public DrawingManagerRenderer(Drawing.Manager.I drawingManager) {
            DrawingManager = drawingManager;
        }

        public override void Paint(Context.I ctx, PaintEventArgs e) {
            DrawingManager.PaintAll(e.Graphics);
        }
    }
}

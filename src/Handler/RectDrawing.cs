using ScreenYu.Handler.Common;

namespace ScreenYu.Handler {
    internal class RectDrawing : EmptyHandler, IDispatcherHandler, IResetHandler {
        private Drawing.Rect? ObjectInProgress;
        private Pen BorderPen;

        public RectDrawing() {
            ObjectInProgress = null;
            BorderPen = new(Color.White, 1); // just init
        }

        public override void MouseDown(Context.I ctx, MouseEventArgs e) {
            var drawingManager = ctx.SharedContext().DrawingManager;

            ObjectInProgress = new(drawingManager.GetStrokeColor(), drawingManager.GetStrokeSize()) {
                X1 = e.X,
                X2 = e.X,
                Y1 = e.Y,
                Y2 = e.Y,
            };
        }

        public override void MouseMove(Context.I ctx, MouseEventArgs e) {
            if (ObjectInProgress == null) return;

            ObjectInProgress.X2 = e.X;
            ObjectInProgress.Y2 = e.Y;

            ctx.SharedContext().Form.Refresh();
        }

        public override void MouseUp(Context.I ctx, MouseEventArgs e) {
            var drawingManager = ctx.SharedContext().DrawingManager;

            if (!ObjectInProgress?.IsEmpty() ?? false) {
                drawingManager.Append(ObjectInProgress);
            }
            ObjectInProgress = null;
        }

        public override void Paint(Context.I ctx, PaintEventArgs e) {
            var drawingManager = ctx.SharedContext().DrawingManager;
            var selectionRect = ctx.SharedContext().SelectionRect;

            BorderPen.Color = drawingManager.GetStrokeColor();
            BorderPen.Width = drawingManager.GetStrokeSize();

            DrawingUtils.DrawSelectionBorder(e.Graphics, selectionRect, BorderPen);
            ObjectInProgress?.PaintTo(e.Graphics);
        }

        public void OnEnter(Context.I ctx) {
            ctx.SharedContext().Form.Cursor = Cursors.Cross;
        }

        public void OnLeave(Context.I? ctx) {
            ObjectInProgress = null;
        }

        public void Reset(Context.I ctx) {
            ObjectInProgress = null;
        }
    }
}
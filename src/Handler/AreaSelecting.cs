using ScreenYu.Handler.Common;

namespace ScreenYu.Handler {
    internal class AreaSelecting : EmptyHandler, IDispatcherHandler, IResetHandler {
        private enum ControlPoints {
            None, New,
            TopLeft, Top, TopRight,
            Left, Inside, Right,
            BottomLeft, Bottom, BottomRight,
        }

        private int ControlPointSensitivity;
        private ControlPoints CurrentControlPoint;       // modifying selection
        private int AnchorX, AnchorY;                    // selecting
        private Pen BorderPen;

        public AreaSelecting(Config config, int controlPointSensitivity) {
            ControlPointSensitivity = controlPointSensitivity;
            CurrentControlPoint = ControlPoints.None;
            BorderPen = new Pen(config.SelectionConfig.SelectionBorderColor, config.SelectionConfig.SelectionBorderStrokeSize) {
                Alignment = System.Drawing.Drawing2D.PenAlignment.Center,
            };
        }

        public override void MouseDown(Context.I ctx, MouseEventArgs e) {
            
            if (e.Button == MouseButtons.Left) {
                var selection = ctx.SharedContext().SelectionRect;
                var form = ctx.SharedContext().Form;

                CurrentControlPoint = SetCursor(form, selection, e.X, e.Y);

                if (CurrentControlPoint == ControlPoints.None) {
                    CurrentControlPoint = ControlPoints.New;
                    selection.X1 = selection.X2 = e.X;
                    selection.Y1 = selection.Y2 = e.Y;
                    return;
                }

                // make x2, y2 be the point to be modifying (except for moving selection)
                if (CurrentControlPoint == ControlPoints.TopLeft ||
                    CurrentControlPoint == ControlPoints.Left ||
                    CurrentControlPoint == ControlPoints.BottomLeft) {
                    SetMinMax(ref selection.X2, ref selection.X1); // set x2 to the smaller value
                }
                if (CurrentControlPoint == ControlPoints.TopRight ||
                    CurrentControlPoint == ControlPoints.Right ||
                    CurrentControlPoint == ControlPoints.BottomRight) {
                    SetMinMax(ref selection.X1, ref selection.X2); // set x2 to the larger value
                }
                if (CurrentControlPoint == ControlPoints.TopLeft ||
                    CurrentControlPoint == ControlPoints.Top ||
                    CurrentControlPoint == ControlPoints.TopRight) {
                    SetMinMax(ref selection.Y2, ref selection.Y1); // set y2 to the smaller value
                }
                if (CurrentControlPoint == ControlPoints.BottomLeft ||
                    CurrentControlPoint == ControlPoints.Bottom ||
                    CurrentControlPoint == ControlPoints.BottomRight) {
                    SetMinMax(ref selection.Y1, ref selection.Y2); // set y2 to the larger value
                }
                if (CurrentControlPoint == ControlPoints.Inside) {
                    selection.OrderXY();
                    AnchorX = e.X;
                    AnchorY = e.Y;
                }
            }
        }
        public override void MouseMove(Context.I ctx, MouseEventArgs e) {
            var selection = ctx.SharedContext().SelectionRect;
            var form = ctx.SharedContext().Form;

            if (CurrentControlPoint == ControlPoints.None) {
                SetCursor(form, selection, e.X, e.Y);
                return;
            }
            if (CurrentControlPoint == ControlPoints.New) {
                selection.X2 = e.X;
                selection.Y2 = e.Y;
            }
            if (CurrentControlPoint == ControlPoints.TopLeft ||
                CurrentControlPoint == ControlPoints.TopRight ||
                CurrentControlPoint == ControlPoints.Left ||
                CurrentControlPoint == ControlPoints.Right ||
                CurrentControlPoint == ControlPoints.BottomLeft ||
                CurrentControlPoint == ControlPoints.BottomRight) {
                selection.X2 = e.X;
            }
            if (CurrentControlPoint == ControlPoints.TopLeft ||
                CurrentControlPoint == ControlPoints.TopRight ||
                CurrentControlPoint == ControlPoints.Top ||
                CurrentControlPoint == ControlPoints.Bottom ||
                CurrentControlPoint == ControlPoints.BottomLeft ||
                CurrentControlPoint == ControlPoints.BottomRight) {
                selection.Y2 = e.Y;
            }
            if (CurrentControlPoint == ControlPoints.Inside) {
                int
                    dx = e.X - AnchorX,
                    dy = e.Y - AnchorY;

                int
                    selectionWidth = selection.X2 - selection.X1,
                    selectionHeight = selection.Y2 - selection.Y1,
                    screenShotWidth = ctx.SharedContext().ScreenShotWidth,
                    screenShotHeight = ctx.SharedContext().ScreenShotHeight;

                selection.X1 += dx;
                selection.X2 += dx;
                selection.Y1 += dy;
                selection.Y2 += dy;

                // check if left margin out of range
                if (selection.X1 < 0) {
                    selection.X1 = 0;
                    selection.X2 = selectionWidth;
                }
                // check if right margin out of range
                else if (selection.X2 >= screenShotWidth) {
                    selection.X2 = screenShotWidth - 1;
                    selection.X1 = selection.X2 - selectionWidth;
                }
                
                // check if top margin out of range
                if (selection.Y1 < 0) {
                    selection.Y1 = 0;
                    selection.Y2 = selectionHeight;
                }
                // check if bottom margin out of range
                else if (selection.Y2 >= screenShotHeight) {
                    selection.Y2 = screenShotHeight - 1;
                    selection.Y1 = selection.Y2 - selectionHeight;
                }

                AnchorX = e.X;
                AnchorY = e.Y;
            }

            form.Refresh();
        }
        public override void MouseUp(Context.I ctx, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                if (CurrentControlPoint == ControlPoints.None) {
                    return;
                }
                ctx.SharedContext().SelectionRect.OrderXY();
                CurrentControlPoint = ControlPoints.None;
            }
        }
        public override void MouseDoubleClick(Context.I ctx, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                var selection = ctx.SharedContext().SelectionRect;

                selection.X1 = selection.Y1 = 0;
                selection.X2 = ctx.SharedContext().ScreenShotWidth - 1;
                selection.Y2 = ctx.SharedContext().ScreenShotHeight - 1;

                ctx.SharedContext().Form.Refresh();
            }
        }
        public override void Paint(Context.I ctx, PaintEventArgs e) {
            DrawingUtils.DrawSelectionBorder(e.Graphics, ctx.SharedContext().SelectionRect, BorderPen);
        }

        private static void SetMinMax(ref int setThisToMin, ref int setThisToMax) {
            if (setThisToMin > setThisToMax) {
                (setThisToMin, setThisToMax) = (setThisToMax, setThisToMin);
            }
        }

        private ControlPoints SetCursor(Form form, SelectionRect selectionRect, int mouseX, int mouseY) {
            if (selectionRect.IsEmpty()) {
                form.Cursor = Cursors.Cross;
                return ControlPoints.None;
            }
            
            int
                minX = Math.Min(selectionRect.X1, selectionRect.X2),
                minY = Math.Min(selectionRect.Y1, selectionRect.Y2),
                maxX = Math.Max(selectionRect.X1, selectionRect.X2),
                maxY = Math.Max(selectionRect.Y1, selectionRect.Y2);

            bool
                isInMinX = Math.Abs(mouseX - minX) <= ControlPointSensitivity,
                isInMinY = Math.Abs(mouseY - minY) <= ControlPointSensitivity,
                isInMaxX = Math.Abs(mouseX - maxX) <= ControlPointSensitivity,
                isInMaxY = Math.Abs(mouseY - maxY) <= ControlPointSensitivity,
                isInWidth = minX < mouseX && mouseX < maxX,
                isInHeight = minY < mouseY && mouseY < maxY,
                isInsideSelection = mouseX > minX && mouseX < maxX && mouseY > minY && mouseY < maxY;


            if (isInMinY && isInMinX) { // top left
                form.Cursor = Cursors.SizeNWSE;
                return ControlPoints.TopLeft;
            }
            else if (isInMinY && isInMaxX) { // top right
                form.Cursor = Cursors.SizeNESW;
                return ControlPoints.TopRight;
            }
            else if (isInMaxY && isInMinX) { // bottom left
                form.Cursor = Cursors.SizeNESW;
                return ControlPoints.BottomLeft;
            }
            else if (isInMaxY && isInMaxX) { // bottom right
                form.Cursor = Cursors.SizeNWSE;
                return ControlPoints.BottomRight;
            }
            else if (isInMinY && isInWidth) { // top
                form.Cursor = Cursors.SizeNS;
                return ControlPoints.Top;
            }
            else if (isInMaxY && isInWidth) { // bottom
                form.Cursor = Cursors.SizeNS;
                return ControlPoints.Bottom;
            }
            else if (isInMinX && isInHeight) { // left
                form.Cursor = Cursors.SizeWE;
                return ControlPoints.Left;
            }
            else if (isInMaxX && isInHeight) { // right
                form.Cursor = Cursors.SizeWE;
                return ControlPoints.Right;
            }
            else if (isInsideSelection) { // inside
                form.Cursor = Cursors.SizeAll;
                return ControlPoints.Inside;
            }
            else { // outside
                form.Cursor = Cursors.Cross;
                return ControlPoints.None;
            }

        }

        public void OnEnter(Context.I ctx) {
            CurrentControlPoint = ControlPoints.None;
            ctx.SharedContext().Form.Cursor = Cursors.Cross;
        }

        public void OnLeave(Context.I? ctx) { }

        public void Reset(Context.I ctx) {
            var selection = ctx.SharedContext().SelectionRect;
            selection.Reset();
            CurrentControlPoint = ControlPoints.None;
        }
    }
}

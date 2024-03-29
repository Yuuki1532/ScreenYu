﻿namespace ScreenYu {
    public partial class CaptureForm {

        private const int CP_SENSITIVITY = 5;

        private enum ControlPoints {
            None,
            TopLeft, Top, TopRight,
            Left, Inside, Right,
            BottomLeft, Bottom, BottomRight,
        }

        private enum SelectionEditState {
            NoSelection,        // no selections yet
            Selecting,          // currently making a new selection
            Selected,           // selection exists
            EditingSelection,   // modifying selection

            DrawingMode = 8,    // any drawing mode has this bit set
            DrawingRectMode,    // drawing rect mode
            DrawingLineMode,    // drawing line mode

            Drawing = 16,       // any drawing has this bit set
            DrawingRect,        // drawing rect
            DrawingLine,        // drawing line
        }

        private class Selection {
            public int x_anchor, y_anchor, x_cursor, y_cursor;  // selecting
            public ControlPoints EditingCP;                     // modifying selection
            public int x1, y1, x2, y2;                          // selection
            public Pen SelectionPen;                            // selection mode pen
            public Brush SelectionBrush;                        // selection dim brush
            public Rectangle _rect;                             // reuse in OnPaint

            public Selection() {
                SelectionPen = new Pen(Config.Selection.SelectionBorderColor, Config.Selection.SelectionBorderStrokeSize) {
                    Alignment = System.Drawing.Drawing2D.PenAlignment.Center,
                };
                SelectionBrush = new SolidBrush(Color.FromArgb(Convert.ToInt32(Config.Selection.DimOutsideSelection * 255), 0, 0, 0));
                _rect = new Rectangle();
            }

            public void SetSelectingVariables(int x_anchor, int y_anchor, int x_cursor, int y_cursor) {
                this.x_anchor = x_anchor;
                this.y_anchor = y_anchor;
                this.x_cursor = x_cursor;
                this.y_cursor = y_cursor;
            }

        }

        private class DrawingObjects {
            public List<Drawing.Object> ObjectList;
            public List<Color> ColorList;
            public int CurrentColorId;
            public float CurrentStrokeSize;

            public DrawingObjects() {
                ObjectList = new List<Drawing.Object>();
                ColorList = Config.Drawing.ColorList;
                CurrentColorId = Config.Drawing.DefaultColorId;
                CurrentStrokeSize = Config.Drawing.DefaultStrokeSize;
            }

        }

        private Bitmap? fullscreenBmp;
        private IntPtr previousFg_hWnd = IntPtr.Zero; // handle of last focus window

        private SelectionEditState seState;
        private bool showHint = false; // show hint (e.g., key bindings) on screen

        private string hintString = string.Format(
            "Cancel: {0}\nReset: {1}\n\nMode: Selection: {2}\n\nMode: Drawing\n\tRectangle: {3}\n\tLine: {4}\n\tStroke Color: 0 to 9\n\tStroke Size: Mouse Wheel Up/Down\n\tUndo: Ctrl-Z",
            Config.KeyBinding.Cancel,
            Config.KeyBinding.Reset,
            Config.KeyBinding.Select,
            Config.KeyBinding.DrawRect,
            Config.KeyBinding.DrawLine
        );


        private Selection selection = new();
        private DrawingObjects drawingObjects = new();


        private static void SetMinMax(ref int setThisToMin, ref int setThisToMax) {
            if (setThisToMin > setThisToMax) {
                var tmp = setThisToMin;
                setThisToMin = setThisToMax;
                setThisToMax = tmp;
            }
        }

        private static void GetFullscreenBmp(ref Bitmap fullscreenBmp) {
            // Get current screenshot and save it to memoryImage

            // Get the screen size the main form is on
            int cx, cy;
            cx = Screen.PrimaryScreen.Bounds.Width;
            cy = Screen.PrimaryScreen.Bounds.Height;

            // get desktop dc (0)
            IntPtr hdcSrc = WinAPI.GetDC(IntPtr.Zero);
            IntPtr hdcDest = WinAPI.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = WinAPI.CreateCompatibleBitmap(hdcSrc, cx, cy);
            IntPtr hOld = WinAPI.SelectObject(hdcDest, hBitmap);

            WinAPI.BitBlt(hdcDest, 0, 0, cx, cy, hdcSrc, 0, 0, (UInt32)CopyPixelOperation.SourceCopy);
            WinAPI.SelectObject(hdcDest, hOld);
            WinAPI.DeleteDC(hdcDest);
            WinAPI.ReleaseDC(IntPtr.Zero, hdcSrc);

            fullscreenBmp = Image.FromHbitmap(hBitmap);

            WinAPI.DeleteObject(hBitmap);

        }

        public void StartCapture(ref Bitmap fullscreenBmp, IWin32Window owner) {

            if (Visible) { // already in capture mode
                return;
            }

            // save handle of (old) foreground window
            IntPtr fg_hWnd = WinAPI.GetForegroundWindow();
            GetFullscreenBmp(ref fullscreenBmp);

            // show capture form
            Size = fullscreenBmp.Size;
            Cursor = Cursors.Cross;
            ShowCaptureForm(fullscreenBmp, fg_hWnd, owner);
            WinAPI.SetForegroundWindow(Handle);
        }

        private void ResetSelectionVariables() {
            seState = SelectionEditState.NoSelection;
            drawingObjects.ObjectList.Clear();
        }

        public void ShowCaptureForm(Bitmap bmp, IntPtr fg_hWnd, IWin32Window owner) {
            ResetSelectionVariables();

            fullscreenBmp = bmp;
            previousFg_hWnd = fg_hWnd;
            Show(owner);
        }

        private void EndCapture() {
            fullscreenBmp!.Dispose(); // fullscreenBmp is guaranteed non-null during capturing
            fullscreenBmp = null;
            Hide();
            if (previousFg_hWnd != IntPtr.Zero) {
                WinAPI.SetForegroundWindow(previousFg_hWnd);
            }

            //this.Owner.Show();
        }

        private bool CommitCapture() {

            if (seState == SelectionEditState.NoSelection) {
                EndCapture();
                return false;
            }

            SetMinMax(ref selection.x1, ref selection.x2);
            SetMinMax(ref selection.y1, ref selection.y2);

            if (selection.x1 < 0 ||
                selection.y1 < 0 ||
                selection.x2 >= fullscreenBmp!.Width ||
                selection.y2 >= fullscreenBmp!.Height) { // fullscreenBmp is guaranteed non-null during capturing

                MessageBox.Show("Selection out of range!", "Error");
                EndCapture();
                return false;
            }


            using (Graphics g = Graphics.FromImage(fullscreenBmp)) {

                foreach (Drawing.Object obj in drawingObjects.ObjectList) {
                    obj.PaintTo(g);
                }

            }


            Clipboard.SetImage(fullscreenBmp.Clone(
                new Rectangle(selection.x1, selection.y1,
                    selection.x2 - selection.x1 + 1, selection.y2 - selection.y1 + 1),
                fullscreenBmp.PixelFormat));

            EndCapture();
            return true;
        }

        private ControlPoints SetCursor(int mouseX, int mouseY) {

            int
                minX = Math.Min(selection.x1, selection.x2),
                minY = Math.Min(selection.y1, selection.y2),
                maxX = Math.Max(selection.x1, selection.x2),
                maxY = Math.Max(selection.y1, selection.y2);

            bool
                isInMinX = Math.Abs(mouseX - minX) <= CP_SENSITIVITY,
                isInMinY = Math.Abs(mouseY - minY) <= CP_SENSITIVITY,
                isInMaxX = Math.Abs(mouseX - maxX) <= CP_SENSITIVITY,
                isInMaxY = Math.Abs(mouseY - maxY) <= CP_SENSITIVITY,
                isInWidth = minX < mouseX && mouseX < maxX,
                isInHeight = minY < mouseY && mouseY < maxY,
                isInsideSelection = mouseX > minX && mouseX < maxX && mouseY > minY && mouseY < maxY;


            if (isInMinY && isInMinX) { // top left
                this.Cursor = Cursors.SizeNWSE;
                return ControlPoints.TopLeft;
            }
            else if (isInMinY && isInMaxX) { // top right
                this.Cursor = Cursors.SizeNESW;
                return ControlPoints.TopRight;
            }
            else if (isInMaxY && isInMinX) { // bottom left
                this.Cursor = Cursors.SizeNESW;
                return ControlPoints.BottomLeft;
            }
            else if (isInMaxY && isInMaxX) { // bottom right
                this.Cursor = Cursors.SizeNWSE;
                return ControlPoints.BottomRight;
            }
            else if (isInMinY && isInWidth) { // top
                this.Cursor = Cursors.SizeNS;
                return ControlPoints.Top;
            }
            else if (isInMaxY && isInWidth) { // bottom
                this.Cursor = Cursors.SizeNS;
                return ControlPoints.Bottom;
            }
            else if (isInMinX && isInHeight) { // left
                this.Cursor = Cursors.SizeWE;
                return ControlPoints.Left;
            }
            else if (isInMaxX && isInHeight) { // right
                this.Cursor = Cursors.SizeWE;
                return ControlPoints.Right;
            }
            else if (isInsideSelection) { // inside
                this.Cursor = Cursors.SizeAll;
                return ControlPoints.Inside;
            }
            else { // outside
                this.Cursor = Cursors.Cross;
                return ControlPoints.None;
            }

        }



    }
}

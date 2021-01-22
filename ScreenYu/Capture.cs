using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenYu {
    public partial class Capture : Form {

        private struct RectFrame {
            public int x1, y1, x2, y2;
        }

        private enum ControlPoints {
            None,
            LeftTop, Top, RightTop,
            Left, Inside, Right,
            LeftBottom, Bottom, RightBottom
        }

        private enum SelectionEditState {
            NoSelection, // no selections yet
            Selecting, // currently making a new selection
            Selected, // a selection exists
            DrawingRectFrame, // drawing rect frame mode
            DrawingPath // drawing mode
        }

        private const int CP_SENSITIVITY = 5;
        private Keys rectFrameKey = Keys.F; // press F to enter / leave rect frame drawing mode
        // private Keys drawKey = Keys.D; // press D to enter drawing mode

        private Bitmap screenShot;
        private IntPtr old_hWnd = IntPtr.Zero; // handle of last focus window

        // private int x1 = -1, y1 = -1, x2 = -1, y2 = -1, old_x, old_y;
        private int x1 = -1, y1, x2, y2, old_x, old_y;
        private bool isClicking = false;
        private ControlPoints currentCP = ControlPoints.None;
        private SelectionEditState state = SelectionEditState.NoSelection;

        private Pen selectionPen;
        private Pen drawingModePen;
        private Brush dimBrush;
        private Rectangle selectionRect;
        private Pen rectFramePen;

        private List<RectFrame> rectFrameList;


        public Capture() {
            //SetProcessDPIAware();
            InitializeComponent();
            
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            
            selectionPen = new Pen(Color.FromArgb(30,120,180), 1.5f);
            selectionPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
            dimBrush = new SolidBrush(Color.FromArgb(Convert.ToInt32(0.3 * 255), 0, 0, 0));
            selectionRect = new Rectangle();

            rectFrameList = new List<RectFrame>();
            rectFramePen = new Pen(Color.FromArgb(255, 160, 0), 2.0f);
            drawingModePen = new Pen(Color.FromArgb(225, 20, 20), 1.5f);
        }

        public void showSelectForm(ref Bitmap b, IntPtr fg_hWnd, IWin32Window owner) {
            x1 = -1;
            isClicking = false;
            currentCP = ControlPoints.None;
            rectFrameList.Clear();
            state = SelectionEditState.NoSelection;

            screenShot = b;
            old_hWnd = fg_hWnd;
            this.Show(owner);
        }
        private void abortClip() {

            screenShot.Dispose();
            screenShot = null;
            this.Hide();
            if (old_hWnd != IntPtr.Zero)
                Utils.SetForegroundWindow(old_hWnd);

            //this.Owner.Show();

        }

        protected override void OnPaint(PaintEventArgs e) {
            if (screenShot == null) return;

            Graphics g = e.Graphics;
            g.DrawImage(screenShot, 0, 0);
            g.FillRectangle(dimBrush, 0, 0, screenShot.Width, screenShot.Height);
            
            
            if (x1 != -1) {
                
                selectionRect.X = Math.Min(x1, x2);
                selectionRect.Y = Math.Min(y1, y2);
                selectionRect.Width = Math.Abs(x2 - x1) + 1;
                selectionRect.Height = Math.Abs(y2 - y1) + 1;

                g.DrawImage(screenShot, selectionRect, selectionRect, GraphicsUnit.Pixel);
                
                Pen borderPen;
                if (state == SelectionEditState.DrawingRectFrame)
                    borderPen = drawingModePen;
                else
                    borderPen = selectionPen;

                g.DrawRectangle(borderPen,
                    Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x2 - x1), Math.Abs(y2 - y1));

                foreach (RectFrame frame in rectFrameList) {
                    g.DrawRectangle(rectFramePen,
                        Math.Min(frame.x1, frame.x2), Math.Min(frame.y1, frame.y2),
                        Math.Abs(frame.x2 - frame.x1), Math.Abs(frame.y2 - frame.y1));
                }
                

            }




            base.OnPaint(e);
        }

        private void Capture_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                abortClip();
                return;
            }

            if (e.KeyCode == rectFrameKey) {
                if (state == SelectionEditState.NoSelection)
                    return;
                if (isClicking)
                    return;

                if (state == SelectionEditState.DrawingRectFrame) { // already in drawing rect frame mode
                    state = SelectionEditState.Selected;
                }
                else {
                    state = SelectionEditState.DrawingRectFrame;
                }

                this.Cursor = Cursors.Cross;
                this.Refresh();
                return;
            }

            if (e.Control && e.KeyCode == Keys.Z){
                if (isClicking)
                    return;
                if (rectFrameList.Count == 0)
                    return;
                rectFrameList.RemoveAt(rectFrameList.Count - 1);
                this.Refresh();
            }

        }

        private void Capture_MouseDown(object sender, MouseEventArgs e) {
            int tx1, ty1, tx2, ty2; // tmp x, y

            if (e.Button == MouseButtons.Left) {
                isClicking = true;

                if (state == SelectionEditState.DrawingRectFrame) { // is drawing rect frame
                    if (e.X < Math.Min(x1, x2) || e.X > Math.Max(x1, x2) || e.Y < Math.Min(y1, y2) || e.Y > Math.Max(y1, y2)) {
                        // start position out of range, cancel current clicking
                        isClicking = false;
                        return;
                    }
                    RectFrame frame = new RectFrame();
                    frame.x1 = e.X;
                    frame.y1 = e.Y;
                    rectFrameList.Add(frame);
                }
                else { // selection

                    switch (currentCP) {
                        case ControlPoints.None: // first/re select
                            state = SelectionEditState.Selecting;
                            rectFrameList.Clear();
                            x1 = x2 = old_x = e.X;
                            y1 = y2 = old_y = e.Y;
                            break;
                        case ControlPoints.Top:
                        case ControlPoints.Left:
                        case ControlPoints.LeftTop: // modifying selection
                            tx2 = Math.Min(x1, x2);
                            ty2 = Math.Min(y1, y2);
                            tx1 = Math.Max(x1, x2);
                            ty1 = Math.Max(y1, y2);
                            x1 = tx1;
                            y1 = ty1;
                            x2 = tx2;
                            y2 = ty2;
                            break;
                        case ControlPoints.Bottom:
                        case ControlPoints.Right:
                        case ControlPoints.RightBottom:
                            tx2 = Math.Max(x1, x2);
                            ty2 = Math.Max(y1, y2);
                            tx1 = Math.Min(x1, x2);
                            ty1 = Math.Min(y1, y2);
                            x1 = tx1;
                            y1 = ty1;
                            x2 = tx2;
                            y2 = ty2;
                            break;
                        case ControlPoints.LeftBottom:
                            tx2 = Math.Min(x1, x2);
                            ty2 = Math.Max(y1, y2);
                            tx1 = Math.Max(x1, x2);
                            ty1 = Math.Min(y1, y2);
                            x1 = tx1;
                            y1 = ty1;
                            x2 = tx2;
                            y2 = ty2;
                            break;
                        case ControlPoints.RightTop:
                            tx2 = Math.Max(x1, x2);
                            ty2 = Math.Min(y1, y2);
                            tx1 = Math.Min(x1, x2);
                            ty1 = Math.Max(y1, y2);
                            x1 = tx1;
                            y1 = ty1;
                            x2 = tx2;
                            y2 = ty2;
                            break;
                        case ControlPoints.Inside:
                            old_x = e.X;
                            old_y = e.Y;
                            break;
                    }


                }

            }
            else if (e.Button == MouseButtons.Right) {

                if (state == SelectionEditState.NoSelection) {
                    abortClip();
                    return;
                }

                commitClip();

            }
        }

        private void Capture_MouseMove(object sender, MouseEventArgs e) {


            if (isClicking) {
                if (state == SelectionEditState.DrawingRectFrame) {
                    RectFrame frame = rectFrameList[rectFrameList.Count - 1];
                    if (e.X < Math.Min(x1, x2))
                        frame.x2 = Math.Min(x1, x2);
                    else if (e.X > Math.Max(x1, x2))
                        frame.x2 = Math.Max(x1, x2);
                    else
                        frame.x2 = e.X;

                    if (e.Y < Math.Min(y1, y2))
                        frame.y2 = Math.Min(y1, y2);
                    else if (e.Y > Math.Max(y1, y2))
                        frame.y2 = Math.Max(y1, y2);
                    else
                        frame.y2 = e.Y;
                    rectFrameList[rectFrameList.Count - 1] = frame;
                }
                else if (currentCP == ControlPoints.None) { // first/re select
                    /*x2 += e.X - old_x;
                    y2 += e.Y - old_y;
                    old_x = e.X;
                    old_y = e.Y;*/
                    x2 = e.X;
                    y2 = e.Y;
                }
                else { // modifying selection
                    if (currentCP == ControlPoints.LeftTop ||
                        currentCP == ControlPoints.LeftBottom ||
                        currentCP == ControlPoints.RightTop ||
                        currentCP == ControlPoints.RightBottom) {

                        x2 = e.X;
                        y2 = e.Y;
                    }

                    else if (currentCP == ControlPoints.Left ||
                            currentCP == ControlPoints.Right) {

                        x2 = e.X;
                    }
                    else if (currentCP == ControlPoints.Top ||
                            currentCP == ControlPoints.Bottom) {

                        y2 = e.Y;
                    }
                    else if (currentCP == ControlPoints.Inside) {
                        int dx, dy, minX, minY, maxX, maxY;
                        dx = e.X - old_x;
                        dy = e.Y - old_y;
                        minX = Math.Min(x1, x2);
                        minY = Math.Min(y1, y2);
                        maxX = Math.Max(x1, x2);
                        maxY = Math.Max(y1, y2);

                        if (minX + dx < 0) {
                            dx = -minX;
                        }
                        else if (maxX + dx >= screenShot.Width) {
                            dx = screenShot.Width - maxX - 1;
                        }

                        if (minY + dy < 0) {
                            dy = -minY;
                        }
                        else if (maxY + dy >= screenShot.Height) {
                            dy = screenShot.Height - maxY - 1;
                        }

                        x1 += dx;
                        y1 += dy;
                        x2 += dx;
                        y2 += dy;
                        old_x = e.X;
                        old_y = e.Y;
                    }


                }
                this.Refresh();
            }
            else { // mouse left button not clicking


                //check if mouse clicked on control points
                if (state == SelectionEditState.Selected) { // not possible to clicked on CP if no selections yet
                    currentCP = getControlPoint(e.X, e.Y);
                }


            }






            //this.Refresh();

        }

        private void Capture_MouseUp(object sender, MouseEventArgs e) {
            isClicking = false;

            if (state == SelectionEditState.Selecting) {

                if (x1 == x2 || y1 == y2) {
                    // x1 = -1;
                    state = SelectionEditState.NoSelection;
                    this.Refresh();
                }
                else {
                    state = SelectionEditState.Selected;
                }

            }

            //x1 = x2 = y1 = y2 = -1;
            //x1 = -1;
            
        }

        private void commitClip() {

            if (Math.Min(x1, x2) < 0 || Math.Min(y1, y2) < 0 ||
                Math.Max(x1, x2) >= screenShot.Width || Math.Max(y1, y2) >= screenShot.Height) {

                MessageBox.Show("Selection out of range!", "Error");
                return;
            }



            using (Graphics g = Graphics.FromImage(screenShot)) {

                foreach (RectFrame frame in rectFrameList) {
                    g.DrawRectangle(rectFramePen,
                        Math.Min(frame.x1, frame.x2), Math.Min(frame.y1, frame.y2),
                        Math.Abs(frame.x2 - frame.x1), Math.Abs(frame.y2 - frame.y1));
                }

            }
            

            Clipboard.SetImage(screenShot.Clone(
                new Rectangle(Math.Min(x1, x2), Math.Min(y1, y2),
                    Math.Abs(x2 - x1) + 1, Math.Abs(y2 - y1) + 1), screenShot.PixelFormat));

            abortClip();
        }

        

        

        private ControlPoints getControlPoint(int mouseX, int mouseY) {
            int minX, minY, maxX, maxY;
            int d_minX, d_minY, d_maxX, d_maxY;
            bool isInside, isInsideHeightSens, isInsideWidthSens;

            minX = Math.Min(x1, x2);
            minY = Math.Min(y1, y2);
            maxX = Math.Max(x1, x2);
            maxY = Math.Max(y1, y2);

            d_minX = Math.Abs(mouseX - minX);
            d_minY = Math.Abs(mouseY - minY);
            d_maxX = Math.Abs(mouseX - maxX);
            d_maxY = Math.Abs(mouseY - maxY);
            isInside = (mouseX > minX && mouseX < maxX && mouseY > minY && mouseY < maxY);
            isInsideWidthSens = (mouseX >= minX - CP_SENSITIVITY && mouseX <= maxX + CP_SENSITIVITY);
            isInsideHeightSens = (mouseY >= minY - CP_SENSITIVITY && mouseY <= maxY + CP_SENSITIVITY);

            if (d_maxX <= CP_SENSITIVITY && d_maxY <= CP_SENSITIVITY) { // right bottom
                this.Cursor = Cursors.SizeNWSE;
                return ControlPoints.RightBottom;
            }
            else if (d_maxX <= CP_SENSITIVITY && d_minY <= CP_SENSITIVITY) { // right top
                this.Cursor = Cursors.SizeNESW;
                return ControlPoints.RightTop;
            }
            else if (d_minX <= CP_SENSITIVITY && d_maxY <= CP_SENSITIVITY) { // left bottom
                this.Cursor = Cursors.SizeNESW;
                return ControlPoints.LeftBottom;
            }
            else if (d_minX <= CP_SENSITIVITY && d_minY <= CP_SENSITIVITY) { // left top
                this.Cursor = Cursors.SizeNWSE;
                return ControlPoints.LeftTop;
            }
            else if (isInsideHeightSens && d_maxX <= CP_SENSITIVITY) { // right
                this.Cursor = Cursors.SizeWE;
                return ControlPoints.Right;
            }
            else if (isInsideWidthSens && d_maxY <= CP_SENSITIVITY) { // bottom
                this.Cursor = Cursors.SizeNS;
                return ControlPoints.Bottom;
            }
            else if (isInsideHeightSens && d_minX <= CP_SENSITIVITY) { // left
                this.Cursor = Cursors.SizeWE;
                return ControlPoints.Left;
            }
            else if (isInsideWidthSens && d_minY <= CP_SENSITIVITY) { // top
                this.Cursor = Cursors.SizeNS;
                return ControlPoints.Top;
            }
            else if (isInside) { // inside
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

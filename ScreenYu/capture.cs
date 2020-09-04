using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ScreenYu {
    public partial class capture : Form {


        private enum ControlPoints {
            None,
            LeftTop, Top, RightTop,
            Left, Inside, Right,
            LeftBottom, Bottom, RightBottom
            
        }
        private const int CP_SENSITIVITY = 5;

        private Bitmap screenShot;
        private int x1 = -1, y1 = -1, x2 = -1, y2 = -1, old_x, old_y;
        private bool isClicking = false;
        private Pen selectionPen;
        private Brush dimBrush;
        private Rectangle selectionRect;
        private ControlPoints currentCP = ControlPoints.None;
        private IntPtr old_hWnd = IntPtr.Zero;


        public capture() {
            //SetProcessDPIAware();
            InitializeComponent();
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            selectionPen = new Pen(Color.FromArgb(30,120,180), 1.5f);
            selectionPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
            dimBrush = new SolidBrush(Color.FromArgb(Convert.ToInt32(0.3 * 255), 0, 0, 0));
            selectionRect = new Rectangle();
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

                g.DrawRectangle(selectionPen,
                    Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x2 - x1), Math.Abs(y2 - y1));
                
                
                
            }




            base.OnPaint(e);
        }

        private void capture_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                abortClip();
                
            }
        }

        private void capture_MouseDown(object sender, MouseEventArgs e) {
            int tx1, ty1, tx2, ty2;

            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                isClicking = true;

                switch (currentCP) {
                    case ControlPoints.None: // first/re select
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
            else if (e.Button == System.Windows.Forms.MouseButtons.Right) {

                if (x1 != -1) {
                    commitClip();
                    
                }
            
            }
        }

        private void capture_MouseMove(object sender, MouseEventArgs e) {

            

            if (isClicking){
                if (currentCP == ControlPoints.None) { // first/re select
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



            //check if mouse clicked on control points
            if (!isClicking && x1 != -1) { // not possible to clicked on cp if no selections yet

                currentCP = getControlPoint(e.X, e.Y);

            }
            

            

            //this.Refresh();

        }

        private void capture_MouseUp(object sender, MouseEventArgs e) {
            isClicking = false;

            if (x1 == x2 || y1 == y2) {
                x1 = -1;
                this.Refresh();
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

            Clipboard.SetImage(screenShot.Clone(
                new Rectangle(Math.Min(x1, x2), Math.Min(y1, y2),
                    Math.Abs(x2 - x1) + 1, Math.Abs(y2 - y1) + 1), screenShot.PixelFormat));

            abortClip();
        }

        private void abortClip() {
            x1 = -1;
            currentCP = ControlPoints.None;

            this.Hide();
            if (old_hWnd != IntPtr.Zero)
                WinAPI.SetForegroundWindow(old_hWnd);
            
            //this.Owner.Show();
        
        }

        public void showSelectForm(Bitmap b, IntPtr fg_hWnd, IWin32Window owner) {
            screenShot = b;
            old_hWnd = fg_hWnd;
            this.Show(owner);
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

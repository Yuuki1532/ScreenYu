using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenYu {
    public partial class CaptureForm : Form {

        public CaptureForm() {
            //SetProcessDPIAware();
            InitializeComponent();
            MouseWheel += CaptureForm_MouseWheel;

            selection = new Selection() {
                SelectionPen = new Pen(Color.FromArgb(30, 120, 180), 1.0f) {
                    Alignment = System.Drawing.Drawing2D.PenAlignment.Center
                },
                SelectionBrush = new SolidBrush(Color.FromArgb(Convert.ToInt32(0.3 * 255), 0, 0, 0)),
                _rect = new Rectangle(),
            };

            drawingObjects = new DrawingObjects() {
                ObjectList = new List<Drawing.Object>(),
                ColorList = new List<Color>() {
                    Color.FromArgb(255, 160, 0), // default
                    Color.FromArgb(230, 30, 30),
                    Color.FromArgb(255, 160, 0),
                    Color.FromArgb(32, 250, 90),
                    Color.FromArgb(40, 90, 250),
                    Color.FromArgb(170, 40, 250),
                },
                CurrentColorId = 0,
                CurrentStrokeSize = 2.0f,
            };
            drawingObjects._pen = new Pen(drawingObjects.ColorList[drawingObjects.CurrentColorId], drawingObjects.CurrentStrokeSize);


        }


        protected override void OnPaint(PaintEventArgs e) {
            if (fullscreenBmp == null) return;

            Graphics g = e.Graphics;
            g.DrawImage(fullscreenBmp, 0, 0);
            // g.FillRectangle(selection.SelectionBrush, 0, 0, fullscreenBmp.Width, fullscreenBmp.Height);

            if (seState == SelectionEditState.NoSelection) {
                g.FillRectangle(selection.SelectionBrush, 0, 0, fullscreenBmp.Width, fullscreenBmp.Height);
            }
            else {
                int
                    minX = Math.Min(selection.x1, selection.x2),
                    minY = Math.Min(selection.y1, selection.y2),
                    maxX = Math.Max(selection.x1, selection.x2),
                    maxY = Math.Max(selection.y1, selection.y2);

                selection._rect.X = minX;
                selection._rect.Y = minY;
                selection._rect.Width = maxX - minX + 1;
                selection._rect.Height = maxY - minY + 1;

                // +---+---+---+
                // | 1 | 2 | 3 |
                // +---+---+---+
                // | 4 | 5 | 6 |
                // +---+---+---+
                // | 7 | 8 | 9 |
                // +---+---+---+

                if (minY > 0)
                    g.FillRectangle(selection.SelectionBrush, 0, 0, fullscreenBmp.Width, minY); // 1 2 3
                if (maxY < fullscreenBmp.Height - 1)
                    g.FillRectangle(selection.SelectionBrush, 0, maxY, fullscreenBmp.Width, fullscreenBmp.Height - maxY); // 7 8 9
                if (minX > 0)
                    g.FillRectangle(selection.SelectionBrush, 0, minY, minX, maxY - minY); // 4
                if (maxX < fullscreenBmp.Width - 1)
                    g.FillRectangle(selection.SelectionBrush, maxX + 1, minY, fullscreenBmp.Width - maxX - 1, maxY - minY); // 6


                // g.DrawImage(fullscreenBmp, selection._rect, selection._rect, GraphicsUnit.Pixel);

                Pen borderPen;
                
                if ((seState & SelectionEditState.DrawingMode) > 0 ||
                    (seState & SelectionEditState.Drawing) > 0) {
                    drawingObjects._pen.Color = drawingObjects.ColorList[drawingObjects.CurrentColorId];
                    drawingObjects._pen.Width = drawingObjects.CurrentStrokeSize;
                    borderPen = drawingObjects._pen;
                }
                else {
                    borderPen = selection.SelectionPen;
                }

                g.DrawRectangle(borderPen, minX, minY, maxX - minX, maxY - minY);

                foreach (Drawing.Object obj in drawingObjects.ObjectList) {
                    obj.PaintTo(g, drawingObjects._pen);
                }

            }


            base.OnPaint(e);
        }

        private void CaptureForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                EndCapture();
                return;
            }
            else if (e.KeyCode == keySelect) {
                if ((seState & SelectionEditState.DrawingMode) > 0) {
                    seState = SelectionEditState.Selected;
                }
                else {
                    return;
                }

                Cursor = Cursors.Cross;
            }
            else if (e.KeyCode == keyDrawRect) {
                if (seState == SelectionEditState.Selected || (seState & SelectionEditState.DrawingMode) > 0) {
                    seState = SelectionEditState.DrawingRectMode;
                }
                else {
                    return;
                }

                Cursor = Cursors.Cross;
            }
            else if (e.KeyCode == keyDrawLine) {
                if (seState == SelectionEditState.Selected || (seState & SelectionEditState.DrawingMode) > 0) {
                    seState = SelectionEditState.DrawingLineMode;
                }
                else {
                    return;
                }

                Cursor = Cursors.Cross;
            }
            else if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) {
                if ((seState & SelectionEditState.DrawingMode) > 0) {
                    int id = e.KeyCode - Keys.D0;
                    if (id < drawingObjects.ColorList.Count) {
                        drawingObjects.CurrentColorId = id;
                    }
                }

            }
            else if (e.Control && e.KeyCode == Keys.Z) {
                if ((seState & SelectionEditState.DrawingMode) > 0) {
                    if (drawingObjects.ObjectList.Count == 0) {
                        return;
                    }
                    drawingObjects.ObjectList.RemoveAt(drawingObjects.ObjectList.Count - 1);
                }
            }
            else { // no need to refresh
                return;
            }

            Refresh();

        }

        private void CaptureForm_MouseDown(object sender, MouseEventArgs e) {

            if (e.Button == MouseButtons.Left) {

                if (seState == SelectionEditState.NoSelection) { // no selection
                    seState = SelectionEditState.Selecting;
                    selection.x1 = e.X;
                    selection.y1 = e.Y;
                }
                else if (seState == SelectionEditState.Selected) { // has selection
                    var cp = SetCursor(e.X, e.Y);

                    if (cp == ControlPoints.None) { // new selection
                        seState = SelectionEditState.Selecting;
                        selection.x1 = e.X;
                        selection.y1 = e.Y;
                    }
                    else { // edit selection
                        seState = SelectionEditState.EditingSelection;
                        selection.EditingCP = cp;


                        // make x2, y2 be the point to be modifying (except for moving selection)
                        if (cp == ControlPoints.TopLeft ||
                            cp == ControlPoints.Left ||
                            cp == ControlPoints.BottomLeft) {
                            SetMinMax(ref selection.x2, ref selection.x1); // set x2 to the smaller value
                        }
                        if (cp == ControlPoints.TopRight ||
                            cp == ControlPoints.Right ||
                            cp == ControlPoints.BottomRight) {
                            SetMinMax(ref selection.x1, ref selection.x2); // set x2 to the larger value
                        }
                        if (cp == ControlPoints.TopLeft ||
                            cp == ControlPoints.Top ||
                            cp == ControlPoints.TopRight) {
                            SetMinMax(ref selection.y2, ref selection.y1); // set y2 to the smaller value
                        }
                        if (cp == ControlPoints.BottomLeft ||
                            cp == ControlPoints.Bottom ||
                            cp == ControlPoints.BottomRight) {
                            SetMinMax(ref selection.y1, ref selection.y2); // set y2 to the larger value
                        }
                        if (cp == ControlPoints.Inside) {
                            selection.x_anchor = e.X;
                            selection.y_anchor = e.Y;
                        }

                    }
                }
                else if (seState == SelectionEditState.DrawingRectMode) {

                    drawingObjects.ObjectList.Add(
                        new Drawing.Rect() {
                            x1 = e.X,
                            x2 = e.X,
                            y1 = e.Y,
                            y2 = e.Y,
                            Color = drawingObjects.ColorList[drawingObjects.CurrentColorId],
                            StrokeSize = drawingObjects.CurrentStrokeSize,
                        }
                    );

                    seState = SelectionEditState.DrawingRect;
                }
                else if (seState == SelectionEditState.DrawingLineMode) {

                    drawingObjects.ObjectList.Add(
                            new Drawing.Line() {
                                x1 = e.X,
                                x2 = e.X,
                                y1 = e.Y,
                                y2 = e.Y,
                                Color = drawingObjects.ColorList[drawingObjects.CurrentColorId],
                                StrokeSize = drawingObjects.CurrentStrokeSize,
                            }
                        );

                    seState = SelectionEditState.DrawingLine;
                }

            }
            else if (e.Button == MouseButtons.Right) {
                CommitCapture();
            }
        }

        private void CaptureForm_MouseMove(object sender, MouseEventArgs e) {

            if (seState == SelectionEditState.Selecting) {
                selection.x2 = e.X;
                selection.y2 = e.Y;
            }
            else if (seState == SelectionEditState.Selected) {
                SetCursor(e.X, e.Y);
            }
            else if (seState == SelectionEditState.EditingSelection) {

                if (selection.EditingCP == ControlPoints.TopLeft ||
                    selection.EditingCP == ControlPoints.TopRight ||
                    selection.EditingCP == ControlPoints.Left ||
                    selection.EditingCP == ControlPoints.Right ||
                    selection.EditingCP == ControlPoints.BottomLeft ||
                    selection.EditingCP == ControlPoints.BottomRight) {
                    selection.x2 = e.X;
                }
                if (selection.EditingCP == ControlPoints.TopLeft ||
                    selection.EditingCP == ControlPoints.TopRight ||
                    selection.EditingCP == ControlPoints.Top ||
                    selection.EditingCP == ControlPoints.Bottom ||
                    selection.EditingCP == ControlPoints.BottomLeft ||
                    selection.EditingCP == ControlPoints.BottomRight) {
                    selection.y2 = e.Y;
                }
                if (selection.EditingCP == ControlPoints.Inside) {
                    int
                        dx = e.X - selection.x_anchor,
                        dy = e.Y - selection.y_anchor;

                    SetMinMax(ref selection.x1, ref selection.x2);
                    SetMinMax(ref selection.y1, ref selection.y2);

                    int
                        selectionWidth = selection.x2 - selection.x1,
                        selectionHeight = selection.y2 - selection.y1;

                    selection.x1 += dx;
                    selection.x2 += dx;
                    selection.y1 += dy;
                    selection.y2 += dy;

                    // check if left margin out of range
                    if (selection.x1 < 0) {
                        selection.x1 = 0;
                        selection.x2 = selectionWidth;
                    }
                    // check if right margin out of range
                    else if (selection.x2 >= fullscreenBmp.Width) {
                        selection.x2 = fullscreenBmp.Width - 1;
                        selection.x1 = selection.x2 - selectionWidth;
                    }

                    // check if top margin out of range
                    if (selection.y1 < 0) {
                        selection.y1 = 0;
                        selection.y2 = selectionHeight;
                    }
                    // check if bottom margin out of range
                    else if (selection.y2 >= fullscreenBmp.Height) {
                        selection.y2 = fullscreenBmp.Height - 1;
                        selection.y1 = selection.y2 - selectionHeight;
                    }

                    selection.x_anchor = e.X;
                    selection.y_anchor = e.Y;

                }

            }
            else if (seState == SelectionEditState.DrawingRect) {
                Drawing.Rect currentRect = (Drawing.Rect)drawingObjects.ObjectList[drawingObjects.ObjectList.Count - 1];
                currentRect.x2 = e.X;
                currentRect.y2 = e.Y;
            }
            else if (seState == SelectionEditState.DrawingLine) {
                Drawing.Line currentLine = (Drawing.Line)drawingObjects.ObjectList[drawingObjects.ObjectList.Count - 1];

                if (ModifierKeys == Keys.Shift) { // straight line
                    if (Math.Abs(e.X - currentLine.x1) >= Math.Abs(e.Y - currentLine.y1)) { // horizontal
                        currentLine.x2 = e.X;
                        currentLine.y2 = currentLine.y1;
                    }
                    else { // vertical
                        currentLine.x2 = currentLine.x1;
                        currentLine.y2 = e.Y;
                    }
                }
                else {
                    currentLine.x2 = e.X;
                    currentLine.y2 = e.Y;
                }
                
            }

            Refresh();

        }

        private void CaptureForm_MouseUp(object sender, MouseEventArgs e) {

            if (seState == SelectionEditState.Selecting ||
                seState == SelectionEditState.EditingSelection) {

                if (selection.x1 == selection.x2 ||
                    selection.y1 == selection.y2) {
                    seState = SelectionEditState.NoSelection;
                    Refresh();
                }
                else {
                    seState = SelectionEditState.Selected;
                }

            }
            else if ((seState & SelectionEditState.Drawing) > 0) {
                Drawing.Object currentObject = drawingObjects.ObjectList[drawingObjects.ObjectList.Count - 1];

                if (currentObject.IsEmpty()) {
                    // empty object, remove it
                    drawingObjects.ObjectList.RemoveAt(drawingObjects.ObjectList.Count - 1);
                }

                seState = (seState ^ SelectionEditState.Drawing) | SelectionEditState.DrawingMode;
            }


        }

        private void CaptureForm_MouseWheel(object sender, MouseEventArgs e) {
            if ((seState & SelectionEditState.DrawingMode) > 0) {
                if (e.Delta > 0) { // away from user
                    drawingObjects.CurrentStrokeSize = Math.Min(drawingObjects.CurrentStrokeSize + 1, maxDrawingStrokeSize);
                }
                else {
                    drawingObjects.CurrentStrokeSize = Math.Max(minDrawingStrokeSize, drawingObjects.CurrentStrokeSize - 1);
                }
                Refresh();

            }

        }



    }
}

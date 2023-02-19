using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenYu {
    public partial class Drawing {
        public abstract class Object {
            public abstract void PaintTo(Graphics g, Pen pen = null);
        }

        public class Rect : Object {
            public int x1, y1, x2, y2;
            public Color Color;
            public float StrokeSize;
            public override void PaintTo(Graphics g, Pen pen = null) {
                // paint this object to graphics g
                // pen: if not null, reuse pen
                if (pen is null) {
                    pen = new Pen(Color, StrokeSize);
                }
                else {
                    pen.Color = Color;
                    pen.Width = StrokeSize;
                }

                g.DrawRectangle(pen,
                    Math.Min(x1, x2), Math.Min(y1, y2),
                    Math.Abs(x2 - x1), Math.Abs(y2 - y1));
            }
        }


    }

}
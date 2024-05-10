namespace ScreenYu.Drawing {
    internal class Line : I {
        public int X1, Y1, X2, Y2;
        public Pen Pen;

        public Line(Color color, float strokeSize) {
            Pen = new Pen(color, strokeSize);
        }

        public void PaintTo(Graphics g) {
            g.DrawLine(Pen, X1, Y1, X2, Y2);
        }

        public bool IsEmpty() {
            return X1 == X2 && Y1 == Y2;
        }
    }
}

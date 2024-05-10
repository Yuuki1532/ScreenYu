namespace ScreenYu.Drawing {
    internal class Rect : I {
        public int X1, Y1, X2, Y2;
        public Pen Pen;

        public Rect(Color color, float strokeSize) {
            Pen = new Pen(color, strokeSize);
        }

        public void PaintTo(Graphics g) {
            g.DrawRectangle(Pen,
                Math.Min(X1, X2), Math.Min(Y1, Y2),
                Math.Abs(X2 - X1), Math.Abs(Y2 - Y1));
        }
        public bool IsEmpty() {
            return X1 == X2 || Y1 == Y2;
        }
    }
}

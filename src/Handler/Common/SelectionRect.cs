namespace ScreenYu.Handler.Common {
    internal class SelectionRect {
        public int X1, Y1, X2, Y2;
        private Brush DimBrush;

        public SelectionRect(Brush dimBrush) {
            DimBrush = dimBrush;
        }

        public bool IsEmpty() {
            return X1 == X2 || Y1 == Y2;
        }
        public void OrderXY() {
            if (X1 > X2) {
                (X1, X2) = (X2, X1);
            }
            if (Y1 > Y2) {
                (Y1, Y2) = (Y2, Y1);
            }
        }
        public void Reset() {
            X1 = Y1 = X2 = Y2 = 0;
        }
        public void PaintTo(Graphics g, int screenShotWidth, int screenShotHeight) {
            int W = screenShotWidth;
            int H = screenShotHeight;

            if (IsEmpty()) {
                g.FillRectangle(DimBrush, 0, 0, W, H);
            }
            else {
                int
                    minX = Math.Min(X1, X2),
                    minY = Math.Min(Y1, Y2),
                    maxX = Math.Max(X1, X2),
                    maxY = Math.Max(Y1, Y2);

                // +---+---+---+
                // | 1 | 2 | 3 |
                // +---+---+---+
                // | 4 | 5 | 6 |
                // +---+---+---+
                // | 7 | 8 | 9 |
                // +---+---+---+

                if (minY > 0)
                    g.FillRectangle(DimBrush, 0, 0, W, minY); // 1 2 3
                if (maxY < H - 1)
                    g.FillRectangle(DimBrush, 0, maxY, W, H - maxY); // 7 8 9
                if (minX > 0)
                    g.FillRectangle(DimBrush, 0, minY, minX, maxY - minY); // 4
                if (maxX < W - 1)
                    g.FillRectangle(DimBrush, maxX + 1, minY, W - maxX - 1, maxY - minY); // 6

                // g.DrawRectangle(selection.SelectionPen, minX, minY, maxX - minX, maxY - minY);
            }
        }

    }
}

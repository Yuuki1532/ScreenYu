namespace ScreenYu.Handler.Common {
    internal static class DrawingUtils {
        public static void DrawSelectionBorder(Graphics g, SelectionRect selectionRect, Pen borderPen) {
            int
                minX = Math.Min(selectionRect.X1, selectionRect.X2),
                minY = Math.Min(selectionRect.Y1, selectionRect.Y2),
                maxX = Math.Max(selectionRect.X1, selectionRect.X2),
                maxY = Math.Max(selectionRect.Y1, selectionRect.Y2);

            g.DrawRectangle(borderPen, minX, minY, maxX - minX, maxY - minY);
        }

    }
}

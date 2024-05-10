namespace ScreenYu.Drawing.Manager {
    internal interface I {
        public void Append(Drawing.I? obj);
        public bool Pop(); // returns true if an object is popped
        public void Clear();
        public void PaintAll(Graphics g);
        public Color GetStrokeColor();
        public void SetStrokeColor(Color color);
        public float GetStrokeSize();
        public void SetStrokeSize(float strokeSize);
    }
}

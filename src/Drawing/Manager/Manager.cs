using ScreenYu.App;

namespace ScreenYu.Drawing.Manager {
    internal class Manager : I, IScreenShotEventHandler {
        private List<Drawing.I> ObjectList;
        private Color StrokeColor;
        private float StrokeSize;

        public Manager(Config config) {
            ObjectList = new();
            StrokeColor = config.DrawingConfig.StrokeColorList[config.DrawingConfig.InitialStrokeColorIdx];
            StrokeSize = config.DrawingConfig.InitialStrokeSize;
        }

        public void Append(Drawing.I? obj) {
            if (obj == null) { // ignore null object
                return;
            }
            ObjectList.Add(obj);
        }
        public bool Pop() {
            if (ObjectList.Count == 0) {
                return false;
            }

            ObjectList.RemoveAt(ObjectList.Count - 1);
            return true;
        }
        public void Clear() => ObjectList.Clear();
        public void PaintAll(Graphics g) {
            foreach (var item in ObjectList) {
                item.PaintTo(g);
            }
        }
        public Color GetStrokeColor() => StrokeColor;
        public void SetStrokeColor(Color color) => StrokeColor = color;
        public float GetStrokeSize() => StrokeSize;
        public void SetStrokeSize(float strokeSize) => StrokeSize = strokeSize;

        public void OnScreenShotStarted(object? sender, ScreenShotEventArgs e) {
            ObjectList.Clear();
        }

        public void OnScreenShotEnded(object? sender, ScreenShotEventArgs e) {
            ObjectList.Clear();
        }
    }
}

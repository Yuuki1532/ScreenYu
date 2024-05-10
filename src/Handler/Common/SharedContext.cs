namespace ScreenYu.Handler.Common {
    internal class SharedContext {

        // set by handler manager
        public ScreenYu.Drawing.Manager.I DrawingManager;
        public SelectionRect SelectionRect;
        public Form Form;
        public int ScreenShotWidth, ScreenShotHeight;

        // set by others
        public Mode? Mode;

        // private Dictionary<string, dynamic?> AdditionalContext;

        public SharedContext(Form form, SelectionRect selectionRect, ScreenYu.Drawing.Manager.I drawingManager, int screenShotWidth, int screenShotHeight) {
            Form = form;
            SelectionRect = selectionRect;
            DrawingManager = drawingManager;
            ScreenShotWidth = screenShotWidth;
            ScreenShotHeight = screenShotHeight;

            // AdditionalContext = new();
        }

        // additional context
        //public (T?, bool) Get<T>(string key) {
        //    if (AdditionalContext.TryGetValue(key, out dynamic? value)) {
        //        return (value!, true);
        //    }
        //    return (default(T), false);
        //}
        //public void Set(string key, dynamic value) {
        //    AdditionalContext[key] = value;
        //}

    }
}

namespace ScreenYu.App {
    internal partial class App {
        private Form MainForm;
        private Config Config;

        // manager
        private Drawing.Manager.Manager DrawingManager; // drawing objects, stroke color, stroke width
        private Handler.Manager.Manager HandlerManager; // handlers

        // global handler
        private GlobalControlHandler _globalControlHandler;    // global key/mouse binding

        // screenshot related
        Bitmap? InitialScreenshot;                      // initial full screenshot without any modification
        Handler.Common.SelectionRect? SelectionRect;    // selection area for screenshot

        private const int HotKeyId = 0;                 // windows hotkey id to register
        private const int ControlPointSensitivity = 5;  // the distance to the border of selection rect when modifying it

        public App() {
            MainForm = new Forms.Main(OnFormWndProc);
            Config = new();
            DrawingManager = new(Config);

            _globalControlHandler = new(this);

            // renderer
            var screenShotRenderer = new Handler.ScreenShotRenderer();                                // draw screenshot background and dim outside selection rect
            var drawingManagerRenderer = new Handler.DrawingManagerRenderer(DrawingManager);          // draw objects on screen
            
            // modes (exclusive, two modes cannot be in the same time)
            var selectAreaHandler = new Handler.AreaSelecting(Config, ControlPointSensitivity);       // edit screenshot selection area
            var drawLineHandler = new Handler.LineDrawing();                                          // line drawing mode
            var drawRectHandler = new Handler.RectDrawing();                                          // rect drawing mode

            // dispatcher for modes
            var modeDispatcher = new Handler.Dispatcher<Handler.Common.Mode>(
                new Dictionary<Handler.Common.Mode, Handler.IDispatcherHandler>() {
                    { Handler.Common.Mode.SelectArea, selectAreaHandler },
                    { Handler.Common.Mode.DrawLine, drawLineHandler },
                    { Handler.Common.Mode.DrawRect, drawRectHandler },
                },
                ctx => ctx.SharedContext().Mode!.Value
            );
            
            HandlerManager = new(new List<Handler.I>(){
                screenShotRenderer,
                _globalControlHandler,
                drawingManagerRenderer,
                modeDispatcher,
            });

            AddEventHandler(new List<IScreenShotEventHandler>() {
                screenShotRenderer,
                modeDispatcher,
            });

            Lib.HotKey.Register(MainForm.Handle, HotKeyId, Config.HotKeyConfig.Key,
                Config.HotKeyConfig.WinKey, Config.HotKeyConfig.Control, Config.HotKeyConfig.Shift, Config.HotKeyConfig.Alt);
        }

        private void AddEventHandler(List<IScreenShotEventHandler> handlerList) {
            MainForm.Paint += HandlerManager.OnFormPaint;
            MainForm.KeyDown += HandlerManager.OnFormKeyDown;
            MainForm.KeyUp += HandlerManager.OnFormKeyUp;
            MainForm.MouseDown += HandlerManager.OnFormMouseDown;
            MainForm.MouseMove += HandlerManager.OnFormMouseMove;
            MainForm.MouseUp += HandlerManager.OnFormMouseUp;
            MainForm.MouseWheel += HandlerManager.OnFormMouseWheel;
            MainForm.MouseDoubleClick += HandlerManager.OnFormMouseDoubleClick;

            ScreenShotStarted += HandlerManager.OnScreenShotStarted;
            ScreenShotStarted += DrawingManager.OnScreenShotStarted;

            ScreenShotEnded += HandlerManager.OnScreenShotEnded;
            ScreenShotEnded += DrawingManager.OnScreenShotEnded;

            foreach (var handler in handlerList) {
                ScreenShotStarted += handler.OnScreenShotStarted;
                ScreenShotEnded += handler.OnScreenShotEnded;
            }
        }

        private void OnFormWndProc(ref Message msg) {
            switch (msg.Msg) {
                case Pkg.Win.WinApi.WM_HOTKEY:
                    if (msg.WParam == HotKeyId) {
                        StartScreenShot();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

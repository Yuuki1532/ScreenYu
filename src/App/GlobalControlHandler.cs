using ScreenYu.Handler;

namespace ScreenYu.App {
    using TKeyHandler = Action<Handler.Context.I, KeyEventArgs>;
    using TMouseHandler = Action<Handler.Context.I, MouseEventArgs>;

    internal partial class App {
        private class GlobalControlHandler : EmptyHandler {
            private App App;

            private Dictionary<(Keys, Keys), TKeyHandler> KeyHandlerDict;                 // (ModifierKeys, Keys) -> Handler
            private Dictionary<MouseButtons, TMouseHandler> MouseButtonHandlerDict;

            public GlobalControlHandler(App app) {
                App = app;
                var keyConfig = app.Config.KeyBindingConfig;

                KeyHandlerDict = new() {
                    { (Keys.None, keyConfig.Cancel), Cancel },
                    { (Keys.None, keyConfig.SelectAreaMode), SelectMode },
                    { (Keys.None, keyConfig.Reset), Reset },
                    { (Keys.None, keyConfig.DrawLineMode), DrawLineMode },
                    { (Keys.None, keyConfig.DrawRectMode), DrawRectMode },
                    { (Keys.Control, Keys.Z), UndoDrawing },
                };

                // change color by key 0~9
                for (int i = 0; i <= 9; i++) {
                    KeyHandlerDict[(Keys.None, Keys.D0 + i)] = ChangeStrokeColorHandlerFactory(i);
                }

                MouseButtonHandlerDict = new() {
                    { MouseButtons.Right, Commit },
                };
            }

            public override void KeyDown(Handler.Context.I ctx, KeyEventArgs e) {
                if (!KeyHandlerDict.ContainsKey((e.Modifiers, e.KeyCode))) {
                    return;
                }
                KeyHandlerDict[(e.Modifiers, e.KeyCode)](ctx, e);
            }
            public override void MouseDown(Handler.Context.I ctx, MouseEventArgs e) {
                if (!MouseButtonHandlerDict.ContainsKey(e.Button)) {
                    return;
                }
                MouseButtonHandlerDict[e.Button](ctx, e);
            }
            public override void MouseWheel(Handler.Context.I ctx, MouseEventArgs e) {
                ChangeStrokeSize(ctx, e);
            }
            private void Cancel(Handler.Context.I ctx, KeyEventArgs e) {
                ctx.Abort();
                App.EndScreenShot();
            }
            private void SelectMode(Handler.Context.I ctx, KeyEventArgs e) {
                ctx.Abort();
                ctx.SharedContext().Mode = Handler.Common.Mode.SelectArea;
                ctx.SharedContext().Form.Refresh();
            }
            private void DrawLineMode(Handler.Context.I ctx, KeyEventArgs e) {
                ctx.Abort();
                ctx.SharedContext().Mode = Handler.Common.Mode.DrawLine;
                ctx.SharedContext().Form.Refresh();
            }
            private void DrawRectMode(Handler.Context.I ctx, KeyEventArgs e) {
                ctx.SharedContext().Mode = Handler.Common.Mode.DrawRect;
                ctx.SharedContext().Form.Refresh();
            }
            private void Commit(Handler.Context.I ctx, MouseEventArgs e) {
                App.CommitScreenShot();
                App.EndScreenShot();
            }
            private void ChangeStrokeSize(Handler.Context.I ctx, MouseEventArgs e) {
                var mode = ctx.SharedContext().Mode;

                if (mode != null && (
                    mode == Handler.Common.Mode.DrawLine ||
                    mode == Handler.Common.Mode.DrawRect
                )) {
                    var drawingManager = ctx.SharedContext().DrawingManager;

                    if (e.Delta > 0) { // away from user
                        drawingManager.SetStrokeSize(
                            Math.Min(drawingManager.GetStrokeSize() + 1, App.Config.DrawingConfig.MaxStrokeSize)
                        );
                    } else {
                        drawingManager.SetStrokeSize(
                            Math.Max(App.Config.DrawingConfig.MinStrokeSize, drawingManager.GetStrokeSize() - 1)
                        );
                    }

                    ctx.SharedContext().Form.Refresh();
                }
            }
            private TKeyHandler ChangeStrokeColorHandlerFactory(int strokeColorIdx) {
                return (Handler.Context.I ctx, KeyEventArgs e) => {
                    var mode = ctx.SharedContext().Mode;

                    if (mode == null || !(
                        mode == Handler.Common.Mode.DrawLine ||
                        mode == Handler.Common.Mode.DrawRect
                    )) {
                        return;
                    }

                    if (strokeColorIdx < 0 || strokeColorIdx >= App.Config.DrawingConfig.StrokeColorList.Count) { // out of index
                        return;
                    }

                    ctx.SharedContext().DrawingManager.SetStrokeColor(App.Config.DrawingConfig.StrokeColorList[strokeColorIdx]);
                    ctx.SharedContext().Form.Refresh();
                };
            }
            private void Reset(Handler.Context.I ctx, KeyEventArgs e) {
                ctx.Abort();
                App.HandlerManager.ResetHandlers(ctx);
                ctx.SharedContext().DrawingManager.Clear();
                ctx.SharedContext().Form.Refresh();
            }
            private void UndoDrawing(Handler.Context.I ctx, KeyEventArgs e) {
                if (ctx.SharedContext().DrawingManager.Pop()) {
                    ctx.SharedContext().Form.Refresh();
                }
            }
        }
    }
}

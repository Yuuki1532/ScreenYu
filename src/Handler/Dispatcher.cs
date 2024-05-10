using ScreenYu.App;
using ScreenYu.Handler.Common;

namespace ScreenYu.Handler {
    internal interface IDispatcherHandler : I {
        public void OnEnter(Context.I ctx);
        public void OnLeave(Context.I? ctx);
    }

    internal class Dispatcher<T> : I, IScreenShotEventHandler, IResetHandler where T : struct {
        public delegate T TMapper(Context.I ctx);

        private Dictionary<T, IDispatcherHandler> HandlerDict;
        private TMapper Mapper;

        private T? PreviousKey;

        public Dispatcher(Dictionary<T, IDispatcherHandler> handlerDict, TMapper mapper) {
            HandlerDict = handlerDict;
            Mapper = mapper;
        }

        private void Dispatch(FormEvent formEvent, Context.I ctx, EventArgs eventArg) {
            T key = Mapper(ctx);

            if (!HandlerDict.ContainsKey(key)) {
                return;
            }

            if (PreviousKey != null && !PreviousKey.Value.Equals(key)) {
                HandlerDict[PreviousKey.Value].OnLeave(ctx);
            }

            if (PreviousKey == null || !PreviousKey.Value.Equals(key)) {
                HandlerDict[key].OnEnter(ctx);
            }

            PreviousKey = key;

            switch (formEvent) {
                case FormEvent.MouseDown:
                    HandlerDict[key].MouseDown(ctx, (MouseEventArgs)eventArg);
                    break;
                case FormEvent.MouseUp:
                    HandlerDict[key].MouseUp(ctx, (MouseEventArgs)eventArg);
                    break;
                case FormEvent.MouseMove:
                    HandlerDict[key].MouseMove(ctx, (MouseEventArgs)eventArg);
                    break;
                case FormEvent.MouseDoubleClick:
                    HandlerDict[key].MouseDoubleClick(ctx, (MouseEventArgs)eventArg);
                    break;
                case FormEvent.MouseWheel:
                    HandlerDict[key].MouseWheel(ctx, (MouseEventArgs)eventArg);
                    break;
                case FormEvent.KeyDown:
                    HandlerDict[key].KeyDown(ctx, (KeyEventArgs)eventArg);
                    break;
                case FormEvent.KeyUp:
                    HandlerDict[key].KeyUp(ctx, (KeyEventArgs)eventArg);
                    break;
                case FormEvent.Paint:
                    HandlerDict[key].Paint(ctx, (PaintEventArgs)eventArg);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void MouseDown(Context.I ctx, MouseEventArgs e) => Dispatch(FormEvent.MouseDown, ctx, e);
        public void MouseUp(Context.I ctx, MouseEventArgs e) => Dispatch(FormEvent.MouseUp, ctx, e);
        public void MouseMove(Context.I ctx, MouseEventArgs e) => Dispatch(FormEvent.MouseMove, ctx, e);
        public void MouseDoubleClick(Context.I ctx, MouseEventArgs e) => Dispatch(FormEvent.MouseDoubleClick, ctx, e);
        public void MouseWheel(Context.I ctx, MouseEventArgs e) => Dispatch(FormEvent.MouseWheel, ctx, e);
        public void KeyDown(Context.I ctx, KeyEventArgs e) => Dispatch(FormEvent.KeyDown, ctx, e);
        public void KeyUp(Context.I ctx, KeyEventArgs e) => Dispatch(FormEvent.KeyUp, ctx, e);
        public void Paint(Context.I ctx, PaintEventArgs e) => Dispatch(FormEvent.Paint, ctx, e);

        public void OnScreenShotStarted(object? sender, ScreenShotEventArgs e) {
            PreviousKey = null;
        }

        public void OnScreenShotEnded(object? sender, ScreenShotEventArgs e) {
            if (PreviousKey != null) {
                HandlerDict[PreviousKey.Value].OnLeave(null);
            }
        }

        public void OnNewSharedContext(SharedContext sharedCtx) {
            sharedCtx.Mode = Mode.SelectArea;
        }

        public void Reset(Context.I ctx) {
            foreach (var handler in HandlerDict.Values) {
                if (handler is IResetHandler resetHandler) {
                    resetHandler.Reset(ctx);
                }
            }
        }

    }
}

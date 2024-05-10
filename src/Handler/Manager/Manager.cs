using ScreenYu.App;
using ScreenYu.Handler.Common;

namespace ScreenYu.Handler.Manager {
    internal class Manager : IScreenShotEventHandler {
        private class Ctx : Context.I {
            private readonly FormEvent FormEvent;
            private readonly Manager Manager;
            private SharedContext _sharedContext;

            public Ctx(Manager manager, FormEvent formEvent, SharedContext sharedContext) {
                Manager = manager;
                FormEvent = formEvent;
                _sharedContext = sharedContext;
            }

            public SharedContext SharedContext() {
                return _sharedContext;
            }
            public void Abort() {
                Manager.HandlerIdxDict[FormEvent] = Manager.HandlerList.Count;
            }
            public void Next() {
                throw new NotImplementedException();
            }
        }

        public SharedContext? SharedContext;
        private Dictionary<FormEvent, int> HandlerIdxDict;
        private List<I> HandlerList;

        public Manager(List<I> handlerList) {
            HandlerList = handlerList;

            HandlerIdxDict = new();
            foreach (FormEvent e in Enum.GetValues(typeof(FormEvent))) {
                HandlerIdxDict[e] = 0;
            }
        }

        private void Handle(FormEvent formEvent, EventArgs eventArg) {
            var ctx = new Ctx(this, formEvent, SharedContext!);
            HandlerIdxDict[formEvent] = 0;

            while (HandlerIdxDict[formEvent] < HandlerList.Count) {
                switch (formEvent) {
                    case FormEvent.MouseDown:
                        HandlerList[HandlerIdxDict[formEvent]].MouseDown(ctx, (MouseEventArgs)eventArg);
                        break;
                    case FormEvent.MouseUp:
                        HandlerList[HandlerIdxDict[formEvent]].MouseUp(ctx, (MouseEventArgs)eventArg);
                        break;
                    case FormEvent.MouseMove:
                        HandlerList[HandlerIdxDict[formEvent]].MouseMove(ctx, (MouseEventArgs)eventArg);
                        break;
                    case FormEvent.MouseDoubleClick:
                        HandlerList[HandlerIdxDict[formEvent]].MouseDoubleClick(ctx, (MouseEventArgs)eventArg);
                        break;
                    case FormEvent.MouseWheel:
                        HandlerList[HandlerIdxDict[formEvent]].MouseWheel(ctx, (MouseEventArgs)eventArg);
                        break;
                    case FormEvent.KeyDown:
                        HandlerList[HandlerIdxDict[formEvent]].KeyDown(ctx, (KeyEventArgs)eventArg);
                        break;
                    case FormEvent.KeyUp:
                        HandlerList[HandlerIdxDict[formEvent]].KeyUp(ctx, (KeyEventArgs)eventArg);
                        break;
                    case FormEvent.Paint:
                        HandlerList[HandlerIdxDict[formEvent]].Paint(ctx, (PaintEventArgs)eventArg);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                HandlerIdxDict[formEvent]++;
            }
        }

        public void OnFormMouseDown(object? sender, MouseEventArgs e) => Handle(FormEvent.MouseDown, e);
        public void OnFormMouseUp(object? sender, MouseEventArgs e) => Handle(FormEvent.MouseUp, e);
        public void OnFormMouseMove(object? sender, MouseEventArgs e) => Handle(FormEvent.MouseMove, e);
        public void OnFormMouseWheel(object? sender, MouseEventArgs e) => Handle(FormEvent.MouseWheel, e);
        public void OnFormMouseDoubleClick(object? sender, MouseEventArgs e) => Handle(FormEvent.MouseDoubleClick, e);
        public void OnFormKeyDown(object? sender, KeyEventArgs e) => Handle(FormEvent.KeyDown, e);
        public void OnFormKeyUp(object? sender, KeyEventArgs e) => Handle(FormEvent.KeyUp, e);
        public void OnFormPaint(object? sender, PaintEventArgs e) => Handle(FormEvent.Paint, e);

        public void OnScreenShotStarted(object? sender, ScreenShotEventArgs e) {
            SharedContext = new(e.Form!, e.SelectionRect!, e.DrawingManager!, e.ScreenShot!.Width, e.ScreenShot!.Height);

            foreach (var handler in HandlerList) {
                handler.OnNewSharedContext(SharedContext);
            }
        }

        public void OnScreenShotEnded(object? sender, ScreenShotEventArgs e) {
            SharedContext = null;
        }

        public void ResetHandlers(Context.I ctx) {
            foreach (var handler in HandlerList) {
                if (handler is IResetHandler resetHandler) {
                    resetHandler.Reset(ctx);
                }
            }
        }
    }
}

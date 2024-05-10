using ScreenYu.Handler.Common;

namespace ScreenYu.Handler.Context {
    internal interface I {
        SharedContext SharedContext();
        void Abort();
        void Next();
    }
}

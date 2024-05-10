using ScreenYu.Pkg.Win;

namespace ScreenYu.Lib {
    internal static class HotKey {
        public static void Register(IntPtr hWnd, int hotKeyId, uint key, bool win, bool control, bool shift, bool alt) {
            uint hotKeyModifiers = (uint)WinApi.ModifierKeys.NoRepeat;

            if (win) {
                hotKeyModifiers |= (uint)WinApi.ModifierKeys.Win;
            }
            if (control) {
                hotKeyModifiers |= (uint)WinApi.ModifierKeys.Control;
            }
            if (shift) {
                hotKeyModifiers |= (uint)WinApi.ModifierKeys.Shift;
            }
            if (alt) {
                hotKeyModifiers |= (uint)WinApi.ModifierKeys.Alt;
            }

            // register hotkey to Windows
            Utils.ThrowIfEqual(IntPtr.Zero,
                WinApi.RegisterHotKey(hWnd, hotKeyId, hotKeyModifiers, key)
            );
        }
    }
}

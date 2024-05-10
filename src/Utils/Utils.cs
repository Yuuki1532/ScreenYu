namespace ScreenYu {
    internal static partial class Utils {
        public static void ThrowIfEqual<T>(T a, T b) where T: IEquatable<T> {
            if (a.Equals(b)) {
                throw new Exception();
            }
        }

        public static void ThrowIfNull<T>(T? a) {
            if (a == null) {
                throw new Exception();
            }
        }

    }
}

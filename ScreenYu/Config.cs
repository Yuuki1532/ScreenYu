namespace ScreenYu {

    public static class Config {

        public static DrawingConfig Drawing = new();
        public static KeyBindingConfig KeyBinding = new();
        public static readonly HotKeyConfig HotKey = new();
        public static SelectionConfig Selection = new();

    }

    public class DrawingConfig {

        public float MinStrokeSize = 1f;
        public float MaxStrokeSize = 6f;
        public List<Color> ColorList = new List<Color>() {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(230, 30, 30),
            Color.FromArgb(255, 160, 0),
            Color.FromArgb(32, 250, 90),
            Color.FromArgb(40, 90, 250),
            Color.FromArgb(170, 40, 250),
        };

        public int DefaultColorId = 2;
        public float DefaultStrokeSize = 2.0f;

    }

    public class SelectionConfig {

        public Color SelectionBorderColor = Color.FromArgb(30, 120, 180);
        public readonly float SelectionBorderStrokeSize = 1f;
        public float DimOutsideSelection = 0.3f;

    }

    public class KeyBindingConfig {

        public Keys Cancel = Keys.Escape;
        public Keys DrawRect = Keys.F;
        public Keys DrawLine = Keys.C;
        public Keys Select = Keys.S;

    }

    public class HotKeyConfig {

        public bool WinKey = false;
        public bool Shift = false;
        public bool Alt = true;
        public bool Control = true;
        public uint Key = (uint)Keys.A;

    }

}

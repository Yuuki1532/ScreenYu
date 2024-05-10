namespace ScreenYu {
    internal class Config {
        public DrawingConfig DrawingConfig;
        public KeyBindingConfig KeyBindingConfig;
        public readonly HotKeyConfig HotKeyConfig;
        public SelectionConfig SelectionConfig;

        public Config() {
            DrawingConfig = new();
            KeyBindingConfig = new();
            HotKeyConfig = new();
            SelectionConfig = new();
        }
    }

    internal class DrawingConfig {
        public readonly float MinStrokeSize;
        public readonly float MaxStrokeSize;

        public List<Color> StrokeColorList;
        public int InitialStrokeColorIdx;
        public float InitialStrokeSize;

        public DrawingConfig() {
            MinStrokeSize = 1f;
            MaxStrokeSize = 6f;
            StrokeColorList = new() {
                Color.FromArgb(0, 0, 0),
                Color.FromArgb(230, 30, 30),
                Color.FromArgb(255, 160, 0),
                Color.FromArgb(32, 250, 90),
                Color.FromArgb(40, 90, 250),
                Color.FromArgb(170, 40, 250),
            };
            InitialStrokeColorIdx = 2;
            InitialStrokeSize = 2f;
        }

    }

    internal class SelectionConfig {
        public Color SelectionBorderColor;
        public float SelectionBorderStrokeSize;
        public float DimOutsideSelection;

        public SelectionConfig() {
            SelectionBorderColor = Color.FromArgb(30, 120, 180);
            SelectionBorderStrokeSize = 1f;
            DimOutsideSelection = 0.3f;
        }
    }

    internal class KeyBindingConfig {
        public Keys Cancel;
        public Keys DrawRectMode;
        public Keys DrawLineMode;
        public Keys SelectAreaMode;
        public Keys Reset;
        public Keys ShowHint;

        public KeyBindingConfig() {
            Cancel = Keys.Escape;
            DrawRectMode = Keys.F;
            DrawLineMode = Keys.C;
            SelectAreaMode = Keys.S;
            Reset = Keys.R;
            ShowHint = Keys.Tab;
        }
    }

    internal class HotKeyConfig {
        public bool WinKey;
        public bool Shift;
        public bool Alt;
        public bool Control;
        public uint Key;

        public HotKeyConfig() {
            WinKey = false;
            Shift = false;
            Alt = true;
            Control = true;
            Key = (uint)Keys.A;
        }
    }
}

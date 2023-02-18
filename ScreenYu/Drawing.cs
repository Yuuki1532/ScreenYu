using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenYu {
    public partial class Drawing {
        public abstract class Object {
        
        }

        public class Rect : Object {
            public int x1, y1, x2, y2;
            public string penId;
        }


    }

}
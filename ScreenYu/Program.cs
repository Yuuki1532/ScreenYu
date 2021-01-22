using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ScreenYu {
    static class Program {
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetProcessDPIAware();
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main() {
            // SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new Main();
            Application.Run();
        }
    }
}

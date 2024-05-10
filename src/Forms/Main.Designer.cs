namespace ScreenYu.Forms {
    partial class Main {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            notifyIconMain = new NotifyIcon(components);
            contextMenuStripNotifyIcon = new ContextMenuStrip(components);
            exitToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStripNotifyIcon.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIconMain
            // 
            notifyIconMain.ContextMenuStrip = contextMenuStripNotifyIcon;
            notifyIconMain.Icon = (Icon)resources.GetObject("notifyIconMain.Icon");
            notifyIconMain.Text = "ScreenYu";
            notifyIconMain.Visible = true;
            // 
            // contextMenuStripNotifyIcon
            // 
            contextMenuStripNotifyIcon.Items.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            contextMenuStripNotifyIcon.Name = "contextMenuStripNotifyIcon";
            contextMenuStripNotifyIcon.Size = new Size(95, 26);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(94, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(22F, 47F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(260, 221);
            DoubleBuffered = true;
            Font = new Font("Microsoft JhengHei UI", 27.75F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(9);
            Name = "Main";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "ScreenYu";
            contextMenuStripNotifyIcon.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon notifyIconMain;
        private ContextMenuStrip contextMenuStripNotifyIcon;
        private ToolStripMenuItem exitToolStripMenuItem;
    }
}
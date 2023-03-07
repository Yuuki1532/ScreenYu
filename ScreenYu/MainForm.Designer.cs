namespace ScreenYu {
    partial class MainForm {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            mainNotifyIcon = new NotifyIcon(components);
            mainNotifyIconContextMenuStrip = new ContextMenuStrip(components);
            exitToolStripMenuItem = new ToolStripMenuItem();
            mainNotifyIconContextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // mainNotifyIcon
            // 
            mainNotifyIcon.ContextMenuStrip = mainNotifyIconContextMenuStrip;
            mainNotifyIcon.Icon = (Icon)resources.GetObject("mainNotifyIcon.Icon");
            mainNotifyIcon.Text = "mainNotifyIcon";
            mainNotifyIcon.Visible = true;
            mainNotifyIcon.MouseClick += mainNotifyIcon_MouseClick;
            // 
            // mainNotifyIconContextMenuStrip
            // 
            mainNotifyIconContextMenuStrip.Items.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            mainNotifyIconContextMenuStrip.Name = "mainNotifyIconContextMenuStrip";
            mainNotifyIconContextMenuStrip.Size = new Size(95, 26);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(94, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(184, 161);
            Name = "MainForm";
            Text = "main";
            FormClosed += MainForm_FormClosed;
            mainNotifyIconContextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon mainNotifyIcon;
        private ContextMenuStrip mainNotifyIconContextMenuStrip;
        private ToolStripMenuItem exitToolStripMenuItem;
    }
}
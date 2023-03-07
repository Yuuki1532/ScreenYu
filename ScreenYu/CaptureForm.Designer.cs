namespace ScreenYu {
    partial class CaptureForm {
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
            SuspendLayout();
            // 
            // CaptureForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(200, 200);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "CaptureForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "CaptureForm";
            KeyDown += CaptureForm_KeyDown;
            MouseDown += CaptureForm_MouseDown;
            MouseMove += CaptureForm_MouseMove;
            MouseUp += CaptureForm_MouseUp;
            ResumeLayout(false);
        }

        #endregion
    }
}
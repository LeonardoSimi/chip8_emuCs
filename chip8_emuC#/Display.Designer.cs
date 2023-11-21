namespace chip8_emu
{
    partial class Display
    {

        private PictureBox pbScreen;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
		private void InitializeComponent()
        {
            pbScreen = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pbScreen).BeginInit();
            SuspendLayout();
            // 
            // pbScreen
            // 
            pbScreen.Dock = DockStyle.Fill;
            pbScreen.Location = new Point(0, 0);
            pbScreen.Margin = new Padding(4, 3, 4, 3);
            pbScreen.Name = "pbScreen";
            pbScreen.Size = new Size(775, 590);
            pbScreen.SizeMode = PictureBoxSizeMode.StretchImage;
            pbScreen.TabIndex = 0;
            pbScreen.TabStop = false;
            pbScreen.Click += pbScreen_Click;
            // 
            // Display
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(775, 590);
            Controls.Add(pbScreen);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Display";
            Text = "C#hip8";
            ((System.ComponentModel.ISupportInitialize)pbScreen).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}
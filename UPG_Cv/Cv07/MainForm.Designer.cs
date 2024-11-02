namespace Cv07
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            bttnSave = new Button();
            drawingPanel = new Panel();
            SuspendLayout();
            // 
            // bttnSave
            // 
            bttnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            bttnSave.Location = new Point(713, 415);
            bttnSave.Name = "bttnSave";
            bttnSave.Size = new Size(75, 23);
            bttnSave.TabIndex = 0;
            bttnSave.Text = "Save";
            bttnSave.UseVisualStyleBackColor = true;
            // 
            // drawingPanel
            // 
            drawingPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            drawingPanel.Location = new Point(12, 12);
            drawingPanel.Name = "drawingPanel";
            drawingPanel.Size = new Size(776, 397);
            drawingPanel.TabIndex = 1;
            drawingPanel.Resize += drawingPanel_Resize;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(drawingPanel);
            Controls.Add(bttnSave);
            Name = "MainForm";
            Text = "Moje okénko";
            ResumeLayout(false);
        }

        #endregion

        private Button bttnSave;
        private Panel drawingPanel;
    }
}

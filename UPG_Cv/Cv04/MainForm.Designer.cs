namespace Cv04
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			drawingPanel = new Panel();
			btnPrint = new Button();
			btnExit = new Button();
			printDocument1 = new System.Drawing.Printing.PrintDocument();
			printPreviewDialog1 = new PrintPreviewDialog();
			SuspendLayout();
			// 
			// drawingPanel
			// 
			drawingPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			drawingPanel.Location = new Point(0, 0);
			drawingPanel.Name = "drawingPanel";
			drawingPanel.Size = new Size(800, 409);
			drawingPanel.TabIndex = 0;
			drawingPanel.Paint += drawingPanel_Paint;
			drawingPanel.Resize += drawingPanel_Resize;
			// 
			// btnPrint
			// 
			btnPrint.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnPrint.Location = new Point(632, 415);
			btnPrint.Name = "btnPrint";
			btnPrint.Size = new Size(75, 23);
			btnPrint.TabIndex = 1;
			btnPrint.Text = "Print ...";
			btnPrint.UseVisualStyleBackColor = true;
			btnPrint.Click += btnPrint_Click;
			// 
			// btnExit
			// 
			btnExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnExit.Location = new Point(713, 415);
			btnExit.Name = "btnExit";
			btnExit.Size = new Size(75, 23);
			btnExit.TabIndex = 2;
			btnExit.Text = "Exit";
			btnExit.UseVisualStyleBackColor = true;
			btnExit.Click += btnExit_Click;
			// 
			// printDocument1
			// 
			printDocument1.PrintPage += printDocument1_PrintPage;
			// 
			// printPreviewDialog1
			// 
			printPreviewDialog1.AutoScrollMargin = new Size(0, 0);
			printPreviewDialog1.AutoScrollMinSize = new Size(0, 0);
			printPreviewDialog1.ClientSize = new Size(400, 300);
			printPreviewDialog1.Document = printDocument1;
			printPreviewDialog1.Enabled = true;
			printPreviewDialog1.Icon = (Icon)resources.GetObject("printPreviewDialog1.Icon");
			printPreviewDialog1.Name = "printPreviewDialog1";
			printPreviewDialog1.Visible = false;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
			Controls.Add(btnExit);
			Controls.Add(btnPrint);
			Controls.Add(drawingPanel);
			Name = "MainForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Moje okénko";
			ResumeLayout(false);
		}

		#endregion

		private Panel drawingPanel;
        private Button btnPrint;
        private Button btnExit;
		private System.Drawing.Printing.PrintDocument printDocument1;
		private PrintPreviewDialog printPreviewDialog1;
	}
}

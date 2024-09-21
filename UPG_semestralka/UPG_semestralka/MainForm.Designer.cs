namespace UPG_semestralka
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
			drawingPanel = new Panel();
			SuspendLayout();
			// 
			// drawingPanel
			// 
			drawingPanel.Dock = DockStyle.Fill;
			drawingPanel.Location = new Point(0, 0);
			drawingPanel.Name = "drawingPanel";
			drawingPanel.Size = new Size(784, 561);
			drawingPanel.TabIndex = 0;
			drawingPanel.Paint += drawingPanel_Paint;
			drawingPanel.Resize += drawingPanel_Resize;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(784, 561);
			Controls.Add(drawingPanel);
			Name = "MainForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Vizualizace elektrostatického pole";
			ResumeLayout(false);
		}

		#endregion

		private Panel drawingPanel;
	}
}

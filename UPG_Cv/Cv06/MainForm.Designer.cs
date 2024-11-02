using System.Drawing;
using System.Windows.Forms;

namespace Cv06
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
			components = new System.ComponentModel.Container();
			drawingPanel = new Panel();
			bttnExit = new Button();
			bttnMakeStarSmaller = new Button();
			btnColor = new Button();
			timer1 = new System.Windows.Forms.Timer(components);
			SuspendLayout();
			// 
			// drawingPanel
			// 
			drawingPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			drawingPanel.Location = new Point(0, 0);
			drawingPanel.Name = "drawingPanel";
			drawingPanel.Size = new Size(800, 392);
			drawingPanel.TabIndex = 0;
			drawingPanel.Paint += drawingPanel_Paint;
			drawingPanel.Resize += drawingPanel_Resize;
			// 
			// bttnExit
			// 
			bttnExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			bttnExit.Location = new Point(713, 415);
			bttnExit.Name = "bttnExit";
			bttnExit.Size = new Size(75, 23);
			bttnExit.TabIndex = 1;
			bttnExit.Text = "E&xit";
			bttnExit.UseVisualStyleBackColor = true;
			bttnExit.Click += bttnExit_Click;
			// 
			// bttnMakeStarSmaller
			// 
			bttnMakeStarSmaller.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			bttnMakeStarSmaller.Location = new Point(569, 415);
			bttnMakeStarSmaller.Name = "bttnMakeStarSmaller";
			bttnMakeStarSmaller.Size = new Size(138, 23);
			bttnMakeStarSmaller.TabIndex = 2;
			bttnMakeStarSmaller.Text = "Make star smaller";
			bttnMakeStarSmaller.UseVisualStyleBackColor = true;
			bttnMakeStarSmaller.Click += bttnMakeStarSmaller_Click;
			// 
			// btnColor
			// 
			btnColor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnColor.Location = new Point(31, 415);
			btnColor.Name = "btnColor";
			btnColor.Size = new Size(132, 23);
			btnColor.TabIndex = 3;
			btnColor.Text = "Zmena barvy";
			btnColor.UseVisualStyleBackColor = true;
			btnColor.Click += btnColor_Click;
			// 
			// timer1
			// 
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
			Controls.Add(btnColor);
			Controls.Add(bttnMakeStarSmaller);
			Controls.Add(bttnExit);
			Controls.Add(drawingPanel);
			Name = "MainForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Moje okénko";
			ResumeLayout(false);
		}

		#endregion

		private Panel drawingPanel;
		private Button bttnExit;
		private Button bttnMakeStarSmaller;
		private Button btnColor;
		private System.Windows.Forms.Timer timer1;
	}
}

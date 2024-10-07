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
			components = new System.ComponentModel.Container();
			drawingPanel = new Panel();
			btnScenario3 = new Button();
			btnScenario2 = new Button();
			btnScenario1 = new Button();
			btnScenario0 = new Button();
			timer = new System.Windows.Forms.Timer(components);
			drawingPanel.SuspendLayout();
			SuspendLayout();
			// 
			// drawingPanel
			// 
			drawingPanel.Controls.Add(btnScenario3);
			drawingPanel.Controls.Add(btnScenario2);
			drawingPanel.Controls.Add(btnScenario1);
			drawingPanel.Controls.Add(btnScenario0);
			drawingPanel.Dock = DockStyle.Fill;
			drawingPanel.Location = new Point(0, 0);
			drawingPanel.Name = "drawingPanel";
			drawingPanel.Size = new Size(784, 561);
			drawingPanel.TabIndex = 0;
			drawingPanel.Paint += drawingPanel_Paint;
			drawingPanel.Resize += drawingPanel_Resize;
			// 
			// btnScenario3
			// 
			btnScenario3.Location = new Point(171, 3);
			btnScenario3.Name = "btnScenario3";
			btnScenario3.Size = new Size(50, 50);
			btnScenario3.TabIndex = 3;
			btnScenario3.Text = "3";
			btnScenario3.UseVisualStyleBackColor = true;
			btnScenario3.Click += btnScenario3_Click;
			// 
			// btnScenario2
			// 
			btnScenario2.Location = new Point(115, 3);
			btnScenario2.Name = "btnScenario2";
			btnScenario2.Size = new Size(50, 50);
			btnScenario2.TabIndex = 2;
			btnScenario2.Text = "2";
			btnScenario2.UseVisualStyleBackColor = true;
			btnScenario2.Click += btnScenario2_Click;
			// 
			// btnScenario1
			// 
			btnScenario1.Location = new Point(59, 3);
			btnScenario1.Name = "btnScenario1";
			btnScenario1.Size = new Size(50, 50);
			btnScenario1.TabIndex = 1;
			btnScenario1.Text = "1";
			btnScenario1.UseVisualStyleBackColor = true;
			btnScenario1.Click += btnScenario1_Click;
			// 
			// btnScenario0
			// 
			btnScenario0.Location = new Point(3, 3);
			btnScenario0.Name = "btnScenario0";
			btnScenario0.Size = new Size(50, 50);
			btnScenario0.TabIndex = 0;
			btnScenario0.Text = "0";
			btnScenario0.UseVisualStyleBackColor = true;
			btnScenario0.Click += btnScenario0_Click;
			// 
			// timer
			// 
			timer.Interval = 10;
			timer.Tick += timer_Tick;
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
			drawingPanel.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private Panel drawingPanel;
		private Button btnScenario0;
		private Button btnScenario3;
		private Button btnScenario2;
		private Button btnScenario1;
		private System.Windows.Forms.Timer timer;
	}
}

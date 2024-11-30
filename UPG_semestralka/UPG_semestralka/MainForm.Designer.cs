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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			drawingPanel = new Panel();
			panel2 = new Panel();
			btnScenario4 = new Button();
			btnScenario0 = new Button();
			btnScenario1 = new Button();
			btnScenario3 = new Button();
			btnScenario2 = new Button();
			panel1 = new Panel();
			radioButton2 = new RadioButton();
			radioButton05 = new RadioButton();
			radioButton0 = new RadioButton();
			radioButton1 = new RadioButton();
			timer = new System.Windows.Forms.Timer(components);
			backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			drawingPanel.SuspendLayout();
			panel2.SuspendLayout();
			panel1.SuspendLayout();
			SuspendLayout();
			// 
			// drawingPanel
			// 
			drawingPanel.Controls.Add(panel2);
			drawingPanel.Controls.Add(panel1);
			drawingPanel.Dock = DockStyle.Fill;
			drawingPanel.Location = new Point(0, 0);
			drawingPanel.Name = "drawingPanel";
			drawingPanel.Size = new Size(784, 561);
			drawingPanel.TabIndex = 0;
			drawingPanel.Paint += drawingPanel_Paint;
			drawingPanel.DoubleClick += drawingPanel_DoubleClick;
			drawingPanel.MouseDown += drawingPanel_MouseDown;
			drawingPanel.MouseMove += drawingPanel_MouseMove;
			drawingPanel.MouseUp += drawingPanel_MouseUp;
			drawingPanel.MouseWheel += drawingPanel_MouseWheel;
			drawingPanel.Resize += drawingPanel_Resize;
			// 
			// panel2
			// 
			panel2.BackColor = SystemColors.ControlDark;
			panel2.Controls.Add(btnScenario4);
			panel2.Controls.Add(btnScenario0);
			panel2.Controls.Add(btnScenario1);
			panel2.Controls.Add(btnScenario3);
			panel2.Controls.Add(btnScenario2);
			panel2.Location = new Point(3, 3);
			panel2.Name = "panel2";
			panel2.Size = new Size(283, 57);
			panel2.TabIndex = 5;
			// 
			// btnScenario4
			// 
			btnScenario4.Location = new Point(227, 3);
			btnScenario4.Name = "btnScenario4";
			btnScenario4.Size = new Size(50, 50);
			btnScenario4.TabIndex = 4;
			btnScenario4.Text = "4";
			btnScenario4.UseVisualStyleBackColor = true;
			btnScenario4.Click += btnScenario4_Click;
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
			// panel1
			// 
			panel1.BackColor = SystemColors.ControlDark;
			panel1.Controls.Add(radioButton2);
			panel1.Controls.Add(radioButton05);
			panel1.Controls.Add(radioButton0);
			panel1.Controls.Add(radioButton1);
			panel1.Location = new Point(308, 10);
			panel1.Name = "panel1";
			panel1.Size = new Size(198, 50);
			panel1.TabIndex = 4;
			// 
			// radioButton2
			// 
			radioButton2.AutoSize = true;
			radioButton2.Location = new Point(150, 16);
			radioButton2.Name = "radioButton2";
			radioButton2.Size = new Size(35, 22);
			radioButton2.TabIndex = 4;
			radioButton2.TabStop = true;
			radioButton2.Text = "2x";
			radioButton2.UseCompatibleTextRendering = true;
			radioButton2.UseVisualStyleBackColor = true;
			radioButton2.CheckedChanged += radioButton2_CheckedChanged;
			// 
			// radioButton05
			// 
			radioButton05.AutoSize = true;
			radioButton05.Location = new Point(55, 16);
			radioButton05.Name = "radioButton05";
			radioButton05.Size = new Size(46, 19);
			radioButton05.TabIndex = 2;
			radioButton05.TabStop = true;
			radioButton05.Text = "0.5x";
			radioButton05.UseVisualStyleBackColor = true;
			radioButton05.CheckedChanged += radioButton05_CheckedChanged;
			// 
			// radioButton0
			// 
			radioButton0.AutoSize = true;
			radioButton0.Location = new Point(12, 16);
			radioButton0.Name = "radioButton0";
			radioButton0.Size = new Size(37, 19);
			radioButton0.TabIndex = 1;
			radioButton0.TabStop = true;
			radioButton0.Text = "0x";
			radioButton0.UseVisualStyleBackColor = true;
			radioButton0.CheckedChanged += radioButton0_CheckedChanged;
			// 
			// radioButton1
			// 
			radioButton1.AutoSize = true;
			radioButton1.Location = new Point(107, 16);
			radioButton1.Name = "radioButton1";
			radioButton1.Size = new Size(37, 19);
			radioButton1.TabIndex = 0;
			radioButton1.TabStop = true;
			radioButton1.Text = "1x";
			radioButton1.UseVisualStyleBackColor = true;
			radioButton1.CheckedChanged += radioButton1_CheckedChanged;
			// 
			// timer
			// 
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
			panel2.ResumeLayout(false);
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private Panel drawingPanel;
		private Button btnScenario0;
		private Button btnScenario3;
		private Button btnScenario2;
		private Button btnScenario1;
		private System.Windows.Forms.Timer timer;
		private Panel panel2;
		private Panel panel1;
		private RadioButton radioButton1;
		private RadioButton radioButton0;
		private RadioButton radioButton2;
		private RadioButton radioButton05;
		private Button btnScenario4;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
	}
}

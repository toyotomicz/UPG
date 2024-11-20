using System.Drawing;
using System.Windows.Forms;

namespace Cv10
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
			cartesianChart1 = new LiveChartsCore.SkiaSharpView.WinForms.CartesianChart();
			SuspendLayout();
			// 
			// cartesianChart1
			// 
			cartesianChart1.Location = new Point(92, 60);
			cartesianChart1.Name = "cartesianChart1";
			cartesianChart1.Size = new Size(596, 317);
			cartesianChart1.TabIndex = 0;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
			Controls.Add(cartesianChart1);
			Name = "MainForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Moje okénko";
			ResumeLayout(false);
		}

		#endregion

		private LiveChartsCore.SkiaSharpView.WinForms.CartesianChart cartesianChart1;
	}
}

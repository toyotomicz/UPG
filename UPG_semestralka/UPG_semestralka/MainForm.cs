using System.Drawing.Drawing2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Drawing.Printing;

namespace UPG_semestralka
{
	public partial class MainForm : Form
	{
		private int scenario; // Scenario obtained from argument
		private const double k = 8.99e9; // Coulomb's constant
		private List<(PointF position, Func<double, double> charge)> charges; // Position and size of each particle's charge
		private PointF probePosition;  // Position of the probe

		// Boundaries of the "real world"
		private double x_min, x_max, y_min, y_max;
		private double world_width, world_height;

		private int gridSpacingX;
		private int gridSpacingY;

		private double time = 0;
		private const double angularVelocity = Math.PI / 6; // Angular velocity of the probe
		private double velocityMultiplier = 1.0; // Angular velocity multiplier

		private int timerInterval = 100;

		private PrintDocument printDocument;

		public MainForm(int scenario, int gridSpacingX, int gridSpacingY)
		{
			InitializeComponent();
			this.MinimumSize = new Size(800, 600);
			this.scenario = scenario;
			this.gridSpacingX = gridSpacingX;
			this.gridSpacingY = gridSpacingY;

			// Enable double buffering
			this.SetStyle(
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.DoubleBuffer,
				true);

			InitializeWorld();

			timer.Start();
			timer.Tick += timer_Tick;
			timer.Interval = timerInterval;
		}

		private void InitializeWorld()
		{
			charges = new List<(PointF position, Func<double, double> charge)>();
			switch (scenario)
			{
				case 0:
					charges.Add((new PointF(0, 0), t => 1));
					break;
				case 1:
					charges.Add((new PointF(-1, 0), t => 1));
					charges.Add((new PointF(1, 0), t => 1));
					break;
				case 2:
					charges.Add((new PointF(-1, 0), t => -1));
					charges.Add((new PointF(1, 0), t => 2));
					break;
				case 3:
					charges.Add((new PointF(-1, -1), t => 1));
					charges.Add((new PointF(1, -1), t => 2));
					charges.Add((new PointF(1, 1), t => -3));
					charges.Add((new PointF(-1, 1), t => -4));
					break;
				case 4:
					charges.Add((new PointF(-1, 0), t => 1 + 0.5 * Math.Sin(Math.PI * t / 2)));
					charges.Add((new PointF(1, 0), t => 1 - 0.5 * Math.Sin(Math.PI * t / 2)));
					break;
			}

			probePosition = new PointF(0, 1);
			UpdateProbePosition();

			// Set the boundaries of the world
			x_min = -2;
			x_max = +2;
			y_min = -2;
			y_max = +2;
			world_width = x_max - x_min;
			world_height = y_max - y_min;

			UpdateTitle();
		}

		private void UpdateTitle()
		{
			this.Text = "Student: A23B0272P, Visualization of Electrostatic Field, Scenario: " + Convert.ToString(scenario);
		}

		private void drawingPanel_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			g.SmoothingMode = SmoothingMode.HighSpeed;

			double scale_x = drawingPanel.Width / world_width;
			double scale_y = drawingPanel.Height / world_height;
			double scale = Math.Min(scale_x, scale_y);

			float offsetX = (float)((drawingPanel.Width - (world_width * scale)) / 2);
			float offsetY = (float)((drawingPanel.Height - (world_height * scale)) / 2);

			DrawGrid(g, scale);

			g.TranslateTransform(offsetX, offsetY);

			foreach (var charge in charges)
			{
				DrawCharge(g, charge.position, charge.charge(time), scale);
			}

			Vector2D forceVector = CalculateForceOnProbe(probePosition, charges);
			DrawProbe(g, probePosition, forceVector, scale);

			g.ResetTransform();
		}

		private void DrawGrid(Graphics g, double scale)
		{
			int width = this.ClientSize.Width;
			int height = this.ClientSize.Height;

			int centerX = width / 2;
			int centerY = height / 2;

			Pen axisPen = new Pen(Color.Black, 2);
			Pen gridPen = new Pen(Color.LightGray, 1);

			// Draw axes
			g.DrawLine(axisPen, centerX, 0, centerX, height);
			g.DrawLine(axisPen, 0, centerY, width, centerY);

			// Drawing grid lines with specified spacing
			for (int x = centerX; x < width; x += gridSpacingX)
			{
				g.DrawLine(gridPen, x, 0, x, height);
			}

			for (int x = centerX; x > 0; x -= gridSpacingX)
			{
				g.DrawLine(gridPen, x, 0, x, height);
			}

			for (int y = centerY; y < height; y += gridSpacingY)
			{
				g.DrawLine(gridPen, 0, y, width, y);
			}

			for (int y = centerY; y > 0; y -= gridSpacingY)
			{
				g.DrawLine(gridPen, 0, y, width, y);
			}
		}

		private void DrawCharge(Graphics g, PointF position, double charge, double scale)
		{
			float x = (float)((position.X - x_min) * scale);
			float y = (float)((y_max - position.Y) * scale);

			float area = (float)(Math.Abs(charge) * 0.2 * scale * scale);
			float radius = (float)Math.Sqrt(area / Math.PI);
			float size = radius * 2;

			Color chargeColor = charge > 0 ? Color.Red : Color.Blue;

			g.FillEllipse(Brushes.Black, x - size / 2, y - size / 2, size, size);

			var path = new GraphicsPath();
			{
				path.AddEllipse(x - radius, y - radius, size, size);

				using (PathGradientBrush pthGrBrush = new PathGradientBrush(path))
				{
					pthGrBrush.CenterColor = chargeColor;
					pthGrBrush.SurroundColors = new Color[] { Color.FromArgb(100, chargeColor) };
					pthGrBrush.FocusScales = new PointF(0.5f, 0.5f);
					g.FillPath(pthGrBrush, path);
				}
			}

			g.DrawEllipse(new Pen(Color.Black, 1), x - radius, y - radius, size, size);

			string chargeText = scenario == 4 ? $"{charge:F2} C" : $"{charge:F0} C";
			using (Font chargeFont = new Font("Arial", (float)(10 * scale / 100), FontStyle.Bold))
			{
				SizeF textSize = g.MeasureString(chargeText, chargeFont);
				g.DrawString(chargeText, chargeFont, Brushes.White, x - textSize.Width / 2, y - textSize.Height / 2);
			}
		}

		private void DrawProbe(Graphics g, PointF position, Vector2D forceVector, double scale)
		{
			float x = (float)((position.X - x_min) * scale);
			float y = (float)((y_max - position.Y) * scale);
			float probeSize = (float)(0.1 * scale);

			g.FillEllipse(Brushes.Green, x - probeSize / 2, y - probeSize / 2, probeSize, probeSize);

			float arrowLength = (float)(0.5 * scale);
			float angleRad = (float)Math.Atan2(-forceVector.Y, forceVector.X);
			PointF arrowEnd = new PointF(
				x + arrowLength * (float)Math.Cos(angleRad),
				y + arrowLength * (float)Math.Sin(angleRad)
			);

			Pen arrowPen = new Pen(Color.Black, (float)(2 * scale / 100));
			g.DrawLine(arrowPen, x, y, arrowEnd.X, arrowEnd.Y);
			DrawArrowHead(g, arrowPen, new PointF(x, y), arrowEnd, (float)(0.1 * scale));

			string forceText = $"{forceVector.Magnitude():E2} N/C";
			using (Font forceFont = new Font("Arial", (float)(8 * scale / 100), FontStyle.Regular))
			{
				g.DrawString(forceText, forceFont, Brushes.Black, x + (float)(0.1 * scale), y - (float)(0.3 * scale));
			}
		}

		private void DrawArrowHead(Graphics g, Pen pen, PointF start, PointF end, float size)
		{
			float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
			PointF point1 = new PointF(
				(float)(end.X - size * Math.Cos(angle - Math.PI / 6)),
				(float)(end.Y - size * Math.Sin(angle - Math.PI / 6))
			);
			PointF point2 = new PointF(
				(float)(end.X - size * Math.Cos(angle + Math.PI / 6)),
				(float)(end.Y - size * Math.Sin(angle + Math.PI / 6))
			);
			g.DrawLine(pen, end, point1);
			g.DrawLine(pen, end, point2);
		}

		private Vector2D CalculateForceOnProbe(PointF probePosition, List<(PointF position, Func<double, double> charge)> charges)
		{
			Vector2D electricField = new Vector2D(0, 0);

			foreach (var charge in charges)
			{
				double q = charge.charge(time);
				Vector2D r = new Vector2D(
					probePosition.X - charge.position.X,
					probePosition.Y - charge.position.Y
				);
				double rMagnitude = r.Magnitude();

				if (rMagnitude == 0)
				{
					continue;
				}

				Vector2D fieldContribution = r / Math.Pow(rMagnitude, 3);
				electricField += fieldContribution * (q * k);
			}

			return electricField;
		}

		private void drawingPanel_Resize(object sender, EventArgs e)
		{
			this.drawingPanel.Invalidate(); // Redraw the panel on resize
		}

		// Buttons for changing scenarios
		private void btnScenario0_Click(object sender, EventArgs e)
		{
			scenario = 0; // Set scenario 0
			InitializeWorld(); // Reinitialize world
			this.drawingPanel.Invalidate(); // Redraw
		}

		private void btnScenario1_Click(object sender, EventArgs e)
		{
			scenario = 1; // Set scenario 1
			InitializeWorld(); // Reinitialize world
			this.drawingPanel.Invalidate(); // Redraw
		}

		private void btnScenario2_Click(object sender, EventArgs e)
		{
			scenario = 2; // Set scenario 2
			InitializeWorld(); // Reinitialize world
			this.drawingPanel.Invalidate(); // Redraw
		}

		private void btnScenario3_Click(object sender, EventArgs e)
		{
			scenario = 3; // Set scenario 3
			InitializeWorld(); // Reinitialize world
			this.drawingPanel.Invalidate(); // Redraw
		}

		private void btnScenario4_Click(object sender, EventArgs e)
		{
			scenario = 4; // Set scenario 4
			InitializeWorld(); // Reinitialize world
			this.drawingPanel.Invalidate(); // Redraw
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			time += 50 / 1000.0; // Increment time
			UpdateProbePosition(); // Update probe position
			this.drawingPanel.Invalidate(); // Redraw
		}

		private void UpdateProbePosition()
		{
			double angle = angularVelocity * velocityMultiplier * time; // Calculate new angle based on time
			probePosition = new PointF(
				(float)Math.Cos(angle), // Update probe's X position
				(float)Math.Sin(angle)  // Update probe's Y position
			);
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			velocityMultiplier = 1;
			timerInterval = 100;
		}

		private void radioButton0_CheckedChanged(object sender, EventArgs e)
		{
			velocityMultiplier = 0;
			timerInterval = 0;
		}

		private void buttonPrint_Click(object sender, EventArgs e)
		{
			PrintDialog printDialog = new PrintDialog
			{
				Document = printDocument
			};

			if (printDialog.ShowDialog() == DialogResult.OK)
			{
				printDocument1.Print();
			}
		}

		private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			Graphics g = e.Graphics;

			// Redraw the charges and probe on the print document
			double scale_x = e.PageBounds.Width / world_width;
			double scale_y = e.PageBounds.Height / world_height;
			double scale = Math.Min(scale_x, scale_y);

			float offsetX = (float)((e.PageBounds.Width - (world_width * scale)) / 2);
			float offsetY = (float)((e.PageBounds.Height - (world_height * scale)) / 2);

			g.TranslateTransform(offsetX, offsetY);

			foreach (var charge in charges)
			{
				//DrawCharge(g, charge.position, charge.charge, scale);
			}

			Vector2D forceVector = CalculateForceOnProbe(probePosition, charges);
			DrawProbe(g, probePosition, forceVector, scale);
		}
	}
}
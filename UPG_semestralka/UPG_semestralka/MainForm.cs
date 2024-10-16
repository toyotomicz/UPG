using System.Drawing.Drawing2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace UPG_semestralka
{
	public partial class MainForm : Form
	{
		private int scenario; // Scenario obtained from argument
		private const double k = 8.99e9; // Coulomb's constant
		private List<(PointF position, int charge)> charges; // Position and size of each particle's charge
		private PointF probePosition;  // Position of the probe

		// Boundaries of the "real world"
		private double x_min, x_max, y_min, y_max;
		private double world_width, world_height;

		private double time = 0;
		private const double angularVelocity = Math.PI / 6; // Angular velocity of the probe

		public MainForm(int scenario)
		{
			InitializeComponent();
			this.MinimumSize = new Size(800, 600);
			this.scenario = scenario;
			InitializeWorld(); // Initialize world based on the scenario

			timer.Tick += timer_Tick; // Attach the timer tick event handler
			timer.Start();

			// DoubleBuffered = true;
		}

		private void InitializeWorld()
		{
			charges = new List<(PointF position, int charge)>();
			switch (scenario)
			{
				case 0:
					charges.Add((new PointF(0, 0), 1)); // Scenario 0
					break;
				case 1:
					charges.Add((new PointF(-1, 0), 1)); // Scenario 1
					charges.Add((new PointF(1, 0), 1));
					break;
				case 2:
					charges.Add((new PointF(-1, 0), -1)); // Scenario 2
					charges.Add((new PointF(1, 0), 2));
					break;
				case 3:
					charges.Add((new PointF(-1, -1), 1)); // Scenario 3
					charges.Add((new PointF(1, -1), 2));
					charges.Add((new PointF(1, 1), -3));
					charges.Add((new PointF(-1, 1), -4));
					break;
			}

			probePosition = new PointF(0, 1); // Initial probe position
			UpdateProbePosition(); // Update probe's position based on initial state

			UpdateTitle();

			// Set the boundaries of the world
			x_min = -2;
			x_max = +2;
			y_min = -2;
			y_max = +2;
			world_width = x_max - x_min;
			world_height = y_max - y_min;
		}

		private void UpdateTitle()
		{
			this.Text = "Student: A23B0272P, Visualization of Electrostatic Field, Scenario: " + Convert.ToString(scenario);
		}

		private void drawingPanel_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;

			// Calculate scale based on drawing panel dimensions
			double scale_x = drawingPanel.Width / world_width;
			double scale_y = drawingPanel.Height / world_height;
			double scale = Math.Min(scale_x, scale_y);

			// Compute offsets to center the drawing
			float offsetX = (float)((drawingPanel.Width - (world_width * scale)) / 2);
			float offsetY = (float)((drawingPanel.Height - (world_height * scale)) / 2);

			DrawGrid(g, scale); // Draw grid with the calculated scale

			g.TranslateTransform(offsetX, offsetY); // Apply offset for centering

			// Draw all charges in the scenario
			foreach (var charge in charges)
			{
				DrawCharge(g, charge.position, charge.charge, scale);
			}

			// Calculate and draw the probe with the calculated field intensity
			Vector2D forceVector = CalculateForceOnProbe(probePosition, charges);
			DrawProbe(g, probePosition, forceVector, scale);

			// Reset transformation to avoid affecting further drawing
			g.ResetTransform();
		}

		private void DrawGrid(Graphics g, double scale)
		{
			int width = this.ClientSize.Width;
			int height = this.ClientSize.Height;

			// Calculate center position
			int centerX = width / 2;
			int centerY = height / 2;

			Pen axisPen = new Pen(Color.Black, 2);
			Pen gridPen = new Pen(Color.LightGray, 1);

			// Draw axes
			g.DrawLine(axisPen, centerX, 0, centerX, height); // Y-axis
			g.DrawLine(axisPen, 0, centerY, width, centerY); // X-axis

			int lines = 4; // Lines per 1 unit

			for (int x = centerX; x < width; x += (int)(scale / lines)) // Right side grid
			{
				g.DrawLine(gridPen, x, 0, x, height);
			}

			for (int x = centerX; x > 0; x -= (int)(scale / lines)) // Left side grid
			{
				g.DrawLine(gridPen, x, 0, x, height);
			}

			for (int y = centerY; y < height; y += (int)(scale / lines)) // Bottom side grid
			{
				g.DrawLine(gridPen, 0, y, width, y);
			}

			for (int y = centerY; y > 0; y -= (int)(scale / lines)) // Top side grid
			{
				g.DrawLine(gridPen, 0, y, width, y);
			}
		}

		//private void DrawStaticProbes(Graphics g, double scale)
		//{
		//	float gridSpacing = (float)(0.125 * scale); // 0.125 spacing between lines

		//	for (int x = 0; x <= 500; x += 10)
		//	{
		//		PointF position = new PointF(x, x);
		//		Vector2D forceVectorForThisProbe = CalculateForceOnProbe(position, charges);

		//		DrawStaticProbe(g, position, forceVectorForThisProbe, scale);
		//	}
		//}

		private void DrawCharge(Graphics g, PointF position, int charge, double scale)
		{
			float x = (float)((position.X - x_min) * scale);
			float y = (float)((y_max - position.Y) * scale);

			// Calculate area of circle based on charge size
			float area = (float)(Math.Abs(charge) * 0.2 * scale * scale); // 0.2 m^2 per charge unit

			// Calculate radius from area
			float radius = (float)Math.Sqrt(area / Math.PI);

			float size = radius * 2; // Diameter of circle

			Color chargeColor = charge > 0 ? Color.Red : Color.Blue; // Positive charge - red, negative - blue

			g.FillEllipse(Brushes.Black, x - size / 2, y - size / 2, size, size); // Black base

			// Create path for the circle
			GraphicsPath path = new GraphicsPath();
			path.AddEllipse(x - radius, y - radius, size, size);

			// Draw charge color with gradient over the black base
			PathGradientBrush pthGrBrush = new PathGradientBrush(path);
			pthGrBrush.CenterColor = chargeColor;
			pthGrBrush.SurroundColors = new Color[] { Color.FromArgb(100, chargeColor) };
			pthGrBrush.FocusScales = new PointF(0.5f, 0.5f);

			// Draw circle with inner shadow
			g.FillPath(pthGrBrush, path);

			// Circle outline
			g.DrawEllipse(new Pen(Color.Black, 1), x - radius, y - radius, size, size);

			// Charge label
			string chargeText = $"{charge} C";
			Font chargeFont = new Font("Arial", (float)(10 * scale / 100), FontStyle.Bold);
			SizeF textSize = g.MeasureString(chargeText, chargeFont);
			g.DrawString(chargeText, chargeFont, Brushes.White, x - textSize.Width / 2, y - textSize.Height / 2);

			// Release resources
			path.Dispose();
			pthGrBrush.Dispose();
		}

		private void DrawStaticProbe(Graphics g, PointF position, Vector2D forceVector, double scale)
		{
			float x = (float)((position.X - x_min) * scale);
			float y = (float)((y_max - position.Y) * scale);
			float probeSize = (float)(0.1 * scale); // 0.1 size of the large probe

			g.FillEllipse(Brushes.Gray, x - probeSize / 2, y - probeSize / 2, probeSize, probeSize);

			// Draw force direction
			float angleRad = (float)Math.Atan2(-forceVector.Y, forceVector.X);
			PointF arrowEnd = new PointF(
				x * (float)Math.Cos(angleRad),
				y * (float)Math.Sin(angleRad)
			);

			Pen arrowPen = new Pen(Color.Gray, (float)(2 * scale / 100));
			g.DrawLine(arrowPen, x, y, arrowEnd.X, arrowEnd.Y);
			DrawArrowHead(g, arrowPen, new PointF(x, y), arrowEnd, (float)(0.1 * scale));

			// Text indicating the magnitude of the charge at the probe's location
			string forceText = $"{forceVector.Magnitude():E2} N/C";
			Font forceFont = new Font("Arial", (float)(8 * scale / 100), FontStyle.Regular);
			g.DrawString(forceText, forceFont, Brushes.Black, x + (float)(0.1 * scale), y - (float)(0.3 * scale));
		}

		private void DrawProbe(Graphics g, PointF position, Vector2D forceVector, double scale)
		{
			float x = (float)((position.X - x_min) * scale);
			float y = (float)((y_max - position.Y) * scale);
			float probeSize = (float)(0.1 * scale); // 0.1 size of the large probe

			g.FillEllipse(Brushes.Green, x - probeSize / 2, y - probeSize / 2, probeSize, probeSize);

			// Draw force direction
			float arrowLength = (float)(0.5 * scale); // Scale arrow for visualization
			float angleRad = (float)Math.Atan2(-forceVector.Y, forceVector.X);
			PointF arrowEnd = new PointF(
				x + arrowLength * (float)Math.Cos(angleRad),
				y + arrowLength * (float)Math.Sin(angleRad)
			);

			Pen arrowPen = new Pen(Color.Black, (float)(2 * scale / 100));
			g.DrawLine(arrowPen, x, y, arrowEnd.X, arrowEnd.Y);
			DrawArrowHead(g, arrowPen, new PointF(x, y), arrowEnd, (float)(0.1 * scale));

			// Text indicating the magnitude of the charge at the probe's location
			string forceText = $"{forceVector.Magnitude():E2} N/C";
			Font forceFont = new Font("Arial", (float)(8 * scale / 100), FontStyle.Regular);
			g.DrawString(forceText, forceFont, Brushes.Black, x + (float)(0.1 * scale), y - (float)(0.3 * scale));
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

		private Vector2D CalculateForceOnProbe(PointF probePosition, List<(PointF position, int charge)> charges)
		{
			Vector2D electricField = new Vector2D(0, 0);

			foreach (var charge in charges) // Iterate over all charges in the scenario, calculate according to Coulomb's law
			{
				double q = charge.charge;
				Vector2D r = new Vector2D(
					probePosition.X - charge.position.X,
					probePosition.Y - charge.position.Y
				);
				double rMagnitude = r.Magnitude();

				if (rMagnitude == 0) // Prevent division by zero
				{
					continue;
				}

				Vector2D fieldContribution = r / Math.Pow(rMagnitude, 3);
				electricField += fieldContribution * (q * k);
			}

			return electricField; // Return the calculated electric field at the probe's position
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

		private void timer_Tick(object sender, EventArgs e)
		{
			time += timer.Interval / 1000.0; // Increment time
			UpdateProbePosition(); // Update probe position
			this.drawingPanel.Invalidate(); // Redraw
		}

		private void UpdateProbePosition()
		{
			double angle = angularVelocity * time; // Calculate new angle based on time
			probePosition = new PointF(
				(float)Math.Cos(angle), // Update probe's X position
				(float)Math.Sin(angle)  // Update probe's Y position
			);
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			timer.Start(); // Start the timer
			timer.Interval = 100; // Set timer interval
		}

		private void radioButton0_CheckedChanged(object sender, EventArgs e)
		{
			timer.Stop(); // Stop the timer
		}
	}
}
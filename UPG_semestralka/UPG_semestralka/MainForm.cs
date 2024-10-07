using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace UPG_semestralka
{
	public partial class MainForm : Form
	{
		private int scenario;
		private const double k = 8.99e9; // Coulombova konstanta
		private List<(PointF position, int charge)> charges;
		private PointF probePosition;

		// Hranice "reálného světa" v metrech
		private double x_min, x_max, y_min, y_max;
		private double world_width, world_height;

		private double time = 0;
		private const double angularVelocity = Math.PI / 6;

		public MainForm(int scenario)
		{
			InitializeComponent();
			this.MinimumSize = new Size(800, 800);
			this.scenario = scenario;
			InitializeWorld();
			UpdateTitle();

			timer.Interval = 200;
			timer.Tick += timer_Tick;
			timer.Start();
		}

		private void InitializeWorld()
		{
			charges = new List<(PointF position, int charge)>();
			switch (scenario)
			{
				case 0:
					charges.Add((new PointF(0, 0), 1));
					break;
				case 1:
					charges.Add((new PointF(-1, 0), 1));
					charges.Add((new PointF(1, 0), 1));
					break;
				case 2:
					charges.Add((new PointF(-1, 0), -1));
					charges.Add((new PointF(1, 0), 2));
					break;
				case 3:
					charges.Add((new PointF(-1, -1), 1));
					charges.Add((new PointF(1, -1), 2));
					charges.Add((new PointF(1, 1), -3));
					charges.Add((new PointF(-1, 1), -4));
					break;
			}

			probePosition = new PointF(0, 1);
			UpdateProbePosition();

			// Nastavení hranic světa
			var positions = charges.Select(c => c.position).ToList();
			positions.Add(probePosition);
			x_min = - 2;
			x_max = + 2;
			y_min = - 2;
			y_max = + 2;
			world_width = x_max - x_min;
			world_height = y_max - y_min;
		}

		private void UpdateTitle()
		{
			this.Text = "Vizualizace elektrostatického pole, scénář: " + Convert.ToString(scenario);
		}

		private void drawingPanel_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;

			// Výpočet měřítka
			double scale_x = drawingPanel.Width / world_width;
			double scale_y = drawingPanel.Height / world_height;
			double scale = Math.Min(scale_x, scale_y);

			// Kreslení mřížky
			DrawGrid(g, scale);

			// Kreslení nábojů
			foreach (var charge in charges)
			{
				DrawCharge(g, charge.position, charge.charge, scale);
			}

			// Výpočet a vykreslení sondy a intenzity pole
			Vector2D forceVector = CalculateForceOnProbe(probePosition, charges);
			DrawProbe(g, probePosition, forceVector, scale);
		}

		private void DrawGrid(Graphics g, double scale)
		{
			Pen gridPen = new Pen(Color.LightGray, 1);
			float gridSpacing = (float)(0.125 * scale); // 0.25 rozestup mezi čarami

			for (float x = 0; x <= drawingPanel.Width; x += gridSpacing)
			{
				g.DrawLine(gridPen, x, 0, x, drawingPanel.Height);
			}

			for (float y = 0; y <= drawingPanel.Height; y += gridSpacing)
			{
				g.DrawLine(gridPen, 0, y, drawingPanel.Width, y);
			}

			// Zvýraznění os
			Pen axisPen = new Pen(Color.Black, 2);
			float originX = (float)(-x_min * scale);
			float originY = (float)((y_max) * scale);
			g.DrawLine(axisPen, originX, 0, originX, drawingPanel.Height);
			g.DrawLine(axisPen, 0, originY, drawingPanel.Width, originY);
		}

		private void DrawCharge(Graphics g, PointF position, int charge, double scale)
		{
			float x = (float)((position.X - x_min) * scale);
			float y = (float)((y_max - position.Y) * scale);
			float size = (float)(Math.Abs(charge) * 0.25 * scale); // 0.2 metru na jednotku náboje

			Color chargeColor = charge > 0 ? Color.Red : Color.Blue;
			g.FillEllipse(new SolidBrush(chargeColor), x - size / 2, y - size / 2, size, size);
			g.DrawEllipse(new Pen(Color.Black, 2), x - size / 2, y - size / 2, size, size);

			string chargeText = $"{charge} C";
			Font chargeFont = new Font("Arial", (float)(10 * scale / 100), FontStyle.Bold);
			SizeF textSize = g.MeasureString(chargeText, chargeFont);
			g.DrawString(chargeText, chargeFont, Brushes.White, x - textSize.Width / 2, y - textSize.Height / 2);
		}

		private void DrawProbe(Graphics g, PointF position, Vector2D forceVector, double scale)
		{
			float x = (float)((position.X - x_min) * scale);
			float y = (float)((y_max - position.Y) * scale);
			float probeSize = (float)(0.1 * scale); // 0.1 metru

			g.FillEllipse(Brushes.Green, x - probeSize / 2, y - probeSize / 2, probeSize, probeSize);

			// Kreslení vektoru síly
			float arrowLength = (float)(0.5 * scale); // Škálování síly pro vizualizaci
			float angleRad = (float)Math.Atan2(-forceVector.Y, forceVector.X);
			PointF arrowEnd = new PointF(
				x + arrowLength * (float)Math.Cos(angleRad),
				y + arrowLength * (float)Math.Sin(angleRad)
			);

			Pen arrowPen = new Pen(Color.Black, (float)(2 * scale / 100));
			g.DrawLine(arrowPen, x, y, arrowEnd.X, arrowEnd.Y);
			DrawArrowHead(g, arrowPen, new PointF(x, y), arrowEnd, (float)(0.1 * scale));

			string forceText = $"{forceVector.Magnitude():0.00} N/C";
			Font forceFont = new Font("Arial", (float)(12 * scale / 100), FontStyle.Regular);
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

			foreach (var charge in charges) //iterace pres vsechny naboje
			{
				double q = charge.charge;
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
			this.drawingPanel.Invalidate();
		}

		// Tlačítka pro změnu scénáře
		private void btnScenario0_Click(object sender, EventArgs e)
		{
			scenario = 0;
			InitializeWorld();
			this.drawingPanel.Invalidate();
		}

		private void btnScenario1_Click(object sender, EventArgs e)
		{
			scenario = 1;
			InitializeWorld();
			this.drawingPanel.Invalidate();
		}

		private void btnScenario2_Click(object sender, EventArgs e)
		{
			scenario = 2;
			InitializeWorld();
			this.drawingPanel.Invalidate();
		}

		private void btnScenario3_Click(object sender, EventArgs e)
		{
			scenario = 3;
			InitializeWorld();
			this.drawingPanel.Invalidate();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			time += timer.Interval / 1000.0; // Převod na sekundy
			UpdateProbePosition();
			this.drawingPanel.Invalidate();
		}
		private void UpdateProbePosition()
		{
			double angle = angularVelocity * time;
			probePosition = new PointF(
				(float)Math.Cos(angle),
				(float)Math.Sin(angle)
			);
		}
	}
}
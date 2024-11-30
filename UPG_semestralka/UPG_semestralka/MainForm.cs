using System.Drawing.Drawing2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Microsoft.VisualBasic.Devices;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Reflection;

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

		private int? gridSpacingX;
		private int? gridSpacingY;

		private double time = 0;
		private const double ANGULAR_VELOCITY = Math.PI / 6; // π/6 rad/s
		private double velocityMultiplier = 1.0;
		private const double FIXED_TIMESTEP = 1.0 / 60.0; // 60 FPS

		// For the dragging of the probes
		private bool isDragging = false;
		private int draggedChargeIndex = -1;
		private PointF dragOffset;

		// For the secondary probe and its graph
		private bool hasSecondaryProbe = false;
		private PointF secondaryProbePosition;
		private List<(double time, double intensity)> secondaryProbeData = new List<(double time, double intensity)>();
		private double secondaryProbeStartTime = 0;
		private GraphForm graphForm;
		private System.Windows.Forms.Timer graphUpdateTimer;
		private const int GRAPH_UPDATE_INTERVAL = 50; // Update every 50ms

		public MainForm(int scenario, int? gridSpacingX, int? gridSpacingY)
		{
			InitializeComponent();
			this.MinimumSize = new Size(800, 600);
			this.scenario = scenario;
			this.gridSpacingX = gridSpacingX;
			this.gridSpacingY = gridSpacingY;

			typeof(Panel).InvokeMember("DoubleBuffered",
				BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
				null, drawingPanel, new object[] { true });

			InitializeWorld();

			timer.Interval = (int)(FIXED_TIMESTEP * 1000);
			timer.Tick += timer_Tick;
			timer.Start();

			drawingPanel.MouseDown += drawingPanel_MouseDown;
			drawingPanel.MouseMove += drawingPanel_MouseMove;
			drawingPanel.MouseUp += drawingPanel_MouseUp;
			drawingPanel.MouseWheel += drawingPanel_MouseWheel;

			graphUpdateTimer = new System.Windows.Forms.Timer();
			graphUpdateTimer.Interval = GRAPH_UPDATE_INTERVAL;
			graphUpdateTimer.Tick += GraphUpdateTimer_Tick;
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

			DrawIntensityMap(g, scale);
			DrawGrid(g, scale);
			DrawStaticProbes(g, scale);
			DrawLegend(g, scale);

			g.TranslateTransform(offsetX, offsetY);

			foreach (var charge in charges)
			{
				DrawCharge(g, charge.position, charge.charge(time), scale);
			}

			Vector2D forceVector = CalculateForceOnProbe(probePosition, charges);
			DrawProbe(g, probePosition, forceVector, scale);

			if (hasSecondaryProbe)
			{
				Vector2D secondaryForceVector = CalculateForceOnProbe(secondaryProbePosition, charges);
				DrawSecondaryProbe(g, secondaryProbePosition, secondaryForceVector, scale);
			}

			g.ResetTransform();
		}

		private void DrawGrid(Graphics g, double scale)
		{
			int width = this.ClientSize.Width;
			int height = this.ClientSize.Height;

			int centerX = width / 2;
			int centerY = height / 2;

			Pen axisPen = new Pen(Color.Black, 4);
			Pen gridPen = new Pen(Color.Black, 1);

			// Draw axes
			g.DrawLine(axisPen, centerX, 0, centerX, height);
			g.DrawLine(axisPen, 0, centerY, width, centerY);

			// Draw vertical grid lines
			if (gridSpacingX.HasValue)
			{
				int spacingX = gridSpacingX.Value;
				for (int x = centerX; x < width; x += spacingX)
				{
					g.DrawLine(gridPen, x, 0, x, height);
				}

				for (int x = centerX; x > 0; x -= spacingX)
				{
					g.DrawLine(gridPen, x, 0, x, height);
				}
			}

			// Draw horizontal grid lines
			if (gridSpacingY.HasValue)
			{
				int spacingY = gridSpacingY.Value;
				for (int y = centerY; y < height; y += spacingY)
				{
					g.DrawLine(gridPen, 0, y, width, y);
				}

				for (int y = centerY; y > 0; y -= spacingY)
				{
					g.DrawLine(gridPen, 0, y, width, y);
				}
			}
		}


		private void DrawCharge(Graphics g, PointF position, double charge, double scale)
		{
			float x = (float)((position.X - x_min) * scale);
			float y = (float)((y_max - position.Y) * scale);

			float area = (float)(Math.Abs(charge) * 0.2 * scale * scale);
			float radius = (float)Math.Sqrt(area / Math.PI);
			float size = radius * 2; // Charges size is equal to its values

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

			string chargeText = scenario == 4 ? $"{charge:F2} C" : $"{charge:F0} C"; // Specially for the 4th scenario it's rounded on 2 decimal places
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

		private void DrawStaticProbe(Graphics g, PointF position, Vector2D forceVector, double scale)
		{
			float x = (float)((position.X - x_min) * scale);
			float y = (float)((y_max - position.Y) * scale);
			float probeSize = (float)(0.05 * scale);

			g.FillEllipse(Brushes.DarkSlateGray, x - probeSize / 2, y - probeSize / 2, probeSize, probeSize);

			float arrowLength = (float)(0.25 * scale);
			float angleRad = (float)Math.Atan2(-forceVector.Y, forceVector.X);
			PointF arrowEnd = new PointF(
				x + arrowLength * (float)Math.Cos(angleRad),
				y + arrowLength * (float)Math.Sin(angleRad)
			);

			Pen arrowPen = new Pen(Color.DarkSlateGray, (float)(2 * scale / 100));
			g.DrawLine(arrowPen, x, y, arrowEnd.X, arrowEnd.Y);
			DrawArrowHead(g, arrowPen, new PointF(x, y), arrowEnd, (float)(0.1 * scale));
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

			// Implementation of the Coulomb's law
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
			// Fixed time step is used, can by manipuled by buttons
			if (velocityMultiplier != 0)
			{
				time += FIXED_TIMESTEP * velocityMultiplier;
				UpdateProbePosition();
				this.drawingPanel.Invalidate();
			}
			// Calculations for the second probe and its graph
			if (hasSecondaryProbe)
			{
				Vector2D force = CalculateForceOnProbe(secondaryProbePosition, charges);
				secondaryProbeData.Add((time - secondaryProbeStartTime, force.Magnitude()));
				graphForm?.UpdateData(secondaryProbeData);
			}
		}

		private void UpdateProbePosition()
		{
			double angle = ANGULAR_VELOCITY * time;
			probePosition = new PointF(
				(float)(Math.Cos(angle)), // Radius = 1
				(float)(Math.Sin(angle))
			);
		}

		// Buttons for the speed of time manipulation
		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			velocityMultiplier = 1.0;
			timer.Start();
		}

		private void radioButton0_CheckedChanged(object sender, EventArgs e)
		{
			velocityMultiplier = 0.0;
			timer.Stop();
		}

		private void radioButton05_CheckedChanged(object sender, EventArgs e)
		{
			velocityMultiplier = 0.5;
			timer.Start();
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			velocityMultiplier = 2.0;
			timer.Start();
		}

		private bool isMouseInCharge(Point mousePosition)
		{
			// Convert the mouse position on the screen to local coordinates of the drawing panel
			Point localPosition = drawingPanel.PointToClient(mousePosition);

			// Scale the dimensions to obtain the coordinates in "real world"
			double scale_x = drawingPanel.Width / world_width;
			double scale_y = drawingPanel.Height / world_height;
			double scale = Math.Min(scale_x, scale_y);

			float offsetX = (float)((drawingPanel.Width - (world_width * scale)) / 2);
			float offsetY = (float)((drawingPanel.Height - (world_height * scale)) / 2);

			// Offset the mouse position to match the origin of the "real world"
			float worldX = (localPosition.X - offsetX) / (float)scale + (float)x_min;
			float worldY = (float)y_max - (localPosition.Y - offsetY) / (float)scale;

			// Check if the mouse point is within any charge
			foreach (var charge in charges)
			{
				// Convert the charge position to screen coordinates
				float x = (float)((charge.position.X - x_min) * scale) + offsetX;
				float y = (float)((y_max - charge.position.Y) * scale) + offsetY;

				// Calculate the area of the charge on screen based on its magnitude
				float area = (float)(Math.Abs(charge.charge(time)) * 0.2 * scale * scale);
				float radius = (float)Math.Sqrt(area / Math.PI);

				// Calculate the distance between the mouse point and the center of the charge
				float distanceToCharge = (float)Math.Sqrt(Math.Pow(worldX - charge.position.X, 2) + Math.Pow(worldY - charge.position.Y, 2));

				// If the mouse is within the radius of the charge, return true
				if (distanceToCharge <= radius / scale)
				{
					return true;
				}
			}

			// If not within any charge, return false
			return false;
		}

		private void drawingPanel_MouseWheel(object sender, MouseEventArgs e)
		{
			double scaleX = drawingPanel.Width / world_width;
			double scaleY = drawingPanel.Height / world_height;
			double scale = Math.Min(scaleX, scaleY);
			float offsetX = (float)((drawingPanel.Width - (world_width * scale)) / 2);
			float offsetY = (float)((drawingPanel.Height - (world_height * scale)) / 2);
			PointF mousePosition = new PointF(
				(e.X - offsetX) / (float)scale + (float)x_min,
				(float)y_max - (e.Y - offsetY) / (float)scale
			);

			// Find the nearest charge within a small radius and increment its charge
			foreach (var charge in charges)
			{
				float chargeX = (float)((charge.position.X - x_min) * scale + offsetX);
				float chargeY = (float)((y_max - charge.position.Y) * scale + offsetY);
				float radius = (float)Math.Sqrt(Math.Abs(charge.charge(time)) * 0.2 * scale * scale / Math.PI);

				// Check if mouse is over charge circle
				if (Math.Pow(e.X - chargeX, 2) + Math.Pow(e.Y - chargeY, 2) <= Math.Pow(radius, 2))
				{
					int index = charges.IndexOf(charge);
					int increment = 0;

					// Determine increment based on scroll direction
					// Positive Delta means scrolling up, negative means scrolling down
					if (e.Delta > 0) // Scrolling up (incrementing)
					{
						if (charge.charge(time) == -1) increment = 2;
						else increment = 1;
					}
					else // Scrolling down (decrementing)
					{
						if (charge.charge(time) == 1) increment = -2;
						else increment = -1;
					}

					charges[index] = (charge.position, t => charge.charge(t) + increment);
					drawingPanel.Invalidate(); // Redraw to reflect change
					break;
				}
			}
		}
		private void DrawIntensityMap(Graphics g, double scale)
		{
			int width = drawingPanel.Width;
			int height = drawingPanel.Height;
			float offsetX = (float)((width - (world_width * scale)) / 2);
			float offsetY = (float)((height - (world_height * scale)) / 2);

			using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
			{
				BitmapData bmpData = bitmap.LockBits(
					new Rectangle(0, 0, width, height),
					ImageLockMode.WriteOnly,
					PixelFormat.Format32bppArgb
				);

				try
				{
					int stride = bmpData.Stride;
					byte[] pixelData = new byte[stride * height];

					const int blockSize = 20; // Higher value for better performance, lower for quality

					// Process pixels in parallel
					Parallel.For(0, (height + blockSize - 1) / blockSize, j =>
					{
						int yStart = j * blockSize;
						int yEnd = Math.Min(yStart + blockSize, height);

						for (int i = 0; i < (width + blockSize - 1) / blockSize; i++)
						{
							int xStart = i * blockSize;
							int xEnd = Math.Min(xStart + blockSize, width);

							// Convert screen coordinates to world coordinates
							float worldX = ((xStart - offsetX) / (float)scale) + (float)x_min;
							float worldY = (float)y_max - ((yStart - offsetY) / (float)scale);

							// Calculate field vector in world coordinates
							Vector2D fieldVector = CalculateForceOnProbe(
								new PointF(worldX, worldY),
								charges
							);

							Color color = IntensityToColor(fieldVector.Magnitude());

							// Fill block with calculated color
							for (int y = yStart; y < yEnd; y++)
							{
								int rowOffset = y * stride;
								for (int x = xStart; x < xEnd; x++)
								{
									int index = rowOffset + x * 4;
									if (index < pixelData.Length - 4)
									{
										pixelData[index] = color.B;
										pixelData[index + 1] = color.G;
										pixelData[index + 2] = color.R;
										pixelData[index + 3] = 255; // Transparency of the color, the aplha channel
									}
								}
							}
						}
					});

					System.Runtime.InteropServices.Marshal.Copy(pixelData, 0, bmpData.Scan0, pixelData.Length);
				}
				finally
				{
					bitmap.UnlockBits(bmpData);
				}

				g.DrawImage(bitmap, 0, 0);
			}
		}
		private Color IntensityToColor(double intensity)
		{
			double normalizedIntensity = intensity > 0 ? Math.Log10(1 + intensity) / Math.Log10(1 + 1e11) : 1;

			normalizedIntensity = Math.Max(0, Math.Min(1, normalizedIntensity));


			double hue = 360 * (1 - normalizedIntensity);
			double saturation = 1;
			double value = 1;

			return HSVToRGB(hue, saturation, value); // Conversion from HSV to RGB
		}
		private Color HSVToRGB(double hue, double saturation, double value)
		{
			int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
			double f = hue / 60 - Math.Floor(hue / 60);

			value *= 255;
			int v = (int)value;
			int p = (int)(value * (1 - saturation));
			int q = (int)(value * (1 - f * saturation));
			int t = (int)(value * (1 - (1 - f) * saturation));

			return hi switch
			{
				0 => Color.FromArgb(v, t, p),
				1 => Color.FromArgb(q, v, p),
				2 => Color.FromArgb(p, v, t),
				3 => Color.FromArgb(p, q, v),
				4 => Color.FromArgb(t, p, v),
				_ => Color.FromArgb(v, p, q),
			};
		}
		private void DrawStaticProbes(Graphics g, double scale)
		{
			if (!gridSpacingX.HasValue || !gridSpacingY.HasValue)
			{
				// Skip drawing if either grid spacing is null
				return;
			}

			int spacingX = gridSpacingX.Value;
			int spacingY = gridSpacingY.Value;

			// Get panel dimensions
			int width = drawingPanel.Width;
			int height = drawingPanel.Height;

			// Calculate center point of the panel
			int centerX = width / 2;
			int centerY = height / 2;

			// Calculate how many grid lines we can fit in each direction from center
			int numLinesX = Math.Min(centerX / spacingX, width / (2 * spacingX));
			int numLinesY = Math.Min(centerY / spacingY, height / (2 * spacingY));

			float offsetX = (float)((width - (world_width * scale)) / 2);
			float offsetY = (float)((height - (world_height * scale)) / 2);

			// Apply transform once before the loop
			g.TranslateTransform(offsetX, offsetY);

			// For each grid intersection
			for (int i = -numLinesX; i <= numLinesX; i++)
			{
				for (int j = -numLinesY; j <= numLinesY; j++)
				{
					// Calculate screen coordinates (without offset since we're using transform)
					int screenX = centerX + (i * spacingX) - (int)offsetX;
					int screenY = centerY + (j * spacingY) - (int)offsetY;

					// Convert to world coordinates
					float worldX = screenX / (float)scale + (float)x_min;
					float worldY = (float)y_max - screenY / (float)scale;

					// Skip if too close to any charge
					bool tooClose = false;
					foreach (var charge in charges)
					{
						double distance = Math.Sqrt(
							Math.Pow(worldX - charge.position.X, 2) +
							Math.Pow(worldY - charge.position.Y, 2)
						);
						if (distance < 0.3) // Minimum distance threshold
						{
							tooClose = true;
							break;
						}
					}

					if (!tooClose)
					{
						PointF probePos = new PointF(worldX, worldY);
						Vector2D forceVector = CalculateForceOnProbe(probePos, charges);

						// Normalize the force vector for visualization
						double magnitude = forceVector.Magnitude();
						if (magnitude > 0)
						{
							double normalizedMagnitude = Math.Log10(1 + magnitude) / Math.Log10(1 + 1e11);
							forceVector = forceVector * (normalizedMagnitude / magnitude);
						}

						DrawStaticProbe(g, probePos, forceVector, scale);
					}
				}
			}

			// Reset transform once after the loop
			g.ResetTransform();
		}
		private void drawingPanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (isDragging) return;

			double scaleX = drawingPanel.Width / world_width;
			double scaleY = drawingPanel.Height / world_height;
			double scale = Math.Min(scaleX, scaleY);

			float offsetX = (float)((drawingPanel.Width - (world_width * scale)) / 2);
			float offsetY = (float)((drawingPanel.Height - (world_height * scale)) / 2);

			// Convert screen coordinates to world coordinates
			PointF worldClick = new PointF(
				(e.X - offsetX) / (float)scale + (float)x_min,
				(float)y_max - (e.Y - offsetY) / (float)scale
			);

			// Find if we clicked on a charge
			for (int i = 0; i < charges.Count; i++)
			{
				var charge = charges[i];
				double distance = Math.Sqrt(
					Math.Pow(worldClick.X - charge.position.X, 2) +
					Math.Pow(worldClick.Y - charge.position.Y, 2)
				);

				// If click is within charge radius
				float chargeRadius = (float)Math.Sqrt(Math.Abs(charge.charge(time)) * 0.2);
				if (distance <= chargeRadius)
				{
					isDragging = true;
					draggedChargeIndex = i;
					dragOffset = new PointF(
						worldClick.X - charge.position.X,
						worldClick.Y - charge.position.Y
					);
					break;
				}
			}
		}
		private void DrawLegend(Graphics g, double scale)
		{
			// Dimensions for a compact, visually appealing legend
			int legendWidth = 40;
			int legendHeight = 100;
			int margin = 20;
			int labelWidth = 70;

			// Positioning legend in the upper-right corner
			int x = drawingPanel.Width - legendWidth - labelWidth;
			int y = margin;

			// Draw background with soft color and padding for a cleaner look
			using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(245, 245, 245)))
			{
				g.FillRectangle(backgroundBrush,
					x - 5, y - 20,  // Slight offset for padding
					legendWidth + labelWidth + 15,  // Extra space for label
					legendHeight + 30);  // Extra bottom padding
			}

			// Create gradient bar for intensity representation
			using (Bitmap gradientBitmap = new Bitmap(legendWidth, legendHeight))
			{
				for (int i = 0; i < legendHeight; i++)
				{
					double normalizedIntensity = 1.0 - ((double)i / legendHeight);
					double intensity = Math.Pow(10, 5 + (normalizedIntensity * 8));
					Color color = IntensityToColor(intensity);

					using (Graphics bmpG = Graphics.FromImage(gradientBitmap))
					using (Pen pen = new Pen(color, 1))
					{
						bmpG.DrawLine(pen, 0, i, legendWidth - 1, i);
					}
				}

				// Draw gradient bitmap onto the main graphics object
				g.DrawImage(gradientBitmap, x, y);

				// Border with subtle color for a refined look
				using (Pen borderPen = new Pen(Color.FromArgb(180, 180, 180), 1))
				{
					g.DrawRectangle(borderPen, x, y, legendWidth, legendHeight);
				}
			}

			// Fonts for title and labels with improved readability
			using (Font labelFont = new Font("Segoe UI", 7))
			using (Font titleFont = new Font("Segoe UI", 8, FontStyle.Bold))
			{
				double[] intensities = {1e13, 1e11, 1e9, 1e7, 1e5};

				for (int i = 0; i < intensities.Length; i++)
				{
					double normalizedY = Math.Log10(intensities[i] / 1e5) / 8.0;
					int labelY = y + (int)(legendHeight * (1 - normalizedY));

					// Shorter, lighter tick marks for a cleaner look
					g.DrawLine(Pens.Gray, x + legendWidth, labelY, x + legendWidth + 4, labelY);

					// Format label in scientific notation
					string label = $"{intensities[i]:E0}";
					using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(60, 60, 60)))
					{
						g.DrawString(label, labelFont, textBrush, x + legendWidth + 6, labelY - 6);
					}
				}

				// Title positioned above the legend with clear color and spacing
				using (SolidBrush titleBrush = new SolidBrush(Color.FromArgb(40, 40, 40)))
				{
					g.DrawString("Field Intensity (N/C)", titleFont, titleBrush, x, y - 18);
				}
			}
		}
		private void drawingPanel_MouseUp(object sender, MouseEventArgs e)
		{
			isDragging = false;
			draggedChargeIndex = -1;
		}
		private void drawingPanel_MouseMove(object sender, MouseEventArgs e)
		{
			drawingPanel.Invalidate();
			if (!isDragging || draggedChargeIndex < 0) return;

			double scaleX = drawingPanel.Width / world_width;
			double scaleY = drawingPanel.Height / world_height;
			double scale = Math.Min(scaleX, scaleY);

			float offsetX = (float)((drawingPanel.Width - (world_width * scale)) / 2);
			float offsetY = (float)((drawingPanel.Height - (world_height * scale)) / 2);

			// Convert screen coordinates to world coordinates
			PointF newWorldPos = new PointF(
				(e.X - offsetX) / (float)scale + (float)x_min,
				(float)y_max - (e.Y - offsetY) / (float)scale
			);

			// Apply the offset from initial click
			newWorldPos = new PointF(
				newWorldPos.X - dragOffset.X,
				newWorldPos.Y - dragOffset.Y
			);

			// Constrain position within world boundaries
			newWorldPos.X = Math.Max((float)x_min, Math.Min((float)x_max, newWorldPos.X));
			newWorldPos.Y = Math.Max((float)y_min, Math.Min((float)y_max, newWorldPos.Y));

			// Update charge position
			var charge = charges[draggedChargeIndex];
			charges[draggedChargeIndex] = (newWorldPos, charge.charge);

			// Trigger redraw
			drawingPanel.Invalidate();
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (isDragging)
			{
				this.Cursor = Cursors.Hand;
			}
			else
			{
				this.Cursor = isMouseInCharge(Control.MousePosition) ?
					Cursors.Hand : Cursors.Default;
			}
		}
		private void drawingPanel_DoubleClick(object sender, EventArgs e)
		{
			if (!hasSecondaryProbe)
			{
				MouseEventArgs me = (MouseEventArgs)e;
				double scale_x = drawingPanel.Width / world_width;
				double scale_y = drawingPanel.Height / world_height;
				double scale = Math.Min(scale_x, scale_y);
				float offsetX = (float)((drawingPanel.Width - (world_width * scale)) / 2);
				float offsetY = (float)((drawingPanel.Height - (world_height * scale)) / 2);

				secondaryProbePosition = new PointF(
					(me.X - offsetX) / (float)scale + (float)x_min,
					(float)y_max - (me.Y - offsetY) / (float)scale
				);

				hasSecondaryProbe = true;
				secondaryProbeStartTime = time;
				secondaryProbeData.Clear();

				// Create and show the graph form if it doesn't exist
				if (graphForm == null || graphForm.IsDisposed)
				{
					graphForm = new GraphForm();
					graphForm.FormClosed += (s, args) =>
					{
						graphUpdateTimer.Stop();
						hasSecondaryProbe = false;
					};
					graphForm.Show();
					graphForm.Location = new Point(this.Location.X + this.Width, this.Location.Y);
				}

				// Start the update timer
				graphUpdateTimer.Start();
			}
		}

		private void GraphUpdateTimer_Tick(object sender, EventArgs e)
		{
			if (hasSecondaryProbe && graphForm != null && !graphForm.IsDisposed)
			{
				Vector2D force = CalculateForceOnProbe(secondaryProbePosition, charges);
				double intensity = Math.Sqrt(force.X * force.X + force.Y * force.Y);
				double currentTime = time - secondaryProbeStartTime;

				secondaryProbeData.Add((currentTime, intensity));

				// Update the graph
				graphForm.UpdateData(new List<(double, double)>(secondaryProbeData));
			}
			else
			{
				graphUpdateTimer.Stop();
			}
		}

		private void DrawSecondaryProbe(Graphics g, PointF position, Vector2D forceVector, double scale)
		{
			try
			{
				float x = (float)((position.X - x_min) * scale);
				float y = (float)((y_max - position.Y) * scale);
				float probeSize = (float)(0.08 * scale);

				// Draw a diamond shape for the secondary probe
				PointF[] diamond = {
					new PointF(x, y - probeSize),
					new PointF(x + probeSize, y),
					new PointF(x, y + probeSize),
					new PointF(x - probeSize, y)
				};

				g.FillPolygon(Brushes.Purple, diamond);

				// Only draw arrow if force vector is not zero
				if (Math.Abs(forceVector.X) > 1e-10 || Math.Abs(forceVector.Y) > 1e-10)
				{
					float arrowLength = (float)(0.3 * scale);
					float angleRad = (float)Math.Atan2(-forceVector.Y, forceVector.X);

					// Normalize the arrow length based on force magnitude
					double forceMagnitude = Math.Sqrt(forceVector.X * forceVector.X + forceVector.Y * forceVector.Y);
					float normalizedLength = (float)(arrowLength * Math.Min(1.0, forceMagnitude / 1e5));

					PointF arrowEnd = new PointF(
						x + normalizedLength * (float)Math.Cos(angleRad),
						y + normalizedLength * (float)Math.Sin(angleRad)
					);

					using (Pen arrowPen = new Pen(Color.Purple, (float)(2 * scale / 100)))
					{
						g.DrawLine(arrowPen, x, y, arrowEnd.X, arrowEnd.Y);
						DrawSecondaryArrowHead(g, arrowPen, new PointF(x, y), arrowEnd, (float)(0.1 * scale));
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error drawing secondary probe: {ex.Message}");
			}
		}

		private void DrawSecondaryArrowHead(Graphics g, Pen pen, PointF start, PointF end, float size)
		{
			float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
			PointF[] arrowHead = new PointF[3];

			arrowHead[0] = end;
			arrowHead[1] = new PointF(
				end.X - size * (float)Math.Cos(angle + Math.PI / 6),
				end.Y - size * (float)Math.Sin(angle + Math.PI / 6));
			arrowHead[2] = new PointF(
				end.X - size * (float)Math.Cos(angle - Math.PI / 6),
				end.Y - size * (float)Math.Sin(angle - Math.PI / 6));

			g.FillPolygon(new SolidBrush(pen.Color), arrowHead);
		}

	}
}
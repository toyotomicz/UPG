using System.Drawing;

namespace Cv03
{
	public partial class MainForm : Form
	{
		//souradnice pozemku v realnych jednotkach
		private readonly double[] obj_x = {33.261905, 73.327375, 37.79762,
			-70.303568, -15.119048, -55.18452, 100};

		private readonly double[] obj_y = {67.27976, 3.023812, 75.595238,
			39.30952, 46.113096, 1.5119, -20};

		//obdelnik v realnem svete v realnych jednotkach (napr. m)
		private double x_min, x_max, y_min, y_max;
		private double world_width, world_height;

		public MainForm()
		{
			InitializeComponent();

			x_min = obj_x.Min();
			x_max = obj_x.Max();
			y_min = obj_y.Min();
			y_max = obj_y.Max();

			world_width = x_max - x_min;
			world_height = y_max - y_min;
		}

		private void drawingPanel_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;

			// mìøítko
			double scale_x = this.drawingPanel.Width / world_width;
			double scale_y = this.drawingPanel.Height / world_height;

			double scale = Math.Min(scale_x, scale_y);

			// vytvoøíme polygon pro zobrazení (ze souøadnic reálného svìta)
			var points = new PointF[obj_x.Length];
			for (int i = 0; i < 5; i++)
			{
				points[i] = new PointF(
					(float)((obj_x[i] - x_min) * scale),
					(float)((obj_y[i] - y_min) * scale));
			}

			// zobrazíme polygon
			var pen = new Pen(Color.Magenta, 3f);
			g.FillPolygon(Brushes.Magenta, points);

			// výpoèet støedu polygonu

			int centroidX = (int)x_min + (int)(x_max - x_min) / 2;
			int centroidY = (int)y_min + (int)(y_max - y_min) / 2;

			int startX = (int)((centroidX - x_min) * scale);
			int startY = (int)((centroidY - y_min) * scale);

			int endX = (int)((100 - x_min) * scale);
			int endY = (int)((-20 - y_min) * scale);

			// vykreslení šipky zaèínající ve støedu polygonu
			DrawArrow2(startX, startY, endX, endY, 40f, g);
		}

		/// <summary>
		/// Zobrazí šipku z bodu A do bodu B
		/// </summary>
		/// <param name="A">poèáteèní bod</param>
		/// <param name="B">koncový bod</param>
		/// <param name="tip_length">délka špièky v px</param>
		/// <param name="g">grafický kontext</param>
		private void DrawArrow(PointF A, PointF B,
			float tip_length, Graphics g)
		{
			DrawArrow(A.X, A.Y, B.X, B.Y, tip_length, g);
		}


		/// <summary>
		/// Zobrazí šipku z bodu A = (x1,y1) do bodu B = (x2,y2) 
		/// </summary>
		/// <param name="x1">A(x)</param>
		/// <param name="y1">A(y)</param>
		/// <param name="x2">B(x)</param>
		/// <param name="y2">B(y)</param>
		/// <param name="tip_length">délka špièky v px</param>
		/// <param name="g">grafický kontext</param>
		private void DrawArrow(float x1, float y1,
			float x2, float y2, float tip_length,
			Graphics g)
		{
			double u_x = x2 - x1;
			double u_y = y2 - y1;
			double u_len1 = 1 / Math.Sqrt(u_x * u_x + u_y * u_y);
			u_x *= u_len1;
			u_y *= u_len1;
			//u ma delku 1

			var pen = new Pen(Color.Black, 4f);
			pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Miter;
			pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
			pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

			g.DrawLine(pen, x1, y1, x2, y2);

			//TODO:
			float c_x = (float)(x2 - u_x * tip_length);
			float c_y = (float)(y2 - u_y * tip_length);

			float d = 0.25f * tip_length;

			float e_x = (float)(c_x + u_y * d);
			float e_y = (float)(c_y - u_x * d);

			float d_x = (float)(c_x - u_y * d);
			float d_y = (float)(c_y + u_x * d);

			var pen2 = new Pen(Color.Blue, 8f);
			//g.DrawLine(pen2, e_x, e_y, x2, y2);
			//g.DrawLine(pen2, d_x, d_y, x2, y2);

			g.DrawLines(pen, new PointF[]
			{
				new PointF(d_x, d_y),
				new PointF(x2, y2),
				new PointF(e_x, e_y)
			});

		}

		private void DrawArrow2(float x1, float y1,
	float x2, float y2, float tip_length,
	Graphics g)
		{
			// Výpoèet jednotkového vektoru smìru šipky
			double u_x = x2 - x1;
			double u_y = y2 - y1;
			double u_len1 = 1 / Math.Sqrt(u_x * u_x + u_y * u_y);
			u_x *= u_len1;
			u_y *= u_len1;
			// Jednotkový vektor má délku 1

			var pen = new Pen(Color.Black, 8f);
			var pen2 = new Pen(Color.Red, 8f);
			pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Miter;
			pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
			pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

			// Kreslení tìla šipky
			g.DrawLine(pen, x1, y1, x2, y2);

			// Výpoèet bodu základny hrotu šipky
			float c_x = (float)(x2 - u_x * tip_length);
			float c_y = (float)(y2 - u_y * tip_length);

			// Zvýšení šíøky hrotu pro tupìjší vzhled
			float d = 0.5f * tip_length; // Zvýšení hodnoty d na 0.5 z pùvodních 0.25

			// Výpoèet bodù pro hrot šipky
			float e_x = (float)(c_x + u_y * d);
			float e_y = (float)(c_y - u_x * d);

			float d_x = (float)(c_x - u_y * d);
			float d_y = (float)(c_y + u_x * d);

			// Vykreslení hrotu šipky
			g.DrawLines(pen, new PointF[]
			{
		new PointF(d_x, d_y),
		new PointF(x2, y2),
		new PointF(e_x, e_y)
			});

			// Vykreslení krize
			g.DrawLines(pen, new PointF[]
			{
		new PointF(d_x, d_y),
		new PointF(x2, y2),
		new PointF(e_x, e_y)
			});
		}


		private void drawingPanel_Resize(object sender, EventArgs e)
		{
			this.drawingPanel.Invalidate();
		}
	}
}

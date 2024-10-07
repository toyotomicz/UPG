using System.Drawing.Drawing2D;

namespace Cv04_skola
{
	public partial class Form1 : Form
	{
		private const float velikostX = 600f;
		private const float velikostY = 400f;
		public Form1()
		{
			InitializeComponent();
			this.ClientSize = new System.Drawing.Size(800, 800);
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			g.TranslateTransform(this.panel1.Width / 10, panel1.Width / 10);

			DrawPacman(panel1.Width / 10, panel1.Width / 10, g);
		}

		private void DrawPacman(float X, float Y, Graphics g)
		{
			var obdelnik = new GraphicsPath();
			obdelnik.AddRectangle(new RectangleF(0f, 0f, 600f, 400f));
			obdelnik.CloseFigure();

			var dilo = new Region(obdelnik);

			// Vytvoøíme LinearGradientBrush s 5 opakováními
			var brushL = new LinearGradientBrush(
				new PointF(0, 0), new PointF(0, 400),
				Color.Black, Color.Black);

			// Definujeme ColorBlend pro pøechod èerná-šedá-èerná
			var blend = new ColorBlend();
			blend.Colors = new Color[]
			{
				Color.Black, Color.Gray, Color.Black,
				Color.Black, Color.Gray, Color.Black,
				Color.Black, Color.Gray, Color.Black,
				Color.Black, Color.Gray, Color.Black,
				Color.Black, Color.Gray, Color.Black
			};
			blend.Positions = new float[]
			{
				0f, 0.1f, 0.2f,
				0.2f, 0.3f, 0.4f,
				0.4f, 0.5f, 0.6f,
				0.6f, 0.7f, 0.8f,
				0.8f, 0.9f, 1f
			};

			brushL.InterpolationColors = blend;



			g.FillRegion(brushL, dilo);

			string s = "TEXT";
			Font font = new Font("Arial", 120);
			SizeF textSize = g.MeasureString(s, font);

			float centerX = (600f / 2) - (textSize.Width / 2);
			float centerY = (400f / 2) - (textSize.Height / 2);

			g.DrawString(s, font, Brushes.White, centerX, centerY);

			float vyskaObdelniku = 400f;
			float prumerKruhu = vyskaObdelniku / 5; 
			//bile
			for (int i = 0; i < 5; i++)
			{
				float yPozice = i * prumerKruhu; 
				g.FillEllipse(Brushes.Gray, -40, yPozice, prumerKruhu, prumerKruhu); 
			}
			//cerne
			for (int i = 0; i < 5; i++)
			{
				float yPozice = i * prumerKruhu;
				g.FillEllipse(Brushes.Black, 600 - 40, yPozice, prumerKruhu, prumerKruhu);
			}
			//okraje
			g.FillRectangle(Brushes.White, -120, 0, 120f, vyskaObdelniku);
			g.FillRectangle(Brushes.White, 600, 0, 120f, vyskaObdelniku);
		}
	}
}

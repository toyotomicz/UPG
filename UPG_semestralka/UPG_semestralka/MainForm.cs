namespace UPG_semestralka
{
	public partial class MainForm : Form
	{
		private int scenario;

		public MainForm()
		{
			InitializeComponent();
			this.MinimumSize = new Size(800, 600);
			this.Text = "Vizualizace elektrostatického pole, scénáø: " + Convert.ToString(scenario);
		}

		private void drawingPanel_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			int cx = Width / 2; // stred platna
			int cy = Height / 2;

			// Kreslí møížku
			DrawGrid(g, cx, cy);

			// Pøíklad bodového náboje
			int charge1 = -1;
			int charge2 = 2;

			// Kreslí náboje na odpovídajících pozicích a zarovná je na stejné úrovni na ose Y
			DrawCharge(g, new Point(cx / 2, cy), charge1, false); // záporný náboj
			DrawCharge(g, new Point(3 * cx / 2, cy), charge2, true); // kladný náboj

			
		}

		private void DrawGrid(Graphics g, int cx, int cy)
		{
			Pen gridPen = new Pen(Color.LightGray, 1);

			// Nastavení vzdálenosti mezi møížkami (napø. každých 50 pixelù)
			int gridSpacing = 50;

			// Kreslení svislých èar
			for (int x = 0; x < Width; x += gridSpacing)
			{
				g.DrawLine(gridPen, x, 0, x, Height);
			}

			// Kreslení vodorovných èar
			for (int y = 0; y < Height; y += gridSpacing)
			{
				g.DrawLine(gridPen, 0, y, Width, y);
			}

			// Zvýraznìní støedového bodu
			g.DrawLine(new Pen(Color.Black, 2), cx, 0, cx, Height); // støedová vertikální èára
			g.DrawLine(new Pen(Color.Black, 2), 0, cy, Width, cy); // støedová horizontální èára
		}

		private void DrawCharge(Graphics g, Point position, int charge, bool isPositive)
		{
			int size = Math.Abs(charge) * 100; // Velikost náboje úmìrná jeho hodnotì
			Color chargeColor = isPositive ? Color.Red : Color.Blue;

			// Kreslení náboje (kruhu)
			g.FillEllipse(new SolidBrush(chargeColor), position.X - size / 2, position.Y - size / 2, size, size);

			// Kreslení textu náboje (zarovnaného na støed kruhu)
			string chargeText = $"{charge} C";
			Font chargeFont = new Font("Arial", 20);

			// Získání velikosti textu, aby mohl být zarovnán doprostøed
			SizeF textSize = g.MeasureString(chargeText, chargeFont);

			// Vykreslení textu pøesnì do støedu náboje
			g.DrawString(chargeText, chargeFont, Brushes.Black, position.X - textSize.Width / 2, position.Y - textSize.Height / 2);
		}

		private void drawingPanel_Resize(object sender, EventArgs e)
		{
			this.drawingPanel.Invalidate();
		}
	}
}

namespace UPG_semestralka
{
	public partial class MainForm : Form
	{
		private int scenario;

		public MainForm()
		{
			InitializeComponent();
			this.MinimumSize = new Size(800, 600);
			this.Text = "Vizualizace elektrostatick�ho pole, sc�n��: " + Convert.ToString(scenario);
		}

		private void drawingPanel_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			int cx = Width / 2; // stred platna
			int cy = Height / 2;

			// Kresl� m��ku
			DrawGrid(g, cx, cy);

			// P��klad bodov�ho n�boje
			int charge1 = -1;
			int charge2 = 2;

			// Kresl� n�boje na odpov�daj�c�ch pozic�ch a zarovn� je na stejn� �rovni na ose Y
			DrawCharge(g, new Point(cx / 2, cy), charge1, false); // z�porn� n�boj
			DrawCharge(g, new Point(3 * cx / 2, cy), charge2, true); // kladn� n�boj

			
		}

		private void DrawGrid(Graphics g, int cx, int cy)
		{
			Pen gridPen = new Pen(Color.LightGray, 1);

			// Nastaven� vzd�lenosti mezi m��kami (nap�. ka�d�ch 50 pixel�)
			int gridSpacing = 50;

			// Kreslen� svisl�ch �ar
			for (int x = 0; x < Width; x += gridSpacing)
			{
				g.DrawLine(gridPen, x, 0, x, Height);
			}

			// Kreslen� vodorovn�ch �ar
			for (int y = 0; y < Height; y += gridSpacing)
			{
				g.DrawLine(gridPen, 0, y, Width, y);
			}

			// Zv�razn�n� st�edov�ho bodu
			g.DrawLine(new Pen(Color.Black, 2), cx, 0, cx, Height); // st�edov� vertik�ln� ��ra
			g.DrawLine(new Pen(Color.Black, 2), 0, cy, Width, cy); // st�edov� horizont�ln� ��ra
		}

		private void DrawCharge(Graphics g, Point position, int charge, bool isPositive)
		{
			int size = Math.Abs(charge) * 100; // Velikost n�boje �m�rn� jeho hodnot�
			Color chargeColor = isPositive ? Color.Red : Color.Blue;

			// Kreslen� n�boje (kruhu)
			g.FillEllipse(new SolidBrush(chargeColor), position.X - size / 2, position.Y - size / 2, size, size);

			// Kreslen� textu n�boje (zarovnan�ho na st�ed kruhu)
			string chargeText = $"{charge} C";
			Font chargeFont = new Font("Arial", 20);

			// Z�sk�n� velikosti textu, aby mohl b�t zarovn�n doprost�ed
			SizeF textSize = g.MeasureString(chargeText, chargeFont);

			// Vykreslen� textu p�esn� do st�edu n�boje
			g.DrawString(chargeText, chargeFont, Brushes.Black, position.X - textSize.Width / 2, position.Y - textSize.Height / 2);
		}

		private void drawingPanel_Resize(object sender, EventArgs e)
		{
			this.drawingPanel.Invalidate();
		}
	}
}

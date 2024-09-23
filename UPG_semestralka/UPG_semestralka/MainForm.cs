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
			Text = "Vizualizace elektrostatick�ho pole, sc�n��: " + Convert.ToString(scenario); //zmena nazvu okna

			List<int> charges = new List<int>();

			switch (scenario)
			{
				case 0:
					charges.Add(1); //prida naboj o velikosti 1

					// Kresl� n�boje na odpov�daj�c�ch pozic�ch a zarovn� je na stejn� �rovni na ose Y
					DrawCharge(g, new Point(cx, cy), charges[0]); //na pozici 0,0 
					break;
				case 1:
					charges.Add(1);
					charges.Add(1);

					DrawCharge(g, new Point(cx / 2, cy), charges[0]); //na pozici -1,0 
					DrawCharge(g, new Point(3 * cx / 2, cy), charges[1]); //na pozici 1,0 
					break;
				case 2:
					charges.Add(-1);
					charges.Add(2);

					
					DrawCharge(g, new Point(cx / 2, cy), charges[0]); //na pozici -1,0 
					DrawCharge(g, new Point(3 * cx / 2, cy), charges[1]); //na pozici 1,0 
					break;
				case 3:
					charges.Add(1);
					charges.Add(2);
					charges.Add(-3);
					charges.Add(-4);

					DrawCharge(g, new Point(cx / 2, cy / 2), charges[0]); //na pozici -1,-1
					DrawCharge(g, new Point(3 * cx / 2, cy / 2), charges[1]); //na pozici 1,-1
					DrawCharge(g, new Point(3 * cx / 2, 3 * cy / 2), charges[2]); //na pozici 1,1
					DrawCharge(g, new Point(cx / 2, 3 * cy / 2), charges[1]); //na pozici -1,1
					break;
			}
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

		private void DrawCharge(Graphics g, Point position, int charge)
		{
			int size = Math.Abs(charge) * 100; // Velikost n�boje �m�rn� jeho hodnot�
			bool isPositive; //znamenko naboje
			if(charge > 0)
			{
				isPositive = true; //naboj je pozitivni
			}
			else
			{
				isPositive = false; //naboj je negativni
			}
			Color chargeColor = isPositive ? Color.Red : Color.Blue; //pozitivni je cervena a negativni modra

			// Kreslen� samotneho n�boje (kruhu)
			g.FillEllipse(new SolidBrush(chargeColor), position.X - size / 2, position.Y - size / 2, size, size);

			// Kreslen� textu n�boje (zarovnan�ho na st�ed kruhu)
			string chargeText = $"{charge} C";
			Font chargeFont = new Font("Arial", 20, FontStyle.Bold);

			// Z�sk�n� velikosti textu, aby mohl b�t zarovn�n doprost�ed
			SizeF textSize = g.MeasureString(chargeText, chargeFont);

			// Vykreslen� textu p�esn� do st�edu n�boje
			g.DrawString(chargeText, chargeFont, Brushes.Black, position.X - textSize.Width / 2, position.Y - textSize.Height / 2);
		}

		private void drawingPanel_Resize(object sender, EventArgs e)
		{
			this.drawingPanel.Invalidate();
		}

		private void btnScenario0_Click(object sender, EventArgs e)
		{
			scenario = 0;
			this.drawingPanel.Invalidate();
		}

		private void btnScenario1_Click(object sender, EventArgs e)
		{
			scenario = 1;
			this.drawingPanel.Invalidate();
		}

		private void btnScenario2_Click(object sender, EventArgs e)
		{
			scenario = 2;
			this.drawingPanel.Invalidate();
		}

		private void btnScenario3_Click(object sender, EventArgs e)
		{
			scenario = 3;
			this.drawingPanel.Invalidate();
		}
	}
}

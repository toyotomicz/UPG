namespace UPG_semestralka
{
	public partial class MainForm : Form
	{
		private int scenario;


		public MainForm()
		{
			InitializeComponent();
			this.MinimumSize = new Size(800, 600);
		}
		private void drawingPanel_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.Clear(Color.WhiteSmoke);

			// Pøíklad bodového náboje
			DrawCharge(g, new Point(200, 200), 5, true);  // Kladný náboj
			DrawCharge(g, new Point(400, 300), -3, false); // Záporný náboj

			// Pøíklad vektoru intenzity pole
			DrawFieldVector(g, new Point(300, 250), new Point(350, 270), 10);
		}
		private void DrawCharge(Graphics g, Point position, int charge, bool isPositive)
		{
			int size = Math.Abs(charge) * 10; // Velikost náboje úmìrná jeho hodnotì
			Color chargeColor = isPositive ? Color.Red : Color.Blue;
			g.FillEllipse(new SolidBrush(chargeColor), position.X - size / 2, position.Y - size / 2, size, size);
			g.DrawString($"{charge} C", new Font("Arial", 10), Brushes.Black, position.X + size / 2, position.Y - size / 2);
		}

		private void DrawFieldVector(Graphics g, Point start, Point end, double magnitude)
		{
			Pen arrowPen = new Pen(Color.Green, 2);
			arrowPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
			g.DrawLine(arrowPen, start, end);
			g.DrawString($"E: {magnitude}", new Font("Arial", 10), Brushes.Black, end.X + 5, end.Y);
		}

		private void drawingPanel_Resize(object sender, EventArgs e)
		{
			this.drawingPanel.Invalidate();
		}

		//public static void Main(string[] args)
		//{
		//	int scenario = args.Length > 0 ? int.Parse(args[0]) : 0;
		//	Application.Run(new MainForm(scenario));
		//}
	}
}

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

			int cx = Width / 2; //stred platna
			int cy = Height / 2;

			// Pøíklad bodového náboje
			DrawCharge(g, new Point(cx / 2, cy), -1, true);  // záporný náboj 0,-1
			DrawCharge(g, new Point(3 * cx / 2, cy), 2, false); // kladný náboj 0,1
		}
		private void DrawCharge(Graphics g, Point position, int charge, bool isPositive)
		{
			int size = Math.Abs(charge) * 75; // Velikost náboje úmìrná jeho hodnotì
			Color chargeColor = isPositive ? Color.Blue : Color.Red;
			g.FillEllipse(new SolidBrush(chargeColor), position.X - size / 2, position.Y - size / 2, size, size);
			g.DrawString($"{charge} C", new Font("Arial", 10), Brushes.Black, position.X + size / 2, position.Y - size / 2);
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

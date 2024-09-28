namespace Cv2._1
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void drawingPanel_Paint(object sender, PaintEventArgs e)
		{
			int cx = Width / 2; //stred platna
			int cy = Height / 2;

			const int r = 50;   //polomer slunicka

			e.Graphics.FillEllipse(Brushes.Yellow, cx - r, cy - r, 2 * r, 2 * r); //slunce
			e.Graphics.DrawEllipse(Pens.Black, cx - r, cy - r, 2 * r, 2 * r); //obrys slunce

			const int rOko = 10;

			e.Graphics.FillEllipse(Brushes.Blue, cx - r / 2 - rOko / 2, cy - r / 2, rOko, rOko); //leve oko
			e.Graphics.FillEllipse(Brushes.Blue, cx + r / 2 - rOko / 2, cy - r / 2, rOko, rOko); //prave oko

			const int rPusa = 20;

			e.Graphics.DrawArc(Pens.Red, cx - rPusa, cy, 2 * rPusa, 2 * rPusa / 2, 180, 180);   //usta

			var pen = new Pen(Color.Orange, 5f);    //vlastni pero

			const int pocetPaprsku = 20;
			for (int i = 0; i < pocetPaprsku; i++)
			{
				double a = i * 2 * Math.PI / pocetPaprsku;
				int x1 = Convert.ToInt32(r * Math.Cos(a));
				int y1 = Convert.ToInt32(r * Math.Sin(a));
				int x2 = Convert.ToInt32(2 * r * Math.Cos(a));
				int y2 = Convert.ToInt32(2 * r * Math.Sin(a));

				e.Graphics.DrawLine(pen, x1 + cx, y1 + cy, x2 + cx, y2 + cy);
			}

			string s = "Slunce";
			Font font = new Font("Arial", 20);
			e.Graphics.DrawString(s, font, Brushes.Black, cx - e.Graphics.MeasureString(s, font).Width / 2, cy + 3 * r); //vypsani textu


		}

		private void drawingPanel_Resize(object sender, EventArgs e)
		{
			this.drawingPanel.Invalidate();
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}

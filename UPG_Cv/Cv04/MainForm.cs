using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Cv04
{
	public partial class MainForm : Form
	{
		private const float PACMAN_R = 350f;

		public MainForm()
		{
			InitializeComponent();
			this.ClientSize = new System.Drawing.Size(800, 800);
		}

		/// <summary>Handles the Paint event of the drawingPanel control.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="PaintEventArgs" /> instance containing the event data.</param>
		private void drawingPanel_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			g.TranslateTransform(this.drawingPanel.Width / 2, this.drawingPanel.Height / 2);

			DrawPacman(PACMAN_R, g);
		}

		/// <summary>Draws the pacman with the center in (0,0) and radius R.</summary>
		/// <param name="R">The radius of the pacman.</param>
		/// <param name="g">The graphics context where to draw.</param>
		private void DrawPacman(float R, Graphics g)
		{
			float ecx = 0;
			float ecy = -R * 0.5f;
			float ro = R * 0.25f;
			float ri = ro * 0.5f;

			var eye = new GraphicsPath();
			eye.FillMode = FillMode.Alternate;
			eye.AddLines(
				new PointF[] {
					new PointF(ecx + ro, ecy),
					new PointF(ecx, ecy + ro),
					new PointF(ecx - ro, ecy),
					new PointF(ecx, ecy - ro),
				}
			);
			eye.CloseFigure();

			eye.AddLines(
				new PointF[] {
					new PointF(ecx, ecy + ri),
					new PointF(ecx - ri, ecy),
					new PointF(ecx, ecy - ri),
					new PointF(ecx + ri, ecy),
				}
			);
			eye.CloseFigure();

			var head = new GraphicsPath();

			head.AddEllipse(-R, -R, 2 * R, 2 * R);
			g.DrawPath(Pens.Black, head);

			var mouth = new GraphicsPath();
			mouth.AddLines(new PointF[]
			{
				new PointF(0,0),
				new PointF(2 * R, 0.5f * R),
				new PointF(2 * R, -0.5f * R)
			});


			var pacman = new Region(head);
			pacman.Exclude(eye);
			pacman.Exclude(mouth);

			var brush = new PathGradientBrush(head);
			brush.CenterPoint = new PointF(0, 0);
			brush.CenterColor = Color.Red;
			//brush.SurroundColors = new Color[] {Color.Yellow};
			brush.InterpolationColors = new ColorBlend()
			{
				Colors = new Color[] { Color.Red, Color.Orange, Color.Yellow },
				Positions = new float[] { 0f, 0.8f, 1f }
			};

			g.FillRegion(brush, pacman);

			var brushL = new LinearGradientBrush(
				new PointF(0, R), new PointF(0, -R),
				Color.Black, Color.Yellow);

			brushL.InterpolationColors = new ColorBlend()
			{
				Colors = new Color[] {
					Color.Black,
					Color.FromArgb(80, Color.Black),
					Color.FromArgb(0, Color.Black)
				},
				Positions = new float[] { 0f, 0.5f, 1f }
			};
			brushL.WrapMode = WrapMode.TileFlipXY;

			g.FillRegion(brushL, pacman);

			var clip = g.Clip;
			clip.Intersect(pacman);

			g.Clip = clip;

			DrawPolka(R, g);
		}

		/// <summary>Draws the polka dots in the area -R..R x -R..R.</summary>
		/// <param name="R">The range of the area.</param>
		/// <param name="g">The graphics context where to draw.</param>
		private void DrawPolka(float R, Graphics g)
		{
			float delta = R * 0.2f;
			float fr = R * 0.05f;
			for (float y = -R; y < R; y += delta)
			{
				for (float x = -R; x < R; x += delta)
				{
					g.FillEllipse(Brushes.Green,
						x - fr, y - fr, 2 * fr, 2 * fr);
				}
			}
		}


		/// <summary>Handles the Resize event of the drawingPanel control by forcing repaint.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void drawingPanel_Resize(object sender, EventArgs e)
		{
			this.drawingPanel.Invalidate();
		}

		private void btnPrint_Click(object sender, EventArgs e)
		{
			var dlg = new PrintDialog();
			dlg.PrinterSettings = this.printDocument1.PrinterSettings;
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				this.printDocument1.PrinterSettings = dlg.PrinterSettings;
				this.printPreviewDialog1.ShowDialog();
			}
			
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			var g = e.Graphics;
			g.TranslateTransform(e.PageBounds.Width / 2, e.PageBounds.Height / 2);
			const float PACMAN_PRINT_R = (50 / 25.4f) * 100;
			DrawPacman(PACMAN_PRINT_R, g);
		}
	}
}

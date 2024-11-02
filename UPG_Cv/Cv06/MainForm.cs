using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cv06
{
	public partial class MainForm : Form
	{
		private int N = 6;                //pocet cipu
		private const float DEFAULT_R = 100;    //vychozi polomer hvezdy

		private float R = DEFAULT_R;            //aktualni polomer
		private GraphicsPath star;              //hvezda k vykresleni

		private Matrix curTr;                   //aktualni transformace souradaneho systemu

		private Color curCol;

		private float rotationAngle = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
		public MainForm()
		{
			InitializeComponent();

			//TODO: pridat tlacitko na uzavreni, vcetne obsluhy


			this.ClientSize = new System.Drawing.Size(800, 800);
			curCol = Color.Orange;

			//TODO: Samostatne pridat tlacitko volaiíci MakeStarSmaller

			this.drawingPanel.MouseClick += (o, e) =>
			{
				if (IsStarHit(e.X, e.Y))
				{
					MakeStarSmaller();
				}
			};

			this.drawingPanel.MouseWheel += (o, e) =>
			{
				if (IsStarHit(e.X, e.Y))
				{
					if (e.Delta > 0)
					{
						N += 1;
					}
					else
					{
						if (N != 3)
						{
							N -= 1;
						}
					}

					MakeStarDefault();
				}
			};
			//TODO: pridat reakci na stisk mysi volající MakeStarSmaller
			//TODO: ale jen pri zasahu hvezdy

			//TODO: po stisku klávesy 'r' zavolat MakeStarDefault
			this.KeyPreview = true; //zajisti se oblusha key, ackovil neni na to focus
			this.KeyDown += (o, e) =>
			{
				if (e.KeyCode == Keys.R)
				{
					MakeStarDefault();
					e.Handled = true; //neposlani udalosti dal
				}
			};

			//vytvorime hvezdicku
			this.star = CreateStar();

			

			var timer = new System.Windows.Forms.Timer();
			timer.Tick += TimerTick;
			timer.Interval = 100;
			timer.Start();
		}


		/// <summary>Handles the Paint event of the drawingPanel control.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="PaintEventArgs" /> instance containing the event data.</param>
		private void drawingPanel_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;

			//vycentruj vse
			g.TranslateTransform(this.drawingPanel.Width / 2, this.drawingPanel.Height / 2);

			g.RotateTransform(rotationAngle);

			curTr = g.Transform;
			curTr.Invert();

			DrawStar(g, curCol);

			drawingPanel.Invalidate();
		}


		/// <summary>Creates the graphical object representing the regular star.</summary>
		/// <remarks>The star has centre in the origin of the coordinate system (0,0),
		/// outer radius R, inner radius R/2 and N points.</remarks>
		/// <returns>
		/// Graphics path representing the star.
		/// </returns>
		private GraphicsPath CreateStar()
		{
			var star = new GraphicsPath();

			var pts = new PointF[2 * N];
			double delta_fi = 2 * Math.PI / N;

			for (int i = 0; i < N; i++)
			{
				pts[2 * i] = new PointF
					(
						(float)(R * Math.Cos(i * delta_fi + 1.5 * Math.PI)),
						(float)(R * Math.Sin(i * delta_fi + 1.5 * Math.PI))
					);

				pts[2 * i + 1] = new PointF
					(
						(float)(0.5 * R * Math.Cos(i * delta_fi + delta_fi * 0.5 + 1.5 * Math.PI)),
						(float)(0.5 * R * Math.Sin(i * delta_fi + delta_fi * 0.5 + 1.5 * Math.PI))
				);
			}

			star.AddLines(pts);
			star.CloseAllFigures();
			return star;
		}


		/// <summary>Draws the star.</summary>
		/// <param name="g2">The graphics context used to draw.</param>
		private void DrawStar(Graphics g2, Color color)
		{
			SolidBrush brush = new SolidBrush(color);
			g2.FillPath(brush, star);
		}

		/// <summary>Resets the star to its default shape.</summary>
		internal void MakeStarDefault()
		{
			//R = DEFAULT_R; //smaller star
			this.star = CreateStar();
			this.drawingPanel.Invalidate();  //vynut prekresleni
		}


		/// <summary>Makes the star smaller.</summary>
		internal void MakeStarSmaller()
		{
			R *= 0.5f; //smaller star
			this.star = CreateStar();
			this.drawingPanel.Invalidate();  //vynut prekresleni
		}


		/// <summary>
		/// Determines whether the star is hit by at the specified [x,y].
		/// </summary>
		/// <param name="x">The x coordinate of the query.</param>
		/// <param name="y">The y coordinate of the quary.</param>
		/// <returns>
		///   <c>true</c> if the star is hit; otherwise, <c>false</c>.
		/// </returns>
		internal bool IsStarHit(float x, float y)
		{
			//TODO: test, zda hvezda zasazena
			var pt = new PointF[] { new PointF(x, y) };
			curTr.TransformPoints(pt);
			return this.star.IsVisible(pt[0]);
		}


		/// <summary>Handles the Resize event of the drawingPanel control by forcing repaint.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void drawingPanel_Resize(object sender, EventArgs e)
		{
			this.drawingPanel.Invalidate();
		}

		private void bttnExit_Click(object sender, EventArgs e)
		{

		}

		private void bttnMakeStarSmaller_Click(object sender, EventArgs e)
		{
			MakeStarSmaller();
		}

		private void btnColor_Click(object sender, EventArgs e)
		{
			if (curCol == Color.Orange)
			{
				curCol = Color.Red;
			}
			else
			{
				curCol = Color.Orange;
			}
		}
		private void RotateStar(object sender, EventArgs e)
		{
			rotationAngle += 18;
			if (rotationAngle >= 360)
			{
				rotationAngle -= 360;
			}
			drawingPanel.Invalidate();
		}

		private void TimerTick(object sender, EventArgs e)
		{
			rotationAngle += 18;
			if (rotationAngle >= 360)
			{
				rotationAngle -= 360;
			}
			drawingPanel.Invalidate();
		}
	}
}

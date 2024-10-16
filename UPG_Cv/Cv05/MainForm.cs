using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Cv05
{
    public partial class MainForm : Form
    {
        private int m_StartTime;
		private float vanPositionX; 
		private const float vanSpeed = 100f;

		private float symbolScale = 1.0f;
		private bool growing = true;

		public MainForm()
        {
            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(1200, 400);

            var timer = new System.Windows.Forms.Timer();
            timer.Tick += TimerTick;
            timer.Interval = 50;

			vanPositionX = -300;

			m_StartTime = Environment.TickCount;
            timer.Start();
        }

		private void TimerTick(object? sender, EventArgs e)
		{
			int currentTime = Environment.TickCount;
			float elapsedTime = (currentTime - m_StartTime) / 1000f; // èas v sekundách

			vanPositionX = -300 + (vanSpeed * elapsedTime);

            if (vanPositionX > this.ClientSize.Width)
            {
                vanPositionX = -300; //reset
                m_StartTime = Environment.TickCount;
            }

			drawingPanel.Invalidate();
		}

		/// <summary>Handles the Paint event of the drawingPanel control.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="PaintEventArgs" /> instance containing the event data.</param>
		private void drawingPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            const float HR = 30f;
            g.FillRectangle(Brushes.Green,
                    0, this.drawingPanel.Height - HR,
                    this.drawingPanel.Width, HR);

			//TODO: cilem je vykreslit na
			//trave auto (van)
			g.TranslateTransform(vanPositionX, this.drawingPanel.Height - HR - 150);
			DrawVan(g);
		}

        /// <summary>
        /// Draws a car, 300x150 px big, 
        /// with top left corner of its bounding 
        /// box being at (0,0)
        /// </summary>
        /// <param name="g">Graphics context used to draw</param>
        private void DrawVan(Graphics g)
        {
            var skelet = new GraphicsPath(FillMode.Winding);
            skelet.AddLines(new PointF[] {
                new PointF(0, 125),
                new PointF(0, 70),
                new PointF(30, 7),
                new PointF(60, 0),
                new PointF(270, 0),
                new PointF(290, 7),
                new PointF(300, 70),
                new PointF(300, 125),
                });
            skelet.CloseFigure();

            skelet.AddLines(new PointF[] {
                new PointF(40, 10),
                new PointF(10, 70),
                new PointF(100, 70),
                new PointF(100, 10),
            });
            skelet.CloseFigure();

            g.FillPath(Brushes.Red, skelet);

            //TODO: vykreslit kola na 50,120 a 250,120
            g.TranslateTransform(50, 120);
            DrawWheel(g);

			g.TranslateTransform(200, 0);
			DrawWheel(g);

			//TODO: samostatne vykreslit symbol 
			//na stred auta (175,70), otoceny o 20 stupnu
			g.TranslateTransform(-75, -50);
            g.RotateTransform(20);
			DrawSymbol(g);
		}


        /// <summary>
        /// Draws a wheel with centre at (0,0) and radius 30 px.
        /// </summary>
        /// <param name="g">Graphics context used to draw</param>
        private void DrawWheel(Graphics g)
        {
            var oldTransorm = g.Transform;

            const float R = 30;

            g.FillEllipse(Brushes.Black, 
                -R, -R, 2 * R, 2 * R);

            float r2 = 0.75f * R; //disk
            g.FillEllipse(Brushes.DarkGray,
                -r2, -r2, 2 * r2, 2 * r2);

            const float r3 = 5; //stred
            g.FillEllipse(Brushes.Orange,
                -r3, -r3, 2 * r3, 2 * r3);

            const float d = 10; //vzdalenost sroubu

            float elapsed = (Environment.TickCount - m_StartTime) / 1000f; //pocet ms od startu
            const float v = 180;

            g.RotateTransform(45 + v*elapsed);
            for (int i = 0; i < 4; i++)
            {
                //TODO: rozesadit srouby
                g.TranslateTransform(d, 0);
                DrawBolt(g);
				g.TranslateTransform(-d, 0);
				g.RotateTransform(90);

            }

            g.Transform = oldTransorm;
		}

        /// <summary>
        /// Draws a tiny bolt in the centre of coordinate system (i.e, at 0,0)
        /// </summary>
        /// <param name="g"></param>
	    private void DrawBolt(Graphics g)
        {
            const float rb = 2; //polomer sroubu
            g.FillEllipse(Brushes.Orange,
                -rb, -rb, 2 * rb, 2 * rb);
        }

        /// <summary>
        /// Draws a symbol with the centre at (0,0) and radius 40.
        /// </summary>
        /// <param name="g"></param>
        private void DrawSymbol(Graphics g)
        {
			var oldTransorm = g.Transform;

            float speed = 0.05f;

			if (growing)
			{
				symbolScale += speed; 
				if (symbolScale >= 2.0f) growing = false;
			}
			else
			{
				symbolScale -= speed; 
				if (symbolScale <= 1.0f) growing = true;
			}

            g.ScaleTransform(symbolScale, symbolScale);

			var pen = new Pen(Color.White, 3f);

            const int R = 20;
            g.DrawEllipse(pen, -R, -R, 2 * R, 2 * R);
            g.DrawLine(pen, 0, -R, 0, R);

            int rc = (int)(R * Math.Cos(Math.PI * (1 / 4.0)));
            int rs = (int)(R * Math.Sin(Math.PI * (1 / 4.0)));
            g.DrawLine(pen, 0, 0, rc, rs);
            g.DrawLine(pen, 0, 0, -rc, rs);

			


			g.Transform = oldTransorm;
		}

        /// <summary>Handles the Resize event of the drawingPanel control by forcing repaint.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void drawingPanel_Resize(object sender, EventArgs e)
        {
            this.drawingPanel.Invalidate();
        }
    }
}

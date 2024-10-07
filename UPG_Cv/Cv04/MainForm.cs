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
            eye.AddLines(
                new PointF[] {
                    new PointF(ecx + ro, ecy),
                    new PointF(ecx, ecy + ro),
                    new PointF(ecx - ro, ecy),
                    new PointF(ecx, ecy - ro),
                }
            );
            eye.CloseFigure();

            //TODO: samostatne pridej vnitrni kosoctverec (ri)
            //s body definovanymi v obracenem poradi            

            //TODO: pridej Region s pacmanem (bez pusy)

            //TODO: samostatne vyriznete pusu - pouzijte Path2D

            g.FillPath(Brushes.Red, eye);

            //DrawPolka(R, g);
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
            //TODO: Pridat na formular PrintDocument
            //TODO: Pridat na formular PrintPreviewDialog a napojit ho na PrintDocument
            //TODO: Pridat sem zavolani dialogu PrintPreviewDialog
            //TODO: Pridat do MainForm obsluhu udalosti PrintPage pridaneho PrintDocument
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

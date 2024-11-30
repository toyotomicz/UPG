
using System.Drawing;

namespace Cv11
{
    public partial class MainForm : Form
    {
        private const int W = 1000;
        private const int H = 720;

        public MainForm()
        {
            InitializeComponent();

            this.ClientSize = new Size(W, H);

            var svg = new SvgNet.SvgGraphics(Color.White);
            DrawFlowers(svg);

            var svgString = svg.WriteSVGString(W, H);
            File.WriteAllText("../../../data/vystup.svg", svgString);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.FillRectangle(Brushes.White, 
                0, 0, this.Width, this.Height);

            DrawFlowers(new SvgNet.GdiGraphics(g));
        }

        private void DrawFlowers(SvgNet.Interfaces.IGraphics g)
        {
            var oldTr = g.Transform;
            DrawFlower(g);

            g.TranslateTransform(200, 700);
            g.RotateTransform(15);
            g.ScaleTransform(0.75f, 0.75f);
            g.TranslateTransform(-200, -700);

            g.TranslateTransform(200, -50);
            DrawFlower(g);

            g.Transform = oldTr;

            const int GR_H = 20;
            g.FillRectangle(Brushes.Green, 0, H - GR_H, W, GR_H);

            g.DrawRectangle(new Pen(Color.FromArgb(0, 128, 0)), 
                0, H - GR_H, W, GR_H);
        }

        private void DrawFlower(SvgNet.Interfaces.IGraphics g)
        {
            var oldTr = g.Transform;
            g.TranslateTransform(200, 200);

            g.DrawLine(new Pen(Brushes.Green, 10), 0, 0, 0, 500);

            const int R = 40;
            g.FillEllipse(Brushes.Yellow, -R, -R, 2 * R, 2 * R);

            var bkPen = new Pen(Brushes.Black, 3);
            g.DrawEllipse(bkPen, -R, -R, 2 * R, 2 * R);

            const int ANGLE = 20;
            const int TX = 105;
            const int TY = 0;
            const int RX = 60;
            const int RY = 10;

            for (int i = 0; i < 360 / ANGLE; i++)
            {
                var at = g.Transform;
                g.RotateTransform((i * ANGLE));
                g.TranslateTransform(TX, TY);
                
                g.FillEllipse(Brushes.WhiteSmoke, -RX, -RY, 2 * RX, 2 * RY);
                g.DrawEllipse(bkPen, -RX, -RY, 2 * RX, 2 * RY);

                g.Transform = at;
            }

            g.Transform = oldTr;
        }
    }
}

     
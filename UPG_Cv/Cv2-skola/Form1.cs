using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cv2_skola
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			int cx = Width / 2; //stred platna
			int cy = Height / 2;

			const int r = 100;   //polomer kruhu
			const int l = 400; // delka hrany

			g.FillRectangle(Brushes.Black, cx - l / 2, cy - l / 2 , l , l ); //bily obdelnik
			g.FillRectangle(Brushes.White, cx - l / 2+10, cy - l / 2+10, l-20, l-20); //bily obdelnik

			g.FillEllipse(Brushes.Black, cx -100, cy -100, 2*r, 2*r); //slunce

			Brush white = new SolidBrush(Color.White);
			Brush black = new SolidBrush(Color.Black);

			g.FillPie(black, cx - 75, cy - 75 +20, 150, 150, 225, 90);
			g.FillPie(white, cx - 60, cy - 60 + 20, 120, 120, 225, 90);

			g.FillPie(black, cx - 45, cy - 45 + 20, 90, 90, 225, 90);
			g.FillPie(white, cx - 35, cy - 35 + 20, 70, 70, 225, 90);

			g.FillPie(black, cx-25, cy-25 + 20, 50, 50, 225, 90);
			g.FillPie(white, cx-15, cy-15 + 20, 30, 30, 225, 90);






			string s = "FREE WIFI";
			Font font = new Font("Arial", 40);
			e.Graphics.DrawString(s, font, Brushes.Blue, cx - e.Graphics.MeasureString(s, font).Width / 2, cy+120); //vypsani textu
		}

		private void panel1_Resize(object sender, EventArgs e)
		{
			panel1.Invalidate();
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cv08
{
	public partial class DrawingPanel : UserControl
	{
		public DrawingPanel()
		{
			InitializeComponent();

			this.Lut = MakeLut(10, 64);
		}

		private const string DATA_FILENAME = "../../../data/mrt8_angio2.raw";
		protected const int DATA_SZ = 128;    //pocet rezu (obrazku)
		protected const int DATA_SY = 320;    //pocet radek v kazdem rezu
		protected const int DATA_SX = 256;    //pocet hodnot v kazdem radku

		private byte[] _data;

		protected Color[] Lut { get; set; }

		private Color[] MakeLut(int min, int max)
		{
			const int N = 256;
			var clr = new Color[N];

			for (int i = 0; i < N; i++)
			{
				if (i >= max)
				{
					clr[i] = Color.Black;
				}
				else if (i <= min)
				{
					clr[i] = Color.FromArgb(255, 255, 0);
				}
				else
				{   //Result = (color2 - color1) * fraction + color1
					float fraction = (float)(i - min) / (max - min);
					int[] color1 = [255, 255, 0];
					int[] color2 = [0, 47, 97];

					int r = (int)((color2[0] - color1[0]) * fraction + color1[0]);
					int g = (int)((color2[1] - color1[1]) * fraction + color1[1]);
					int b = (int)((color2[2] - color1[2]) * fraction + color1[2]);

					clr[i] = Color.FromArgb(r, g, b);
				}
			}

			return clr;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var bpm = new Bitmap(Width, Height, e.Graphics);
			var g = Graphics.FromImage(bpm);
			g.FillRectangle(new SolidBrush(this.BackColor), 
				0, 0, Width, Height);

			var new_e = new PaintEventArgs(g, e.ClipRectangle);
			base.OnPaint(new_e); //kresli se do bitmapy

			//do leveho horniho rohu zobrazime rez XY
			g.DrawImageUnscaled(this.SliceXZ, 0, 0);

			//vypis, kde se nachazime
			const float GAP = 16;
			var fnt = new Font(FontFamily.GenericSansSerif, 12);
			g.DrawString("y = " + Y, fnt, Brushes.Black, 0, this.SliceXZ.Height + GAP);

			e.Graphics.DrawImageUnscaled(bpm, 0, 0);
		}

		protected byte[] Data
		{
			get
			{
				if (_data == null)
				{
					//nacti data
					_data = new byte[DATA_SX * DATA_SY * DATA_SZ];
					using (FileStream sr = new FileStream(DATA_FILENAME, FileMode.Open))
					{
						sr.Read(_data, 0, _data.Length);
					}
				}

				return _data;
			}
		}

		private Image? _slice_XY;   //rez kolmy na osu z (prochazejici Z)
		private Image? _slice_XZ;   //rez kolmy na osu y (prochazejici Y)
		private Image? _slice_YZ;   //rez kolmy na osu x (prochazejici X)

		private int _x;
		private int _y;
		private int _z;

		/// <summary>
		/// Gets the maximum value of Z.
		/// </summary>
		public int ZMax => DATA_SZ - 1;

		/// <summary>
		/// Gets or sets the position of SliceXY.
		/// </summary>
		/// <value>
		/// The z coordinate from 0 to ZMax.
		/// </value>
		public int Z
		{
			get => _z;
			set
			{
				//            Contract.Requires<ArgumentException>(value >= 0 && value <= ZMax);

				if (_z != value)
				{
					_slice_XY = null;   //zrus rez
					_z = value;

					this.Invalidate(); //vynut prekresleni
				}
			}
		}

		/// <summary>
		/// Gets the maximum value of Y.
		/// </summary>
		public int YMax => DATA_SY - 1;

		/// <summary>
		/// Gets or sets the position of SliceXZ.
		/// </summary>
		/// <value>
		/// The z coordinate from 0 to YMax.
		/// </value>
		public int Y
		{
			get => _y;
			set
			{
				//                Contract.Requires<ArgumentException>(value >= 0 && value <= YMax);

				if (_y != value)
				{
					_slice_XZ = null;   //zrus rez
					_y = value;

					this.Invalidate(); //vynut prekresleni
				}
			}
		}

		/// <summary>
		/// Gets the maximum value of X.
		/// </summary>
		public int XMax => DATA_SX - 1;

		/// <summary>
		/// Gets or sets the position of SliceXZ.
		/// </summary>
		/// <value>
		/// The z coordinate from 0 to YMax.
		/// </value>
		public int X
		{
			get => _x;
			set
			{
				//              Contract.Requires<ArgumentException>(value >= 0 && value <= XMax);

				if (_x != value)
				{
					_slice_YZ = null;   //zrus rez
					_x = value;

					this.Invalidate(); //vynut prekresleni
				}
			}
		}

		/// <summary>
		/// Gets the slice XY constructed in Z.
		/// </summary>
		protected Image SliceXY
		{
			get
			{
				//cashovani vysledku
				if (this._slice_XY != null)
				{
					return _slice_XY;
				}

				var img = new Bitmap(DATA_SX, DATA_SY,
					System.Drawing.Imaging.PixelFormat.Format24bppRgb);

				var bpmIn = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
					System.Drawing.Imaging.ImageLockMode.WriteOnly,
					System.Drawing.Imaging.PixelFormat.Format24bppRgb);

				byte[] rgb = new byte[bpmIn.Height * bpmIn.Stride];
				Marshal.Copy(bpmIn.Scan0, rgb, 0, rgb.Length);

				int offset = Z * (DATA_SX * DATA_SY);
				for (int y = 0, idx_row = 0; y <= YMax; y++, idx_row += bpmIn.Stride)
				{
					for (int x = 0, idx = idx_row; x <= XMax; x++, idx += 3)
					{
						//znam x, y, z
						byte v = this.Data[offset + x];
						Color color = this.Lut[v];

						rgb[idx] =		color.R;
						rgb[idx + 1] =	color.G;
						rgb[idx + 2] =	color.B;
					}
					offset += DATA_SX;
				}

				Marshal.Copy(rgb, 0, bpmIn.Scan0, rgb.Length);

				img.UnlockBits(bpmIn);

				_slice_XY = img;
				return img;
			}
		}

		/// <summary>
		/// Gets the slice XZ constructed in Y.
		/// </summary>
		protected Image SliceXZ
		{
			get
			{
				// Caching the result
				if (this._slice_XZ != null)
				{
					return _slice_XZ;
				}

				var img = new Bitmap(DATA_SX, DATA_SZ,
					System.Drawing.Imaging.PixelFormat.Format24bppRgb);

				var bpmIn = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
					System.Drawing.Imaging.ImageLockMode.WriteOnly,
					System.Drawing.Imaging.PixelFormat.Format24bppRgb);

				byte[] rgb = new byte[bpmIn.Height * bpmIn.Stride];
				Marshal.Copy(bpmIn.Scan0, rgb, 0, rgb.Length);

				int offset = Y * DATA_SX;
				for (int z = 0, idx_row = 0; z <= ZMax; z++, idx_row += bpmIn.Stride)
				{
					for (int x = 0, idx = idx_row; x <= XMax; x++, idx += 3)
					{
						byte v = this.Data[offset + x];
						Color color = this.Lut[v];

						rgb[idx] =		color.B;
						rgb[idx + 1] =	color.G;
						rgb[idx + 2] =	color.R;
					}
					offset += DATA_SX * DATA_SY;
				}

				Marshal.Copy(rgb, 0, bpmIn.Scan0, rgb.Length);

				img.UnlockBits(bpmIn);

				_slice_XZ = img;
				return img;
			}
		}

		/// <summary>
		/// Gets the slice YZ constructed in X.
		/// </summary>
		protected Image SliceYZ
		{
			get
			{
				var img = new Bitmap(DATA_SY, DATA_SZ,
					System.Drawing.Imaging.PixelFormat.Format24bppRgb);

				//TODO: samostatne

				return img;
			}
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			this.Invalidate();
		}
	}
}
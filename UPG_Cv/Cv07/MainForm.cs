using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Cv07
{
    public partial class MainForm : Form
    {
        const string DATA_ROOT = "../../../data";

        private Image? _finalImage;

        /// <summary>
        /// Gets the final image to be displayed on the drawing panel.
        /// </summary>
        /// <value>
        /// The final image.
        /// </value>
        public Image FinalImage
        {
            get
            {
                if (_finalImage == null)
                {
                    //vytvor vysledny obrazek
                    _finalImage = CreateFinalImage(this.TransformedBackground, 
                        this.Foreground, this.Alpha);
                }
                return _finalImage;
            }
        }

        private Image? _transformedBackground;
        /// <summary>
        /// Gets the transformed background.
        /// </summary>
        /// <value>
        /// The transformed background.
        /// </value>
        public Image? TransformedBackground
        {
            get
            {
                if (this._transformedBackground == null)
                {
                    this._transformedBackground = CreateTransformedBackground(this.Background);
                }

                return this._transformedBackground;
            }
        }


        private Image? _background;

        /// <summary>
        /// Gets or sets the background image.
        /// </summary>
        /// <value>
        /// The background image.
        /// </value>
        public Image? Background {  
            get => _background;
            set
            {
                if (_background != value)
                {
                    _background = value;
                    _transformedBackground = null;
                    _finalImage = null;

                    this.drawingPanel.Invalidate(); //redraw
                }
            } 
        }


        
        private Image? _foreground;

        /// <summary>
        /// Gets or sets the foreground image.
        /// </summary>
        /// <value>
        /// The foreground image.
        /// </value>
        public Image? Foreground
        {
            get => _foreground; set
            {
                if (_foreground != value)
                {
                    _foreground = value;
                    _finalImage = null;
                    this.drawingPanel.Invalidate();
                }
            }
        }

        private float _alpha = 1f;

        /// <summary>
        /// Gets or sets the alpha used to blend foreground and background images.
        /// </summary>
        /// <value>
        /// The alpha must be in range 0 (transparent) - 1 (opaque).
        /// </value>
        public float Alpha
        { 
            get => _alpha;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException("Alpha must be from the range <0-1>");
                }


                _alpha = value;
                _finalImage = null;
                this.drawingPanel.Invalidate();
            }
        }

        public MainForm()
        {
            InitializeComponent();

            this.Background = Bitmap.FromFile(Path.Combine(DATA_ROOT, "Jungle.jpg"));

            this.Foreground = Bitmap.FromFile(Path.Combine(DATA_ROOT, "Girl.jpg"));

			drawingPanel.Paint += DrawingPanel_Paint;

            bttnSave.Click += BttnSave_Click;

            var startTime = Environment.TickCount;
            var tm = new System.Windows.Forms.Timer() { 
                Interval = 20,                
            };

            tm.Tick += (o, e) => { 
                //pocet sekund, ktere uplynuly
                float elapsed = (Environment.TickCount - startTime) / 1000.0f;

                //TODO: BONUS - zmente alphu na korektni hodnotu v zavislosti na elapsed                
            };            
        }

        /// <summary>
        /// Draws on the panel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>       
        private void DrawingPanel_Paint(object? sender, PaintEventArgs e)
        {
            DrawScene(e.Graphics, this.drawingPanel.Width, this.drawingPanel.Height);

            //TODO: SAMOSTATNE (az po realizaci DrawScene) prepnout na double-buffering        
        }

        /// <summary>
        /// Draws the scene containing the merged image.
        /// </summary>
        /// <param name="g">The graphics context used to draw.</param>
        /// <param name="W">The width.</param>
        /// <param name="H">The height.</param>
        private void DrawScene(Graphics g, int W, int H)
        {
            g.FillRectangle(Brushes.DarkGray, 0, 0, W, H);

            //vykreslime obrazek v rozliseni 1:1
            //g.DrawImageUnscaled(FinalImage, 0, 0);

            float scale = Math.Min
                (
                    (float)H / FinalImage.Height,
                    (float)W / FinalImage.Width
                );

			float centreX = W / 2 - FinalImage.Width * scale / 2;
			float centreY = H / 2 - FinalImage.Height * scale / 2;

			g.DrawImage(FinalImage, centreX, centreY,
                FinalImage.Width * scale,
                FinalImage.Height * scale
            );


            //TODO: zobraz obrazek roztazeny na velikost okna
            //se zachovanim pomeru stran

            //TODO: SAMOSTATNE zobraz vycentrovane
        }


		/// <summary>
		/// Creates the transformed version of the image.
		/// </summary>
		/// <param name="img">The original image.</param>
		/// <returns>The transformed version of the image.</returns>        
		private Image? CreateTransformedBackground(Image? img)
		{
			if (img == null || img is not Bitmap img_in)
				return null;

			var img_out = new Bitmap(img_in.Width, img_in.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

			var bpm_in = img_in.LockBits(new Rectangle(0, 0, img_in.Width, img_in.Height),
				System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

			var bpm_out = img_out.LockBits(new Rectangle(0, 0, img_out.Width, img_out.Height),
				System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

			byte[] rgb_in = new byte[bpm_in.Height * bpm_in.Stride];
			byte[] rgb_out = new byte[bpm_out.Height * bpm_out.Stride];

			Marshal.Copy(bpm_in.Scan0, rgb_in, 0, rgb_in.Length);

			for (int y = 0; y < img_in.Height; y++)
			{
				int rowStartIn = y * bpm_in.Stride;
				int rowStartOut = y * bpm_out.Stride;

				for (int x = 0; x < img_in.Width; x++)
				{
					int srcIndex = rowStartIn + x * 3;
					int destIndex = rowStartOut + (img_in.Width - 1 - x) * 3;

					rgb_out[destIndex] = rgb_in[srcIndex];
					rgb_out[destIndex + 1] = rgb_in[srcIndex + 1];
					rgb_out[destIndex + 2] = rgb_in[srcIndex + 2];
				}
			}

			Marshal.Copy(rgb_out, 0, bpm_out.Scan0, rgb_out.Length);

			img_in.UnlockBits(bpm_in);
			img_out.UnlockBits(bpm_out);

			return img_out;
		}



		/// <summary>
		/// Merges the foreground and background image layers.
		/// </summary>
		/// <param name="bkgnd">The background image.</param>
		/// <param name="fgnd">The foreground image.</param>
		/// <param name="alpha">The alpha used to blend images.</param>
		/// <returns>
		/// Final image
		/// </returns>
		private Image CreateFinalImage(Image? bkgnd, Image? fgnd, float alpha)
        {
            //pokud nemame nastavene pozadi, vrat popredi prip. dummy obrazek
            if (bkgnd == null)
            {
                return (fgnd ?? new Bitmap(100, 100, 
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb));
            }

            if (fgnd == null)
            {
                return bkgnd;
            }

            //mame TransformedBackground i Foreground
            //TODO: BONUS = merge TransformedBackground a Foreground do noveho obrazku 
            //(s moznym vyuzitim alpha) - ten pak se vratit na vystup

            return bkgnd;
        }

        /// <summary>
        /// Handles click on the Save button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void BttnSave_Click(object? sender, EventArgs e)
        {
            SaveImage(Path.Combine(DATA_ROOT,"vystup.jpg"));
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="path">The full path (including the filename and extension).</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void SaveImage(string path)
        {
			var out_img = (FinalImage as Bitmap);
            if(out_img != null)
            {
                int imW = 4000;
                int imH = 4000 * out_img.Height / out_img.Width;

                var bpm = new Bitmap(imW, imH, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                Graphics g = Graphics.FromImage(bpm);
                g.DrawImage(out_img, 0, 0, imW, imH);
                g.Dispose();
				bpm.Save(path);
            }
        }

        private void drawingPanel_Resize(object sender, EventArgs e)
        {
            this.drawingPanel.Invalidate();
        }
    }
}

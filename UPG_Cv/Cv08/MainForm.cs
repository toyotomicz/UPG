using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Cv08
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
      
            var startTime = Environment.TickCount;
            var tm = new System.Windows.Forms.Timer()
            {
                Interval = 20,
            };

            tm.Tick += (o, e) =>
            {
                //pocet sekund, ktere uplynuly
                float elapsed = (Environment.TickCount - startTime) / 1000.0f;

                this.drawingPanel.Y = (this.drawingPanel.Y < this.drawingPanel.YMax) ? this.drawingPanel.Y + 1 : 0;
            };

            tm.Start();
        }
    }
}

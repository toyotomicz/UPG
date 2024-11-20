using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace UPG_semestralka
{
    public partial class GraphForm : Form
    {
        // Configuration constants
        private const int MARGIN_LEFT = 70;    // Increased for scientific notation
        private const int MARGIN_RIGHT = 30;
        private const int MARGIN_TOP = 30;
        private const int MARGIN_BOTTOM = 50;  // Increased for better label spacing
        private const int TICK_LENGTH = 5;
        private const int TICK_COUNT = 5;
        
        // Styling
        private static readonly Color GraphColor = Color.FromArgb(0, 120, 212);  // Professional blue
        private static readonly Color GridColor = Color.FromArgb(230, 230, 230); // Light grey
        private static readonly float LineThickness = 2f;
        
        private List<(double time, double intensity)> data = new List<(double time, double intensity)>();
        private Rectangle plotArea;
        
        public GraphForm()
        {
            InitializeComponents();
            SetupForm();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(600, 400);  // Larger default size
            this.MinimumSize = new Size(400, 300);
            this.Text = "Electric Field Intensity Over Time";
            this.Paint += GraphForm_Paint;
            this.Resize += GraphForm_Resize;
            this.DoubleBuffered = true;
        }

        private void SetupForm()
        {
            // Add a gradient background
            this.BackColor = Color.White;
            
            // Calculate initial plot area
            UpdatePlotArea();
        }

        private void UpdatePlotArea()
        {
            plotArea = new Rectangle(
                MARGIN_LEFT,
                MARGIN_TOP,
                this.ClientSize.Width - MARGIN_LEFT - MARGIN_RIGHT,
                this.ClientSize.Height - MARGIN_TOP - MARGIN_BOTTOM
            );
        }

        public void UpdateData(List<(double time, double intensity)> newData)
        {
            data = new List<(double time, double intensity)>(newData);
            this.Invalidate();
        }

        private void GraphForm_Paint(object sender, PaintEventArgs e)
        {
            if (data.Count < 2) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                DrawGrid(g);
                DrawAxes(g);
                DrawData(g);
                DrawLabels(g);
            }
            catch (Exception ex)
            {
                DrawErrorMessage(g, ex.Message);
            }
        }

        private void DrawGrid(Graphics g)
        {
            using (var gridPen = new Pen(GridColor, 1))
            {
                // Draw vertical grid lines
                for (int i = 0; i <= TICK_COUNT; i++)
                {
                    float x = plotArea.Left + (i * plotArea.Width / TICK_COUNT);
                    g.DrawLine(gridPen, x, plotArea.Top, x, plotArea.Bottom);
                }

                // Draw horizontal grid lines
                for (int i = 0; i <= TICK_COUNT; i++)
                {
                    float y = plotArea.Bottom - (i * plotArea.Height / TICK_COUNT);
                    g.DrawLine(gridPen, plotArea.Left, y, plotArea.Right, y);
                }
            }
        }

        private void DrawAxes(Graphics g)
        {
            using (var axisPen = new Pen(Color.Black, 1.5f))
            {
                // Draw X and Y axes
                g.DrawLine(axisPen, plotArea.Left, plotArea.Bottom, plotArea.Right, plotArea.Bottom);
                g.DrawLine(axisPen, plotArea.Left, plotArea.Top, plotArea.Left, plotArea.Bottom);
                
                DrawAxisTicks(g);
            }
        }

        private void DrawAxisTicks(Graphics g)
        {
            if (data.Count < 2) return;

            var (minTime, maxTime, minIntensity, maxIntensity) = GetDataRange();
            
            using (var tickFont = new Font("Segoe UI", 8))
            {
                // Draw X-axis ticks
                for (int i = 0; i <= TICK_COUNT; i++)
                {
                    float x = plotArea.Left + (i * plotArea.Width / TICK_COUNT);
                    float y = plotArea.Bottom;
                    
                    g.DrawLine(Pens.Black, x, y, x, y + TICK_LENGTH);
                    
                    double time = minTime + (i * (maxTime - minTime) / TICK_COUNT);
                    string timeLabel = time.ToString("F2");
                    var timeLabelSize = g.MeasureString(timeLabel, tickFont);
                    
                    g.DrawString(timeLabel, tickFont, Brushes.Black,
                        x - timeLabelSize.Width / 2,
                        y + TICK_LENGTH + 2);
                }

                // Draw Y-axis ticks
                for (int i = 0; i <= TICK_COUNT; i++)
                {
                    float x = plotArea.Left;
                    float y = plotArea.Bottom - (i * plotArea.Height / TICK_COUNT);
                    
                    g.DrawLine(Pens.Black, x - TICK_LENGTH, y, x, y);
                    
                    double intensity = minIntensity + (i * (maxIntensity - minIntensity) / TICK_COUNT);
                    string intensityLabel = intensity.ToString("E2");
                    var intensityLabelSize = g.MeasureString(intensityLabel, tickFont);
                    
                    g.DrawString(intensityLabel, tickFont, Brushes.Black,
                        x - TICK_LENGTH - intensityLabelSize.Width - 5,
                        y - intensityLabelSize.Height / 2);
                }
            }
        }

        private void DrawData(Graphics g)
        {
            if (data.Count < 2) return;

            var (minTime, maxTime, minIntensity, maxIntensity) = GetDataRange();
            
            using (var dataPen = new Pen(GraphColor, LineThickness))
            {
                dataPen.StartCap = LineCap.Round;
                dataPen.EndCap = LineCap.Round;

                for (int i = 1; i < data.Count; i++)
                {
                    var point1 = TransformDataToScreen(data[i - 1], minTime, maxTime, minIntensity, maxIntensity);
                    var point2 = TransformDataToScreen(data[i], minTime, maxTime, minIntensity, maxIntensity);
                    
                    if (IsValidPoint(point1) && IsValidPoint(point2))
                    {
                        g.DrawLine(dataPen, point1, point2);
                    }
                }
            }
        }

        private void DrawLabels(Graphics g)
        {
            using (var labelFont = new Font("Segoe UI", 10, FontStyle.Regular))
            {
                // X-axis label
                string timeLabel = "Time (s)";
                var timeLabelSize = g.MeasureString(timeLabel, labelFont);
                g.DrawString(timeLabel, labelFont, Brushes.Black,
                    plotArea.Left + (plotArea.Width - timeLabelSize.Width) / 2,
                    this.ClientSize.Height - MARGIN_BOTTOM + 20);

                // Y-axis label
                using (var matrix = new Matrix())
                {
                    string intensityLabel = "Field Intensity (N/C)";
                    var intensityLabelSize = g.MeasureString(intensityLabel, labelFont);
                    
                    matrix.RotateAt(-90,
                        new PointF(MARGIN_LEFT - 40,
                        plotArea.Top + (plotArea.Height + intensityLabelSize.Width) / 2));
                    
                    g.Transform = matrix;
                    g.DrawString(intensityLabel, labelFont, Brushes.Black,
                        MARGIN_LEFT - 40,
                        plotArea.Top + (plotArea.Height + intensityLabelSize.Width) / 2);
                    g.ResetTransform();
                }
            }
        }

        private void DrawErrorMessage(Graphics g, string message)
        {
            using (var errorFont = new Font("Segoe UI", 10))
            {
                g.DrawString($"Error rendering graph: {message}",
                    errorFont, Brushes.Red, MARGIN_LEFT, MARGIN_TOP);
            }
        }

        private (double minTime, double maxTime, double minIntensity, double maxIntensity) GetDataRange()
        {
            double minTime = double.MaxValue, maxTime = double.MinValue;
            double minIntensity = double.MaxValue, maxIntensity = double.MinValue;

            foreach (var point in data)
            {
                minTime = Math.Min(minTime, point.time);
                maxTime = Math.Max(maxTime, point.time);
                minIntensity = Math.Min(minIntensity, point.intensity);
                maxIntensity = Math.Max(maxIntensity, point.intensity);
            }

            // Add padding to intensity range
            double intensityPadding = (maxIntensity - minIntensity) * 0.1;
            maxIntensity += intensityPadding;
            minIntensity = Math.Max(0, minIntensity - intensityPadding);  // Ensure non-negative

            return (minTime, maxTime, minIntensity, maxIntensity);
        }

        private PointF TransformDataToScreen((double time, double intensity) dataPoint,
            double minTime, double maxTime, double minIntensity, double maxIntensity)
        {
            float x = plotArea.Left + (float)((dataPoint.time - minTime) / (maxTime - minTime) * plotArea.Width);
            float y = plotArea.Bottom - (float)((dataPoint.intensity - minIntensity) / (maxIntensity - minIntensity) * plotArea.Height);
            return new PointF(x, y);
        }

        private bool IsValidPoint(PointF point)
        {
            return !float.IsInfinity(point.X) && !float.IsInfinity(point.Y) &&
                   !float.IsNaN(point.X) && !float.IsNaN(point.Y);
        }

        private void GraphForm_Resize(object sender, EventArgs e)
        {
            UpdatePlotArea();
            this.Invalidate();
        }
    }
}
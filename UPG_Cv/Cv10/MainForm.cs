using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Cv10.model;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Cv10
{
	public partial class MainForm : Form
	{
		private void InitializeBoxPlot(CourseDb courseDb)
		{
			var boxPlotSeries = new List<ISeries>();

			foreach (var course in courseDb.Courses)
			{
				

				var grades1 = course.Statistics.Select(s => (double)s.Grade1).ToArray();
				var grades2 = course.Statistics.Select(s => (double)s.Grade2).ToArray();
				var grades3 = course.Statistics.Select(s => (double)s.Grade3).ToArray();
				var grades4 = course.Statistics.Select(s => (double)s.Grade4).ToArray();

				// pro kazdou znamku
				boxPlotSeries.Add(new BoxSeries<double>
				{
					Values = new[] { new BoxValue(grades1) },
					Name = $"{course.Name} - Grade 1",
					Fill = new SolidColorPaint(SKColors.Green)
				});

				boxPlotSeries.Add(new BoxSeries<double>
				{
					Values = new[] { new BoxValue(grades2) },
					Name = $"{course.Name} - Grade 2",
					Fill = new SolidColorPaint(SKColors.Blue)
				});

				boxPlotSeries.Add(new BoxSeries<double>
				{
					Values = new[] { new BoxValue(grades3) },
					Name = $"{course.Name} - Grade 3",
					Fill = new SolidColorPaint(SKColors.Orange)
				});

				boxPlotSeries.Add(new BoxSeries<double>
				{
					Values = new[] { new BoxValue(grades4) },
					Name = $"{course.Name} - Grade 4",
					Fill = new SolidColorPaint(SKColors.Red)
				});
			}

			cartesianChart1.Series = boxPlotSeries;
			cartesianChart1.XAxes = new[]
			{
				new Axis
				{
					Labels = boxPlotSeries.Select(s => s.Name).ToList(),
					LabelsRotation = 45
				}
			};

			cartesianChart1.YAxes = new[]
			{
				new Axis
				{
					Name = "Number of Students"
				}
			};
		}

		public MainForm()
		{
			InitializeComponent();
			var cdb = new CourseDb();
			InitializeBoxPlot(cdb);
		}
	}
}
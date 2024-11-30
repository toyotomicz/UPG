using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq.Expressions;
using System.Windows.Forms;
using Cv10.model;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using System.Collections.Generic;


namespace Cv10
{
    public partial class MainForm : Form
    {
        //see:https://livecharts.dev/docs/WinForms/2.0.0-rc2/Overview.Installation         

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            var cdb = new CourseDb();

            InitializeBarChart(cdb.Courses);
            
            //TODO: 5. Pridat titulek a popisy os 
            //TODO: 6. SAMOSTATNE zmenit na % uspesnosti
            //TODO: 7. Pridat popisky dat
            //TODO: 8. Pridat legendu
            //TODO: 9. Vizualni upravy: sirka mezer, fonty, ...
            //TODO: 10. SAMOSTATNE LineSeries

        }

        private void InitializeBarChart(IList<Course> courses)
        {
            var labels = new List<string>();
            foreach (var course in courses)
            {
                labels.AddRange(course.Statistics.Select(x => x.Label).ToList());
            }
            labels.Sort(
                (x, y) => (x.Substring(3, 4) + (x[0] == 'Z' ? '0' : '1'))
                .CompareTo((y.Substring(3, 4) + (y[0] == 'Z' ? '0' : '1'))
            ));
            labels = labels.Distinct().ToList();

			var series = new List<ISeries>(courses.Count);
            foreach(var course in courses)
            {
                var cb = new ColumnSeries<CourseDataItem>()
                {
                    Name = course.Name,
                    //Values = course.Statistics.Select(x => x.Students).ToList(),
                    Values = course.Statistics,
                    Mapping = (cd, p) => new(labels.IndexOf(cd.Label), cd.Students)
                };
                series.Add(cb);
            }

            chart.Series = series;
            chart.XAxes = new Axis[1]
            {
                new Axis
                {
                    Name = "Rocnik",
                    Labels = labels,
                }
            };
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;

namespace Cv10.model
{

    /// <summary>
    /// Reprezentuje statistiku uspesnosti predmetu v ramci jednoho roku
    /// </summary>    
    internal class CourseDataItem
    {
        //TODO: predelat na record, nebo alespon namisto set mit init

        public string Label { get; set; } //oznaceni rocniku ve formatu "[ZL]S yyyy/yy", napr. LS 2020/2021
        public int Students { get; set; }    //pocet studentu
        public int CourseCredit { get; set; } //zapocet
        public int Grade4 { get; set; }  //znamka 4 = nevyhovel
        public int Grade3 { get; set; }  //znamka 3 = dobre
        public int Grade2 { get; set; }  //znamka 2 = velmi dobre
        public int Grade1 { get; set; }  //znamka 1 = vyborne

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Label}: {Students}/{CourseCredit}/{Grade1}:{Grade2}:{Grade3}:{Grade4}";
        }
    }

    /// <summary>
    /// Reprezentuje jeden konkretni predmet
    /// </summary>
    internal class Course
    {
        public string Name { get; set; } //nazev predmetu
        public List<CourseDataItem> Statistics { get; private set; } //statistiky uspesnosti

        public Course()
        {
            this.Statistics = new List<CourseDataItem>();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in Statistics) {
                sb.AppendLine(item.ToString());
            }

            return $"{Name}\n-------\n" + sb.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class CourseDb
    {
        private const string DATA_DIR = "../../../data/";
        private const string MASTER_FILE = "index.txt";

        /// <summary>
        /// Gets the courses.
        /// </summary>
        public IList<Course> Courses { get; private set; }

        public CourseDb()
        {
            this.Courses = LoadCourses();
        }

        private IList<Course> LoadCourses()
        {
            var courses = new List<Course>();

            using (var fs = new StreamReader(Path.Combine(DATA_DIR, MASTER_FILE)))
            {
                while (!fs.EndOfStream)
                {
                    var courseName = fs.ReadLine();
                    courses.Add(LoadCourse(courseName,
                        Path.Combine(DATA_DIR, courseName + ".txt")
                        ));
                }
            }

            return courses;
        }

        /// <summary>
        /// Loads the course.
        /// </summary>
        /// <param name="courseName">Name of the course.</param>
        /// <param name="fileName">Path to a file containing the data of the course.</param>
        /// <returns>The loaded course</returns>
        private Course LoadCourse(string courseName, string fileName)
        {
            var c = new Course();
            c.Name = courseName;

            using (var fs = new StreamReader(fileName))
            {
                fs.ReadLine();  //skip header

                while (!fs.EndOfStream)
                {
                    var line = fs.ReadLine();

                    CourseDataItem dt = new CourseDataItem();
                    String[] parts = line.Split("|");
                    dt.Label = parts[0];
                    dt.Students = Convert.ToInt32(parts[1].Trim());
                    dt.CourseCredit = Convert.ToInt32(parts[6].Trim());
                    dt.Grade1 = Convert.ToInt32(parts[2].Trim());
                    dt.Grade2 = Convert.ToInt32(parts[3].Trim());
                    dt.Grade3 = Convert.ToInt32(parts[4].Trim());
                    dt.Grade4 = Convert.ToInt32(parts[5].Trim());
                    c.Statistics.Add(dt);
                }            
            }

            return c;
        }
    }
}

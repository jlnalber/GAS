using System.Linq;
using System.Windows.Controls;
using Utils;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für ShowCourse.xaml
    /// </summary>
    public partial class ShowCourse : UserControl
    {
        public ShowCourse()
        {
            InitializeComponent();
        }

        public void LoadCourse(Schedule schedule, Course course)
        {
            //Stelle die ID dar:
            this.ID.OutputText = course.ID;

            //Stelle die Anzahl der Stunden dar:
            this.AmountPeriods.OutputText = course.Periods.Length.ToString();

            //Stelle den LuL dar:
            this.LuL.OutputText = course.Teacher.Name == "" ? course.Teacher.ID : course.Teacher.Name;

            //Stelle die Stunden dar:
            this.Periods.Items.Clear();
            foreach (Period period in course.Periods)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = period.ToString();
                listBoxItem.Height = 25;
                listBoxItem.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                this.Periods.Items.Add(listBoxItem);
            }

            //Stelle die SuS dar:
            this.SuS.Items.Clear();
            foreach (Student student in course.Students)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = student.Name == "" ? student.ID : student.Name;
                listBoxItem.Height = 25;
                listBoxItem.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                this.SuS.Items.Add(listBoxItem);
            }

            //Stelle die Partnerkurse dar:
            this.PartnerCourses.Items.Clear();
            foreach (Course course2 in course.PartnerCourses)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = course2.ID;
                listBoxItem.Height = 25;
                listBoxItem.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                this.PartnerCourses.Items.Add(listBoxItem);
            }
            if (this.PartnerCourses.Items.Count == 0)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = "Keine Partnerkurse!";
                listBoxItem.Height = 25;
                listBoxItem.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                this.PartnerCourses.Items.Add(listBoxItem);
            }

            //Stelle die parallelen Kurse dar:
            this.ParallelCourses.Items.Clear();
            foreach (Course course2 in (from c in schedule.Courses where (from p in c.Periods where course.Periods.Contains(p2 => p2 == p) select p).ToArray().Length != 0 select c).ToArray())
            {
                if (course2 != course)
                {
                    ListBoxItem listBoxItem = new();
                    listBoxItem.Content = course2.ID;
                    listBoxItem.Height = 25;
                    listBoxItem.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                    this.ParallelCourses.Items.Add(listBoxItem);
                }
            }
            if (this.ParallelCourses.Items.Count == 0)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = "Keine parallele Kurse!";
                listBoxItem.Height = 25;
                listBoxItem.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                this.ParallelCourses.Items.Add(listBoxItem);
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für Schedule_Window.xaml
    /// </summary>
    public partial class Schedule_Window : Window
    {
        (Person, ComboBoxItem)[] PeopleC;
        (Course, ComboBoxItem)[] CoursesC;
        Schedule Schedule;

        public Schedule_Window(Schedule schedule)
        {
            InitializeComponent();

            this.Schedule = schedule;

            HashSet<Person> people = new();
            people.Add(new Person(schedule.Courses, "Alle Kurse"));

            foreach (Course i in schedule.Courses)
            {
                if (!i.Teacher.HideTeacher)
                {
                    people.Add(i.Teacher);
                }
            }
            foreach (Course i in schedule.Courses)
            {
                foreach (Student j in i.Students)
                {
                    people.Add(j);
                }
            }

            this.PeopleC = new (Person, ComboBoxItem)[people.Count];

            int counter = 0;
            foreach (Person i in people)
            {
                ComboBoxItem comboBoxItem = new();
                comboBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                this.PersonPicker.Items.Add(comboBoxItem);
                this.PeopleC[counter] = (i, comboBoxItem);
                counter++;
            }

            List<(Course, ComboBoxItem)> coursesC = new();
            for (int i = 0; i < this.Schedule.Courses.Length; i++)
            {
                if (!this.Schedule.Courses[i].HideCourse)
                {
                    ComboBoxItem comboBoxItem = new();
                    comboBoxItem.Content = this.Schedule.Courses[i].ID;
                    this.CoursePicker.Items.Add(comboBoxItem);
                    coursesC.Add((this.Schedule.Courses[i], comboBoxItem));
                }
            }
            this.CoursesC = coursesC.ToArray();

            this.PersonPicker.SelectedIndex = 0;
            this.CoursePicker.SelectedIndex = 0;
        }

        private void PersonPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Person person = (from t in PeopleC where t.Item2 == this.PersonPicker.SelectedItem select t.Item1).First();

            this.GridSchedules.Children.Remove(this.Timetable);
            this.Timetable = new Timetabel();
            this.Timetable.Margin = new Thickness(20, 80, 20, 20);
            this.GridSchedules.Children.Add(this.Timetable);
            this.Timetable.DisplaySchedule(person);
        }

        private void CoursePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Course course = (from t in CoursesC where t.Item2 == this.CoursePicker.SelectedItem select t.Item1).First();

            this.Course.LoadCourse(this.Schedule, course);
        }
    }
}

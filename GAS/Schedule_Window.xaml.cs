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

        public Schedule_Window(Schedule schedule)
        {
            InitializeComponent();

            HashSet<Person> people = new();
            people.Add(new Person(schedule.Courses, "Alle Kurse"));

            foreach (Course i in schedule.Courses)
            {
                people.Add(i.Teacher);
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

            this.PersonPicker.SelectedIndex = 0;
        }

        private void PersonPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Person person = (from t in PeopleC where t.Item2 == this.PersonPicker.SelectedItem select t.Item1).First();

            this.Grid.Children.Remove(this.Timetable);
            this.Timetable = new Timetabel();
            this.Timetable.Margin = new Thickness(20, 80, 20, 20);
            this.Grid.Children.Add(this.Timetable);
            this.Timetable.DisplaySchedule(person);
        }
    }
}

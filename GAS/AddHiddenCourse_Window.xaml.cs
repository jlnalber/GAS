using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utils;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für AddHiddenCourse_Window.xaml
    /// </summary>
    public partial class AddHiddenCourse_Window : Window
    {
        private MainWindow MainWindow;
        private List<(Person, ListBoxItem)> PeopleC;
        private List<(Period, ListBoxItem)> PeriodsC;
        private ListBoxItem LastListBoxItemPerson;
        private ListBoxItem LastListBoxItemPeriod;

        public AddHiddenCourse_Window(MainWindow mainWindow)
        {
            InitializeComponent();

            this.MainWindow = mainWindow;

            this.PeopleC = new();
            this.PeriodsC = new();

            foreach (Teacher i in this.MainWindow.GetTeachers())
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                listBoxItem.MouseDoubleClick += ListBoxItemPerson_MouseDoubleClick;
                listBoxItem.LostFocus += ListBoxItemPerson_LostFocus;
                this.PeopleC.Add((i, listBoxItem));
                this.NotSelectedPeople.Items.Add(listBoxItem);
            }

            foreach (Student i in this.MainWindow.GetStudents())
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                listBoxItem.MouseDoubleClick += ListBoxItemPerson_MouseDoubleClick;
                listBoxItem.LostFocus += ListBoxItemPerson_LostFocus;
                this.PeopleC.Add((i, listBoxItem));
                this.NotSelectedPeople.Items.Add(listBoxItem);
            }
        }

        private void ListBoxItemPerson_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItemPerson = sender as ListBoxItem;
        }

        private void ListBoxItemPerson_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;

            if (this.SelectedPeople.Items.Contains(listBoxItem))
            {
                this.SelectedPeople.Items.Remove(listBoxItem);
                this.NotSelectedPeople.Items.Add(listBoxItem);
            }
            else
            {
                this.NotSelectedPeople.Items.Remove(listBoxItem);
                this.SelectedPeople.Items.Add(listBoxItem);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (this.ID.InputText.Replace(" ", "") == "")
            {
                SystemSounds.Asterisk.Play();
                this.ID.SetLabelColor(Brushes.Red);
            }
            else
            {
                try
                {
                    Student[] students = (from j in this.PeopleC where j.Item1 is Student && this.SelectedPeople.Items.Contains(j.Item2) select j.Item1).ToArray().Transform(p => p as Student);

                    Teacher[] teachers = (from j in this.PeopleC where j.Item1 is Teacher && this.SelectedPeople.Items.Contains(j.Item2) select j.Item1).ToArray().Transform(p => p as Teacher);

                    string ID = this.ID.GetValueString();

                    if (teachers.Length == 0)
                    {
                        teachers = new Teacher[] { new Teacher(new Course[0], ID + "Teacher_PlaceHolder" + new Random().Next(int.MaxValue)) { HideTeacher = true } };
                    }

                    Period[] periods = (from i in this.PeriodsC where this.PeriodsSelection.Items.Contains(i.Item2) select i.Item1).ToArray();

                    GroupCourse groupCourse = new(periods, students, teachers, ID);
                    groupCourse.FixPeriods = true;
                    groupCourse.FixParticipants = true;
                    groupCourse.HideCourse = true;

                    this.MainWindow.AddCourse(groupCourse);

                    this.Close();
                }
                catch (FormatException) { }
            }
        }

        private void SwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.LastListBoxItemPerson != null)
            {
                if (this.SelectedPeople.Items.Contains(this.LastListBoxItemPerson))
                {
                    this.SelectedPeople.Items.Remove(this.LastListBoxItemPerson);
                    this.NotSelectedPeople.Items.Add(this.LastListBoxItemPerson);
                }
                else
                {
                    this.NotSelectedPeople.Items.Remove(this.LastListBoxItemPerson);
                    this.SelectedPeople.Items.Add(this.LastListBoxItemPerson);
                }
            }
        }

        private void AddPeriod_Click(object sender, RoutedEventArgs e)
        {
            AddPeriod_Window addPeriod_Window = new AddPeriod_Window();
            bool? dialog = addPeriod_Window.ShowDialog();
            if (dialog == true && (from i in this.PeriodsC where i.Item1 == addPeriod_Window.Period select i).Count() == 0)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = addPeriod_Window.Period.ToString();
                listBoxItem.LostFocus += ListBoxItemPeriod_LostFocus;
                this.PeriodsSelection.Items.Add(listBoxItem);
                this.PeriodsC.Add((addPeriod_Window.Period, listBoxItem));
            }
            else if (dialog == true)
            {
                SystemSounds.Asterisk.Play();
            }
        }

        private void ListBoxItemPeriod_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItemPeriod = sender as ListBoxItem;
        }

        private void RemovePeriod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.PeriodsSelection.Items.Remove(this.LastListBoxItemPeriod);
                this.PeriodsC.Remove((from i in this.PeriodsC where i.Item2 == this.LastListBoxItemPeriod select i).First());
            }
            catch { }
        }

        private void AddAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ListBoxItem> items = new();
                foreach (ListBoxItem i in this.NotSelectedPeople.Items)
                {
                    items.Add(i);
                }
                foreach (ListBoxItem i in items)
                {
                    this.NotSelectedPeople.Items.Remove(i);
                    this.SelectedPeople.Items.Add(i);
                }
            }
            catch { }
        }

        private void RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ListBoxItem> items = new();
                foreach (ListBoxItem i in this.SelectedPeople.Items)
                {
                    items.Add(i);
                }
                foreach (ListBoxItem i in items)
                {
                    this.SelectedPeople.Items.Remove(i);
                    this.NotSelectedPeople.Items.Add(i);
                }
            }
            catch { }
        }
    }
}

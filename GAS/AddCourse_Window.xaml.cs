using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für AddCourse_Window.xaml
    /// </summary>
    public partial class AddCourse_Window : Window
    {
        private MainWindow MainWindow;
        private List<(Teacher, ListBoxItem)> TeachersC;
        private List<(Student, ListBoxItem)> StudentsC;
        private List<(Period, ListBoxItem)> PeriodsC;
        private ListBoxItem LastListBoxItemT;
        private ListBoxItem LastListBoxItemS;
        private ListBoxItem LastListBoxItemP;

        public AddCourse_Window(MainWindow mainWindow)
        {
            InitializeComponent();

            this.MainWindow = mainWindow;

            this.TeachersC = new();
            this.StudentsC = new();
            this.PeriodsC = new();

            foreach (Teacher i in this.MainWindow.GetTeachers())
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                listBoxItem.MouseDoubleClick += ListBoxItemT_MouseDoubleClick;
                listBoxItem.LostFocus += ListBoxItemT_LostFocus;
                this.TeachersC.Add((i, listBoxItem));
                this.NotSelectedTeachers.Items.Add(listBoxItem);
            }

            foreach (Student i in this.MainWindow.GetStudents())
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                listBoxItem.MouseDoubleClick += ListBoxItemS_MouseDoubleClick;
                listBoxItem.LostFocus += ListBoxItemS_LostFocus;
                this.StudentsC.Add((i, listBoxItem));
                this.NotSelectedStudents.Items.Add(listBoxItem);
            }
        }

        private void ListBoxItemT_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItemT = sender as ListBoxItem;
        }

        private void ListBoxItemT_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;

            if (this.SelectedTeachers.Items.Contains(listBoxItem))
            {
                this.SelectedTeachers.Items.Remove(listBoxItem);
                this.NotSelectedTeachers.Items.Add(listBoxItem);
            }
            else
            {
                this.NotSelectedTeachers.Items.Remove(listBoxItem);
                this.SelectedTeachers.Items.Add(listBoxItem);
            }
        }

        private void ListBoxItemS_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItemS = sender as ListBoxItem;
        }

        private void ListBoxItemS_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;

            if (this.SelectedStudents.Items.Contains(listBoxItem))
            {
                this.SelectedStudents.Items.Remove(listBoxItem);
                this.NotSelectedStudents.Items.Add(listBoxItem);
            }
            else
            {
                this.NotSelectedStudents.Items.Remove(listBoxItem);
                this.SelectedStudents.Items.Add(listBoxItem);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (this.ID.InputText.Replace(" ", "") == "")
            {
                SystemSounds.Asterisk.Play();
                this.ID.Label.Foreground = Brushes.Red;
            }
            else
            {
                try
                {
                    Student[] students = (from j in this.StudentsC where this.SelectedStudents.Items.Contains(j.Item2) select j.Item1).ToArray();

                    if (this.SelectedTeachers.Items.Count == 0) throw new InvalidOperationException();
                    Teacher[] teachers = (from j in this.TeachersC where this.SelectedTeachers.Items.Contains(j.Item2) select j.Item1).ToArray();

                    string ID = this.ID.GetValueString();

                    if (this.FixPeriods.IsChecked == true)
                    {
                        Period[] periods = (from i in this.PeriodsC where this.PeriodsSelection.Items.Contains(i.Item2) select i.Item1).ToArray();

                        GroupCourse groupCourse = new(periods, students, teachers, ID);
                        groupCourse.FixPeriods = true;

                        this.MainWindow.AddCourse(groupCourse);
                    }
                    else
                    {
                        int periods = this.Periods.GetValueInt();

                        this.MainWindow.AddCourse(new GroupCourse(periods, students, teachers, ID));
                    }
                    this.Close();
                }
                catch (InvalidOperationException)
                {
                    SystemSounds.Asterisk.Play();
                    this.TeachersLabel.Foreground = Brushes.Red;
                }
                catch (FormatException) { }
            }
        }

        private void SwitchButtonT_Click(object sender, RoutedEventArgs e)
        {
            if (this.LastListBoxItemT != null)
            {
                if (this.SelectedTeachers.Items.Contains(this.LastListBoxItemT))
                {
                    this.SelectedTeachers.Items.Remove(this.LastListBoxItemT);
                    this.NotSelectedTeachers.Items.Add(this.LastListBoxItemT);
                }
                else
                {
                    this.NotSelectedTeachers.Items.Remove(this.LastListBoxItemT);
                    this.SelectedTeachers.Items.Add(this.LastListBoxItemT);
                }
            }
        }

        private void SwitchButtonS_Click(object sender, RoutedEventArgs e)
        {
            if (this.LastListBoxItemS != null)
            {
                if (this.SelectedStudents.Items.Contains(this.LastListBoxItemS))
                {
                    this.SelectedStudents.Items.Remove(this.LastListBoxItemS);
                    this.NotSelectedStudents.Items.Add(this.LastListBoxItemS);
                }
                else
                {
                    this.NotSelectedStudents.Items.Remove(this.LastListBoxItemS);
                    this.SelectedStudents.Items.Add(this.LastListBoxItemS);
                }
            }
        }

        private void FixPeriods_Checked(object sender, RoutedEventArgs e)
        {
            this.Periods.IsEnabled = false;
            this.PeriodsSelection.IsEnabled = true;
            this.AddPeriod.IsEnabled = true;
            this.RemovePeriod.IsEnabled = true;
        }

        private void FixPeriods_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Periods.IsEnabled = true;
            this.PeriodsSelection.IsEnabled = false;
            this.AddPeriod.IsEnabled = false;
            this.RemovePeriod.IsEnabled = false;
        }

        private void AddPeriod_Click(object sender, RoutedEventArgs e)
        {
            AddPeriod_Window addPeriod_Window = new AddPeriod_Window();
            if (addPeriod_Window.ShowDialog() == true && (from i in this.PeriodsC where i.Item1 == addPeriod_Window.Period select i).Count() == 0)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = addPeriod_Window.Period.ToString();
                listBoxItem.LostFocus += ListBoxItemP_LostFocus;
                this.PeriodsSelection.Items.Add(listBoxItem);
                this.PeriodsC.Add((addPeriod_Window.Period, listBoxItem));
            }
            else
            {
                SystemSounds.Asterisk.Play();
            }
        }

        private void ListBoxItemP_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItemP = sender as ListBoxItem;
        }

        private void RemovePeriod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.PeriodsSelection.Items.Remove(this.LastListBoxItemP);
                this.PeriodsC.Remove((from i in this.PeriodsC where i.Item2 == this.LastListBoxItemP select i).First());
            }
            catch { }
        }
    }
}

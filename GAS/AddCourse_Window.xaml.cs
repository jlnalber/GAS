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
        private List<(Teacher, ComboBoxItem)> TeachersC;
        private List<(Student, ListBoxItem)> StudentsC;
        private ListBoxItem LastListBoxItem;

        public AddCourse_Window(MainWindow mainWindow)
        {
            InitializeComponent();

            this.MainWindow = mainWindow;

            this.TeachersC = new();
            this.StudentsC = new();

            foreach (Teacher i in this.MainWindow.GetTeachers())
            {
                ComboBoxItem comboBoxItem = new();
                comboBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                this.TeachersC.Add((i, comboBoxItem));
                this.Teacher.Items.Add(comboBoxItem);
            }
            if (this.TeachersC.Count != 0)
            {
                this.Teacher.SelectedIndex = 0;
            }

            foreach (Student i in this.MainWindow.GetStudents())
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                listBoxItem.MouseDoubleClick += ListBoxItem_MouseDoubleClick;
                listBoxItem.LostFocus += ListBoxItem_LostFocus;
                this.StudentsC.Add((i, listBoxItem));
                this.NotSelectedStudents.Items.Add(listBoxItem);
            }
        }

        private void ListBoxItem_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItem = sender as ListBoxItem;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
            if (this.ID.Text.Replace(" ", "") == "")
            {
                SystemSounds.Asterisk.Play();
                this.IDLabel.Foreground = Brushes.Red;
            }
            else
            {
                try
                {
                    int periods = int.Parse(this.Periods.Text);

                    Student[] students = new Student[this.SelectedStudents.Items.Count];
                    for (int i = 0; i < this.SelectedStudents.Items.Count; i++)
                    {
                        students[i] = (from j in this.StudentsC where j.Item2 == this.SelectedStudents.Items[i] select j.Item1).First();
                    }

                    Teacher teacher = (from i in this.TeachersC where i.Item2 == this.Teacher.SelectedItem select i.Item1).First();

                    string ID = this.ID.Text;

                    this.MainWindow.AddCourse(new Course(periods, students, teacher, ID));
                    this.Close();
                }
                catch (InvalidOperationException)
                {
                    SystemSounds.Asterisk.Play();
                    this.TeacherLabel.Foreground = Brushes.Red;
                }
                catch (FormatException)
                {
                    SystemSounds.Asterisk.Play();
                    this.PeriodsLabel.Foreground = Brushes.Red;
                }
                catch (Schedule.InvalidIDException)
                {
                    SystemSounds.Asterisk.Play();
                    this.IDLabel.Foreground = Brushes.Red;
                }
            }
        }

        private void SwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.LastListBoxItem != null)
            {
                if (this.SelectedStudents.Items.Contains(this.LastListBoxItem))
                {
                    this.SelectedStudents.Items.Remove(this.LastListBoxItem);
                    this.NotSelectedStudents.Items.Add(this.LastListBoxItem);
                }
                else
                {
                    this.NotSelectedStudents.Items.Remove(this.LastListBoxItem);
                    this.SelectedStudents.Items.Add(this.LastListBoxItem);
                }
            }
        }
    }
}

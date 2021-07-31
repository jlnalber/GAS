﻿using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für AddTeacher_Window.xaml
    /// </summary>
    public partial class AddTeacher_Window : Window
    {
        private List<(Course, ListBoxItem)> CoursesC;
        MainWindow MainWindow;
        private ListBoxItem LastListBoxItem;

        public AddTeacher_Window(MainWindow mainWindow)
        {
            InitializeComponent();

            this.MainWindow = mainWindow;

            this.CoursesC = new();

            foreach (Course i in this.MainWindow.GetCourses())
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = i.ID;
                listBoxItem.MouseDoubleClick += ListBoxItem_MouseDoubleClick;
                listBoxItem.LostFocus += ListBoxItem_LostFocus;
                this.CoursesC.Add((i, listBoxItem));
                this.NotSelectedCourses.Items.Add(listBoxItem);
            }
        }

        private void ListBoxItem_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItem = sender as ListBoxItem;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;

            if (this.SelectedCourses.Items.Contains(listBoxItem))
            {
                this.SelectedCourses.Items.Remove(listBoxItem);
                this.NotSelectedCourses.Items.Add(listBoxItem);
            }
            else
            {
                this.NotSelectedCourses.Items.Remove(listBoxItem);
                this.SelectedCourses.Items.Add(listBoxItem);
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
                Course[] courses = new Course[this.SelectedCourses.Items.Count];
                for (int i = 0; i < this.SelectedCourses.Items.Count; i++)
                {
                    courses[i] = (from j in this.CoursesC where j.Item2 == this.SelectedCourses.Items[i] select j.Item1).First();
                }

                string ID = this.ID.GetValueString();

                string name = this.NameT.GetValueString();

                Teacher teacher = new(courses, ID);
                teacher.Name = name;

                this.MainWindow.AddTeacher(teacher);
                this.Close();
            }
        }

        private void SwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.LastListBoxItem != null)
            {
                if (this.SelectedCourses.Items.Contains(this.LastListBoxItem))
                {
                    this.SelectedCourses.Items.Remove(this.LastListBoxItem);
                    this.NotSelectedCourses.Items.Add(this.LastListBoxItem);
                }
                else
                {
                    this.NotSelectedCourses.Items.Remove(this.LastListBoxItem);
                    this.SelectedCourses.Items.Add(this.LastListBoxItem);
                }
            }
        }
    }
}

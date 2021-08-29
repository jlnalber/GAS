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
    public partial class MainWindow
    {
        //Methoden für das Bearbeiten der Kurse:
        #region
        private List<(Teacher, ListBoxItem)> TeachersC_Course = new();
        private List<(Student, ListBoxItem)> StudentsC_Course = new();
        private List<(Period, ListBoxItem)> PeriodsC_Course = new();
        private ListBoxItem LastListBoxItemT_Course;
        private ListBoxItem LastListBoxItemS_Course;
        private ListBoxItem LastListBoxItemP_Course;

        private void Course_Picker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                GroupCourse course = (from i in this.CoursesC where i.Item2 == this.Course_Picker.SelectedItem select i.Item1).First();
                this.DisplayCourse(course);
            }
            catch { }
        }

        private void SwitchButton1T_Click(object sender, RoutedEventArgs e)
        {
            if (this.LastListBoxItemT_Course != null)
            {
                if (this.SelectedTeachers_Course.Items.Contains(this.LastListBoxItemT_Course))
                {
                    this.SelectedTeachers_Course.Items.Remove(this.LastListBoxItemT_Course);
                    this.NotSelectedTeachers_Course.Items.Add(this.LastListBoxItemT_Course);
                }
                else
                {
                    this.NotSelectedTeachers_Course.Items.Remove(this.LastListBoxItemT_Course);
                    this.SelectedTeachers_Course.Items.Add(this.LastListBoxItemT_Course);
                }
            }
        }

        private void SwitchButton1S_Click(object sender, RoutedEventArgs e)
        {
            if (this.LastListBoxItemS_Course != null)
            {
                if (this.SelectedStudents_Course.Items.Contains(this.LastListBoxItemS_Course))
                {
                    this.SelectedStudents_Course.Items.Remove(this.LastListBoxItemS_Course);
                    this.NotSelectedStudents_Course.Items.Add(this.LastListBoxItemS_Course);
                }
                else
                {
                    this.NotSelectedStudents_Course.Items.Remove(this.LastListBoxItemS_Course);
                    this.SelectedStudents_Course.Items.Add(this.LastListBoxItemS_Course);
                }
            }
        }

        private void SaveCourse_Click(object sender, RoutedEventArgs e)
        {
            this.ResetColors_Course();
            try
            {
                GroupCourse course = (from i in this.CoursesC where i.Item2 == this.Course_Picker.SelectedItem select i.Item1).First();

                //Aktualisiere die ID.
                course.ID = this.ID_Course.GetValueString();

                //Aktualisiere die Stunden.
                if (this.FixPeriods_Course.IsChecked == true)
                {
                    course.FixPeriods = true;
                    course.Periods = (from i in this.PeriodsC_Course where this.PeriodsSelection_Course.Items.Contains(i.Item2) select i.Item1).ToArray();
                }
                else
                {
                    course.FixPeriods = false;
                    course.Periods = new Period[this.Periods_Course.GetValueInt()];
                }

                //Aktualisiere die LuL.
                if (this.SelectedTeachers_Course.Items.Count == 0) throw new InvalidOperationException();
                foreach (Teacher i in from s in TeachersC_Course where this.SelectedTeachers_Course.Items.Contains(s.Item2) && !course.Teachers.Contains(s.Item1) select s.Item1)
                {
                    course.AddTeacher(i);
                }
                foreach (Teacher i in from s in TeachersC_Course where this.NotSelectedTeachers_Course.Items.Contains(s.Item2) && course.Teachers.Contains(s.Item1) select s.Item1)
                {
                    course.RemoveTeacher(i);
                }

                //Aktualisiere die SuS.
                foreach (Student i in from s in StudentsC_Course where this.SelectedStudents_Course.Items.Contains(s.Item2) && !course.Students.Contains(s.Item1) select s.Item1)
                {
                    i.AddToCourse(course);
                }
                foreach (Student i in from s in StudentsC_Course where this.NotSelectedStudents_Course.Items.Contains(s.Item2) && course.Students.Contains(s.Item1) select s.Item1)
                {
                    i.RemoveFromCourse(course);
                }

                //Aktualisiere die anderen Tabs.
                this.RefreshTeachers();
                this.RefreshStudents();
                this.RefreshHiddenCourses();
            }
            catch (FormatException) { }
            catch (InvalidOperationException)
            {
                SystemSounds.Asterisk.Play();
                this.TeachersLabel_Course.Foreground = Brushes.Red;
            }
        }

        private void ResetColors_Course()
        {
            this.Periods_Course.SetLabelColor(Brushes.Black);
            this.TeachersLabel_Course.Foreground = Brushes.Black;
        }

        private void DisplayCourse(GroupCourse course)
        {
            this.ResetColors_Course();

            //Lade die InputFields neu:
            this.ID_Course.InputText = course.ID;

            this.Periods_Course.InputText = course.Periods.Length.ToString();
            this.FixPeriods_Course.IsChecked = course.FixPeriods;
            this.PeriodsC_Course.Clear();
            this.PeriodsSelection_Course.Items.Clear();
            if (course.FixPeriods)
            {
                foreach (Period i in course.Periods)
                {
                    ListBoxItem listBoxItem = new();
                    listBoxItem.Content = i.ToString();
                    listBoxItem.LostFocus += ListBoxItemP_Course_LostFocus;
                    this.PeriodsSelection_Course.Items.Add(listBoxItem);
                    this.PeriodsC_Course.Add((i, listBoxItem));
                }
            }

            //Stelle die LuL dar:
            this.TeachersC_Course.Clear();
            this.SelectedTeachers_Course.Items.Clear();
            this.NotSelectedTeachers_Course.Items.Clear();
            foreach (Teacher i in this.Teachers)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                listBoxItem.MouseDoubleClick += ListBoxItemT_MouseDoubleClick;
                listBoxItem.LostFocus += ListBoxItemT_LostFocus;
                this.TeachersC_Course.Add((i, listBoxItem));

                if (course.Teachers.Contains(i))
                {
                    this.SelectedTeachers_Course.Items.Add(listBoxItem);
                }
                else
                {
                    this.NotSelectedTeachers_Course.Items.Add(listBoxItem);
                }
            }

            //Stelle die SuS dar:
            this.StudentsC_Course.Clear();
            this.SelectedStudents_Course.Items.Clear();
            this.NotSelectedStudents_Course.Items.Clear();
            foreach (Student i in this.Students)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                listBoxItem.MouseDoubleClick += ListBoxItemS_Course_MouseDoubleClick;
                listBoxItem.LostFocus += ListBoxItemS_Course_LostFocus;
                this.StudentsC_Course.Add((i, listBoxItem));

                if (course.Students.Contains(i))
                {
                    this.SelectedStudents_Course.Items.Add(listBoxItem);
                }
                else
                {
                    this.NotSelectedStudents_Course.Items.Add(listBoxItem);
                }
            }
        }

        private void ListBoxItemT_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItemT_Course = sender as ListBoxItem;
        }

        private void ListBoxItemT_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;

            if (this.SelectedTeachers_Course.Items.Contains(listBoxItem))
            {
                this.SelectedTeachers_Course.Items.Remove(listBoxItem);
                this.NotSelectedTeachers_Course.Items.Add(listBoxItem);
            }
            else
            {
                this.NotSelectedTeachers_Course.Items.Remove(listBoxItem);
                this.SelectedTeachers_Course.Items.Add(listBoxItem);
            }
        }

        private void ListBoxItemS_Course_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItemS_Course = sender as ListBoxItem;
        }

        private void ListBoxItemS_Course_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;

            if (this.SelectedStudents_Course.Items.Contains(listBoxItem))
            {
                this.SelectedStudents_Course.Items.Remove(listBoxItem);
                this.NotSelectedStudents_Course.Items.Add(listBoxItem);
            }
            else
            {
                this.NotSelectedStudents_Course.Items.Remove(listBoxItem);
                this.SelectedStudents_Course.Items.Add(listBoxItem);
            }
        }

        private void DeleteCourse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Suche den Kurs:
                GroupCourse course = (from i in this.CoursesC where i.Item2 == this.Course_Picker.SelectedItem select i.Item1).First();

                //Entferne den Kurs aus den SuS:
                foreach (Student i in course.Students)
                {
                    i.Courses = i.Courses.RemoveFromArray(course);
                }

                //Entferne den Kurs aus den LuL:
                foreach (Teacher i in course.Teachers)
                {
                    i.Courses = i.Courses.RemoveFromArray(course);
                }

                //Entferne den Kurs:
                this.Courses.Remove(course);

                //Aktualisieren
                this.RefreshAll();
            }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
        }

        private void FixPeriods_Course_Checked(object sender, RoutedEventArgs e)
        {
            this.Periods_Course.IsEnabled = false;
            this.PeriodsSelection_Course.IsEnabled = true;
            this.AddPeriod_Course.IsEnabled = true;
            this.RemovePeriod_Course.IsEnabled = true;
        }

        private void FixPeriods_Course_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Periods_Course.IsEnabled = true;
            this.PeriodsSelection_Course.IsEnabled = false;
            this.AddPeriod_Course.IsEnabled = false;
            this.RemovePeriod_Course.IsEnabled = false;
        }

        private void AddPeriod_Course_Click(object sender, RoutedEventArgs e)
        {
            if (this.Course_Picker.Items.Count != 0)
            {
                AddPeriod_Window addPeriod_Window = new AddPeriod_Window();
                bool? dialog = addPeriod_Window.ShowDialog();
                if (dialog == true && (from i in this.PeriodsC_Course where i.Item1 == addPeriod_Window.Period select i).Count() == 0)
                {
                    ListBoxItem listBoxItem = new();
                    listBoxItem.Content = addPeriod_Window.Period.ToString();
                    listBoxItem.LostFocus += ListBoxItemP_Course_LostFocus;
                    this.PeriodsSelection_Course.Items.Add(listBoxItem);
                    this.PeriodsC_Course.Add((addPeriod_Window.Period, listBoxItem));
                }
                else if (dialog == true)
                {
                    SystemSounds.Asterisk.Play();
                }
            }
        }

        private void ListBoxItemP_Course_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItemP_Course = sender as ListBoxItem;
        }

        private void RemovePeriod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.PeriodsSelection_Course.Items.Remove(this.LastListBoxItemP_Course);
                this.PeriodsC_Course.Remove((from i in this.PeriodsC_Course where i.Item2 == this.LastListBoxItemP_Course select i).First());
            }
            catch { }
        }
        #endregion

        //Methoden für das Bearbeiten der LuL:
        #region
        private void Teacher_Picker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Teacher teacher = (from i in this.TeachersC where i.Item2 == this.Teacher_Picker.SelectedItem select i.Item1).First();
                this.DisplayTeacher(teacher);
            }
            catch { }
        }

        private void SaveTeacher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Teacher teacher = (from i in this.TeachersC where i.Item2 == this.Teacher_Picker.SelectedItem select i.Item1).First();

                //Aktualisiere die ID.
                teacher.ID = this.ID_Teacher.GetValueString();

                //Aktualisiere den Namen.
                teacher.Name = this.Name_Teacher.GetValueString();

                //Aktualisiere die anderen Tabs.
                this.RefreshCourses();
                this.RefreshStudents();
                this.RefreshHiddenCourses();
            }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
        }

        private void DisplayTeacher(Teacher teacher)
        {
            this.ID_Teacher.InputText = teacher.ID;

            this.Name_Teacher.InputText = teacher.Name;

            this.Courses_Teacher.Items.Clear();
            foreach (GroupCourse i in teacher.Courses)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = i.ID;
                this.Courses_Teacher.Items.Add(listBoxItem);
            }
        }

        private void DeleteTeacher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Teachers.Count == 1)
                {
                    SystemSounds.Asterisk.Play();
                }
                else
                {
                    //Suche nach dem LuL:
                    Teacher teacher = (from i in this.TeachersC where i.Item2 == this.Teacher_Picker.SelectedItem select i.Item1).First();

                    //Ersetze den LuL mit einem anderen:
                    Teacher replaceBy = this.Teachers.ToArray().Filter((Teacher t) => t != teacher)[0];
                    foreach (GroupCourse i in teacher.Courses)
                    {
                        i.Teachers[Array.IndexOf(i.Teachers, i)] = replaceBy;
                        replaceBy.Courses = replaceBy.Courses.AddToArray(i);
                    }

                    //Entferne den LuL:
                    this.Teachers.Remove(teacher);

                    //Aktualisieren
                    this.RefreshAll();
                }
            }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
        }
        #endregion

        //Methoden für das Bearbeiten der SuS:
        #region
        private List<(Course, ListBoxItem)> CoursesC_Student = new();
        private ListBoxItem LastListBoxItem_Student;

        private void Student_Picker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Student student = (from i in this.StudentsC where i.Item2 == this.Student_Picker.SelectedItem select i.Item1).First();
                this.DisplayStudent(student);
            }
            catch { }
        }

        private void SwitchButton3_Click(object sender, RoutedEventArgs e)
        {
            if (this.LastListBoxItem_Student != null)
            {
                if (this.SelectedCourses_Student.Items.Contains(this.LastListBoxItem_Student))
                {
                    this.SelectedCourses_Student.Items.Remove(this.LastListBoxItem_Student);
                    this.NotSelectedCourses_Student.Items.Add(this.LastListBoxItem_Student);
                }
                else
                {
                    this.NotSelectedCourses_Student.Items.Remove(this.LastListBoxItem_Student);
                    this.SelectedCourses_Student.Items.Add(this.LastListBoxItem_Student);
                }
            }
        }

        private void SaveStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Student student = (from i in this.StudentsC where i.Item2 == this.Student_Picker.SelectedItem select i.Item1).First();

                //Aktualisiere die ID.
                student.ID = this.ID_Student.GetValueString();

                //Aktualisiere den Namen.
                student.Name = this.Name_Student.GetValueString();

                //Aktualisiere die Kurse.
                foreach (GroupCourse i in from c in CoursesC_Student where this.SelectedCourses_Student.Items.Contains(c.Item2) && !student.Courses.Contains(c.Item1) select c.Item1)
                {
                    i.AddStudent(student);
                }
                foreach (GroupCourse i in from c in CoursesC_Student where this.NotSelectedCourses_Student.Items.Contains(c.Item2) && student.Courses.Contains(c.Item1) select c.Item1)
                {
                    i.RemoveStudent(student);
                }

                //Aktualisiere die anderen Tabs.
                this.RefreshCourses();
                this.RefreshTeachers();
                this.RefreshHiddenCourses();
            }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
        }

        private void DisplayStudent(Student student)
        {
            //Lade die InputViews neu:
            this.ID_Student.InputText = student.ID;

            this.Name_Student.InputText = student.Name;

            //Stelle die Kurse dar:
            this.CoursesC_Student.Clear();
            this.SelectedCourses_Student.Items.Clear();
            this.NotSelectedCourses_Student.Items.Clear();
            foreach (GroupCourse i in this.Courses)
            {
                if (!i.HideCourse)
                {
                    ListBoxItem listBoxItem = new();
                    listBoxItem.Content = i.ID;
                    listBoxItem.MouseDoubleClick += ListBoxItem_Student_MouseDoubleClick;
                    listBoxItem.LostFocus += ListBoxItem_Student_LostFocus;
                    this.CoursesC_Student.Add((i, listBoxItem));

                    if (student.Courses.Contains(i))
                    {
                        this.SelectedCourses_Student.Items.Add(listBoxItem);
                    }
                    else
                    {
                        this.NotSelectedCourses_Student.Items.Add(listBoxItem);
                    }
                }
            }
        }

        private void ListBoxItem_Student_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItem_Student = sender as ListBoxItem;
        }

        private void ListBoxItem_Student_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;

            if (this.SelectedCourses_Student.Items.Contains(listBoxItem))
            {
                this.SelectedCourses_Student.Items.Remove(listBoxItem);
                this.NotSelectedCourses_Student.Items.Add(listBoxItem);
            }
            else
            {
                this.NotSelectedCourses_Student.Items.Remove(listBoxItem);
                this.SelectedCourses_Student.Items.Add(listBoxItem);
            }
        }

        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Suche den SuS:
                Student student = (from i in this.StudentsC where i.Item2 == this.Student_Picker.SelectedItem select i.Item1).First();

                //Entferne den SuS aus dem Kurs:
                foreach (GroupCourse i in student.Courses)
                {
                    i.Students = i.Students.RemoveFromArray(student);
                }

                //Entferne den SuS:
                this.Students.Remove(student);

                this.RefreshAll();
            }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
        }
        #endregion

        //Methoden für das Bearbeiten der Sperrstunden:
        #region
        private List<(Person, ListBoxItem)> PeopleC_HiddenCourse = new();
        private List<(Period, ListBoxItem)> PeriodsC_HiddenCourse = new();
        private ListBoxItem LastListBoxItemPerson_HiddenCourse;
        private ListBoxItem LastListBoxItemPeriod_HiddenCourse;

        private void HiddenCourse_Picker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                GroupCourse course = (from i in this.HiddenCoursesC where i.Item2 == this.HiddenCourse_Picker.SelectedItem select i.Item1).First();
                this.DisplayHiddenCourse(course);
            }
            catch { }
        }

        private void DisplayHiddenCourse(GroupCourse course)
        {
            this.ResetColors_HiddenCourse();

            //Lade die InputFields neu:
            this.ID_HiddenCourse.InputText = course.ID;

            this.PeriodsC_HiddenCourse.Clear();
            this.PeriodsSelection_HiddenCourse.Items.Clear();
            foreach (Period i in course.Periods)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = i.ToString();
                listBoxItem.LostFocus += ListBoxItemPeriod_HiddenCourse_LostFocus;
                this.PeriodsSelection_HiddenCourse.Items.Add(listBoxItem);
                this.PeriodsC_HiddenCourse.Add((i, listBoxItem));
            }

            //Stelle die Personen dar:
            this.PeopleC_HiddenCourse.Clear();
            this.SelectedPeople_HiddenCourse.Items.Clear();
            this.NotSelectedPeople_HiddenCourse.Items.Clear();
            foreach (Teacher i in this.Teachers)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                listBoxItem.MouseDoubleClick += ListBoxItemPerson_MouseDoubleClick;
                listBoxItem.LostFocus += ListBoxItemPerson_LostFocus;
                this.PeopleC_HiddenCourse.Add((i, listBoxItem));

                if (course.Teachers.Contains(i))
                {
                    this.SelectedPeople_HiddenCourse.Items.Add(listBoxItem);
                }
                else
                {
                    this.NotSelectedPeople_HiddenCourse.Items.Add(listBoxItem);
                }
            }
            foreach (Student i in this.Students)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                listBoxItem.MouseDoubleClick += ListBoxItemPerson_MouseDoubleClick;
                listBoxItem.LostFocus += ListBoxItemPerson_LostFocus;
                this.PeopleC_HiddenCourse.Add((i, listBoxItem));

                if (course.Students.Contains(i))
                {
                    this.SelectedPeople_HiddenCourse.Items.Add(listBoxItem);
                }
                else
                {
                    this.NotSelectedPeople_HiddenCourse.Items.Add(listBoxItem);
                }
            }
        }

        private void ResetColors_HiddenCourse()
        {
            this.PeopleLabel_HiddenCourse.Foreground = Brushes.Black;
        }

        private void ListBoxItemPeriod_HiddenCourse_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItemPeriod_HiddenCourse = sender as ListBoxItem;
        }

        private void ListBoxItemPerson_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;

            if (this.SelectedPeople_HiddenCourse.Items.Contains(listBoxItem))
            {
                this.SelectedPeople_HiddenCourse.Items.Remove(listBoxItem);
                this.NotSelectedPeople_HiddenCourse.Items.Add(listBoxItem);
            }
            else
            {
                this.NotSelectedPeople_HiddenCourse.Items.Remove(listBoxItem);
                this.SelectedPeople_HiddenCourse.Items.Add(listBoxItem);
            }
        }

        private void ListBoxItemPerson_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItemPerson_HiddenCourse = sender as ListBoxItem;
        }

        private void SwitchButtonP_Click(object sender, RoutedEventArgs e)
        {
            if (this.LastListBoxItemPerson_HiddenCourse != null)
            {
                if (this.SelectedPeople_HiddenCourse.Items.Contains(this.LastListBoxItemPerson_HiddenCourse))
                {
                    this.SelectedPeople_HiddenCourse.Items.Remove(this.LastListBoxItemPerson_HiddenCourse);
                    this.NotSelectedPeople_HiddenCourse.Items.Add(this.LastListBoxItemPerson_HiddenCourse);
                }
                else
                {
                    this.NotSelectedPeople_HiddenCourse.Items.Remove(this.LastListBoxItemPerson_HiddenCourse);
                    this.SelectedPeople_HiddenCourse.Items.Add(this.LastListBoxItemPerson_HiddenCourse);
                }
            }
        }

        private void AddPeriod_HiddenCourse_Click(object sender, RoutedEventArgs e)
        {
            if (this.HiddenCourse_Picker.Items.Count != 0)
            {
                AddPeriod_Window addPeriod_Window = new AddPeriod_Window();
                bool? dialog = addPeriod_Window.ShowDialog();
                if (dialog == true && (from i in this.PeriodsC_Course where i.Item1 == addPeriod_Window.Period select i).Count() == 0)
                {
                    ListBoxItem listBoxItem = new();
                    listBoxItem.Content = addPeriod_Window.Period.ToString();
                    listBoxItem.LostFocus += ListBoxItemPeriod_HiddenCourse_LostFocus;
                    this.PeriodsSelection_HiddenCourse.Items.Add(listBoxItem);
                    this.PeriodsC_HiddenCourse.Add((addPeriod_Window.Period, listBoxItem));
                }
                else if (dialog == true)
                {
                    SystemSounds.Asterisk.Play();
                }
            }
        }

        private void RemovePeriod_HiddenCourse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.PeriodsSelection_HiddenCourse.Items.Remove(this.LastListBoxItemPeriod_HiddenCourse);
                this.PeriodsC_HiddenCourse.Remove((from i in this.PeriodsC_HiddenCourse where i.Item2 == this.LastListBoxItemPeriod_HiddenCourse select i).First());
            }
            catch { }
        }

        private void SaveHiddenCourse_Click(object sender, RoutedEventArgs e)
        {
            this.ResetColors_Course();
            try
            {
                GroupCourse course = (from i in this.HiddenCoursesC where i.Item2 == this.HiddenCourse_Picker.SelectedItem select i.Item1).First();

                //Aktualisiere die ID.
                course.ID = this.ID_HiddenCourse.GetValueString();

                //Aktualisiere die Stunden.
                course.FixPeriods = true;
                course.FixParticipants = true;
                course.HideCourse = true;
                course.Periods = (from i in this.PeriodsC_HiddenCourse where this.PeriodsSelection_HiddenCourse.Items.Contains(i.Item2) select i.Item1).ToArray();

                //Aktualisiere die LuL.
                foreach (Teacher i in from s in PeopleC_HiddenCourse where s.Item1 is Teacher && this.SelectedPeople_HiddenCourse.Items.Contains(s.Item2) && !course.Teachers.Contains(s.Item1) select s.Item1)
                {
                    course.AddTeacher(i);
                }
                foreach (Teacher i in from s in PeopleC_HiddenCourse where s.Item1 is Teacher && this.NotSelectedPeople_HiddenCourse.Items.Contains(s.Item2) && course.Teachers.Contains(s.Item1) select s.Item1)
                {
                    course.RemoveTeacher(i);
                }

                //Aktualisiere die SuS.
                foreach (Student i in from s in PeopleC_HiddenCourse where s.Item1 is Student && this.SelectedPeople_HiddenCourse.Items.Contains(s.Item2) && !course.Students.Contains(s.Item1) select s.Item1)
                {
                    i.AddToCourse(course);
                }
                foreach (Student i in from s in PeopleC_HiddenCourse where s.Item1 is Student && this.NotSelectedPeople_HiddenCourse.Items.Contains(s.Item2) && course.Students.Contains(s.Item1) select s.Item1)
                {
                    i.RemoveFromCourse(course);
                }

                //Aktualisiere die anderen Tabs.
                this.RefreshCourses();
                this.RefreshTeachers();
                this.RefreshStudents();
            }
            catch (FormatException) { }
        }

        private void DeleteHiddenCourse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Suche den Kurs:
                GroupCourse course = (from i in this.HiddenCoursesC where i.Item2 == this.HiddenCourse_Picker.SelectedItem select i.Item1).First();

                //Entferne den Kurs aus den SuS:
                foreach (Student i in course.Students)
                {
                    i.Courses = i.Courses.RemoveFromArray(course);
                }

                //Entferne den Kurs aus den LuL:
                foreach (Teacher i in course.Teachers)
                {
                    i.Courses = i.Courses.RemoveFromArray(course);
                }

                //Entferne den Kurs:
                this.Courses.Remove(course);

                //Aktualisieren
                this.RefreshAll();
            }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
        }

        private void AddAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ListBoxItem> items = new();
                foreach (ListBoxItem i in this.NotSelectedPeople_HiddenCourse.Items)
                {
                    items.Add(i);
                }
                foreach (ListBoxItem i in items)
                {
                    this.NotSelectedPeople_HiddenCourse.Items.Remove(i);
                    this.SelectedPeople_HiddenCourse.Items.Add(i);
                }
            }
            catch { }
        }

        private void RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ListBoxItem> items = new();
                foreach (ListBoxItem i in this.SelectedPeople_HiddenCourse.Items)
                {
                    items.Add(i);
                }
                foreach (ListBoxItem i in items)
                {
                    this.SelectedPeople_HiddenCourse.Items.Remove(i);
                    this.NotSelectedPeople_HiddenCourse.Items.Add(i);
                }
            }
            catch { }
        }
        #endregion
    }
}

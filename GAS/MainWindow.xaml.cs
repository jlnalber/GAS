using GeneticFramework;
using Microsoft.Win32;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Course> Courses;
        private List<Teacher> Teachers;
        private List<Student> Students;
        private List<(Course, ComboBoxItem)> CoursesC;
        private List<(Teacher, ComboBoxItem)> TeachersC;
        private List<(Student, ComboBoxItem)> StudentsC;

        public MainWindow()
        {
            InitializeComponent();

            this.Courses = new();
            this.Teachers = new();
            this.Students = new();

            this.CoursesC = new();
            this.TeachersC = new();
            this.StudentsC = new();
        }

        //Methoden zum Neuladen der Kurse, Lehrer und Schüler:
        #region
        public void RefreshCourses()
        {
            int index = this.Course_Picker.Items.Count != 0 ? this.Course_Picker.SelectedIndex : 0;

            this.Course_Picker.Items.Clear();
            this.CoursesC.Clear();

            foreach (Course i in this.Courses)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = i.ID;
                this.CoursesC.Add((i, comboBoxItem));
                this.Course_Picker.Items.Add(comboBoxItem);
            }

            try
            {
                this.Course_Picker.SelectedIndex = index;
            }
            catch { }

            if (this.Course_Picker.Items.Count == 0)
            {
                this.ID_Course.Content = "";
                this.Periods_Course.InputText = "";
                this.Teacher_Course.Items.Clear();
                this.SelectedStudents_Course.Items.Clear();
                this.NotSelectedStudents_Course.Items.Clear();
            }
        }

        public void RefreshTeachers()
        {
            int index = this.Teacher_Picker.Items.Count != 0 ? this.Teacher_Picker.SelectedIndex : 0;

            this.Teacher_Picker.Items.Clear();
            this.TeachersC.Clear();

            foreach (Teacher i in this.Teachers)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                this.TeachersC.Add((i, comboBoxItem));
                this.Teacher_Picker.Items.Add(comboBoxItem);
            }

            try
            {
                this.Teacher_Picker.SelectedIndex = index;
            }
            catch { }

            if (this.Teacher_Picker.Items.Count == 0)
            {
                this.ID_Teacher.Content = "";
                this.Name_Teacher.InputText = "";
                this.Courses_Teacher.Items.Clear();
            }
        }

        public void RefreshStudents()
        {
            int index = this.Student_Picker.Items.Count != 0 ? this.Student_Picker.SelectedIndex : 0;

            this.Student_Picker.Items.Clear();
            this.StudentsC.Clear();

            foreach (Student i in this.Students)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                this.StudentsC.Add((i, comboBoxItem));
                this.Student_Picker.Items.Add(comboBoxItem);
            }

            try
            {
                this.Student_Picker.SelectedIndex = index;
            }
            catch { }

            if (this.Student_Picker.Items.Count == 0)
            {
                this.ID_Student.Content = "";
                this.Name_Student.InputText = "";
                this.SelectedCourses_Student.Items.Clear();
                this.NotSelectedCourses_Student.Items.Clear();
            }
        }

        public void RefreshAll()
        {
            this.RefreshCourses();
            this.RefreshTeachers();
            this.RefreshStudents();
        }
        #endregion

        //Methoden, mit denen Dateiexport/-import gemacht werden kann:
        #region
        private void ImportData_Click(object sender, RoutedEventArgs e)
        {
            new Import_Window(this).Show();
        }

        public void LoadFromFile(string path, int startCol, int endCol, int startRow, int endRow, int rowIDsCourses = -1, int columnNamesStudents = -1)
        {
            CSVReader reader = new(path);

            Course[] courses = new Course[endCol - startCol];
            List<Teacher> teachers = new();
            List<Student> students = new();

            for (int i = 0; i < endCol - startCol; i++)
            {
                Teacher teacher = new Teacher(new Course[0], "T" + (i + 1));
                string ID = rowIDsCourses == -1 ? "C" + (i + 1) : reader[i + startCol, rowIDsCourses];
                Course course = new(1, new Student[0], teacher, ID);
                teacher.Courses = new Course[1] { course };
                courses[i] = course;
                teachers.Add(teacher);
            }

            for (int i = 0; i < endRow - startRow; i++)
            {
                Student student = new(new Course[0], "S" + (i + 1));
                if (columnNamesStudents != -1)
                {
                    student.Name = reader[columnNamesStudents, i + startRow];
                }
                for (int j = 0; j < endCol - startCol; j++)
                {
                    if (reader[j + startCol, i + startRow].ToUpper() == "X")
                    {
                        student.AddToCourse(courses[j]);
                    }
                }
                students.Add(student);
            }

            this.Courses.Clear();
            this.Courses.AddRange(courses);
            this.Teachers = teachers;
            this.Students = students;

            this.RefreshAll();
        }

        private void ExportData_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Comma-separated values|*csv";
            saveFileDialog.Title = "Daten exportieren";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    this.ExportToFile(saveFileDialog.FileName);
                }
                catch
                {
                    SystemSounds.Asterisk.Play();
                }
            }
        }

        public void ExportToFile(string path)
        {
            CSVWriter csvWriter = new(this.Courses.Count + 1, this.Students.Count + 1);

            csvWriter[0, 0] = "-";

            for (int i = 0; i < this.Courses.Count; i++)
            {
                csvWriter[i + 1, 0] = this.Courses[i].ID;
            }

            for (int i = 0; i < this.Students.Count; i++)
            {
                csvWriter[0, i + 1] = this.Students[i].Name;

                for (int j = 0; j < this.Courses.Count; j++)
                {
                    string str = ".";
                    if (this.Students[i].Courses.Contains(this.Courses[j]))
                    {
                        str = "x";
                    }
                    csvWriter[j + 1, i + 1] = str;
                }
            }

            csvWriter.Save(path);
        }
        #endregion

        //Methoden für die "Hinzufügen"-Buttons:
        #region
        private void AddCourse_Click(object sender, RoutedEventArgs e)
        {
            new AddCourse_Window(this).Show();
        }

        private void AddTeacher_Click(object sender, RoutedEventArgs e)
        {
            new AddTeacher_Window(this).Show();
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            new AddStudent_Window(this).Show();
        }
        #endregion

        //Methoden für die Verwaltung der Kurse, Lehrer und Eltern:
        #region
        public List<Course> GetCourses()
        {
            return this.Courses;
        }

        public List<Teacher> GetTeachers()
        {
            return this.Teachers;
        }

        public List<Student> GetStudents()
        {
            return this.Students;
        }

        public void AddCourse(Course course)
        {
            this.Courses.Add(course);

            course.Teacher.Courses = Utils.AddToArray(course.Teacher.Courses, course);

            foreach (Student s in course.Students)
            {
                s.Courses = Utils.AddToArray(s.Courses, course);
            }

            this.RefreshAll();
        }

        public void AddTeacher(Teacher teacher)
        {
            this.Teachers.Add(teacher);

            foreach (Course c in teacher.Courses)
            {
                c.Teacher.Courses = Utils.RemoveFromArray(c.Teacher.Courses, c);
                c.Teacher = teacher;
            }

            this.RefreshAll();
        }

        public void AddStudent(Student student)
        {
            this.Students.Add(student);

            foreach (Course c in student.Courses)
            {
                c.Students = Utils.AddToArray(c.Students, student);
            }

            this.RefreshAll();
        }
        #endregion

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new ScheduleCalculator_Window(new Schedule(this.Courses.ToArray())).Show();
            }
            catch (Schedule.InvalidIDException)
            {
                MessageBox.Show("Eine odere mehrere ID(s) tauchen mehrfach auf!");
            }
        }

        //Methoden für das Bearbeiten der Kurse:
        #region
        private List<(Teacher, ComboBoxItem)> TeachersC_Course = new();
        private List<(Student, ListBoxItem)> StudentsC_Course = new();
        private ListBoxItem LastListBoxItem_Course;

        private void Course_Picker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Course course = (from i in this.CoursesC where i.Item2 == this.Course_Picker.SelectedItem select i.Item1).First();
                this.DisplayCourse(course);
            }
            catch { }
        }

        private void SwitchButton1_Click(object sender, RoutedEventArgs e)
        {
            if (this.LastListBoxItem_Course != null)
            {
                if (this.SelectedStudents_Course.Items.Contains(this.LastListBoxItem_Course))
                {
                    this.SelectedStudents_Course.Items.Remove(this.LastListBoxItem_Course);
                    this.NotSelectedStudents_Course.Items.Add(this.LastListBoxItem_Course);
                }
                else
                {
                    this.NotSelectedStudents_Course.Items.Remove(this.LastListBoxItem_Course);
                    this.SelectedStudents_Course.Items.Add(this.LastListBoxItem_Course);
                }
            }
        }

        private void SaveCourse_Click(object sender, RoutedEventArgs e)
        {
            this.ResetColors_Course();
            try
            {
                Course course = (from i in this.CoursesC where i.Item2 == this.Course_Picker.SelectedItem select i.Item1).First();

                //Aktualisiere die ID.
                course.ID = this.ID_Course.GetValueString();

                //Aktualisiere die Stundenanzahl.
                course.Periods = new Period[this.Periods_Course.GetValueInt()];

                //Aktualisiere den LuL.
                Teacher newTeacher = (from i in this.TeachersC_Course where i.Item2 == this.Teacher_Course.SelectedItem select i.Item1).First();
                course.Teacher.RemoveFromCourse(course, newTeacher);

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
            }
            catch (FormatException) { }
            catch (InvalidOperationException)
            {
                SystemSounds.Asterisk.Play();
                this.TeacherLabel_Course.Foreground = Brushes.Red;
            }
        }

        private void ResetColors_Course()
        {
            this.Periods_Course.Label.Foreground = Brushes.Black;
            this.TeacherLabel_Course.Foreground = Brushes.Black;
        }

        private void DisplayCourse(Course course)
        {
            this.ResetColors_Course();

            this.ID_Course.InputText = course.ID;

            this.Periods_Course.InputText = course.Periods.Length.ToString();

            this.TeachersC_Course.Clear();
            this.Teacher_Course.Items.Clear();
            ComboBoxItem boxItem = new();
            foreach (Teacher i in this.Teachers)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                this.TeachersC_Course.Add((i, comboBoxItem));
                this.Teacher_Course.Items.Add(comboBoxItem);
                if (i == course.Teacher)
                {
                    boxItem = comboBoxItem;
                }
            }
            this.Teacher_Course.SelectedItem = boxItem;

            this.StudentsC_Course.Clear();
            this.SelectedStudents_Course.Items.Clear();
            this.NotSelectedStudents_Course.Items.Clear();
            foreach (Student i in this.Students)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                listBoxItem.MouseDoubleClick += ListBoxItem_Course_MouseDoubleClick;
                listBoxItem.LostFocus += ListBoxItem_Course_LostFocus;
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

        private void ListBoxItem_Course_LostFocus(object sender, RoutedEventArgs e)
        {
            this.LastListBoxItem_Course = sender as ListBoxItem;
        }

        private void ListBoxItem_Course_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                Course course = (from i in this.CoursesC where i.Item2 == this.Course_Picker.SelectedItem select i.Item1).First();

                foreach (Student i in course.Students)
                {
                    i.Courses = Utils.RemoveFromArray(i.Courses, course);
                }

                course.Teacher.Courses = Utils.RemoveFromArray(course.Teacher.Courses, course);

                this.Courses.Remove(course);

                this.RefreshAll();
            }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
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
            foreach (Course i in teacher.Courses)
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
                    Teacher teacher = (from i in this.TeachersC where i.Item2 == this.Teacher_Picker.SelectedItem select i.Item1).First();

                    Teacher replaceBy = Utils.Filter(this.Teachers.ToArray(), (Teacher t) => t != teacher)[0];
                    foreach (Course i in teacher.Courses)
                    {
                        i.Teacher = replaceBy;
                        replaceBy.Courses = Utils.AddToArray(replaceBy.Courses, i);
                    }

                    this.Teachers.Remove(teacher);

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
                foreach (Course i in from c in CoursesC_Student where this.SelectedCourses_Student.Items.Contains(c.Item2) && !student.Courses.Contains(c.Item1) select c.Item1)
                {
                    student.AddToCourse(i);
                }
                foreach (Course i in from c in CoursesC_Student where this.NotSelectedCourses_Student.Items.Contains(c.Item2) && student.Courses.Contains(c.Item1) select c.Item1)
                {
                    student.RemoveFromCourse(i);
                }

                //Aktualisiere die anderen Tabs.
                this.RefreshCourses();
                this.RefreshTeachers();
            }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
        }

        private void DisplayStudent(Student student)
        {
            this.ID_Student.InputText = student.ID;

            this.Name_Student.InputText = student.Name;

            this.CoursesC_Student.Clear();
            this.SelectedCourses_Student.Items.Clear();
            this.NotSelectedCourses_Student.Items.Clear();
            foreach (Course i in this.Courses)
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
                Student student = (from i in this.StudentsC where i.Item2 == this.Student_Picker.SelectedItem select i.Item1).First();

                foreach (Course i in student.Courses)
                {
                    i.Students = Utils.RemoveFromArray(i.Students, student);
                }

                this.Students.Remove(student);

                this.RefreshAll();
            }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
        }
        #endregion
    }
}

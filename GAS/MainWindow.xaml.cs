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
        private List<GroupCourse> Courses;
        private List<Teacher> Teachers;
        private List<Student> Students;
        private List<(GroupCourse, ComboBoxItem)> CoursesC;
        private List<(Teacher, ComboBoxItem)> TeachersC;
        private List<(Student, ComboBoxItem)> StudentsC;
        private string Files;

        public MainWindow()
        {
            InitializeComponent();

            this.Courses = new();
            this.Teachers = new();
            this.Students = new();

            this.CoursesC = new();
            this.TeachersC = new();
            this.StudentsC = new();

            //Lese die Aufrufeparamter aus:
            Files = Environment.GetCommandLineArgs()[0].Replace("bin\\Debug\\net5.0-windows\\GAS.dll", "");
            Files = Files.Replace("bin\\Debug\\net5.0-windows\\GAS.exe", "");
            Files = Files.Replace("bin\\Release\\net5.0-windows\\GAS.dll", "");
            Files = Files.Replace("bin\\Release\\net5.0-windows\\GAS.exe", "");
            Files = Files.Replace("GAS.exe", "");
            Files = Files.Replace("GAS.dll", "");
        }

        //Methoden zum Neuladen der Kurse, Lehrer und Schüler:
        #region
        public void RefreshCourses()
        {
            //Speichere den Index des ausgewählten Kurses:
            int index = this.Course_Picker.Items.Count != 0 ? this.Course_Picker.SelectedIndex : 0;

            //Leere die Listen:
            this.Course_Picker.Items.Clear();
            this.CoursesC.Clear();

            //Erstelle die neuen Items:
            foreach (GroupCourse i in this.Courses)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = i.ID;
                this.CoursesC.Add((i, comboBoxItem));
                this.Course_Picker.Items.Add(comboBoxItem);
            }

            //Wähle wieder den alten Kurs aus:
            try
            {
                if (index >= this.Course_Picker.Items.Count)
                {
                    index = 0;
                }
                this.Course_Picker.SelectedIndex = index;
            }
            catch { }

            if (this.Course_Picker.Items.Count == 0)
            {
                //Leere alles, falls es keine Kurse gibt:
                this.ID_Course.Content = "";
                this.Periods_Course.InputText = "";
                this.SelectedTeachers_Course.Items.Clear();
                this.NotSelectedTeachers_Course.Items.Clear();
                this.SelectedStudents_Course.Items.Clear();
                this.NotSelectedStudents_Course.Items.Clear();
                this.PeriodsSelection_Course.Items.Clear();
                this.FixPeriods_Course.IsChecked = false;
            }
        }

        public void RefreshTeachers()
        {
            //Speichere den Index des ausgewählten LuL:
            int index = this.Teacher_Picker.Items.Count != 0 ? this.Teacher_Picker.SelectedIndex : 0;

            //Leere die Listen:
            this.Teacher_Picker.Items.Clear();
            this.TeachersC.Clear();

            //Erstelle die neuen Items:
            foreach (Teacher i in this.Teachers)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                this.TeachersC.Add((i, comboBoxItem));
                this.Teacher_Picker.Items.Add(comboBoxItem);
            }

            //Wähle wieder den alten LuL aus:
            try
            {
                if (index >= this.Teacher_Picker.Items.Count)
                {
                    index = 0;
                }
                this.Teacher_Picker.SelectedIndex = index;
            }
            catch { }

            if (this.Teacher_Picker.Items.Count == 0)
            {
                //Leere alles, falls es keine LuL gibt:
                this.ID_Teacher.Content = "";
                this.Name_Teacher.InputText = "";
                this.Courses_Teacher.Items.Clear();
            }
        }

        public void RefreshStudents()
        {
            //Speichere den Index des ausgewählten SuS:
            int index = this.Student_Picker.Items.Count != 0 ? this.Student_Picker.SelectedIndex : 0;

            //Leere die Listen:
            this.Student_Picker.Items.Clear();
            this.StudentsC.Clear();

            //Erstelle die neuen Items:
            foreach (Student i in this.Students)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = i.Name == "" ? i.ID : i.Name;
                this.StudentsC.Add((i, comboBoxItem));
                this.Student_Picker.Items.Add(comboBoxItem);
            }

            //Wähle wieder den alten SuS aus:
            try
            {
                if (index >= this.Student_Picker.Items.Count)
                {
                    index = 0;
                }
                this.Student_Picker.SelectedIndex = index;
            }
            catch { }

            if (this.Student_Picker.Items.Count == 0)
            {
                //Leere alles, falls es keine SuS gibt:
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

        public void LoadFromFile(string path, int startCol, int endCol, int startRow, int endRow, int rowIDsCourses = -1, int columnNamesStudents = -1, int rowPeriods = -1, int defaultPeriods = 2, int thresholdCourse = 23)
        {
            //Lese die Datei ein:
            CSVReader reader = new(path);

            //Erstelle die Elemente für den Stundenplan:
            GroupCourse[] courses = new GroupCourse[endCol - startCol];
            List<Teacher> teachers = new();
            List<Student> students = new();

            //Lese die Kurse ein:
            for (int i = 0; i < endCol - startCol; i++)
            {
                Teacher[] teacher = new Teacher[1] { new Teacher(new GroupCourse[0], "T" + (i + 1)) };
                string ID = rowIDsCourses == -1 ? "C" + (i + 1) : reader[i + startCol, rowIDsCourses];
                int periods = defaultPeriods;
                try
                {
                    periods = int.Parse(reader[i + startCol, rowPeriods]);
                }
                catch { }
                GroupCourse course = new(periods, new Student[0], teacher, ID);
                teacher[0].Courses = new GroupCourse[1] { course };
                courses[i] = course;
                teachers.Add(teacher[0]);
            }

            //Lese die Schüler ein:
            for (int i = 0; i < endRow - startRow; i++)
            {
                Student student = new(new GroupCourse[0], "S" + (i + 1));
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

            //Füge solange Lehrer hinzu, bis es mit dem Schwellenwert aufgeht:
            foreach (GroupCourse i in courses)
            {
                while (i.Students.Length > thresholdCourse * i.Teachers.Length)
                {
                    Teacher teacher = new Teacher(new Course[0], "T" + (teachers.Count() + 1));
                    i.AddTeacher(teacher);
                    teachers.Add(teacher);
                }
            }

            //Füge die Daten zur Verwaltung hinzu:
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
            //Erstelle eine Tabelle:
            CSVWriter csvWriter = new(this.Courses.Count + 1, this.Students.Count + 1);

            csvWriter[0, 0] = "-";

            //Schreibe die IDs der Kurse in die Tabelle:
            for (int i = 0; i < this.Courses.Count; i++)
            {
                csvWriter[i + 1, 0] = this.Courses[i].ID;
            }
            //Schreibe die Namen der Schüler und deren Informationen in die Tabelle:
            for (int i = 0; i < this.Students.Count; i++)
            {
                csvWriter[0, i + 1] = this.Students[i].Name == "" ? this.Students[i].ID : this.Students[i].Name;

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

            //Speicher die Tabelle in einer Datei:
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
        public List<GroupCourse> GetCourses()
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

        public void AddCourse(GroupCourse course)
        {
            //Füge den Kurs hinzu.
            this.Courses.Add(course);

            foreach (Teacher t in course.Teachers)
            {
                t.Courses = t.Courses.AddToArray(course);
            }
            foreach (Student s in course.Students)
            {
                s.Courses = s.Courses.AddToArray(course);
            }

            this.RefreshAll();
        }

        public void AddTeacher(Teacher teacher)
        {
            //Füge den Lehrer hinzu.
            this.Teachers.Add(teacher);

            foreach (GroupCourse c in teacher.Courses)
            {
                c.Teachers = c.Teachers.AddToArray(teacher);
            }

            this.RefreshAll();
        }

        public void AddStudent(Student student)
        {
            //Füge den Schüler hinzu.
            this.Students.Add(student);

            foreach (GroupCourse c in student.Courses)
            {
                c.Students = c.Students.AddToArray(student);
            }

            this.RefreshAll();
        }
        #endregion

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Erstelle eine Kopie von den Kursen:
                GroupCourse[] groupCourses = GroupCourse.GetDeepCopy(this.Courses.ToArray());

                //Erstelle eine Array für die neu verteilten Kurse:
                Course[] courses = new Course[Utils.Sum(groupCourses, (GroupCourse c) => c.Teachers.Length)];

                //Verteile die Kurse und weise sie dem Array zu:
                int counter = 0;
                for (int i = 0; i < groupCourses.Length; i++)
                {
                    foreach (Course c in groupCourses[i].Split())
                    {
                        courses[counter] = c;
                        counter++;
                    }
                }

                //Erstelle ein neues Fenster, um den Stundenplan zu berechnen:
                new ScheduleCalculator_Window(new Schedule(courses)).Show();
            }
            catch (Schedule.Exceptions.InvalidIDException)
            {
                MessageBox.Show("Eine odere mehrere ID(s) tauchen mehrfach auf!");
            }
        }

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
            this.Periods_Course.Label.Foreground = Brushes.Black;
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

        private void AddPeriod_Click(object sender, RoutedEventArgs e)
        {
            AddPeriod_Window addPeriod_Window = new AddPeriod_Window();
            if (addPeriod_Window.ShowDialog() == true && (from i in this.PeriodsC_Course where i.Item1 == addPeriod_Window.Period select i).Count() == 0)
            {
                ListBoxItem listBoxItem = new();
                listBoxItem.Content = addPeriod_Window.Period.ToString();
                listBoxItem.LostFocus += ListBoxItemP_Course_LostFocus;
                this.PeriodsSelection_Course.Items.Add(listBoxItem);
                this.PeriodsC_Course.Add((addPeriod_Window.Period, listBoxItem));
            }
            else
            {
                SystemSounds.Asterisk.Play();
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

        private void Hilfe_Click(object sender, RoutedEventArgs e)
        {
            //Diese Methode wird ausgeführt, wenn das MenuItem "HilfeDatei" geklickt wird
            string path = "";
            try
            {
                //Öffne die Hilfe-Datei im Verzeichnis "Hilfe" mit dem Namen "Hilfe.pdf" mit dem Standardprogramm
                if (Files.Contains("GAS.exe"))
                {
                    path = Files.Substring(0, Files.LastIndexOf(@"\GAS.exe")) + @"\Hilfe\Hilfe.pdf";
                }
                else
                {
                    path = Files + @"Hilfe\Hilfe.pdf";
                }
                //System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(@"\Hilfe\Hilfe.pdf"));
                new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo(path)
                    {
                        UseShellExecute = true
                    }
                }.Start();
                //System.Diagnostics.Process.Start(path);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.Message + " " + path);
            }
        }
    }
}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using Utils;

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
        private List<(GroupCourse, ComboBoxItem)> HiddenCoursesC;
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
            this.HiddenCoursesC = new();

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
                if (!i.HideCourse)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = i.ID;
                    this.CoursesC.Add((i, comboBoxItem));
                    this.Course_Picker.Items.Add(comboBoxItem);
                }
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

        public void RefreshHiddenCourses()
        {
            //Speichere den Index der ausgewählten Sperrstunde:
            int index = this.HiddenCourse_Picker.Items.Count != 0 ? this.HiddenCourse_Picker.SelectedIndex : 0;

            //Leere die Listen:
            this.HiddenCourse_Picker.Items.Clear();
            this.HiddenCoursesC.Clear();

            //Erstelle die neuen Items:
            foreach (GroupCourse i in this.Courses)
            {
                if (i.HideCourse)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = i.ID;
                    this.HiddenCoursesC.Add((i, comboBoxItem));
                    this.HiddenCourse_Picker.Items.Add(comboBoxItem);
                }
            }

            //Wähle wieder die alte Sperrstunde aus:
            try
            {
                if (index >= this.HiddenCourse_Picker.Items.Count)
                {
                    index = 0;
                }
                this.HiddenCourse_Picker.SelectedIndex = index;
            }
            catch { }

            if (this.HiddenCourse_Picker.Items.Count == 0)
            {
                //Leere alles, falls es keine Sperrstunden gibt:
                this.ID_HiddenCourse.Content = "";
                this.SelectedPeople_HiddenCourse.Items.Clear();
                this.NotSelectedPeople_HiddenCourse.Items.Clear();
                this.PeriodsSelection_HiddenCourse.Items.Clear();
                this.FixPeriods_Course.IsChecked = false;
            }
        }

        public void RefreshAll()
        {
            this.RefreshCourses();
            this.RefreshTeachers();
            this.RefreshStudents();
            this.RefreshHiddenCourses();
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

        private void AddHiddenCourse_Click(object sender, RoutedEventArgs e)
        {
            new AddHiddenCourse_Window(this).Show();
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
                Course[] courses = new Course[ArrayExt.Sum(groupCourses, (GroupCourse c) => c.Teachers.Length)];

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
            catch { }
        }
    }
}

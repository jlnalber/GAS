using Microsoft.Win32;
using System;
using System.Windows;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für Import_Window.xaml
    /// </summary>
    public partial class Import_Window : Window
    {
        MainWindow MainWindow;

        public Import_Window(MainWindow mainWindow)
        {
            InitializeComponent();

            this.MainWindow = mainWindow;
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Comma-separated values|*.csv";

            if (openFileDialog.ShowDialog() == true)
            {
                this.Path.Text = openFileDialog.FileName;
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Lese die Werte für die Tabelle ein:
                int colStart = this.ColumnStart.GetValueInt() - 1;
                int colEnd = this.ColumnEnd.GetValueInt();
                int rowStart = this.RowStart.GetValueInt() - 1;
                int rowEnd = this.RowEnd.GetValueInt();
                string path = this.Path.Text;

                //Lese die optionalen Werte ein:
                int rowIDsCourses = -1;
                if (this.ImportIDsCourses.IsChecked == true)
                {
                    rowIDsCourses = this.RowCoursesIDs.GetValueInt() - 1;
                }

                int columnNamesStudents = -1;
                if (this.ImportNamesStudents.IsChecked == true)
                {
                    columnNamesStudents = this.ColumnStudentsNames.GetValueInt() - 1;
                }

                int rowPeriods = -1;
                if (this.ImportRowPeriods.IsChecked == true)
                {
                    rowPeriods = this.RowPeriods.GetValueInt() - 1;
                }

                //Lese weitere Werte ein:
                int defaultPeriods = this.DefaultPeriods.GetValueInt();

                int thresholdCourse = this.ThresholdCourse.GetValueInt();

                //Lade die Datei im MainWindow und schließe das Fenster:
                this.MainWindow.LoadFromFile(path, colStart, colEnd, rowStart, rowEnd, rowIDsCourses, columnNamesStudents, rowPeriods, defaultPeriods, thresholdCourse);

                this.Close();
            }
            catch (FormatException) { }
            catch (System.IO.IOException)
            {
                MessageBox.Show("Fehler bei der Datei, die Datei wird vielleicht von einem anderen Prozess bereits verwendet");
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Ungültige Angaben! Konnte nicht die Datei richtig auslesen...");
            }
            catch { }
        }

        //Methoden für den Import der Namen, der IDs und der Stunden:
        #region
        private void ImportNamesStudents_Checked(object sender, RoutedEventArgs e)
        {
            this.ColumnStudentsNames.IsEnabled = true;
        }

        private void ImportNamesStudents_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ColumnStudentsNames.IsEnabled = false;
        }

        private void ImportIDsCourses_Checked(object sender, RoutedEventArgs e)
        {
            this.RowCoursesIDs.IsEnabled = true;
        }

        private void ImportIDsCourses_Unchecked(object sender, RoutedEventArgs e)
        {
            this.RowCoursesIDs.IsEnabled = false;
        }

        private void ImportRowPeriods_Checked(object sender, RoutedEventArgs e)
        {
            this.RowPeriods.IsEnabled = true;
        }

        private void ImportRowPeriods_Unchecked(object sender, RoutedEventArgs e)
        {
            this.RowPeriods.IsEnabled = false;
        }
        #endregion
    }
}

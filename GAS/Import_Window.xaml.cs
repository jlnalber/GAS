using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
                int colStart = this.ColumnStart.GetValueInt() - 1;
                int colEnd = this.ColumnEnd.GetValueInt();
                int rowStart = this.RowStart.GetValueInt() - 1;
                int rowEnd = this.RowEnd.GetValueInt();
                string path = this.Path.Text;

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

                this.MainWindow.LoadFromFile(path, colStart, colEnd, rowStart, rowEnd, rowIDsCourses, columnNamesStudents);

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

        //Methoden für den Import der Namen und der IDs:
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
        #endregion
    }
}

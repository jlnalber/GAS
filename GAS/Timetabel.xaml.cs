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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für Timetabel.xaml
    /// </summary>
    public partial class Timetabel : UserControl
    {
        public Timetabel()
        {
            InitializeComponent();
        }

        public void DisplaySchedule(Schedule schedule)
        {
            //Stelle die Ergebniss dar.
            for (int i = 1; i <= 5; i++)
            {
                for (int j = 1; j <= 11; j++)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = string.Join(", ", from k in schedule.Courses where k.Periods.Contains(new Period((Weekday)i, (Hour)j)) select k.ID);
                    textBlock.TextAlignment = TextAlignment.Center;

                    Grid.SetColumn(textBlock, i);
                    Grid.SetRow(textBlock, j);

                    this.Timetable.Children.Add(textBlock);

                }
            }
        }
    }
}

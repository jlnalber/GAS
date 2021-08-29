using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
                    textBlock.Text = string.Join(", ", from k in schedule.Courses where k.Periods.Contains(new Period((Weekday)i, (Hour)j)) && !k.HideCourse select k.ID);
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.TextWrapping = TextWrapping.WrapWithOverflow;

                    Grid.SetColumn(textBlock, i);
                    Grid.SetRow(textBlock, j);

                    this.Timetable.Children.Add(textBlock);

                }
            }
        }

        public void DisplaySchedule(Person person)
        {
            //Stelle die Ergebniss dar.
            for (int i = 1; i <= 5; i++)
            {
                for (int j = 1; j <= 11; j++)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = string.Join(", ", from k in person.Courses where k.Periods.Contains(new Period((Weekday)i, (Hour)j)) && !k.HideCourse select k.ID);
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.TextWrapping = TextWrapping.WrapWithOverflow;

                    Grid.SetColumn(textBlock, i);
                    Grid.SetRow(textBlock, j);

                    this.Timetable.Children.Add(textBlock);

                }
            }
        }
    }
}

using GeneticFramework;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für Schedule_Window.xaml
    /// </summary>
    public partial class Schedule_Window : Window
    {
        Schedule Schedule;
        public Schedule_Window(Schedule schedule)
        {
            InitializeComponent();

            this.Schedule = schedule;

            this.Timetable.DisplaySchedule(this.Schedule);
        }
    }
}

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
using GeneticFramework;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für ScheduleCalculator_Window.xaml
    /// </summary>
    public partial class ScheduleCalculator_Window : Window
    {
        Schedule Schedule;

        public ScheduleCalculator_Window(Schedule schedule)
        {
            InitializeComponent();

            this.Schedule = schedule;

            Schedule[] population = new Schedule[20];
            for (int i = 0; i < population.Length; i++)
            {
                population[i] = this.Schedule.GetRandomInstance();
            }

            GeneticAlgorithm<Schedule> geneticAlgorithm = new(population, 1);
            geneticAlgorithm.ExtraCondition = (Schedule s) => s.AllApplies();
            this.Schedule = geneticAlgorithm.Run();
        }
    }
}

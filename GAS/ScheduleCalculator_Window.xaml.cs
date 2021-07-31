using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
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
        }

        public void WriteLine(string text)
        {
            DateTime dateTime = DateTime.Now;
            this.Console.Text += "[" + dateTime.ToString() + "]: " + text + "\n";
        }

        private async void Calculate_Click(object sender, RoutedEventArgs e)
        {
            this.Console.Text = "";
            try
            {
                int initialPopulationSize = this.InitialPopulationSize.GetValueInt();

                int maxGenerations = this.MaxGenerations.GetValueInt();

                double mutationChance = this.MutationChance.GetValueDouble();

                double crossoverChance = this.CrossoverChance.GetValueDouble();

                GeneticAlgorithm<Schedule>.SelectionTypeEnum selectionType = GeneticAlgorithm<Schedule>.SelectionTypeEnum.Tournament;
                if (this.SelectionType.SelectedItem == this.RouletteSelectionType)
                {
                    selectionType = GeneticAlgorithm<Schedule>.SelectionTypeEnum.Roulette;
                }
                else if (this.SelectionType.SelectedItem == this.TournamentSelectionType)
                {
                    selectionType = GeneticAlgorithm<Schedule>.SelectionTypeEnum.Tournament;
                }

                Schedule[] population = new Schedule[initialPopulationSize];
                for (int i = 0; i < population.Length; i++)
                {
                    population[i] = this.Schedule.GetRandomInstance();
                }

                GeneticAlgorithm<Schedule> geneticAlgorithm = new(population, 1, maxGenerations, mutationChance, crossoverChance, selectionType);
                geneticAlgorithm.ExtraCondition = (Schedule s) => s.AllApplies();
                geneticAlgorithm.ForEachGeneration = (int gen, Schedule[] schedules, (Schedule, double) best) => WriteLine("Generation: " + gen + ", Beste Fitness: " + best.Item2);
                this.Schedule = await geneticAlgorithm.RunAsync();

                new Schedule_Window(this.Schedule).Show();
            }
            catch (FormatException) { }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
        }
    }
}

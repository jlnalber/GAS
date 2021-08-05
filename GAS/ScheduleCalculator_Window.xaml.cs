using GeneticFramework;
using System;
using System.Media;
using System.Threading.Tasks;
using System.Windows;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für ScheduleCalculator_Window.xaml
    /// </summary>
    public partial class ScheduleCalculator_Window : Window
    {
        Schedule Schedule;
        Schedule BestSchedule;

        public ScheduleCalculator_Window(Schedule schedule)
        {
            InitializeComponent();

            this.Schedule = schedule;
        }

        private async void Calculate_Click(object sender, RoutedEventArgs e)
        {
            this.Generation.Content = "0";
            this.Fitness.Content = "0";
            this.Calculate.IsEnabled = false;
            try
            {
                int initialPopulationSize = this.InitialPopulationSize.GetValueInt();

                int maxGenerations = int.MaxValue;
                if (this.UnlimitedGenerations.IsChecked == false)
                {
                    maxGenerations = this.MaxGenerations.GetValueInt();
                }

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

                this.Status.Content = "Erstelle die erste Population...";

                Schedule[] population = new Schedule[initialPopulationSize];
                for (int i = 0; i < population.Length; i++)
                {
                    population[i] = await Task.Run(() => this.Schedule.GetRandomInstance());
                }

                this.OpenBest.IsEnabled = true;
                this.Status.Content = "Suche nach einem Stundenplan...";
                GeneticAlgorithm<Schedule> geneticAlgorithm = new(population, 1, maxGenerations, mutationChance, crossoverChance, selectionType: selectionType);
                geneticAlgorithm.ExtraCondition = (Schedule s) => s.AllApplies();
                geneticAlgorithm.ForEachGeneration = (int gen, Schedule[] schedules, (Schedule, double) best) => { this.Generation.Content = gen + 1; this.Fitness.Content = best.Item2; this.BestSchedule = best.Item1; };
                this.Schedule = await geneticAlgorithm.RunAsync();

                /*this.Status.Content = "Optimiere die Lösung...";
                await Task.Run(() => this.Schedule.Optimize());*/

                new Schedule_Window(this.Schedule).Show();

                this.Status.Content = "Warte auf Eingabe...";
            }
            catch (FormatException) { }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
            this.Calculate.IsEnabled = true;
        }

        private void OpenBest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new Schedule_Window(this.BestSchedule).Show();
            }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
        }

        private void UnlimitedGenerations_Checked(object sender, RoutedEventArgs e)
        {
            this.MaxGenerations.IsEnabled = false;
        }

        private void UnlimitedGenerations_Unchecked(object sender, RoutedEventArgs e)
        {
            this.MaxGenerations.IsEnabled = true;
        }
    }
}

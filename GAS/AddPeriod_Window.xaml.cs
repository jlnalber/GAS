using System.Windows;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für AddPeriod_Window.xaml
    /// </summary>
    public partial class AddPeriod_Window : Window
    {
        private static int WeekdayStartIndex = 0;
        private static int HourStartIndex = 0;

        public AddPeriod_Window()
        {
            InitializeComponent();

            this.Weekday.SelectedIndex = WeekdayStartIndex;
            this.Hour.SelectedIndex = HourStartIndex;
        }

        private void ButtonDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public Period Period
        {
            get
            {
                WeekdayStartIndex = this.Weekday.SelectedIndex;
                HourStartIndex = this.Hour.SelectedIndex;
                return new Period((Weekday)(this.Weekday.SelectedIndex + 1), (Hour)(this.Hour.SelectedIndex + 1));
            }
        }
    }
}

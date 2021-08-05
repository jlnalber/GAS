using System.Windows;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für AddPeriod_Window.xaml
    /// </summary>
    public partial class AddPeriod_Window : Window
    {
        public AddPeriod_Window()
        {
            InitializeComponent();
        }

        private void ButtonDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public Period Period
        {
            get
            {
                return new Period((Weekday)(this.Weekday.SelectedIndex + 1), (Hour)(this.Hour.SelectedIndex + 1));
            }
        }
    }
}

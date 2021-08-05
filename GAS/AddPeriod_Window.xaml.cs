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

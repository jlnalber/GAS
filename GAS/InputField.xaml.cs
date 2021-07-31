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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GAS
{
    /// <summary>
    /// Interaktionslogik für InputField.xaml
    /// </summary>
    public partial class InputField : UserControl
    {
        public string LabelContent
        {
            get
            {
                return this.Label.Content.ToString();
            }
            set
            {
                this.Label.Content = value;
            }
        }

        public string InputText
        {
            get
            {
                return this.Input.Text;
            }
            set
            {
                this.Input.Text = value;
            }
        }

        public double Distance
        {
            get
            {
                return this.Input.Margin.Left - this.Label.Margin.Left;
            }
            set
            {
                this.Input.Margin = new Thickness(value + this.Label.Margin.Left, this.Input.Margin.Top, this.Input.Margin.Right, this.Input.Margin.Bottom);
            }
        }

        public InputField()
        {
            InitializeComponent();
        }

        public void ResetColor()
        {
            this.Label.Foreground = Brushes.Black;
        }

        public int GetValueInt()
        {
            this.ResetColor();
            try
            {
                return int.Parse(this.Input.Text);
            }
            catch
            {
                SystemSounds.Asterisk.Play();
                this.Label.Foreground = Brushes.Red;
            }
            throw new FormatException();
        }

        public double GetValueDouble()
        {
            this.ResetColor();
            try
            {
                return double.Parse(this.Input.Text);
            }
            catch
            {
                SystemSounds.Asterisk.Play();
                this.Label.Foreground = Brushes.Red;
            }
            throw new FormatException();
        }

        public string GetValueString()
        {
            this.ResetColor();
            return this.Input.Text;
        }
    }
}

using GeneticFramework;
using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        private bool allowLetterConversion = false;
        public bool AllowLetterConversion
        {
            get
            {
                return allowLetterConversion;
            }
            set
            {
                this.allowLetterConversion = value;
            }
        }

        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

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
                if (this.AllowLetterConversion)
                {
                    int num = 0;
                    string content = this.Input.Text.Replace(" ", "").Replace(";", "").Replace(".", "").Replace(",", "").Replace(":", "").Reverse().ToLower();
                    for (int i = 0; i < content.Length; i++)
                    {
                        num += (Alphabet.IndexOf(content[i]) + 1) * (int)Math.Pow(26, i);
                    }
                    return num;
                }
                else
                {
                    SystemSounds.Asterisk.Play();
                    this.Label.Foreground = Brushes.Red;
                }
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

﻿using GeneticFramework;
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

            //Stelle die Ergebniss dar.
            for (int i = 1; i <= 5; i++)
            {
                for (int j = 1; j <= 11; j++)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = string.Join(", ", from k in this.Schedule.Courses where k.Periods.Contains(new Period((Weekday)i, (Hour)j)) select k.ID);
                    textBlock.TextAlignment = TextAlignment.Center;

                    Grid.SetColumn(textBlock, i);
                    Grid.SetRow(textBlock, j);

                    this.Timetable.Children.Add(textBlock);

                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace GAS
{
    public class Person
    {
        public Course[] Courses;
        public string Name = "";
        public string ID;

        private const double RATE_RANGES = 10.0;
        private const double RATE_PERIODS = 1.0;
        private const double RATE_FREEDAYS = 1.0;

        public Person(Course[] courses, string ID)
        {
            this.Courses = courses;
            this.ID = ID;
        }

        public bool IsFreeAt(Period period, Course[] exceptFor)
        {
            foreach (Course i in this.Courses)
            {
                if (!exceptFor.Contains(i))
                {
                    foreach (Period j in i.Periods)
                    {
                        if (j == period)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool IsFreeAt(Period period, Course exceptFor)
        {
            foreach (Course i in this.Courses)
            {
                if (exceptFor != i)
                {
                    foreach (Period j in i.Periods)
                    {
                        if (j == period)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool IsFreeAt(Period period)
        {
            foreach (Course i in this.Courses)
            {
                foreach (Period j in i.Periods)
                {
                    if (j == period)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public int Issues
        {
            get
            {
                //Berechne, wieviele Stunden in Konflikt kommen:
                int sum = 0;
                foreach (Course c in this.Courses)
                {
                    foreach (Period p in c.Periods)
                    {
                        if (!this.IsFreeAt(p, c)) sum++;
                    }
                }
                return sum / 2;
            }
        }

        public double GetScore()
        {
            //Berechne einen Score anhand von den Sunden:
            var periods = this.GetPeriods();
            return RATE_FREEDAYS * this.GetFreeDays() + RATE_PERIODS * IEnumerableExt.Average(periods, p => RatePeriod(p, periods)) + RATE_RANGES / this.GetAverageRange();
        }

        public static double RatePeriod(Period period, Period[] periods)
        {
            //Gewichte die einzelnen Stunden:
            double result = 1.0;
            if ((int)period.Hour <= 6)
            {
                result *= 2.0;
            }
            if (periods.Contains(p => period.IsNeighbourTo(p)))
            {
                result *= 2.0;
            }

            return result;
        }

        private double GetAverageRange()
        {
            //Schaue, um wie viele Stunden die einzelnen Wochentage der Person durchschnittlich auseinandergehen:
            (int, int)[] ranges = new (int, int)[5];

            foreach (Period period in this.GetPeriods())
            {
                try
                {
                    (int, int) value = ranges[(int)period.Weekday - 1];
                    value.Item1 = value.Item1 == 0 || value.Item1 > (int)period.Hour ? (int)period.Hour : value.Item1;
                    value.Item2 = value.Item2 == 0 || value.Item2 < (int)period.Hour ? (int)period.Hour : value.Item2;
                    ranges[(int)period.Weekday - 1] = value;
                }
                catch { }
            }

            return ArrayExt.Sum(ranges, t => t.Item2 - t.Item1) / (5.0 - this.GetFreeDays());
        }

        private Period[] GetPeriods()
        {
            //Gebe alle Stunden der Person zurück:
            HashSet<Period> periods = new();

            foreach (Course c in this.Courses)
            {
                foreach (Period p in c.Periods)
                {
                    periods.Add(p);
                }
            }

            return periods.ToArray();
        }

        private int GetFreeDays()
        {
            //Gebe die Anzahl der freien Wochentage zurück:
            HashSet<Weekday> weekdays = new();

            foreach (Course c in this.Courses)
            {
                foreach (Period period in c.Periods)
                {
                    weekdays.Add(period.Weekday);
                }
            }

            return 5 - weekdays.Count;
        }
    }

    public class Teacher : Person
    {
        public Teacher(Course[] courses, string ID) : base(courses, ID) { }

        public void AddToCourse(Course course)
        {
            course.Teacher.Courses = course.Teacher.Courses.RemoveFromArray(course);
            course.Teacher = this;
            this.Courses = this.Courses.AddToArray(course);
        }

        public void RemoveFromCourse(Course course, Teacher replaceBy)
        {
            course.Teacher = replaceBy;
            replaceBy.Courses = replaceBy.Courses.AddToArray(course);
            this.Courses = this.Courses.RemoveFromArray(course);
        }
    }

    public class Student : Person
    {
        public Student(Course[] courses, string ID) : base(courses, ID) { }

        public void AddToCourse(Course course)
        {
            course.Students = course.Students.AddToArray(this);
            this.Courses = this.Courses.AddToArray(course);
        }

        public void RemoveFromCourse(Course course)
        {
            course.Students = course.Students.RemoveFromArray(this);
            this.Courses = this.Courses.RemoveFromArray(course);
        }
    }
}

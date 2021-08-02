﻿using GeneticFramework;
using System;

namespace GAS
{
    public class Course
    {
        public Period[] Periods;
        public Student[] Students;
        public Teacher Teacher;
        public string ID;
        public Course[] PartnerCourses;

        public Course(int periods, Student[] students, Teacher teacher, string ID)
        {
            this.Periods = new Period[periods];
            this.Students = students;
            this.Teacher = teacher;
            this.ID = ID;
            this.PartnerCourses = new Course[0];
        }

        public Course(Period[] periods, Student[] students, Teacher teacher, string ID)
        {
            this.Periods = periods;
            this.Students = students;
            this.Teacher = teacher;
            this.ID = ID;
            this.PartnerCourses = new Course[0];
        }

        public bool AppliesForAll()
        {
            return this.Issues() == 0;
        }

        public int Issues()
        {
            int issues = 0;
            foreach (Student i in this.Students)
            {
                foreach (Period j in this.Periods)
                {
                    if (!i.IsFreeAt(j, this))
                    {
                        issues++;
                    }
                }
            }

            foreach (Period i in this.Periods)
            {
                if (!this.Teacher.IsFreeAt(i, this))
                {
                    issues++;
                }
            }

            return issues;
        }

        public bool CanPutItThere(Period period)
        {
            foreach (Period period1 in this.Periods)
            {
                try
                {
                    if (period1 == period)
                    {
                        return false;
                    }
                }
                catch { }
            }
            return true;
        }

        public void AddStudent(Student student)
        {
            this.Students = Utils.AddToArray(this.Students, student);
            student.Courses = Utils.AddToArray(student.Courses, this);
        }

        public void RemoveStudent(Student student)
        {
            this.Students = Utils.RemoveFromArray(this.Students, student);
            student.Courses = Utils.RemoveFromArray(student.Courses, this);
        }
    }

    /// <summary>
    /// Enum for saving a Weekday. Ranges from 1 to (including) 5.
    /// </summary>
    public enum Weekday
    {
        Monday = 1, Tuesday, Wednesday, Thursday, Friday
    }

    /// <summary>
    /// Enum for saving an Hour. Ranges from 1 to (including) 11.
    /// </summary>
    public enum Hour
    {
        First = 1, Second, Third, Fourth, Fifth, Sixth, Seventh, Eighth, Ninth, Tenth, Eleventh
    }

    /// <summary>
    /// Period contains the data for a period (weekday and hour). Weekday ranges from 1 to (including) 5, Hour from 1 to (including) 11.
    /// </summary>
    public struct Period
    {
        public Weekday Weekday;
        public Hour Hour;
        public Period(Weekday weekday, Hour hour)
        {
            this.Weekday = weekday;
            this.Hour = hour;
        }

        public static bool operator ==(Period period1, Period period2)
        {
            //Vergleiche, ob die beiden Structs gleiche Werte haben.
            return period1.Weekday == period2.Weekday && period1.Hour == period2.Hour;
        }

        public static bool operator !=(Period period1, Period period2)
        {
            //Vergleiche, ob die beiden Structs verschiedene Werte haben.
            return !(period1 == period2);
        }

        public static bool operator <(Period period1, Period period2)
        {
            //Vergleiche welche Stunde zuerst kommt:
            return (int)period1.Weekday < (int)period2.Weekday || (period1.Weekday == period2.Weekday && (int)period1.Hour < (int)period2.Hour);
        }

        public static bool operator <=(Period period1, Period period2)
        {
            //Vergleiche welche Stunde zuerst kommt:
            return (int)period1.Weekday < (int)period2.Weekday || (period1.Weekday == period2.Weekday && (int)period1.Hour <= (int)period2.Hour);
        }

        public static bool operator >(Period period1, Period period2)
        {
            //Vergleiche welche Stunde zuerst kommt:
            return !(period1 <= period2);
        }

        public static bool operator >=(Period period1, Period period2)
        {
            //Vergleiche welche Stunde zuerst kommt:
            return !(period1 < period2);
        }

        public static Period GetRandomPeriod(Course forCourse)
        {
            //Erstelle eine zufällige Stunde:
            Random random = new();
            Period period = new((Weekday)random.Next(1, 6), (Hour)random.Next(1, 12));

            //Gehe solange durch, bis eine passende Stunde gefunden wurde.
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (forCourse.CanPutItThere(period))
                    {
                        return period;
                    }
                    period.Hour = (Hour)((int)period.Hour % 11 + 1);
                }
                period.Weekday = (Weekday)((int)period.Weekday % 5 + 1);
            }

            //Falls nichts gefunden wurde, gebe eine Exception aus:
            throw new Schedule.Exceptions.PeriodNotFoundException();// Unerreichbarer Code in der Praxis.
        }

        public static Period GetRandomPeriod(Course forCourse, Course fromCourse)
        {
            //Suche eine zufällige Stunde aus.
            Random random = new();
            int index = random.Next(fromCourse.Periods.Length);

            //Gehe solange durch, bis eine passende Stunde gefunden wurde.
            for (int i = 0; i < fromCourse.Periods.Length; i++)
            {
                if (forCourse.CanPutItThere(fromCourse.Periods[(index + i) % fromCourse.Periods.Length]))
                {
                    return fromCourse.Periods[(index + i) % fromCourse.Periods.Length];
                }
            }

            //Falls nichts gefunden wurde, gib eine Exception aus:
            throw new Schedule.Exceptions.PeriodNotFoundException();
        }

        public static bool AreNeighbours(Period period1, Period period2)
        {
            //Überprüfe, ob die beiden Stunden aneinander angrenzen:
            return period1.Weekday == period2.Weekday && Math.Abs((int)period1.Hour - (int)period2.Hour) == 1;
        }

        public bool IsNeighbourTo(Period period)
        {
            //Überprüfe, ob die beiden Stunden aneinander angrenzen:
            return AreNeighbours(this, period);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

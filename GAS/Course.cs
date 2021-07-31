using System;
using GeneticFramework;

namespace GAS
{
    public class Course
    {
        public Period[] Periods;
        public Student[] Students;
        public Teacher Teacher;
        public string ID;

        public Course(int periods, Student[] students, Teacher teacher, string ID)
        {
            this.Periods = new Period[periods];
            this.Students = students;
            this.Teacher = teacher;
            this.ID = ID;
        }

        public Course(Period[] periods, Student[] students, Teacher teacher, string ID)
        {
            this.Periods = periods;
            this.Students = students;
            this.Teacher = teacher;
            this.ID = ID;
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

    public enum Weekday
    {
        Monday = 1, Tuesday, Wednesday, Thursday, Friday
    }

    public enum Hour
    {
        First = 1, Second, Third, Fourth, Fifth, Sixth, Seventh, Eighth, Ninth, Tenth, Eleventh
    }

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
            return period1.Weekday == period2.Weekday && period1.Hour == period2.Hour;
        }

        public static bool operator !=(Period period1, Period period2)
        {
            return !(period1 == period2);
        }

        public static Period GetRandomPeriod(Course forCourse)
        {
            Random random = new();
            Period period = new((Weekday)random.Next(1, 6), (Hour)random.Next(1, 12));
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
            return period; // Unerreichbarer Code in der Praxis.
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

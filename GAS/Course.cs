using GeneticFramework;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public int IssuesWith(Period period)
        {
            //Methode, die zurückgibt, wie viele Probleme der Kurs wegen einer bestimmten Stunde hat.
            return Utils.Sum(this.Students, (Student s) => s.IsFreeAt(period, this) ? 0 : 1) + (this.Teacher.IsFreeAt(period, this) ? 0 : 1);
        }

        public Person[] GetPeopleGroup()
        {
            //Schreibe in ein HashSet alle Personen hinein:
            HashSet<Person> people = new();

            //Schreibe zuerst die eigenen SuS und den eigenen LuL in das HashSet...
            foreach (Student i in this.Students)
            {
                people.Add(i);
            }
            people.Add(this.Teacher);

            //... und dann die der Partnerkurse.
            foreach (Course c in this.PartnerCourses)
            {
                foreach (Student s in c.Students)
                {
                    people.Add(s);
                }
                people.Add(c.Teacher);
            }

            //Rückgabe
            return (from n in people select n).ToArray();
        }

        public Student[] GetStudentsGroup()
        {
            //Schreibe in ein HashSet alle SuS hinein:
            HashSet<Student> students = new();

            //Schreibe zuerst die eigenen SuS in das HashSet...
            foreach (Student i in this.Students)
            {
                students.Add(i);
            }

            //... und dann die der Partnerkurse.
            foreach (Course c in this.PartnerCourses)
            {
                foreach (Student s in c.Students)
                {
                    students.Add(s);
                }
            }

            //Rückgabe
            return (from n in students select n).ToArray();
        }

        public (Course, Student)[] GetStudentsGroupPairs()
        {
            //Schreibe in eine Liste alle Schüler hinein:
            List<(Course, Student)> students = new();

            //Schreibe zuerst die eigenen SuS in die Liste...
            foreach (Student i in this.Students)
            {
                students.Add((this, i));
            }

            //... und dann die der Partnerkurse.
            foreach (Course c in this.PartnerCourses)
            {
                foreach (Student s in c.Students)
                {
                    students.Add((c, s));
                }
            }

            //Rückgabe
            return students.ToArray();
        }

        public Teacher[] GetTeachersGroup()
        {
            //Schreibe in ein HashSet alle LuL hinein:
            HashSet<Teacher> teachers = new();

            //Schreibe zuerst den eigenen LuL in das HashSet...
            teachers.Add(this.Teacher);

            //... und dann die der Partnerkurse.
            foreach (Course c in this.PartnerCourses)
            {
                teachers.Add(c.Teacher);
            }

            //Rückgabe
            return (from n in teachers select n).ToArray();
        }

        public Course[] GetGroup()
        {
            //Sammle alle Partnerkurse und sich selbst in einer Array:
            Course[] courses = new Course[this.PartnerCourses.Length + 1];
            for (int i = 0; i < this.PartnerCourses.Length; i++)
            {
                courses[i] = this.PartnerCourses[i];
            }
            courses[this.PartnerCourses.Length] = this;

            //Rückgabe
            return courses;
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

        public static Period[] GetAllPeriods()
        {
            //Erstelle ein Array, mit allen Stunden der Woche.
            Period[] periods = new Period[5 * 11];
            for (int i = 1; i <= 5; i++)
            {
                for (int j = 1; j <= 11; j++)
                {
                    periods[(i - 1) * 11 + j - 1] = new Period((Weekday)i, (Hour)j);
                }
            }

            //Rückgabe
            return periods;
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

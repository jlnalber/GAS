using System;
using System.Linq;

namespace GAS
{
    public class Person
    {
        public Course[] Courses;
        public string Name = "";
        public string ID;

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
    }

    public class Teacher : Person
    {
        public Teacher(Course[] courses, string ID) : base(courses, ID) { }
    }

    public class Student : Person
    {
        public Student(Course[] courses, string ID) : base(courses, ID) { }
    }
}

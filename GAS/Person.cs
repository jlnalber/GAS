using GeneticFramework;
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
            throw new NotImplementedException();
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

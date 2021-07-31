using System;
using System.Linq;
using GeneticFramework;

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

        public void AddToCourse(Course course)
        {
            course.Teacher.Courses = Utils.RemoveFromArray(course.Teacher.Courses, course);
            course.Teacher = this;
            this.Courses = Utils.AddToArray(this.Courses, course);
        }

        public void RemoveFromCourse(Course course, Teacher replaceBy)
        {
            course.Teacher = replaceBy;
            replaceBy.Courses = Utils.AddToArray(replaceBy.Courses, course);
            this.Courses = Utils.RemoveFromArray(this.Courses, course);
        }
    }

    public class Student : Person
    {
        public Student(Course[] courses, string ID) : base(courses, ID) { }

        public void AddToCourse(Course course)
        {
            course.Students = Utils.AddToArray(course.Students, this);
            this.Courses = Utils.AddToArray(this.Courses, course);
        }

        public void RemoveFromCourse(Course course)
        {
            course.Students = Utils.RemoveFromArray(course.Students, this);
            this.Courses = Utils.RemoveFromArray(this.Courses, course);
        }
    }
}

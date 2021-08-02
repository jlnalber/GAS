using GeneticFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GAS
{
    public class Schedule : Chromosome
    {
        private const double MUTATE_INCREMENTAL = 0.9;
        private const double MUTATE_INCREMENTAL_CHANGE_HOUR = 0.6;
        public Course[] Courses;

        public Schedule(Course[] courses)
        {
            this.Courses = courses;

            HashSet<string> IDs = new();
            HashSet<Person> people = new();

            foreach (Course i in this.Courses)
            {
                if (IDs.Contains(i.ID))
                {
                    throw new InvalidIDException();
                }
                IDs.Add(i.ID);

                foreach (Student j in i.Students)
                {
                    people.Add(j);
                }
                people.Add(i.Teacher);
            }

            foreach (Person i in people)
            {
                if (IDs.Contains(i.ID))
                {
                    throw new InvalidIDException();
                }
                IDs.Add(i.ID);
            }
        }

        public override (Chromosome, Chromosome) Crossover(Chromosome chromosome)
        {
            Random random = new();

            Schedule schedule1 = this.GetDeepCopy();
            Schedule schedule2 = (chromosome as Schedule).GetDeepCopy();

            Course course1 = schedule1.Courses[random.Next(schedule1.Courses.Length)];
            Course course2 = (from i in schedule2.Courses where i.ID == course1.ID select i).First();

            Period[] temp = course1.Periods;
            course1.Periods = course2.Periods;
            course2.Periods = temp;

            return (schedule1, schedule2);


            //TODO: Nur einzelne Stunden austauschen (VORSICHT: CanPutItThere).
        }

        public override double Fitness()
        {
            return 1.0 / (this.Issues() / 2 + 1);
        }

        public override Schedule GetRandomInstance()
        {
            Schedule newSchedule = this.GetDeepCopy();
            foreach (Course i in newSchedule.Courses)
            {
                for (int j = 0; j < i.Periods.Length; j++)
                {
                    i.Periods[j] = Period.GetRandomPeriod(i);
                }
            }
            return newSchedule;
        }

        public override void Mutate()
        {
            Random random = new();

            if (random.NextDouble() < MUTATE_INCREMENTAL)
            {
                Course course = this.Courses[random.Next(this.Courses.Length)];
                int pos = random.Next(course.Periods.Length);
                if (random.NextDouble() < 0.5)
                {
                    if (random.NextDouble() < MUTATE_INCREMENTAL_CHANGE_HOUR)
                    {
                        Period newPeriod = new(course.Periods[pos].Weekday, (Hour)((int)course.Periods[pos].Hour % 11 + 1));
                        if (course.CanPutItThere(newPeriod))
                        {
                            course.Periods[pos] = newPeriod;
                        }
                    }
                    else
                    {
                        Period newPeriod = new((Weekday)((int)course.Periods[pos].Weekday % 5 + 1), course.Periods[pos].Hour);
                        if (course.CanPutItThere(newPeriod))
                        {
                            course.Periods[pos] = newPeriod;
                        }
                    }
                }
                else
                {
                    if (random.NextDouble() < MUTATE_INCREMENTAL_CHANGE_HOUR)
                    {
                        int temp = (int)course.Periods[pos].Hour - 1;
                        Period newPeriod = new(course.Periods[pos].Weekday, temp == 0 ? Hour.Eleventh : (Hour)temp);
                        if (course.CanPutItThere(newPeriod))
                        {
                            course.Periods[pos] = newPeriod;
                        }
                    }
                    else
                    {
                        int temp = (int)course.Periods[pos].Weekday - 1;
                        Period newPeriod = new(temp == 0 ? Weekday.Friday : (Weekday)temp, course.Periods[pos].Hour);
                        if (course.CanPutItThere(newPeriod))
                        {
                            course.Periods[pos] = newPeriod;
                        }
                    }
                }
            }
            else
            {
                Course course = this.Courses[random.Next(this.Courses.Length)];
                course.Periods[random.Next(course.Periods.Length)] = Period.GetRandomPeriod(course);
            }
        }

        public Schedule GetDeepCopy()
        {
            //Sammle die Schüler und Lehrer.
            HashSet<Student> oldStudents = new();
            HashSet<Teacher> oldTeachers = new();
            foreach (Course i in this.Courses)
            {
                foreach (Student j in i.Students)
                {
                    oldStudents.Add(j);
                }
                oldTeachers.Add(i.Teacher);
            }

            //Kopiere die Schüler und Lehrer.
            Student[] newStudents = new Student[oldStudents.Count];
            Teacher[] newTeachers = new Teacher[oldTeachers.Count];
            int counter = 0;
            foreach (Student i in oldStudents)
            {
                Course[] courses = new Course[i.Courses.Length];
                for (int j = 0; j < i.Courses.Length; j++)
                {
                    courses[j] = i.Courses[j];
                }
                newStudents[counter] = new Student(courses, i.ID);
                newStudents[counter].Name = i.Name;
                counter++;
            }
            counter = 0;
            foreach (Teacher i in oldTeachers)
            {
                Course[] courses = new Course[i.Courses.Length];
                for (int j = 0; j < i.Courses.Length; j++)
                {
                    courses[j] = i.Courses[j];
                }
                newTeachers[counter] = new Teacher(courses, i.ID);
                newTeachers[counter].Name = i.Name;
                counter++;
            }

            //Kopiere die Kurse.
            Course[] newCourses = new Course[this.Courses.Length];
            for (int i = 0; i < this.Courses.Length; i++)
            {
                Course newCourse = new Course(this.Courses[i].Periods.Length, null, null, this.Courses[i].ID);

                //Zuerst die Zeitpläne...
                Period[] periods = new Period[this.Courses[i].Periods.Length];
                for (int j = 0; j < this.Courses[i].Periods.Length; j++)
                {
                    periods[j] = this.Courses[i].Periods[j];
                }
                newCourse.Periods = periods;

                //... dann die Schüler...
                Student[] students = new Student[this.Courses[i].Students.Length];
                for (int j = 0; j < students.Length; j++)
                {
                    Student student = (from s in newStudents where s.ID == this.Courses[i].Students[j].ID select s).First();
                    student.Courses = student.Courses.RemoveFromArray(this.Courses[i]);
                    student.Courses = student.Courses.AddToArray(newCourse);
                    students[j] = student;
                }
                newCourse.Students = students;

                //... und dann den Lehrer.
                Teacher teacher = (from t in newTeachers where t.ID == this.Courses[i].Teacher.ID select t).First();
                teacher.Courses = teacher.Courses.RemoveFromArray(this.Courses[i]);
                teacher.Courses = teacher.Courses.AddToArray(newCourse);
                newCourse.Teacher = teacher;

                //Zuweisung:
                newCourses[i] = newCourse;
            }
            //Kopiere die Partnerkurse:
            for (int i = 0; i < this.Courses.Length; i++)
            {
                newCourses[i].PartnerCourses = new Course[this.Courses[i].PartnerCourses.Length];
                for (int j = 0; j < this.Courses[i].PartnerCourses.Length; j++)
                {
                    newCourses[i].PartnerCourses[j] = (from k in newCourses where k.ID == this.Courses[i].PartnerCourses[j].ID select k).First();
                }
            }

            //Rückgabe
            return new Schedule(newCourses);
        }

        public bool AllApplies()
        {
            return this.Issues() == 0;
        }

        public int Issues()
        {
            int issues = 0;
            foreach (Course i in this.Courses)
            {
                issues += i.Issues();
            }
            return issues;
        }

        public void Optimize()
        {
            throw new NotImplementedException();
        }

        public class InvalidIDException : Exception
        {
            public InvalidIDException(string message) : base(message) { }
            public InvalidIDException() : base() { }
        }
    }
}

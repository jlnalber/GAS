using GeneticFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GAS
{
    public class Schedule : Chromosome
    {
        //Konstanten für den Zufall:
        private const double MUTATE_INCREMENTAL = 0.9;
        private const double MUTATE_INCREMENTAL_CHANGE_HOUR = 0.6;
        private const double MUTATE_PARTICIPANTS = 0.1;
        private const double CROSSOVER_CROSS_ONE_PERIOD = 0.5;
        private const double CROSSOVER_CROSS_PARTICIPANTS = 0.1;
        private const double CROSSOVER_CROSS_TEACHERS = 0.1;
        private const double ADDITION_RANDOM_INSTANCE_CHOICES = 4.0;

        public Course[] Courses;

        public Schedule(Course[] courses)
        {
            //Lege die Kurse fest:
            this.Courses = courses;

            //Überprüfe, ob IDs mehrfach vorkommen, wenn ja löse eine Exception aus:
            HashSet<string> IDs = new();
            HashSet<Person> people = new();
            foreach (Course i in this.Courses)
            {
                if (IDs.Contains(i.ID))
                {
                    throw new Exceptions.InvalidIDException();
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
                    throw new Exceptions.InvalidIDException();
                }
                IDs.Add(i.ID);
            }
        }

        public override (Chromosome, Chromosome) Crossover(Chromosome chromosome)
        {
            //Kopiere die Pläne und wähle zufällig Kurse für das Crossover aus.
            Random random = new();

            Schedule schedule1 = this.GetDeepCopy();
            Schedule schedule2 = (chromosome as Schedule).GetDeepCopy();

            Course course1 = schedule1.Courses[random.Next(schedule1.Courses.Length)];
            Course course2 = (from i in schedule2.Courses where i.ID == course1.ID select i).First();

            //Crossover mit den Teilnehmern:
            if (course1.PartnerCourses.Length != 0 && random.NextDouble() < CROSSOVER_CROSS_PARTICIPANTS)
            {
                if (random.NextDouble() < CROSSOVER_CROSS_TEACHERS)
                {
                    //Mache ein Crossover mit den Lehrern:
                    Teacher[] teachers1 = course1.GetTeachersGroup();
                    Teacher[] teachers2 = course2.GetTeachersGroup();

                    //Tausche zuerst die eigenen Lehrer aus...
                    Teacher t = course1.Teacher;
                    course1.Teacher = (from i in teachers1 where i.ID == course2.Teacher.ID select i).First();
                    course2.Teacher = (from i in teachers2 where i.ID == t.ID select i).First();

                    //... und dann die der Partnerkurse.
                    for (int i = 0; i < course1.PartnerCourses.Length; i++)
                    {
                        Teacher temp = course1.PartnerCourses[i].Teacher;
                        course1.PartnerCourses[i].Teacher = (from j in teachers1 where j.ID == course2.PartnerCourses[i].Teacher.ID select j).First();
                        course2.PartnerCourses[i].Teacher = (from j in teachers2 where j.ID == temp.ID select j).First();
                    }
                }
                else
                {
                    //Mache ein Crossover mit den Schülern:
                    Student[] students1 = course1.GetStudentsGroup();
                    Student[] students2 = course2.GetStudentsGroup();

                    //Tausche zuerst die eigenen Schüler aus...
                    Student[] t = course1.Students;
                    course1.Students = (from i in students1 where course2.Students.Contains((Student s) => s.ID == i.ID) select i).ToArray();
                    course2.Students = (from i in students2 where t.Contains((Student s) => s.ID == i.ID) select i).ToArray();

                    //... und dann die der Partnerkurse.
                    for (int i = 0; i < course1.PartnerCourses.Length; i++)
                    {
                        Student[] temp = course1.PartnerCourses[i].Students;
                        course1.PartnerCourses[i].Students = (from j in students1 where course2.PartnerCourses[i].Students.Contains((Student s) => s.ID == j.ID) select j).ToArray();
                        course2.PartnerCourses[i].Students = (from j in students2 where temp.Contains((Student s) => s.ID == j.ID) select j).ToArray();
                    }
                }

                //Rückgabe
                return (schedule1, schedule2);
            }
            //Crossover mit den Stunden:
            else
            {
                //Vertausche mit einer gewissen Wahrscheinlichkeit zwei Stunden miteinander...
                if (random.NextDouble() < CROSSOVER_CROSS_ONE_PERIOD)
                {
                    try
                    {
                        course1.Periods[random.Next(course1.Periods.Length)] = Period.GetRandomPeriod(course1, course2);
                        course2.Periods[random.Next(course2.Periods.Length)] = Period.GetRandomPeriod(course2, course1);
                        return (schedule1, schedule2);
                    }
                    catch { }
                }

                //... und sonst zwei komplette Zeitpläne für einen Kurs.
                Period[] temp = course1.Periods;
                course1.Periods = course2.Periods;
                course2.Periods = temp;

                return (schedule1, schedule2);
            }
        }

        public override double Fitness()
        {
            return 1.0 / (this.Issues() / 2 + 1);
        }

        public override Schedule GetRandomInstance()
        {
            //Methode, die eine zufällige Instanz zurückgibt
            //Kopiere den Plan, füge zufällige Stunden hinzu, aber wahrscheinlicher so, dass es 
            Schedule newSchedule = this.GetDeepCopy();
            Random random = new();
            foreach (Course i in newSchedule.Courses)
            {
                for (int j = 0; j < i.Periods.Length; j++)
                {
                    i.Periods[j] = Utils.Choices((from p in Period.GetAllPeriods() select (p, i.CanPutItThere(p) ? (random.NextDouble() + ADDITION_RANDOM_INSTANCE_CHOICES) / (i.IssuesWith(p) + 1) : 0.0)).ToArray());
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

        public HashSet<Person> GetPeople()
        {
            HashSet<Person> people = new();
            foreach (Course c in this.Courses)
            {
                people.Add(c.Teacher);
                foreach (Student s in c.Students)
                {
                    people.Add(s);
                }
            }
            return people;
        }

        public double GetScore()
        {
            return Utils.Average(this.GetPeople(), (Person p) => p.GetScore());
        }

        public class Exceptions
        {
            public class InvalidIDException : Exception
            {
                public InvalidIDException(string message) : base(message) { }
                public InvalidIDException() : base() { }
            }

            public class PeriodNotFoundException : Exception
            {
                public PeriodNotFoundException(string message) : base(message) { }
                public PeriodNotFoundException() : base() { }
            }
        }
    }
}

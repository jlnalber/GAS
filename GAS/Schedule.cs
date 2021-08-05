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
        private const double MUTATE_TEACHERS = 0.1;
        private const double MUTATE_TEACHERS_NEW_COURSE = 0.2;
        private const double MUTATE_STUDENTS_NEW_COURSE = 0.2; 
        private const double CROSSOVER_CROSS_ONE_PERIOD = 0.5;
        private const double CROSSOVER_CROSS_PARTICIPANTS = 0.1;
        private const double CROSSOVER_CROSS_TEACHERS = 0.1;
        private const double ADDITION_RANDOM_INSTANCE_CHOICES = 4.0;
        private const double ADDITION_CHOOSE_COURSE = 5.0;
        private const double ADDITION_CHOOSE_TEACHER_STUDENT = 2.0;
        private const double ADDITION_CHOOSE_PERIOD = 1.0;

        //Die Kurse:
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
            //Kopiere die Pläne und wähle zufällig Kurse mit vielen Issues für das Crossover aus.
            Random random = new();

            Schedule schedule1 = this.GetDeepCopy();
            Schedule schedule2 = (chromosome as Schedule).GetDeepCopy();

            Course course1 = Utils.Choices((from i in schedule1.Courses select (i, (i.Issues() + 1.0) * (random.NextDouble() + ADDITION_CHOOSE_COURSE))).ToArray());
            Course course2 = (from i in schedule2.Courses where i.ID == course1.ID select i).First();

            //Crossover mit den Teilnehmern:
            if ((course1.PartnerCourses.Length != 0 && random.NextDouble() < CROSSOVER_CROSS_PARTICIPANTS && !course1.FixParticipants) || (!course1.FixParticipants && course1.FixPeriods && course1.PartnerCourses.Length != 0))
            {
                if (random.NextDouble() < CROSSOVER_CROSS_TEACHERS)
                {
                    //Mache ein Crossover mit den Lehrern:
                    Teacher[] teachers1 = course1.GetTeachersGroup();
                    Teacher[] teachers2 = course2.GetTeachersGroup();

                    //Sammle die ganzen Kurse in einer Gruppe:
                    Course[] courses1 = course1.GetGroup();
                    Course[] courses2 = course2.GetGroup();

                    //Speichere von beiden Gruppen die IDs ab:
                    (string, string)[] ID1 = new (string, string)[courses1.Length];
                    (string, string)[] ID2 = new (string, string)[courses2.Length];//In der Praxis sind die beiden Arrays gleich lang, weil immer die korrespondierenden Kurse ausgewählt wurden...
                    for (int i = 0; i < courses1.Length; i++)
                    {
                        ID1[i] = (courses1[i].ID, courses1[i].Teacher.ID);
                        ID2[i] = (courses2[i].ID, courses2[i].Teacher.ID);
                    }

                    //Tausche die LuL aus, so wie es jeweils beim korrespondierenden Kurs der Fall ist:
                    foreach ((string, string) IDs in ID2)
                    {
                        //curCourse ist der jetzige Kurs, teacher der jetzige Lehrer, oldCourse ist der alte Kurs in den Partnerkursen von course1, in dem teacher war.
                        Course curCourse = (from i in courses1 where i.ID == IDs.Item1 select i).First();
                        Teacher teacher = (from i in teachers1 where i.ID == IDs.Item2 select i).First();
                        Course oldCourse = (from i in courses1 where i.Teacher == teacher select i).First();

                        teacher.RemoveFromCourse(oldCourse, curCourse.Teacher);
                        teacher.AddToCourse(curCourse);
                    }
                    foreach ((string, string) IDs in ID1)
                    {
                        //curCourse ist der jetzige Kurs, teacher der jetzige Lehrer, oldCourse ist der alte Kurs in den Partnerkursen von course1, in dem teacher war.
                        Course curCourse = (from i in courses2 where i.ID == IDs.Item1 select i).First();
                        Teacher teacher = (from i in teachers2 where i.ID == IDs.Item2 select i).First();
                        Course oldCourse = (from i in courses2 where i.Teacher == teacher select i).First();

                        teacher.RemoveFromCourse(oldCourse, curCourse.Teacher);
                        teacher.AddToCourse(curCourse);
                    }
                }
                else
                {
                    //Mache ein Crossover mit den Schülern:
                    Student[] students1 = course1.GetStudentsGroup();
                    Student[] students2 = course2.GetStudentsGroup();

                    //Sammle die ganzen Kurse in einer Gruppe:
                    Course[] courses1 = course1.GetGroup();
                    Course[] courses2 = course2.GetGroup();

                    //Speichere die IDs der Schüler ab:
                    (string, string[])[] ID1 = new (string, string[])[courses1.Length];
                    (string, string[])[] ID2 = new (string, string[])[courses2.Length];//In der Praxis sind die beiden Arrays gleich lang, weil immer die korrespondierenden Kurse ausgewählt wurden...
                    for (int i = 0; i < courses1.Length; i++)
                    {
                        ID1[i] = (courses1[i].ID, courses1[i].Students.Transform((Student s) => s.ID));
                        ID2[i] = (courses2[i].ID, courses2[i].Students.Transform((Student s) => s.ID));
                    }

                    //Tausche die SuS aus, so wie es jeweils beim korrespondierenden Kurs der Fall ist:
                    foreach ((string, string[]) IDs in ID2)
                    {
                        Course curCourse = (from i in courses1 where i.ID == IDs.Item1 select i).First();
                        Student[] students = (from i in students1 where IDs.Item2.Contains(i.ID) select i).ToArray();

                        foreach (Student s in students)
                        {
                            Course oldCourse = (from i in s.Courses where courses1.Contains(i) select i).First();
                            s.RemoveFromCourse(oldCourse);
                            s.AddToCourse(curCourse);
                        }
                    }
                    foreach ((string, string[]) IDs in ID1)
                    {
                        Course curCourse = (from i in courses2 where i.ID == IDs.Item1 select i).First();
                        Student[] students = (from i in students2 where IDs.Item2.Contains(i.ID) select i).ToArray();

                        foreach (Student s in students)
                        {
                            Course oldCourse = (from i in s.Courses where courses2.Contains(i) select i).First();
                            s.RemoveFromCourse(oldCourse);
                            s.AddToCourse(curCourse);
                        }
                    }
                }
            }
            //Crossover mit den Stunden:
            else if (!course1.FixPeriods)
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
            }

            //Rückgabe
            return (schedule1, schedule2);
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
                for (int j = 0; j < i.Periods.Length && !i.FixPeriods; j++)
                {
                    i.Periods[j] = Utils.Choices((from p in Period.GetAllPeriods() select (p, i.CanPutItThere(p) ? (random.NextDouble() + ADDITION_RANDOM_INSTANCE_CHOICES) / (i.IssuesWith(p) + 1) : 0.0)).ToArray());
                }
            }
            return newSchedule;
        }

        public override void Mutate()
        {
            //Erstelle eine Zufallsvariable und wähle einen Kurs je nach Issues aus:
            Random random = new();
            Course course = Utils.Choices((from i in this.Courses select (i, (i.Issues() + 1.0) * (random.NextDouble() + ADDITION_CHOOSE_COURSE))).ToArray());

            //Mutiere die Teilnehmer...
            if ((course.PartnerCourses.Length != 0 && random.NextDouble() < MUTATE_PARTICIPANTS && !course.FixParticipants) || (!course.FixParticipants && course.FixPeriods && course.PartnerCourses.Length != 0))
            {
                //Tausche Lehrer:
                if (random.NextDouble() < MUTATE_TEACHERS)
                {
                    //Mache eine komplett neue Zuteilung der Lehrer:
                    if (random.NextDouble() < MUTATE_TEACHERS_NEW_COURSE)
                    {
                        //Hole die Gruppe von Kursen und erstelle einen Platzhalter-Lehrer:
                        Course[] courses = course.GetGroup();
                        Teacher dummy = new Teacher(new Course[0], "");

                        //Erstelle jeweils Paare aus Kurs und dazugehörogen Lehrer, entferne die Lehrer aus ihren Kursen:
                        (Course, Teacher)[] oldTupels = (from i in courses select (i, i.Teacher)).ToArray();
                        foreach((Course, Teacher) i in oldTupels)
                        {
                            i.Item2.RemoveFromCourse(i.Item1, dummy);
                        }

                        //Mische die Kurse und Lehrer neu und füge die Lehrer zu ihren neuen Kursen hinzu:
                        (Course, Teacher)[] newTupels = Utils.ShuffleTupels(oldTupels);
                        foreach ((Course, Teacher) i in newTupels)
                        {
                            i.Item2.AddToCourse(i.Item1);
                        }
                    }
                    //Tausche je zwei Lehrer miteinander aus:
                    else
                    {
                        //Wähle einen zufälligen Partnerkurs aus und speichere seinen Lehrer temporär ab:
                        Course course2 = Utils.Choices((from i in course.PartnerCourses select (i, i.Teacher.Issues + ADDITION_CHOOSE_TEACHER_STUDENT)).ToArray());
                        Teacher temp = course2.Teacher;

                        //Tausche die beiden Lehrer aus:
                        temp.RemoveFromCourse(course2, course.Teacher);
                        course.Teacher.RemoveFromCourse(course, temp);
                    }
                }
                //Tausche Schüler:
                else
                {
                    //Mache eine komplett neue Zuteilung der Schüler:
                    if (random.NextDouble() < MUTATE_STUDENTS_NEW_COURSE)
                    {
                        //Erstelle die Tupel (jeweils jeder Schüler mit seinem Kurs in der Gruppe), entferne die SuS aus den Kursen:
                        (Course, Student)[] oldTupels = course.GetStudentsGroupPairs();
                        foreach ((Course, Student) i in oldTupels)
                        {
                            i.Item2.RemoveFromCourse(i.Item1);
                        }

                        //Vermische die Kurse und die SuS, füge dann nach der neuen Zuordnung die Schüler ihren neuen Kursen hinzu:
                        (Course, Student)[] newTupels = Utils.ShuffleTupels(oldTupels);
                        foreach ((Course, Student) i in newTupels)
                        {
                            i.Item2.AddToCourse(i.Item1);
                        }
                    }
                    //Tausche je zwei Schüler miteinander aus:
                    else
                    {
                        //Wähle einen zufälligen Partnerkurs aus wähle zwei zufällige Schüler aus Kurs und Partnerkurs:
                        Course course2 = course.PartnerCourses[random.Next(course.PartnerCourses.Length)];
                        Student student1 = Utils.Choices((from i in course.Students select (i, i.Issues + ADDITION_CHOOSE_TEACHER_STUDENT)).ToArray());
                        Student student2 = Utils.Choices((from i in course2.Students select (i, i.Issues + ADDITION_CHOOSE_TEACHER_STUDENT)).ToArray());

                        //Tausche die Schüler miteinander aus:
                        student1.RemoveFromCourse(course);
                        student1.AddToCourse(course2);
                        student2.RemoveFromCourse(course2);
                        student2.AddToCourse(course);
                    }
                }
            }
            //... oder die Stunden.
            else if (!course.FixPeriods)
            {
                //Verschiebe Kurse jeweils um 1 hin und her...
                if (random.NextDouble() < MUTATE_INCREMENTAL)
                {
                    //Wähle eine Stunde aus:
                    int pos = course.Periods.IndexOf(Utils.Choices((from i in course.Periods select (i, course.IssuesWith(i) + ADDITION_CHOOSE_PERIOD)).ToArray()));

                    //Verschiebe die Stunde nach vorne um eins:
                    if (random.NextDouble() < 0.5)
                    {
                        //Um eine Stunde:
                        if (random.NextDouble() < MUTATE_INCREMENTAL_CHANGE_HOUR)
                        {
                            Period newPeriod = new(course.Periods[pos].Weekday, (Hour)((int)course.Periods[pos].Hour % 11 + 1));
                            if (course.CanPutItThere(newPeriod))
                            {
                                course.Periods[pos] = newPeriod;
                            }
                        }
                        //Um einen Tag:
                        else
                        {
                            Period newPeriod = new((Weekday)((int)course.Periods[pos].Weekday % 5 + 1), course.Periods[pos].Hour);
                            if (course.CanPutItThere(newPeriod))
                            {
                                course.Periods[pos] = newPeriod;
                            }
                        }
                    }
                    //Verschiebe die Stunde nach hinten um eins:
                    else
                    {
                        //Um eine Stunde:
                        if (random.NextDouble() < MUTATE_INCREMENTAL_CHANGE_HOUR)
                        {
                            int temp = (int)course.Periods[pos].Hour - 1;
                            Period newPeriod = new(course.Periods[pos].Weekday, temp == 0 ? Hour.Eleventh : (Hour)temp);
                            if (course.CanPutItThere(newPeriod))
                            {
                                course.Periods[pos] = newPeriod;
                            }
                        }
                        //Um einen Tag:
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
                //... oder verschiebe sie an eine ganz neue Stelle.
                else
                {
                    course.Periods[course.Periods.IndexOf(Utils.Choices((from i in course.Periods select (i, course.IssuesWith(i) + ADDITION_CHOOSE_PERIOD)).ToArray()))] = Period.GetRandomPeriod(course);
                }
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

                //Kopiere die Daten über die fixen Stunden und die fixen SuS:
                newCourse.FixPeriods = this.Courses[i].FixPeriods;
                newCourse.FixParticipants = this.Courses[i].FixParticipants;

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
            //Überprüfe, ob es keine Issues mehr gibt:
            return this.Issues() == 0;
        }

        public int Issues()
        {
            //Zähle die Issues der Kurse und gebe sie zurück:
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
            //Sammle alle Teilnehmer des Stundenplans in einem HashSet und gebe es zurück:
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
            //Berechne einen Durchschnitts Score:
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

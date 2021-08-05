using GeneticFramework;
using System.Collections.Generic;
using System.Linq;

namespace GAS
{
    public class GroupCourse : Course
    {
        public Teacher[] Teachers;

        public GroupCourse(int periods, Student[] students, Teacher[] teachers, string ID) : base(periods, students, teachers[0], ID)
        {
            this.Teachers = teachers;
        }

        public GroupCourse(Period[] periods, Student[] students, Teacher[] teachers, string ID) : base(periods, students, teachers[0], ID)
        {
            this.Teachers = teachers;
        }

        public void AddTeacher(Teacher teacher)
        {
            this.Teachers = this.Teachers.AddToArray(teacher);
            teacher.Courses = teacher.Courses.AddToArray(this);
        }

        public void RemoveTeacher(Teacher teacher)
        {
            this.Teachers = this.Teachers.RemoveFromArray(teacher);
            teacher.Courses = teacher.Courses.RemoveFromArray(this);
        }

        public Course[] Split()
        {
            //Erstelle die Kurse:
            Course[] courses = new Course[this.Teachers.Length];
            for (int i = 0; i < this.Teachers.Length; i++)
            {
                Teacher teacher = this.Teachers[i];
                string ID = courses.Length == 1 ? this.ID : this.ID + "_" + (i + 1);
                courses[i] = new(this.Periods, new Student[0], teacher, ID);
                teacher.Courses = teacher.Courses.RemoveFromArray(this);
                teacher.Courses = teacher.Courses.AddToArray(courses[i]);
            }
            for (int i = 0; i < courses.Length; i++)
            {
                courses[i].PartnerCourses = courses.Filter((Course c) => c != courses[i]);
                courses[i].FixParticipants = this.FixParticipants;
                courses[i].FixPeriods = this.FixPeriods;
            }

            //Verteile die SuS:
            for (int i = 0; i < this.Students.Length; i++)
            {
                Student student = this.Students[i];
                Course course = courses[i % this.Teachers.Length];
                course.Students = course.Students.AddToArray(student);
                student.Courses = student.Courses.RemoveFromArray(this);
                student.Courses = student.Courses.AddToArray(course);
            }

            //Rückgabe
            return courses;
        }

        public static GroupCourse[] GetDeepCopy(GroupCourse[] courses)
        {
            //Sammle die Schüler und Lehrer.
            HashSet<Student> oldStudents = new();
            HashSet<Teacher> oldTeachers = new();
            foreach (GroupCourse i in courses)
            {
                foreach (Student j in i.Students)
                {
                    oldStudents.Add(j);
                }
                foreach (Teacher j in i.Teachers)
                {
                    oldTeachers.Add(j);
                }
            }

            //Kopiere die Schüler und Lehrer.
            Student[] newStudents = new Student[oldStudents.Count];
            Teacher[] newTeachers = new Teacher[oldTeachers.Count];
            int counter = 0;
            foreach (Student i in oldStudents)
            {
                GroupCourse[] coursesS = new GroupCourse[i.Courses.Length];
                for (int j = 0; j < i.Courses.Length; j++)
                {
                    coursesS[j] = i.Courses[j] as GroupCourse;
                }
                newStudents[counter] = new Student(courses, i.ID);
                newStudents[counter].Name = i.Name;
                counter++;
            }
            counter = 0;
            foreach (Teacher i in oldTeachers)
            {
                GroupCourse[] coursesT = new GroupCourse[i.Courses.Length];
                for (int j = 0; j < i.Courses.Length; j++)
                {
                    coursesT[j] = i.Courses[j] as GroupCourse;
                }
                newTeachers[counter] = new Teacher(courses, i.ID);
                newTeachers[counter].Name = i.Name;
                counter++;
            }

            //Kopiere die Kurse.
            GroupCourse[] newCourses = new GroupCourse[courses.Length];
            for (int i = 0; i < courses.Length; i++)
            {
                GroupCourse newCourse = new GroupCourse(courses[i].Periods.Length, null, new Teacher[1] { new Teacher(null, null) }, courses[i].ID);

                //Kopiere die Daten...
                newCourse.FixPeriods = courses[i].FixPeriods;
                newCourse.FixParticipants = courses[i].FixParticipants;

                //... dann die Stunden...
                for (int j = 0; j < courses[i].Periods.Length; j++)
                {
                    newCourse.Periods[j] = courses[i].Periods[j];
                }

                //... dann die Schüler...
                Student[] students = new Student[courses[i].Students.Length];
                for (int j = 0; j < students.Length; j++)
                {
                    Student student = (from s in newStudents where s.ID == courses[i].Students[j].ID select s).First();
                    student.Courses = student.Courses.RemoveFromArray(courses[i]);
                    student.Courses = student.Courses.AddToArray(newCourse);
                    students[j] = student;
                }
                newCourse.Students = students;

                //... und dann die Lehrer.
                Teacher[] teachers = new Teacher[courses[i].Teachers.Length];
                for (int j = 0; j < teachers.Length; j++)
                {
                    Teacher teacher = (from s in newTeachers where s.ID == courses[i].Teachers[j].ID select s).First();
                    teacher.Courses = teacher.Courses.RemoveFromArray(courses[i]);
                    teacher.Courses = teacher.Courses.AddToArray(newCourse);
                    teachers[j] = teacher;
                }
                newCourse.Teachers = teachers;

                //Zuweisung:
                newCourses[i] = newCourse;
            }

            //Rückgabe
            return newCourses;
        }
    }
}

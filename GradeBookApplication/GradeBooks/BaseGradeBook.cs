using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GradeBookApplication.GradeBooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GradeBookApplication.Enums;

namespace GradeBookApplication.GradeBooks
{
    public class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }

        public BaseGradeBook(string name)
        {
            Name = name;
            Students = new List<Student>();
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A name is required to add a student to a gradeBook.");
            Students.Add(student);
            
        }

        public void RemoveStudent(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A name is required to remove a student from a gradeBook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, please try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            switch (letterGrade)
            {
                case 'A':
                    return 4;
                case 'B':
                    return 3;
                case 'C':
                    return 2;
                case 'D':
                    return 1;
            }
            return 0;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationPoints = 0d;
            var standardPoints = 0d;
            var honourPoints = 0d;
            var dualEnrollmentPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honours:
                        honourPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrollmentPoints += student.AverageGrade;
                        break;
                }

                Console.WriteLine("Average grade for all students is:" + (allStudentsPoints / Students.Count));
                if (campusPoints != 0)
                    Console.WriteLine("Average for only local students is: " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
                if (statePoints != 0)
                    Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
                if (nationalPoints != 0)
                    Console.WriteLine("Average for only national students (excluding local and state) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
                if (internationPoints != 0)
                    Console.WriteLine("Average for only international students is " + (internationPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
                if (standardPoints != 0)
                    Console.WriteLine("Average for students excluding honours and dual enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
                if (honourPoints != 0)
                    Console.WriteLine("Average for only honour students is " + (honourPoints / Students.Where(e => e.Type == StudentType.Honours).Count()));
                if (dualEnrollmentPoints != 0)
                    Console.WriteLine("Average for only dual enrollment students is " + (dualEnrollmentPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
            }
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        public static dynamic ConvertToGradeBook(string json)
        {
            var gradeBookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                from type in assembly.GetTypes()
                                where type.FullName == "GradeBook.Enums.GradeBookType"
                                select type).FirstOrDefault();

            var jObject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jObject.Property("Type")?.Value?.ToString();

            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradeBookEnum, int.Parse(gradeBookType));
            }

            var gradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();

            if (gradeBook == null)
                gradeBook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();

            return JsonConvert.DeserializeObject(json, gradeBook);
        }
    }
}

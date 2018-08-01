using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

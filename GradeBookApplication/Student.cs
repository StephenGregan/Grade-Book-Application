using GradeBookApplication.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeBookApplication
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }

        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollmentType)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollmentType;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
                Grades.Add(grade);
            
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

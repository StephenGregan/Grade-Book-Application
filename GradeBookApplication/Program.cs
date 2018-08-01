using GradeBookApplication.UserInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeBookApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#================#");
            Console.WriteLine("# Welcome to GradeBook #");
            Console.WriteLine("#================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();

            Console.WriteLine("Thank yopu for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

using GradeBookApplication.GradeBooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeBookApplication.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What wouldm you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
            {
                CreateCommand(command);
            }
            else if (command.StartsWith("load"))
            {
                LoadCommand(command);
            }
            else if (command == "help")
            {
                HelpCommand(command);
            }
            else if (command == "quit")
            {
                Quit = true;
            }
            else
            {
                Console.WriteLine("{0} was not recognized, please try again.", command);
            }
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Create requires a name.");
                return;
            }
            var name = parts[1];
            BaseGradeBook gradebook = new BaseGradeBook(name);
            Console.WriteLine("Created greadeBook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradebook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
            {
                return;
            }
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand(string command)
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' - Creates a new gradeBook where 'Name' is the name of gradeBook.");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradeBook with the provided 'Name'");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all excepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exists the application");
        }
    }
}

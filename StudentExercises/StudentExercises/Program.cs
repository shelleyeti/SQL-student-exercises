using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StudentExercises
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}


//Add the following to your program:
//Find all the students in the database.Include each student's cohort AND each student's list of exercises.
//Write a method in the Repository class that accepts an Exercise and
//a Cohort and assigns that exercise to each student in the cohort IF and ONLY IF the student has not already been assigned the exercise.


//Advanced Challenge
//NOTE: Only work on this challenge if you've completed ALL the other exercises assigned during Orientation.

//Modify your program to present the user with a menu and accept input from the user using the Console.ReadLine() method.Use the following program as an example for creating a menu.
//using System;
//using System.Linq;
//namespace UserInputExample
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            while (true)
//            {
//                Console.WriteLine();
//                Console.WriteLine("------------------------");
//                Console.WriteLine("Choose a menu option:");
//                Console.WriteLine("1. Shout it out.");
//                Console.WriteLine("2. Reverse it.");
//                Console.WriteLine("3. Exit.");

//                string option = Console.ReadLine();
//                if (option == "1")
//                {
//                    Console.Write("What should I shout? ");
//                    string input = Console.ReadLine();
//                    Console.WriteLine(input.ToUpper() + "!!!");
//                }
//                else if (option == "2")
//                {
//                    Console.Write("What should I reverse? ");
//                    string input = Console.ReadLine();
//                    Console.WriteLine(new string(input.Reverse().ToArray()));
//                }
//                else if (option == "3")
//                {
//                    Console.WriteLine("Goodbye");
//                    break; // break out of the while loop
//                }
//                else
//                {
//                    Console.WriteLine($"Invalid option: {option}");
//                }
//            }
//        }
//    }
//}
//Create menu options to allow the user to perform the following tasks:
//Display all students.
//Display all instructors.
//Display all exercises.
//Display all cohorts.
//Search students by last name.
//Create a new cohort.
//Create a new student and assign them to an existing cohort.
//Create a new instructor and assign them to an existing cohort.
//Display all students in a given cohort.
//Move an existing student to another existing cohort.
//List the exercises for a given student.
//Assign an existing exercise to an existing student.

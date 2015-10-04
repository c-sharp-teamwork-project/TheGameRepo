using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameClasses
{
    class Program
    {
        static string GetValidInput()
        {
            // Method works with lowercase and uppercase characters from a-j, 
            // including whitespaces in the beginning, middle or end
            Regex withDirectionRGX = new Regex(@"^[a-jA-J]\s*[\d]\s*[udlrUDLR]\s*$");
            Regex withoutDirectionRGX = new Regex(@"^[a-jA-J]\s*[\d]\s*$");
            Regex directionRGX = new Regex(@"^\s*[udlrUDLR]\s*$");

            Console.WriteLine("Where to place your ship?");
            while (true)
            {
                string command = Console.ReadLine();
                if (withDirectionRGX.Match(command).Success)
                {
                    command = command.Replace(@"s+", "").ToLower();
                    return command;
                }
                else if (withoutDirectionRGX.Match(command).Success)
                {
                    Console.WriteLine("Give me direction!");
                    while (true)
                    {
                        string direction = Console.ReadLine();
                        if (directionRGX.Match(direction).Success)
                        {
                            command = command.Replace(@"s+", "").ToLower();
                            direction = direction.Replace(@"s+", "").ToLower();
                            return command + direction;
                        }
                        Console.WriteLine("Ughh, can you repeat directions!");
                    }
                }
                Console.WriteLine("Arrgh! I didnt get that..");
            }
        }

        static void Main(string[] args)
        {
            string command = GetValidInput();
        }
    }
}

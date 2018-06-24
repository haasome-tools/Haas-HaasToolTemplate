using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaasToolTemplate
{
    class Utils
    {
        public static void ShowBanner()
        {
            Console.WriteLine(" _    _              _______          _ ");
            Console.WriteLine("| |  | |            |__   __|        | |");
            Console.WriteLine("| |__| | __ _  __ _ ___| | ___   ___ | |");
            Console.WriteLine("|  __  |/ _` |/ _` / __| |/ _ \\ / _ \\| |");
            Console.WriteLine("| |  | | (_| | (_| \\__ \\ | (_) | (_) | |");
            Console.WriteLine("|_|  |_|\\__,_|\\__,_|___/_|\\___/ \\___/|_|");
            Console.WriteLine("By. AwesomeCoderDude");
            Console.WriteLine();
        }

        public static string[] SplitArgumentsSaftley(string arg)
        {
            string[] arguments = arg.Split(' ');

            if (arg.Equals(""))
            {
                arguments = new string[] { };
            }

            return arguments;
        }
    }
}

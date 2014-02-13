using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dani
{
    public class Program
    {
        private static bool quitted;
        
        public static void Quit()
        {
            quitted = true;
        }
        
        //The main loop is kept simple, most of the important code is in the Command object
        static void Main(string[] args)
        {
            string input = "";
            quitted = false;

            //Main loop
            while (!quitted)
            {
                input = Console.ReadLine();
                
                if (input != "")
                {
                    if (input.StartsWith("/"))
                        Command.Run(input.Substring(1));
                    else
                        Command.Talk(input);
                }
            } 
        }
    }
}

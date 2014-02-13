using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dani
{
    public static class Command
    {
        //Dictionary of all the Danis in the conversation by name
        public static Dictionary<string, Dani> danis = new Dictionary<string,Dani>();

        //Dictionary of all public languages by name
        public static Dictionary<string, Language> languages = new Dictionary<string, Language>();

        //Who you are currently talking to
        public static string talkingTo = "";
        
        //Run commands to interact with the conversation
        public static void Run(string input)
        {
            //Use Dani's tokenizer to split commands into commands and arguments
            List<List<string>> commands = Tokenizer.Tokenize(input, false);

            //Multiple commands may be run when split by normal sentence ending characters
            for (int i = 0; i < commands.Count(); i++)
            {
                //Split the command list into a command and arguments
                string command = commands[i][0];
                List<string> arguments = commands[i];
                arguments.RemoveAt(0);

                switch (command.ToLower())
                {
                    //Quit commands
                    case "quit":
                    case "exit":
                    case "x":
                        Program.Quit();
                    break;

                    //Clear screen commands
                    case "clear":
                    case "cls":
                    case "c":
                        Console.Clear();
                    break;

                    //Generic command for creating new objects
                    case "create":
                    case "new":
                    case "add":
                    case "n":
                        //Make sure the user specifies what object to create
                        if (arguments.Count == 0)
                            Console.WriteLine("\nUsage: " + command + " [object] [arguments]\n");
                        else
                            CreateObject(command, arguments);
                    break;

                    //Generic command for changing conversation
                    case "talk":
                    case "speak":
                    case "converse":
                    case "t":
                        //Make sure the user specifies who to talk to
                        if (arguments.Count != 1)
                            Console.WriteLine("\nUsage: " + command + " [who]\n");
                        else
                        {
                            talkingTo = arguments[0].ToLower();
                            Console.WriteLine("\nNow talking to " + arguments[0] + "\n");
                        }
                    break;

                    case "book":
                    case "read":
                    case "file":
                    case "teach":
                    case "r":
                        //Make sure the user specifies the language
                        if (arguments.Count == 3)
                        {
                            Console.WriteLine("\n" + arguments[0] + " is opening the book...\n");
                            string text = LoadFile(command, arguments[2]);

                            if (text != "")
                            {
                                Console.WriteLine(arguments[0] + " is reading the book...\n");

                                if (!danis.ContainsKey(arguments[0].ToLower()))
                                {
                                    Console.WriteLine("\nA dani named \"" + arguments[0] + "\" does not exist\n");
                                    break;
                                }

                                if (danis[arguments[0].ToLower()].Tell(arguments[1], text))
                                    Console.WriteLine("\nYou read " + arguments[2] + " to " + arguments[0] + "\n");
                                else
                                    Console.WriteLine("\n" + arguments[0] + " does not know the language: " + arguments[1] + "\n");
                            }
                        }
                        else
                        if (arguments.Count == 2)
                        {
                            if (!languages.ContainsKey(arguments[0].ToLower()))
                            {
                                Console.WriteLine("\nA language called \"" + arguments[0] + "\" does not exist\n");
                                break;
                            }
                            string text = LoadFile(command, arguments[1]);

                            languages[arguments[0].ToLower()].AddText(text, true);

                            Console.WriteLine("\nYou added the file " + arguments[1] + " to the language " + arguments[0] + "\n");
                        }
                        else
                            Console.WriteLine("\nUsage: " + command + " [dani] [language] [file]\nOr: " + command + " [language] [file]\n");
                    break;

                    //Generic command for listing data
                    case "list":
                    case "all":
                    case "show":
                    case "a":
                        //Make sure the user specifies what to list
                        if (arguments.Count != 1)
                            Console.WriteLine("\nUsage: " + command + " [object]\n");
                        else
                            ListObject(command, arguments);
                    break;

                    default:
                        Console.WriteLine("\nUnknown command: \"" + command + "\"\n");
                    break;
                }
            }
        }

        //Read data from a file if it exists
        private static string LoadFile(string command, string filename)
        {
            string data;

            try
            {
                data = System.IO.File.ReadAllText(filename + ".txt");
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("\nFile not found: " + filename + ".txt\n");
                data = "";
            }

            return data;
        }

        //Lists all of the given object
        private static void ListObject(string command, List<string> arguments)
        {
            string object_name = arguments[0];
            arguments.RemoveAt(0);

            Console.WriteLine("\n");
            switch (object_name.ToLower())
            {
                case "language":
                case "languages":
                case "lang":
                case "langs":
                case "l":
                    Console.WriteLine(languages.Count + " languages:");
                    foreach (KeyValuePair<string, Language> val in languages)
                        Console.WriteLine("  " + val.Value.name);
                break;

                case "dani":
                case "person":
                case "speaker":
                case "friend":
                case "bot":
                case "danis":
                case "people":
                case "speakers":
                case "friends":
                case "bots":
                case "d":
                    Console.WriteLine(danis.Count + " Danis:");
                    foreach (KeyValuePair<string, Dani> val in danis)
                        Console.WriteLine("  " + val.Key);
                break;

                default:
                    Console.WriteLine("\nUnknown object: \"" + object_name + "\"\n");
                break;
            }
            Console.WriteLine("\n");
        }

        //Create a new instance of an object
        private static void CreateObject(string command, List<string> arguments)
        {
            string object_name = arguments[0];
            arguments.RemoveAt(0);
            
            switch (object_name.ToLower())
            {
                case "language":
                case "lang":
                case "l":
                    //Make sure the user specifies what to name the language
                    if (arguments.Count != 1)
                        Console.WriteLine("\nUsage: " + command + " " + object_name + " [name]\n");
                    else
                    {
                        if (languages.ContainsKey(arguments[0].ToLower()))
                            Console.WriteLine("\nLanguage already exists: \"" + arguments[0] + "\"\n");
                        else
                        {
                            languages.Add(arguments[0].ToLower(), new Language(arguments[0]));
                            Console.WriteLine("\nCreated language: " + arguments[0] + "\n");
                        }
                    }
                break;

                case "dani":
                case "person":
                case "speaker":
                case "friend":
                case "bot":
                case "d":
                    //Make sure the user specifies what to language for Dani to use
                    if (arguments.Count != 2)
                        Console.WriteLine("\nUsage: " + command + " " + object_name + " [name] [language]\n");
                    else
                    {
                        if (danis.ContainsKey(object_name.ToLower()))
                            Console.WriteLine("\nA Dani already exists with the name: \"" + arguments[0] + "\"\n");
                        else
                        {
                            if (languages.ContainsKey(arguments[1].ToLower()))
                            {
                                danis.Add(arguments[0].ToLower(), new Dani(languages[arguments[1].ToLower()]));
                                Console.WriteLine("\nCreated new Dani named \"" + arguments[0] + "\" using communal language: " + arguments[1] + "\n");
                            }
                            else
                            {
                                danis.Add(arguments[0].ToLower(), new Dani(arguments[1]));
                                Console.WriteLine("\nCreated new Dani named \"" + arguments[0] + "\" with personal language: " + arguments[1] + "\n");
                            }
                        }
                    }
                break;

                default:
                    Console.WriteLine("\nUnknown object: \"" + object_name + "\"\n");
                break;
            }
        }

        //Talk to the Dani you're currently talking to
        public static void Talk(string input)
        {
            if (danis.Count() == 0)
            {
                Console.WriteLine("\nThere is nobody here...\n");
                return;
            }

            if (talkingTo == "")
            {
                Console.WriteLine("\nYou're just kind of talking to the air at the moment.\n");
                return;
            }

            if (!danis.ContainsKey(talkingTo))
            {
                Console.WriteLine("\nNobody knows who " + talkingTo + " is.\n");
                return;
            }

            Console.WriteLine("\n\n" + talkingTo + ":\n" + danis[talkingTo].GetResponse(input) + "\n");
        }
    }
}

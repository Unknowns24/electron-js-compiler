namespace ConsoleApp
{
    class Program
    {
        public static List<Option> options;
        static void Main(string[] args)
        {
            Console.Title = "UNKCode compiler";

            // Menu options
            options = new List<Option>
            {
                new Option("Obfuscate & Compile files", () => WriteTemporaryMessage("Hi")),
                new Option("Compile files", () => WriteTemporaryMessage("Hi")),
                new Option("Code obfuscation", () =>  WriteTemporaryMessage("How Are You")),
                new Option("Exit", () => Environment.Exit(0)),
            };

            // Set the default index of the selected item to be the first
            int index = 0;

            // Write the menu out
            WriteMenu(options, options[index]);

            // Store key info in here
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteMenu(options, options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenu(options, options[index]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[index].Selected.Invoke();
                    index = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);

            Console.ReadKey();
        }

        // Default action of all the options. You can create more methods
        static void WriteTemporaryMessage(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Thread.Sleep(3000);
            WriteMenu(options, options.First());
        }

        static void ShowBanner()
        {
            Console.WriteLine("\n");
            Console.WriteLine(@"  _   _   _   _   _  __   ____               _                                               _   _               ");
            Console.WriteLine(@" | | | | | \ | | | |/ /  / ___|   ___     __| |   ___      ___    ___    _ __ ___    _ __   (_) | |   ___   _ __ ");
            Console.WriteLine(@" | | | | |  \| | | ' /  | |      / _ \   / _` |  / _ \    / __|  / _ \  | '_ ` _ \  | '_ \  | | | |  / _ \ | '__|");
            Console.WriteLine(@" | |_| | | |\  | | . \  | |___  | (_) | | (_| | |  __/   | (__  | (_) | | | | | | | | |_) | | | | | |  __/ | |   ");
            Console.WriteLine(@"  \___/  |_| \_| |_|\_\  \____|  \___/   \__,_|  \___|    \___|  \___/  |_| |_| |_| | .__/  |_| |_|  \___| |_|   ");
            Console.WriteLine(@"                                                                                    |_|                          ");
            Console.WriteLine("\n");
            Console.WriteLine("electron - js - compiler");
            Console.WriteLine("compile your js files for electron application");
            Console.WriteLine("\n");
        }

        static void WriteMenu(List<Option> options, Option selectedOption)
        {
            Console.Clear();

            ShowBanner();

            foreach (Option option in options)
            {
                if (option == selectedOption)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write(" ");
                }

                Console.WriteLine(option.Name);
            }

        }
    }

    public class Option
    {
        public string Name { get; }
        public Action Selected { get; }

        public Option(string name, Action selected)
        {
            Name = name;
            Selected = selected;
        }
    }

}
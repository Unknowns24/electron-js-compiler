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
                new Option("Obfuscate & Compile files", () => CompileAndObfuscate()),
                new Option("Compile files", () => Compile()),
                new Option("Code obfuscation", () =>  Obfuscate()),
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

        static string AskForSomething(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine() ?? "";
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

        static string[] ScanJsFiles(string path, bool withIndex, string? indexPath)
        {
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            List<string> jsFiles = new List<string>();

            foreach (var file in files)
            {
                if (file.Contains("node_modules"))
                    continue;

                if (!withIndex)
                {
                    if (file == indexPath)
                        continue;
                }

                if (file.Contains(".js") && !file.Contains(".json"))
                {
                    jsFiles.Add(file);
                }
            }

            return jsFiles.ToArray();
        }


        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            var dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
            {
                throw new Exception("\n[ERROR]: The project directory path was not found");
            }

            var finalDir = new DirectoryInfo(destinationDir);

            if (finalDir.Exists)
            {
                throw new Exception("\n[ERROR]: There is alredy a compiled folder");
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    if (subDir.Name == ".git") continue;
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        // Creating main functions
        static string[] MainProcess()
        {
            try
            {
                Console.Clear();
                ShowBanner();

                string projectPath = AskForSomething("insert the project full path:");
                Console.WriteLine("\n");
                string mainFile = AskForSomething("insert the index file full path:");
                
                string projectDir = Path.GetFileName(projectPath) ?? "";

                if (projectDir == "")
                {
                    throw new Exception("[ERROR]: Cannot Getting project directory name");
                }
            
                if (!File.Exists(mainFile))
                {
                    throw new Exception("[ERROR]: Index file does not exists");
                }

                string compiledPath = projectPath.Replace(projectDir, "compiledProject");
                string compiledIndex = mainFile.Replace(projectDir, "compiledProject");

                Console.Clear();
                ShowBanner();

                Console.WriteLine("- Creating a copy of the project on: " + $"{compiledPath}\n");

                CopyDirectory(projectPath, compiledPath, true);

                Console.WriteLine("- Project copied successfully \n");
                Console.WriteLine("- Searching JavaScript files on the project \n");

                string[] jsFiles = ScanJsFiles(compiledPath, false, compiledIndex);

                Console.WriteLine($"- Founded {jsFiles.Length} JavaScript files \n");

                return jsFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("\n Program restarting on 3 seconds");
                Thread.Sleep(3000);
                WriteMenu(options, options.First());
                string[] files = new string[0];
                return files;
            }
        }

        static void CompileAndObfuscate()
        {
            string[] jsFiles = MainProcess();
            if (jsFiles.Length > 0) return;
        }

        static void Compile()
        {
            string[] jsFiles = MainProcess();
            if (jsFiles.Length > 0) return;
        }

        static void Obfuscate()
        {
            string[] jsFiles = MainProcess();
            if (jsFiles.Length > 0) return;
        }

        // Default action of all the options. You can create more methods
        static void WriteTemporaryMessage(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Thread.Sleep(3000);
            WriteMenu(options, options.First());
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
namespace ConsoleApp
{
    class Program
    {
        public static List<Option> options;
        public static string IndexFile;
        public static string Project;
        public static string ignoreFile = "";

        static void Main(string[] args)
        {
            Console.Title = "UNKCode compiler";

            // Menu options
            options = new List<Option>
            {
                new Option("Compile files", () => Compile()),
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
            List<string> ignoreFiles = new List<string>();
            

            if (File.Exists(ignoreFile))
            {
                string[] ignoreFileContent = File.ReadAllLines(ignoreFile);
                foreach (string line in ignoreFileContent)
                {
                    ignoreFiles.Add(line);
                }
            }

            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            List<string> jsFiles = new List<string>();

            foreach (var file in files)
            {
                bool ignoreThisFile = false;

                foreach (var ignore in ignoreFiles)
                {
                    if (file == ignore)
                    {
                        ignoreThisFile = true;
                        break;
                    }
                }

                if (ignoreThisFile)
                    continue;

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
                IndexFile = compiledIndex;
                Project = compiledPath;

                Console.Clear();
                ShowBanner();

                Console.WriteLine("- Creating a copy of the project on: " + $"{compiledPath}\n");

                CopyDirectory(projectPath, compiledPath, true);

                Console.WriteLine("- Project copied successfully \n");

                Console.WriteLine("- Checking for .unkignore files in project \n");

                ignoreFile = $"{compiledPath}/.unkignore";

                if (File.Exists(ignoreFile))
                {
                    Console.WriteLine("- A .unkignore file was founded \n");
                    File.WriteAllText(ignoreFile, File.ReadAllText(ignoreFile).Replace(projectDir, "compiledProject"));
                }

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

        static void Compile()
        {
            string[] jsFiles = MainProcess();
            if (jsFiles.Length <= 0) return;
            Console.WriteLine("- Creating a Bundle file");
            CompileFiles(jsFiles);
        }

        // Main function Logic 
        static void CompileFiles(string[] files)
        {
            // Check index file integrity
            if (IndexFile == null)
            {
                WriteTemporaryMessage("[ERROR]: Index file is null");
                return;
            }
            
            // Check Project path integrity
            if (Project == null)
            {
                WriteTemporaryMessage("[ERROR]: Project path is null");
                return;
            }
            
            // Initial variables
            string[] indexContent = File.ReadAllLines(IndexFile);
            string newIndexContent = "";
            List<string> exportName = new List<string>(); 
            List<string> fileExports = new List<string>();
            List<string> indexExports = new List<string>();

            // Inspect every line of index content
            foreach (string line in indexContent)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (line.Contains("require") & line.Contains("./") & !line.Contains(".json") & !line.Contains("{")) // If line is requiring a local file 
                {
                    exportName.Add(line.Split("=")[0].Split(" ")[1]); // Get the name of the export and add it to exportName list
                }
                else if (line.Contains("require") & !line.Contains("./")) // If line is requiring a glob file like fs or electron
                {
                    indexExports.Add(line); // Get the name of the export and add it to indexExports list 
                    newIndexContent += line + "\n"; // If line is requiring globs add line to the new index content
                }
                else
                {
                    newIndexContent += line + "\n"; // If line is not requiring anything add line to the new index content
                }
            }

            // Get local exports and replace the call to the export to nothing
            // This is beacause all local files content and functions will be on the index file
            /*
             * Example: This will be replaced
             * 
             * const module1 = require("./module1")
             * function hello() {
             *      module1.someFunction();
             * }
             * 
             * to this:
             * function hello() {
             *    someFunction();
             * }
             * 
             */
            foreach (string name in exportName)
            {
                if (name == "config") continue;
                newIndexContent = newIndexContent.Replace($"{name}.", "");
            }


            // Inspect every js file admited
            foreach (string file in files)
            {
                string[] fileContent = File.ReadAllLines(file); 
                string newFileContent = "";
                List<string> exports = new List<string>();

                // Inspect every file line
                foreach (string line in fileContent)
                {
                    if (string.IsNullOrEmpty(line)) continue; // If line is empty continue with next

                    if (line.Contains("require") & line.Contains("./") & !line.Contains("{")) // If line is requiring a local file 
                    {
                        exports.Add(line.Split("=")[0].Split(" ")[1]); // Get the name of the export and add it to exports list
                    }
                    else if (line.Contains("require") & !line.Contains("./")) // If line is requiring a glob file like fs or electron
                    {
                        fileExports.Add(line); // Get the name of the export and add it to fileExports list where we will check what is alredy exported on index file content
                    }
                    else if(!line.Contains("require")) // If there is not a require line add it to newFileContent to create file 
                    {
                        newFileContent += line + "\n";
                    }
                }

                // Get local exports and replace the call to the export to nothing
                // This is beacause all local files content and functions will be on the index file
                foreach (string name in exports) 
                {
                    if (name == "config") continue;
                    newFileContent = newFileContent.Replace($"{name}.", "");
                }

                newIndexContent += "\n" + newFileContent; // Add file content to index file
                File.Delete(file); // Delete file
            }

            // Check exports to prevent duplicate require files
            foreach (string export in fileExports)
            {
                bool exists = false;
                bool componentAdded = false;

                // get all existen glob exports on index file and compare with news
                foreach (string existentExport in indexExports)
                {
                    if (export == existentExport)
                    {
                        exists = true;
                        break;
                    }

                    // Get module names by the split of the string
                    // for example:
                    // const hello = require("hello") => require("hello") => "hello") => hello
                    string exportModuleName = export.Split("=")[1].Split("(")[1].Replace(")", "").Replace("\"", "").Replace("'", "").Replace("`", "").Replace(";", "");
                    string existentModuleName = existentExport.Split("=")[1].Split("(")[1].Replace(")", "").Replace("\"", "").Replace("'", "").Replace("`", "").Replace(";", "");


                    // If export conteins the existent module like electron || fs || uid, then..
                    if (exportModuleName == existentModuleName) 
                    {
                        exists = true;
                        if (export.Contains("{")) 
                        {
                            // Get all require components
                            string[] components = export.Split("=")[0].Replace(" ", "").Replace("const", "").Replace("var", "").Replace("let", "").Replace("{", "").Replace("}", "").Split(",");
                            string addedComponents = "";

                            // Check if component is alredy required
                            foreach (string component in components)
                            {
                                if (!existentExport.Contains(component))
                                {
                                    componentAdded = true;
                                    
                                    if (addedComponents == "")
                                    {
                                        addedComponents += component;
                                    } 
                                    else
                                    {
                                        addedComponents += "," + component;
                                    }
                                }
                            }

                            if (componentAdded)
                            {
                                indexExports.Remove(existentExport); // Remove this existent export 

                                // Update it on file and list
                                string updatedExistentExport = existentExport.Replace("{", "{ " + $"{addedComponents}, ");
                                newIndexContent = newIndexContent.Replace(existentExport, updatedExistentExport);
                                indexExports.Add(updatedExistentExport);
                                break;
                            }
                        }
                    }
                }

                // if is not exist then add it at the top of the index file content and add it to the list of index exports
                if (!exists)
                {
                    newIndexContent = export + "\n" + newIndexContent; 
                    indexExports.Add(export);
                }
            }

            File.WriteAllText(IndexFile, newIndexContent);

            Console.WriteLine("\n- Bundle created");

            string answer = "";
            while (answer == "")
            {
                string temp = AskForSomething("\nIs index js content correct: (yes) or (no)");
                Console.WriteLine("\n");
                if (temp == "yes" || temp == "no")
                {
                    answer = temp;
                    break;
                }
            }

            if (answer == "no")
            {
                Console.WriteLine("Please fix your bundle or report the issue to: https://github.com/Unknowns24/electron-js-compiler");
                return;
            }

            Console.WriteLine("- Compiling js bundle");

            if (Environment.OSVersion.Platform.ToString().Contains("Win32"))
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = $"/C cd {Project} && bytenode --electron -c ./{Path.GetRelativePath(Project, IndexFile)}";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            } 
            else
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "bash",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    }
                };
                process.Start();
                process.StandardInput.WriteLine($"cd {Project} && bytenode --electron -c ./{Path.GetRelativePath(Project, IndexFile)}");
                process.WaitForExit();
            }

            Console.WriteLine("\n- Bundle compiled");

            Console.WriteLine("\n- Modifing files");

            Thread.Sleep(5000);

            string IndexFolder = Path.GetFullPath(IndexFile).Replace(Path.GetFileName(IndexFile), "");

            File.WriteAllText(IndexFile, $"const bytenode = require(\"bytenode\");\nconst start = require(\"./main.jsc\");\nstart;");
            File.Move(Path.GetFullPath(IndexFile).Replace(".js", ".jsc"), $"{IndexFolder}main.jsc");

            Console.WriteLine("\n- Proncess finished");
            WriteTemporaryMessage("Going back to Menu in 3s");
        }

        // Menu functions
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
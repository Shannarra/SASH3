using System;
using System.Linq;

namespace SASH3
{
    /// <summary>
    /// Base of all commands with arguments.
    /// </summary>
    internal interface IArgumentedCommand
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gives back a string containing all arguments and short explanation of the command.
        /// </summary>
        /// <returns>string</returns>
        string GetHelp();

        /// <summary>
        /// Gives back a <see cref="System.Collections.Generic.IEnumerable{string}"/> with all possible arguments.
        /// </summary>
        /// <returns>IEnumerable<string></returns>
        System.Collections.Generic.IEnumerable<string> GetPossibleArgs();
    }

    /// <summary>
    /// A CD (current directory) command.
    /// </summary>
    class Cd
    {
        /// <summary>
        /// The current execution path.
        /// </summary>
        public static string CurrentPath = System.IO.Directory.GetCurrentDirectory();

        /// <summary>
        /// Sets the current execution path to the given <paramref name="path"/> argument.
        /// </summary>
        /// <param name="path">The new path to set to.</param>
        public Cd(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (path == "..")
                try
                {
                    CurrentPath = CurrentPath.Substring(0, CurrentPath.LastIndexOf(@"\"));
                }
                catch (IndexOutOfRangeException) {; }
                catch (ArgumentOutOfRangeException) {; }
            else if (path.Contains("../"))
                System.Threading.Tasks.Parallel.ForEach(path.Split('/'), (x) => 
                {
                    try
                    {
                        CurrentPath = CurrentPath.Substring(0, CurrentPath.LastIndexOf(@"\"));
                    }
                    catch (IndexOutOfRangeException) {; }
                    catch (ArgumentOutOfRangeException) {; }  
                });
                    
            else if (System.IO.Directory.Exists(System.IO.Path.Combine(CurrentPath, path)))
                CurrentPath += $@"\{path}";
            else
                Console.WriteLine($"Given path \"{CurrentPath + $@"\{path}"}\" does not exist!");
        }
    }

    /// <summary>
    /// A basic outline of a command.
    /// </summary>
    struct Command
    {
        public string Name;
        public string[] Args;

        public Command(string name, string[] args)
        {
            this.Name = name;
            this.Args = args;
        }
    }

    static class Start
    {
        /// <summary>
        /// Works in the same way as <see cref="string.Substring(int, int)"/> but for an array.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="arr">The array.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <returns>T[]</returns>
        static T[] SubArr<T>(this T[] arr, int start, int end)
        {
            if ((end - start) <= 0)
                return arr;
            try
            {
                System.Collections.Generic.List<T> list = arr.ToList();
                list.RemoveRange(start + 1, (end - start) + end);
                return list.ToArray();
            }
            catch (ArgumentException) { return arr; }
            
        }

        /// <summary>
        /// Parses a new <see cref="Command"/>, removes all empty spaces in the given command line.
        /// </summary>
        /// <param name="str">The command line.</param>
        /// <returns>Command</returns>
        static Command ParseCommand(string str)
            => new Command(str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].ToLower(),
                str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray());

        /// <summary>
        /// Executes the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        static void Execute(Command command)
        {
            if (command.Name == "clear")
            {
                Console.Clear();
                return;
            }

            if (command.Name == "ls")
            {
                new Ls(null);
                return;
            }

            var reader = new System.Xml.XmlDocument();
            reader.Load("../../execution.xml");

            foreach (System.Xml.XmlElement item in reader.GetElementsByTagName("command"))
                if (command.Name == item.Attributes["name"].Value.ToLower())
                {
                    if (command.Args.Length == 1)
                    {
                        Run(command);
                        return;
                    }
                    if (item.Attributes["ignoreRest"].Value == "true" &&
                        command.Args.Length > int.Parse(item.Attributes["maxPossibleArgs"].Value))
                        command.Args = command.Args.SubArr(
                            int.Parse(item.Attributes["minPossibleArgs"].Value),
                            int.Parse(item.Attributes["maxPossibleArgs"].Value));

                    Run(command);
                    return;
                }

            Console.WriteLine($"COMMAND \"{command.Name}\" NOT FOUND!");
        }

        /// <summary>
        /// Creates a new instance object and runs the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to run.</param>
        static void Run(Command command)
        {
            switch (command.Name)
            {
                case "run": new Run(command.Args); break;
                case "delete": new Delete(command.Args); break;
                case "cd": new Cd(command.Args[0]); break;
                case "curl": new Curl(command.Args); break;
                case "cat": new Cat(command.Args[0]); break;
            }
        }

        static void Main()
        {
            Console.Write(Cd.CurrentPath + "> "); var cmd = ParseCommand(Console.ReadLine());
            while (cmd.Name != "exit")
            {
                try
                {
                    if (cmd.Args.Contains("&&"))
                    {
                        int ind = 0;
                        for (int i = 0; i < cmd.Args.Length; i++)
                            if (cmd.Args[i] == "&&")
                                ind = i;
                        System.Collections.Generic.List<string> first = new System.Collections.Generic.List<string>();
                        System.Collections.Generic.List<string> second = new System.Collections.Generic.List<string>();

                        for (int i = 0; i < ind; i++)
                            first.Add(cmd.Args[i]);
                        for (int i = ind + 2; i < cmd.Args.Length; i++)
                            second.Add(cmd.Args[i]);

                        Execute(new Command(cmd.Name, first.ToArray()));
                        Execute(new Command(cmd.Args[ind + 1], second.ToArray()));
                    }
                    else
                        Execute(cmd);
                }
                catch (IndexOutOfRangeException) { Console.WriteLine("Illegal number of arguments!"); }
                Console.Write(Cd.CurrentPath + "> "); cmd = ParseCommand(Console.ReadLine());
            }
        }
    }
}


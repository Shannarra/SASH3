using System;
using System.Linq;

namespace SASH3
{
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
        public static void Execute(Command command)
        {
            if (command.Name == "time")
            {
                if (command.Args[0] == "nth")
                {
                    Console.WriteLine("Do not try to time \"nth\" command.");
                    Execute(new Command(command.Args[0], command.Args.Skip(1).ToArray()));
                    return;
                }

                DateTime start = DateTime.Now;
                Execute(new Command(command.Args[0], command.Args.Skip(1).ToArray()));
                Console.WriteLine($"The command \"{command.Args[0]}\" took {((DateTime.Now - start).Ticks) / 10000}ms");
                return;
            }

            if (command.Name == "nth")
            {
                new Nth(new Command(command.Args[0], command.Args.Skip(1).ToArray()));
                return;
            }

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
            try
            {
                Console.Write(Cd.CurrentPath + "> "); var cmd = ParseCommand(Console.ReadLine());
                while (cmd.Name != "exit")
                {
                    if (cmd.Args.Contains("&&")) // chaining infinite number of commands
                    {
                        string str = "";
                        foreach (string item in cmd.Args)
                            str += item + " ";

                        string[] cmds = str.Split(new string[] { "&&" }, 
                            StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < cmds.Length; i++)
                        {
                            if (i == 0)
                                Execute(new Command(cmd.Name, cmds[i].Split(' ')));
                            else // cmds[i] -> a command to be executed
                                Execute(ParseCommand(cmds[i]));
                        }
                    }
                    else
                        Execute(cmd);

                    Console.Write(Cd.CurrentPath + "> "); cmd = ParseCommand(Console.ReadLine());
                }
            }
            catch (IndexOutOfRangeException) { Console.WriteLine("Illegal number of arguments!"); Main(); }
        }
    }
}


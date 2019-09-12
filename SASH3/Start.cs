using System;
using System.Linq;

namespace SASH3
{
    class Cd
    {
        public static string CurrentPath = System.IO.Directory.GetCurrentDirectory();

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
            else
                CurrentPath += $@"\{path}";
        }
    }

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
        static T[] SubArr<T>(this T[] arr, int start, int end)
        {
            if ((end - start) <= 0)
                return arr;
            System.Collections.Generic.List<T> list = arr.ToList();
            list.RemoveRange(start + 1, (end - start) + end);
            return list.ToArray();
        }

        static Command ParseCommand(string str)
            => new Command(str.Split(' ')[0].ToLower(), str.Split(' ').Skip(1).ToArray());

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

            Console.WriteLine("COMMAND NOT FOUND!");
        }

        static void Run(Command command)
        {
            switch (command.Name)
            {
                case "run": new Run(command.Args); break;
                case "delete": new Delete(command.Args); break;
                case "cd": new Cd(command.Args[0]); break;
            }
        }

        static void Main()
        {
            Console.Write(Cd.CurrentPath + "> "); var cmd = ParseCommand(Console.ReadLine());
            while (cmd.Name != "exit")
            {
                try
                {
                    Execute(cmd);
                }
                catch (IndexOutOfRangeException) { Console.WriteLine("Illegal number of arguments!"); }
                Console.Write(Cd.CurrentPath + "> "); cmd = ParseCommand(Console.ReadLine());
            }
        }
    }
}


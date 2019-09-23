namespace SASH3
{
    /*
	 Miscellaneous classes and structs for commands.
	 */

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
        public static string InitDirectory = System.IO.Directory.GetCurrentDirectory();

        /// <summary>
        /// The current execution path.
        /// </summary>
        public static string CurrentPath = (string.IsNullOrEmpty(CurrentPath) ? InitDirectory : CurrentPath + @"\");

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
                    CurrentPath = CurrentPath.Substring(0, CurrentPath.LastIndexOf(@"\")) + @"\";
                }
                catch (System.IndexOutOfRangeException) {; }
                catch (System.ArgumentOutOfRangeException) {; }
            else if (path.Contains("../"))
            {
                System.Threading.Tasks.Parallel.ForEach(path.Split('/'), (x) =>
                {
                    try
                    {
                        CurrentPath = CurrentPath.Substring(0, CurrentPath.LastIndexOf(@"\"));
                    }
                    catch (System.IndexOutOfRangeException) {; }
                    catch (System.ArgumentOutOfRangeException) {; }
                });
                CurrentPath += @"\";
            }
            else if (System.IO.Directory.Exists(System.IO.Path.Combine(CurrentPath, path)))
                CurrentPath += $@"\{path}";
            else
                System.Console.WriteLine($"Given path \"{CurrentPath + $@"\{path}"}\" does not exist!");
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

    /// <summary>
    /// Displays the contents of a file.
    /// </summary>
    struct Cat
    {
        public Cat(string file)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(Cd.CurrentPath, file)))
            {
                using (System.IO.StreamReader reader =
                    new System.IO.StreamReader(System.IO.Path.Combine(Cd.CurrentPath, file)))
                    System.Console.WriteLine('\n' + reader.ReadToEnd());
            }
            else
                System.Console.WriteLine($"The file \"{file}\" doesn't exist!");
        }
    }

	/// <summary>
    /// Spawns a new thread in order to execute a command.
    /// </summary>
    class Nth
    {
        private readonly System.Threading.Thread thread;
		
        public Nth(Command command)
        {
            thread = new System.Threading.Thread(() => { Start.Execute(command); });
            thread.Start();
        }

        ~Nth()
        {
            if (thread.IsAlive)
                thread.Abort();
        }
    }
}

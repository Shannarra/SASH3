using System.Collections.Generic;

namespace SASH3
{
    /// <summary>
    /// Runs a given executable file with a given window style.
    /// </summary>
    class Run : IArgumentedCommand
    {
        /// <summary>
        /// The current working process.
        /// </summary>
        readonly System.Diagnostics.Process process = new System.Diagnostics.Process();

        public string Name => nameof(Run);

        /// <summary>
        /// Runs the given executable with the given style.
        /// </summary>
        public Run(string[] args)
        {
            if (args[0]=="help" || args[0] == "-h")
            {
                System.Console.WriteLine(GetHelp());
                return;
            }

            process.StartInfo.FileName = args[0];
            if (args.Length == 2)
                switch (args[1])
                {
                    case "minimized":
                        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized; break;
                    case "normal":                      
                        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal; break;
                    case "maximized":                   
                        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized; break;
                    default:                            
                        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                        break;
                }
            else
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

            try
            {
                process.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                System.Console.WriteLine("File to run was not found!");
            }
        }

        public string GetHelp()
        {
            string help = "Help for command " + this.Name;
            help += "\nPossible arguments for the command:";
            System.Threading.Tasks.Parallel.ForEach(GetPossibleArgs(), curr => help += $"\n\t{curr}");
            return help;
        }

        public IEnumerable<string> GetPossibleArgs()
            => new string[]
            {
                "PROGRAM_NAME",
                "PROGRAM_NAME STYLE(MINIMIZED | NORMAL | MAXIMIZED) = NORMAL"
            };
    }
}

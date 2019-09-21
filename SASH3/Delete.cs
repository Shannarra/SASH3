using System;
using System.Collections.Generic;
using System.IO;

namespace SASH3
{
    /// <summary>
    /// Deletes file(s) in the current directory.
    /// </summary>
    class Delete : IArgumentedCommand
    {
        public string Name => nameof(Delete);

        /// <summary>
        /// Deep traverses the current directory tree, starting with the given <paramref name="root"/>.
        /// Asks a question if <paramref name="forced"/> isn't false, deletes all files where the parameter 
        /// "<paramref name="where"/>" matches the current file/folder name.
        /// </summary>
        /// <param name="root">The root of the directory tree.</param>
        /// <param name="where">The common sign of all files/folders to be deleted.</param>
        /// <param name="forced">Just delete all files in the current tree?</param>
        static void DeleteDirectoryTree(DirectoryInfo root, string where = null, bool forced = false)
        {
            if (!forced)
            {
                Console.Write($"Do you want to delete all files within the folder: \"{root.FullName}\" [Y/n]: ");
                char ans = Console.ReadLine()[0];

                if (ans != 'y' && ans != 'Y')
                    return;
            }
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

            try
            {
                files = root.GetFiles("*.*");
            }
            catch {; }

            if (files != null)
            {
                foreach (FileInfo fi in files)
                    if (where != null)
                    {
                        if (where[0] == '*') // *.xml
                            if (fi.FullName.EndsWith(where.Remove(0, 2)))
                                File.Delete(fi.FullName);
                    }
                    else
                        File.Delete(fi.FullName);

                subDirs = root.GetDirectories();

                foreach (DirectoryInfo dirInfo in subDirs)
                    DeleteDirectoryTree(dirInfo, where, forced);
            }
            try
            {
                Directory.Delete(root.FullName);
            }
            catch {; }
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
                "FILENAME",
                "*",
                "FILENAME in DIR",
                "FILENAME in . || FILENAME in path",
                "* in DIR where SIGN"
            };
            

        /// <summary>
        /// Runs the delete command with the given <paramref name="args"/>
        /// </summary>
        /// <param name="args">The command's arguments.</param>
        public Delete(string[] args)
        {
            /*
             delete main.c -> deletes that file
             delete * -> deletes all files in the current directory
             delete main.c in d:\dev\c\ -> deletes the file "main.c" in the given directory
             delete main.c in path -> deletes the file "main.c" in the current directory
             delete main.c in . -> same as ^
             delete * in c:\ where *.cs -> this will delete ALL FILES with extension ".cs"
             */

            if (args.Length == 1 && (args[0] == "help" || args[0] == "-h"))
                Console.WriteLine(GetHelp());

            if (args.Length == 1 && args[0] != "*") // delete FILENAME
                File.Delete(args[0]);
            else if (args.Length == 1 && args[0] == "*") // delete *
                DeleteDirectoryTree(new DirectoryInfo(Cd.CurrentPath));

            if (args.Length == 3) // delete .. in ..
            {
                if (args[1] != "in")
                {
                    Console.WriteLine($"Expected argument \"in\" at the place of {args[1]}");
                    return;
                }

                if (args[0] == "*") // delete * in PATH
                {
                    if (!Directory.Exists(args[2]) && args[2] != "path" && args[2] != ".")
                    {
                        Console.WriteLine("Unexpected given path");
                        return;
                    }

                    if (args[2] == "path" || args[2] == ".")
                        args[2] = Cd.CurrentPath;

                    if (args[0] == "*") // delete * in [path or c:\users\..]
                        DeleteDirectoryTree(new DirectoryInfo(args[2]), forced: true);
                }
                else // delete FILE in PATH
                {
                    if (!Directory.Exists(args[2]) && args[2] != "path" && args[2] != ".")
                    {
                        Console.WriteLine("Unexpected given path");
                        return;
                    }

                    if (args[2] == "path" || args[2] == ".")
                        args[2] = Cd.CurrentPath;

                    File.Delete(Path.Combine(args[2], args[0]));
                }
            }

            if (args.Length == 5) // delete * in PATH where SIGN
            {
                if (args[1] != "in" && args[3] != "where")
                {
                    Console.WriteLine($"Expected argument \"in\" at the place of {args[1]}" +
                        $" and argument \"where\" at the place of {args[3]}");
                    return;
                }

                if (args[0] == "*") // delete * in PATH where SIGN
                {
                    if (!Directory.Exists(args[2]) && args[2] != "path")
                    {
                        Console.WriteLine("Unexpected given path");
                        return;
                    }

                    if (args[2] == "path")
                        args[2] = Cd.CurrentPath;

                    if (args[0] == "*") // delete * in [path or c:\users\..] where .xml
                        DeleteDirectoryTree(new DirectoryInfo(args[2]), args[4], true);
                }
            }
        }
    }
}

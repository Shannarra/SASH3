using System;
using System.IO;

namespace SASH3
{
    /// <summary>
    /// Lists everything in the current execution directory.
    /// </summary>
    class Ls
    {
        /// <summary>
        /// Walks the current directory tree and prints out all items in it.
        /// </summary>
        /// <param name="root">The tree's root.</param>
        static void WalkDirectoryTree(DirectoryInfo root)
        {
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
                    Console.WriteLine("[F] " + fi.FullName);

                subDirs = root.GetDirectories();

                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    Console.WriteLine("[D] " + dirInfo.FullName);
                    WalkDirectoryTree(dirInfo);
                }
            }
        }

        public Ls(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                WalkDirectoryTree(new DirectoryInfo(Cd.CurrentPath));
        }
    }
}

using System;
using System.IO;

namespace SASH3
{
    class Ls
    {
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

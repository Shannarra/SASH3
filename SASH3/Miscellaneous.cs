namespace SASH3
{
    /*
	 Miscellaneous classes and structs for commands.
	 */

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

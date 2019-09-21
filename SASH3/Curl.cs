using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SASH3
{
    /// <summary>
    /// Copies an URL address.
    /// </summary>
    class Curl
    {
        public Curl(string[] args)
        {
            string www = "";
            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                    www += client.DownloadString(args[0]);
            }
            catch (System.Net.WebException) { Console.WriteLine("The URL could not be resolved."); }
            if (args.Length == 1)
                Console.WriteLine(www);
            else
            {
                // the file doesn't exist, so create it!
                if (!System.IO.File.Exists(args[1]))
                    using (System.IO.FileStream str = System.IO.File.Create(System.IO.Path.Combine(Cd.CurrentPath, args[1])))
                        try
                        {
                            Console.WriteLine($"File \"{args[1]}\" was created in the current directory!");
                        }
                        catch (UnauthorizedAccessException)
                        {
                            Console.WriteLine("Couldn't create a new file in the current directory! \nMaybe step out of it?");
                        }

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(args[1]))
                    writer.WriteLine(www);
            }
        }
    }
}

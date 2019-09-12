namespace SASH3
{
    class Run
    {
        readonly System.Diagnostics.Process process = new System.Diagnostics.Process();

        public Run(string[] args)
        {
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
    }
}

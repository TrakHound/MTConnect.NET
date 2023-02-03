using System;
using System.Reflection;

namespace MTConnect.Applications
{
    public class Program
    {
        // This is the Application Name shown in the Console header information
        // If you are implementing this into your own application, you can change this to be more specific (ex. Fanuc MTConnect Adapter, Mazak MTConnect Adapter, etc.)
        private const string ApplicationName = "MTConnect SDHR Adapter";

        // Copyright statement for the application. If you are implementing this into your own application, you can change this to your own copyright, or set it to 'null'.
        // This is just what is shown in the console header.
        private const string ApplicationCopyright = "Copyright 2023";

        public static void Main(string[] args)
        {
            // Print an application header to the console
            PrintConsoleHeader();

            var engine = new AdapterEngine();

            // Create a new MTConnect Adapter Application
            var app = new AdapterApplication(engine);

            // Run the Agent ('true' parameter blocks the call so the application does not continue)
            app.Run(args, true);
        }


        private static void PrintConsoleHeader()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            Console.WriteLine("--------------------");
            if (!string.IsNullOrEmpty(ApplicationCopyright)) Console.WriteLine(ApplicationCopyright);
            Console.WriteLine(ApplicationName + " : Version " + version.ToString());
            Console.WriteLine("--------------------");
            Console.WriteLine("This application is licensed under the MIT License (https://choosealicense.com/licenses/mit/)");
            Console.WriteLine("Source code available at Github.com (https://github.com/TrakHound/MTConnect.NET)");
            Console.WriteLine("--------------------");
        }
    }
}

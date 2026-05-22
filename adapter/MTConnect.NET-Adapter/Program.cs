// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Reflection;

namespace MTConnect.Applications
{
    /// <summary>
    /// Process entry point for the standalone <c>mtconnect.net-adapter</c>
    /// host. Wires the bundled <see cref="DataSource"/> implementation to
    /// an <see cref="MTConnectAdapterApplication"/> and runs the adapter
    /// in blocking mode. The CLI surface (commands such as <c>run</c>,
    /// <c>debug</c>, <c>install</c>, …) is implemented by
    /// <c>MTConnectAdapterApplication</c>; see <c>docs/reference/cli.md</c>
    /// for the full reference.
    /// </summary>
    public class Program
    {
        // This is the Application Name shown in the Console header information
        // If you are implementing this into your own application, you can change this to be more specific (ex. Fanuc MTConnect Adapter, Mazak MTConnect Adapter, etc.)
        private const string ApplicationName = "MTConnect.NET Adapter";

        // Copyright statement for the application. If you are implementing this into your own application, you can change this to your own copyright, or set it to 'null'.
        // This is just what is shown in the console header.
        private const string ApplicationCopyright = "Copyright 2024 TrakHound Inc., All Rights Reserved";


        /// <summary>
        /// Process entry point. Prints the console header, instantiates
        /// the bundled <see cref="DataSource"/>, and delegates to
        /// <see cref="MTConnectAdapterApplication.Run(string[], bool)"/>
        /// in blocking mode.
        /// </summary>
        /// <param name="args">Command-line arguments forwarded to
        /// <see cref="MTConnectAdapterApplication"/>.</param>
        public static void Main(string[] args)
        {
            // Print an application header to the console
            PrintConsoleHeader();

            var dataSource = new DataSource();

            // Create a new MTConnect Adapter Application
            var app = new MTConnectAdapterApplication(dataSource);

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

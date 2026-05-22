// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Reflection;

namespace MTConnect.Applications
{
    /// <summary>
    /// Process entry point for the standalone <c>mtconnect.net-agent</c>
    /// host. Prints a console header, instantiates an
    /// <see cref="MTConnectAgentApplication"/>, and runs it in blocking
    /// mode. The CLI surface (commands such as <c>run</c>, <c>debug</c>,
    /// <c>install</c>, …) is implemented by <c>MTConnectAgentApplication</c>;
    /// see <c>docs/reference/cli.md</c> for the full reference.
    /// </summary>
    public class Program
    {
        // This is the Application Name shown in the Console header information
        // If you are implementing this into your own application, you can change this to be more specific (ex. Fanuc MTConnect Agent, Mazak MTConnect Agent, etc.)
        private const string ApplicationName = "MTConnect.NET Agent";

        // Copyright statement for the application. If you are implementing this into your own application, you can change this to your own copyright.
        // This is just what is shown in the console header. If you want to show support for the MTConnect.NET project, you can reference it using the links in the default header
        private const string ApplicationCopyright = "Copyright 2025 TrakHound Inc., All Rights Reserved";

        /// <summary>
        /// Process entry point. Prints the console header, then delegates
        /// to <see cref="MTConnectAgentApplication.Run(string[], bool)"/>
        /// in blocking mode so the process stays alive for the lifetime
        /// of the agent.
        /// </summary>
        /// <param name="args">Command-line arguments forwarded verbatim
        /// to <see cref="MTConnectAgentApplication"/>.</param>
        public static void Main(string[] args)
        {
            // Print an application header to the console
            PrintConsoleHeader();

            // Create a new MTConnect Agent Application
            // This handles all MTConnect Agent functionality along with
            // an HTTP server, SHDR Adapters, Command line arguments, Device management, Buffer management, Logging, Windows Service, and Configuration File management
            var agentApplication = new MTConnectAgentApplication();

            // Run the Agent ('true' parameter blocks the call so the application does not continue)
            agentApplication.Run(args, true);
        }


        private static void PrintConsoleHeader()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            Console.WriteLine("--------------------");
            Console.WriteLine(ApplicationCopyright);
            Console.WriteLine(ApplicationName + " : Version " + version.ToString());
            Console.WriteLine("--------------------");
            Console.WriteLine("This application is licensed under the MIT License (https://choosealicense.com/licenses/mit/)");
            Console.WriteLine("Source code available at Github.com (https://github.com/TrakHound/MTConnect.NET)");
            Console.WriteLine("--------------------");
        }
    }
}

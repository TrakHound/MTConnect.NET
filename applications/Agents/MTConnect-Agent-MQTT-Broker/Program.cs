// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Applications.Agents;
using System.Reflection;

namespace MTConnect.Applications
{
    public class Program
    {
        // This is the Application Name shown in the Console header information
        // If you are implementing this into your own application, you can change this to be more specific (ex. Fanuc MTConnect Agent, Mazak MTConnect Agent, etc.)
        private const string ApplicationName = "MTConnect MQTT Broker Agent";

        // Copyright statement for the application. If you are implementing this into your own application, you can change this to your own copyright.
        // This is just what is shown in the console header. If you want to show support for the MTConnect.NET project, you can reference it using the links in the default header
        private const string ApplicationCopyright = "Copyright 2022 TrakHound Inc., All Rights Reserved";

        public static void Main(string[] args)
        {
            // Print an application header to the console
            PrintConsoleHeader();

            // Create a new MTConnect Agent Application
            // This handles all MTConnect Agent functionality along with
            // an HTTP server, SHDR Adapters, Command line arguments, Device management, Buffer management, Logging, Windows Service, and Configuration File management
            var agentApplication = new MTConnectShdrMqttBrokerAgentApplication();

            // Use the regular MTConnectHttpAgentApplication if you are not using SHDR Adapters
            ///var agentApplication = new MTConnectHttpAgentApplication();

            // Run the Agent ('true' parameter blocks the call so the application does not continue)
            agentApplication.Run(args, true);

            // Use the 'false' parameter if you are implementing this into an existing application or are handling blocking elsewhere
            //agentApplication.Run(args, false);

            // ** This is where the rest of your application can go. **
            // For example, if you are developing the Agent to read directly from a PLC, this would be where you can place the PLC reading code 
        }


        private static void PrintConsoleHeader()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            Console.WriteLine("--------------------");
            Console.WriteLine(ApplicationCopyright);
            Console.WriteLine(ApplicationName + " : Version " + version.ToString());
            Console.WriteLine("--------------------");
            Console.WriteLine("This application is licensed under the Apache Version 2.0 License (https://www.apache.org/licenses/LICENSE-2.0)");
            Console.WriteLine("Source code available at Github.com (https://github.com/TrakHound/MTConnect.NET)");
            Console.WriteLine("--------------------");
        }
    }
}

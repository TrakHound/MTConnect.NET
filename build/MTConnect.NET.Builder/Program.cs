// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Reflection;

namespace TrakHound.Builder
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
#if DEBUG
            await Debug();
#else
            await Run(args);
#endif
        }

        private static async Task Run(string[] args)
        {
            PrintConsoleHeader();

            var cmdArgs = new List<string>();
            if (args != null && args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg.Contains(' ')) cmdArgs.Add($"\"{arg}\"");
                    else cmdArgs.Add(arg);
                }
            }

            var cmd = string.Join(' ', cmdArgs);
            await CommandParser.Run(cmd);
        }

        private static async Task Debug()
        {
            PrintConsoleHeader();

            while (true)
            {
                Console.Write('>');
                var cmd = Console.ReadLine();
                if (!string.IsNullOrEmpty(cmd))
                {
                    await CommandParser.Run(cmd);
                }
            }
        }

        private static void PrintConsoleHeader()
        {
            var assembly = Assembly.GetEntryAssembly();
            var version = assembly.GetName().Version;
            var buildDate = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);

            Console.WriteLine("--------------------");
            Console.WriteLine($"Copyright {buildDate.Year} TrakHound Inc., All Rights Reserved");
            Console.WriteLine($"MTConnect.NET Builder : Version {version}");
            Console.WriteLine("--------------------");
            Console.WriteLine("This application is licensed under the MIT License (https://choosealicense.com/licenses/mit/)");
            Console.WriteLine("Source code available at Github.com (https://github.com/TrakHound/MTConnect.NET)");
            Console.WriteLine("--------------------");
        }
    }
}
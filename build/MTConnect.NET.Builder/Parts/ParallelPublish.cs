namespace TrakHound.Builder
{
    /// <summary>
    /// Publishes a project for several Runtimes / Frameworks at the same time, and prepares the outputs for an installer
    /// </summary>
    internal static class ParallelPublish
    {
        /// <summary>
        /// For each Runtime : restore the project once, then run that Runtime's publish commands (one per Framework) in parallel
        /// with --no-restore. The Runtimes run one after another because a restore replaces the project's assets file.
        /// Publishes that fail are retried one at a time with a normal restore (in case parallel builds of shared
        /// referenced projects collided). Exits the Builder if a publish fails
        /// </summary>
        public static void Run(string label, string projectDirectory, IReadOnlyList<(string Runtime, string Label, string Command)> publishes)
        {
            foreach (var runtimePublishes in publishes.GroupBy(o => o.Runtime))
            {
                var runtime = runtimePublishes.Key;
                var runtimeList = runtimePublishes.ToList();

                Console.Write($"Restoring {label} Project ({runtime})..");
                if (CommandLine.Run($"dotnet restore -r {runtime} -p:Configuration=Release", projectDirectory))
                {
                    Console.Write("DONE");
                    Console.WriteLine();
                }
                else
                {
                    Console.Write("ERROR");
                    Console.WriteLine();
                    Environment.Exit(1);
                }

                Console.Write($"Building {label} Project ({string.Join(", ", runtimeList.Select(o => o.Label))})..");
                var results = CommandLine.RunParallel(runtimeList.Select(o => (o.Label, $"{o.Command} --no-restore", projectDirectory)).ToList());

                if (results.All(o => o))
                {
                    Console.Write("DONE");
                    Console.WriteLine();
                    continue;
                }

                Console.WriteLine();
                for (var i = 0; i < runtimeList.Count; i++)
                {
                    if (results[i]) continue;

                    Console.Write($"Retrying {label} Project ({runtimeList[i].Label})..");
                    if (CommandLine.Run(runtimeList[i].Command, projectDirectory))
                    {
                        Console.Write("DONE");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write("ERROR");
                        Console.WriteLine();
                        Environment.Exit(1);
                    }
                }
            }
        }

        /// <summary>
        /// Move the files that are identical in every Runtime's publish output ([publishDirectory]/[runtime]/[framework]) to
        /// [publishDirectory]/common/[framework], so an installer that includes several Runtimes only compresses them once.
        /// Returns the number of files moved
        /// </summary>
        public static int MoveCommonFiles(string publishDirectory, string[] runtimes, string framework)
        {
            if (runtimes == null || runtimes.Length < 2) return 0;

            var runtimeDirectories = runtimes.Select(o => Path.Combine(publishDirectory, o, framework)).ToArray();
            var commonDirectory = Path.Combine(publishDirectory, "common", framework);

            var files = Directory.EnumerateFiles(runtimeDirectories[0], "*", SearchOption.AllDirectories)
                .Select(o => Path.GetRelativePath(runtimeDirectories[0], o))
                .ToArray();

            // Compare in parallel: the first read of each newly published file is slow (ex. antivirus scanning), not the comparison itself
            var common = new System.Collections.Concurrent.ConcurrentBag<string>();
            Parallel.ForEach(files, relativePath =>
            {
                var file = Path.Combine(runtimeDirectories[0], relativePath);
                if (runtimeDirectories.Skip(1).All(o => FilesEqual(file, Path.Combine(o, relativePath)))) common.Add(relativePath);
            });

            foreach (var relativePath in common)
            {
                var commonPath = Path.Combine(commonDirectory, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(commonPath));
                File.Move(Path.Combine(runtimeDirectories[0], relativePath), commonPath, true);
                foreach (var runtimeDirectory in runtimeDirectories.Skip(1)) File.Delete(Path.Combine(runtimeDirectory, relativePath));
            }

            return common.Count;
        }

        private static bool FilesEqual(string path1, string path2)
        {
            var file1 = new FileInfo(path1);
            var file2 = new FileInfo(path2);
            if (!file2.Exists || file1.Length != file2.Length) return false;

            using var stream1 = file1.OpenRead();
            using var stream2 = file2.OpenRead();
            var buffer1 = new byte[81920];
            var buffer2 = new byte[81920];

            int read1;
            while ((read1 = stream1.ReadAtLeast(buffer1, buffer1.Length, false)) > 0)
            {
                var read2 = stream2.ReadAtLeast(buffer2, read1, false);
                if (read2 != read1 || !buffer1.AsSpan(0, read1).SequenceEqual(buffer2.AsSpan(0, read2))) return false;
            }

            return true;
        }
    }
}

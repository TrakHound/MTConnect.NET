using System.Text.RegularExpressions;

namespace TrakHound.Builder
{
    internal class AppVersion
    {
        /// <summary>
        /// The VersionPrefix from the VersionInfoPath file (ex. 7.2.0)
        /// </summary>
        public static string GetVersion(string configurationId)
        {
            var config = Configuration.Read(configurationId);

            if (File.Exists(config.VersionInfoPath))
            {
                var version = ReadVersionPrefix(File.ReadAllText(config.VersionInfoPath));
                if (!string.IsNullOrEmpty(version)) return version;
            }

            Fail($"VersionPrefix not found in {config.VersionInfoPath}");
            return null;
        }

        /// <summary>
        /// The VersionSuffix from the VersionInfoPath file (ex. prerelease), or null if it isn't set
        /// </summary>
        public static string GetVersionSuffix(string configurationId)
        {
            var config = Configuration.Read(configurationId);

            if (File.Exists(config.VersionInfoPath))
            {
                var match = new Regex("<VersionSuffix>\\s*([^<]*?)\\s*<").Match(File.ReadAllText(config.VersionInfoPath));
                if (match.Success && !string.IsNullOrEmpty(match.Groups[1].Value)) return match.Groups[1].Value;
            }

            return null;
        }

        /// <summary>
        /// [VersionPrefix].[Revision] (ex. 7.2.0.71)
        /// </summary>
        public static string GetRevisionVersion(string configurationId)
        {
            return $"{GetVersion(configurationId)}.{GetSourceInfo(configurationId).Revision}";
        }

        /// <summary>
        /// The version with the Version Suffix, unless the Configuration removes it (ex. 7.2.0.71-prerelease)
        /// </summary>
        public static string AddVersionSuffix(string configurationId, string version)
        {
            var config = Configuration.Read(configurationId);
            var versionSuffix = GetVersionSuffix(configurationId);

            if (!string.IsNullOrEmpty(versionSuffix) && !config.RemoveVersionSuffix) return $"{version}-{versionSuffix}";
            return version;
        }

        /// <summary>
        /// The version with the Version Suffix and the Commit as build metadata (ex. 7.2.0.71-prerelease+abc1234 or +abc1234.dirty).
        /// Used for the AssemblyInformationalVersion so a binary can be traced to its Commit
        /// </summary>
        public static string GetInformationalVersion(string configurationId, string version)
        {
            var sourceInfo = GetSourceInfo(configurationId);
            var metadata = sourceInfo.IsDirty ? $"{sourceInfo.ShortCommit}.dirty" : sourceInfo.ShortCommit;
            return $"{AddVersionSuffix(configurationId, version)}+{metadata}";
        }


        /// <summary>
        /// The Commit and Revision of the Source (git) repository containing the VersionInfoPath file.
        /// The Revision is the "git height" : the number of commits (including the commit itself) since the VersionPrefix line was last changed,
        /// so every build of a commit gets the same version on any machine. The Revision is 0 when the VersionPrefix has been changed but not committed
        /// </summary>
        public static SourceInfo GetSourceInfo(string configurationId)
        {
            lock (_sourceInfoLock)
            {
                if (_sourceInfos.TryGetValue(configurationId, out var cached)) return cached;

                var config = Configuration.Read(configurationId);

                var path = config.VersionInfoPath;
                var directory = Path.GetDirectoryName(path);
                var repositoryRoot = Path.GetFullPath(Git(directory, "rev-parse", "--show-toplevel"));

                // Resolve the file's case as tracked by git (ex. a Configuration's "Directory.build.props" is tracked as "Directory.Build.props")
                var relativePath = Path.GetRelativePath(repositoryRoot, path).Replace('\\', '/');
                var trackedPath = Git(repositoryRoot, "ls-files", "--full-name", $":(icase){relativePath}").Split('\n')[0].Trim();
                if (string.IsNullOrEmpty(trackedPath)) Fail($"{path} is not tracked by git");

                var commit = Git(repositoryRoot, "rev-parse", "HEAD");
                var isDirty = !string.IsNullOrEmpty(Git(repositoryRoot, "status", "--porcelain", "--untracked-files=no"));

                int revision = 0;
                var headPrefix = ReadVersionPrefix(Git(repositoryRoot, "show", $"HEAD:{trackedPath}"));
                if (headPrefix == GetVersion(configurationId))
                {
                    // The last commit that added or removed a VersionPrefix line (empty if it was never changed)
                    var versionCommit = Git(repositoryRoot, "log", "-n", "1", "--format=%H", "-G", "<VersionPrefix>", "--", trackedPath);
                    var range = !string.IsNullOrEmpty(versionCommit) ? $"{versionCommit}..HEAD" : "HEAD";
                    revision = Git(repositoryRoot, "rev-list", "--count", range).ToInt() + (!string.IsNullOrEmpty(versionCommit) ? 1 : 0);
                }

                var sourceInfo = new SourceInfo(repositoryRoot, commit, commit.Substring(0, 7), isDirty, revision);
                _sourceInfos[configurationId] = sourceInfo;
                return sourceInfo;
            }
        }

        /// <summary>
        /// Exits the Builder if the Source has uncommitted changes (to tracked files). Published versions must be built from a commit
        /// since the version (Revision) only identifies the commit
        /// </summary>
        public static void EnsureCommitted(string configurationId)
        {
            var sourceInfo = GetSourceInfo(configurationId);
            if (sourceInfo.IsDirty) Fail($"{sourceInfo.RepositoryRoot} has uncommitted changes : commit them before publishing");
        }

        public record SourceInfo(string RepositoryRoot, string Commit, string ShortCommit, bool IsDirty, int Revision);

        private static readonly Dictionary<string, SourceInfo> _sourceInfos = new();
        private static readonly object _sourceInfoLock = new();


        private static string ReadVersionPrefix(string fileContents)
        {
            var match = new Regex("<VersionPrefix>\\s*([^<]*?)\\s*<").Match(fileContents ?? "");
            return match.Success ? match.Groups[1].Value : null;
        }

        // Run git and return its trimmed output (exits the Builder if it fails)
        private static string Git(string workingDirectory, params string[] arguments)
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo("git")
                {
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                foreach (var argument in arguments) startInfo.ArgumentList.Add(argument);

                using (var process = System.Diagnostics.Process.Start(startInfo))
                {
                    var errorTask = process.StandardError.ReadToEndAsync();
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0) Fail($"git {string.Join(' ', arguments)} : {errorTask.Result.Trim()}");
                    return output.Trim();
                }
            }
            catch (Exception ex)
            {
                Fail($"git {string.Join(' ', arguments)} : {ex.Message}");
                return null;
            }
        }

        private static void Fail(string message)
        {
            Console.WriteLine();
            Console.WriteLine($"ERROR : {message}");
            Environment.Exit(1);
        }
    }
}

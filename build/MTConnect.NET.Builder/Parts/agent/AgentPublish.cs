namespace TrakHound.Builder.Instance
{
    /// <summary>
    /// Publishes the Agent project (used by the Installer and Docker builds)
    /// </summary>
    internal static class AgentPublish
    {
        public const string BuildInfoFilename = "build.json";


        public static string GetPublishDirectory(Configuration config, string projectPath) => Path.Combine(config.Input, projectPath, "bin", "publish");

        /// <summary>
        /// Publish the Agent for each Runtime and Framework to [projectPath]/bin/publish/[runtime]/[framework].
        /// The publishes run in parallel (see ParallelPublish). Exits the Builder if a publish fails.
        /// Writes the version and Commit to [output]/[version]/build.json (used by build.ps1 to check the release before publishing)
        /// </summary>
        public static void Publish(string configurationId, string label, string projectPath, string[] runtimes, string[] frameworks)
        {
            var config = Configuration.Read(configurationId);

            var projectDirectory = Path.Combine(config.Input, projectPath);
            var outputDirectory = GetPublishDirectory(config, projectPath);
            if (Directory.Exists(outputDirectory)) Directory.Delete(outputDirectory, true);

            // The Assembly Version stays [VersionPrefix]; the File and Informational Versions identify the Commit
            var version = AppVersion.GetVersion(configurationId);
            var revisionVersion = AppVersion.GetRevisionVersion(configurationId);
            var informationalVersion = AppVersion.GetInformationalVersion(configurationId, revisionVersion);

            var publishes = new List<(string Runtime, string Label, string Command)>();
            foreach (var runtime in runtimes)
            {
                foreach (var framework in frameworks)
                {
                    var publishOutput = Path.Combine(outputDirectory, runtime, framework);

                    var parts = new List<string>();
                    parts.Add("dotnet publish");
                    parts.Add("-c:Release");
                    parts.Add($"-r:{runtime}");
                    parts.Add($"-f:{framework}");
                    parts.Add($"-p:VersionPrefix=\"{version}\"");
                    if (config.RemoveVersionSuffix) parts.Add($"-p:VersionSuffix=\"\"");
                    parts.Add($"-p:FileVersion=\"{revisionVersion}\"");
                    parts.Add($"-p:InformationalVersion=\"{informationalVersion}\"");
                    parts.Add("-p:WarningLevel=0");
                    parts.Add("--no-self-contained");
                    parts.Add($"--output \"{publishOutput}/\"");

                    publishes.Add((runtime, $"{runtime} : {framework}", string.Join(' ', parts)));
                }
            }

            ParallelPublish.Run(label, projectDirectory, publishes);

            WriteBuildInfo(configurationId);
        }

        private static void WriteBuildInfo(string configurationId)
        {
            var config = Configuration.Read(configurationId);
            var sourceInfo = AppVersion.GetSourceInfo(configurationId);
            var version = AppVersion.GetVersion(configurationId);

            var outputPath = Path.Combine(config.Output, version);
            Directory.CreateDirectory(outputPath);

            var buildInfo = new { version, revisionVersion = AppVersion.GetRevisionVersion(configurationId), commit = sourceInfo.Commit, dirty = sourceInfo.IsDirty };
            File.WriteAllText(Path.Combine(outputPath, BuildInfoFilename), System.Text.Json.JsonSerializer.Serialize(buildInfo));
        }
    }
}

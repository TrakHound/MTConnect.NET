using System.Diagnostics;

namespace TrakHound.Builder
{
    [CommandGroup("global")]
    internal static class Global
    {
        [Command("build")]
        public static async Task Build([CommandOption] string configurationId = null, [CommandOption] bool verbose = false)
        {
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            var config = Configuration.Read(configurationId);
            var version = AppVersion.GetVersion(configurationId);
            var outputPath = Path.Combine(config.Output, version);

            Console.WriteLine($"Build All (v{version}) : Configuration ID = {configurationId}");
            Console.WriteLine();

            var stpw = Stopwatch.StartNew();

            await Libraries.Nuget.Build(configurationId, verbose);
            await Instance.Installer.Build(configurationId, verbose);
            await Instance.Docker.Build(configurationId, verbose);

            stpw.Stop();
            Console.WriteLine();
            Console.WriteLine($"Build All (v{version}) Completed Successfully in {stpw.Elapsed.ToString("hh\\:mm\\:ss")}");

            // Open the output directory when run interactively on Windows
            if (OperatingSystem.IsWindows() && !Console.IsOutputRedirected)
            {
                Process.Start("explorer.exe", Path.TrimEndingDirectorySeparator(outputPath) + Path.DirectorySeparatorChar);
            }
        }

        [Command("version")]
        public static async Task Version([CommandOption] string configurationId = null)
        {
            await Task.CompletedTask;
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            var sourceInfo = AppVersion.GetSourceInfo(configurationId);
            var revisionVersion = AppVersion.GetRevisionVersion(configurationId);

            Console.WriteLine($"Version        : {AppVersion.GetVersion(configurationId)}");
            Console.WriteLine($"Package        : {AppVersion.AddVersionSuffix(configurationId, revisionVersion)}");
            Console.WriteLine($"Informational  : {AppVersion.GetInformationalVersion(configurationId, revisionVersion)}");
            Console.WriteLine($"Commit         : {sourceInfo.Commit}{(sourceInfo.IsDirty ? " (uncommitted changes)" : "")}");
        }

        [Command("publish")]
        public static async Task Publish([CommandOption] string configurationId = null, [CommandOption] bool verbose = false)
        {
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            var version = AppVersion.GetVersion(configurationId);

            Console.WriteLine($"Publish All (v{version}) : Configuration ID = {configurationId}");
            Console.WriteLine();

            var stpw = Stopwatch.StartNew();

            await Libraries.Nuget.Publish(configurationId, verbose);
            await Instance.Docker.Publish(configurationId, verbose);
            await GitHub.Release.Publish(configurationId);

            stpw.Stop();
            Console.WriteLine();
            Console.WriteLine($"Publish All (v{version}) Completed Successfully in {stpw.Elapsed.ToString("hh\\:mm\\:ss")}");
        }
    }
}

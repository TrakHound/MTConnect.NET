using System.Diagnostics;

namespace TrakHound.Builder
{
    [CommandGroup("global")]
    internal static class Global
    {
        [Command("build")]
        public static async Task Build([CommandParameter] string configurationId = null)
        {
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            var config = Configuration.Read(configurationId);
            var version = AppVersion.GetVersion(configurationId);
            var outputPath = Path.Combine(Environment.CurrentDirectory, config.Output, version);

            Console.WriteLine($"Build All (v{version}) : Configuration ID = {configurationId}");
            Console.WriteLine();

            var stpw = Stopwatch.StartNew();

            await Libraries.Nuget.Build(configurationId);
            await Instance.Installer.Build(configurationId);
            await Instance.Docker.Build(configurationId);

            stpw.Stop();
            Console.WriteLine();
            Console.WriteLine($"Build All (v{version}) Completed Successfully in {stpw.Elapsed.ToString("hh\\:mm\\:ss")}");

            Process.Start("explorer.exe", Path.TrimEndingDirectorySeparator(outputPath) + Path.DirectorySeparatorChar);
        }

        [Command("publish")]
        public static async Task Publish([CommandParameter] string configurationId)
        {
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            var config = Configuration.Read(configurationId);
            var version = AppVersion.GetVersion(configurationId);
            var outputPath = Path.Combine(Environment.CurrentDirectory, config.Output, version);

            Console.WriteLine($"Publish All (v{version}) : Configuration ID = {configurationId}");
            Console.WriteLine();

            var stpw = Stopwatch.StartNew();

            await Libraries.Nuget.Publish(configurationId);
            await Instance.Docker.Publish(configurationId);
            await GitHub.Release.Publish(configurationId);

            stpw.Stop();
            Console.WriteLine();
            Console.WriteLine($"Publish All (v{version}) Completed Successfully in {stpw.Elapsed.ToString("hh\\:mm\\:ss")}");
        }
    }
}

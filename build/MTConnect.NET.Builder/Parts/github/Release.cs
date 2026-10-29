using Octokit;
using System.Text.Json;

namespace TrakHound.Builder.GitHub
{
    [CommandGroup("github release")]
    internal static class Release
    {
        [Command("publish")]
        public static async Task Publish([CommandOption] string configurationId = null)
        {
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            Console.WriteLine($"Publish GitHub Release : Configuration ID = {configurationId}");
            Console.WriteLine("-----------------------------------------------------");

            await PublishRelease(configurationId);
        }


        static async Task PublishRelease(string configurationId)
        {
            var config = Configuration.Read(configurationId);

            if (config.GitHubRepo != null)
            {
                var githubClient = new GitHubClient(new ProductHeaderValue("mtconnect-net-builder"));

                var tokenAuth = new Credentials(config.GitHubToken);
                githubClient.Credentials = tokenAuth;

                // Verify all files exist before creating the draft so a missing file doesn't leave an empty release
                var releaseFiles = GetReleaseFiles(config);
                var missingFiles = releaseFiles.Where(o => !File.Exists(o)).ToArray();
                if (missingFiles.Length > 0)
                {
                    foreach (var missingFile in missingFiles) Console.WriteLine($"ERROR : Release File Not Found : {missingFile}");
                    Environment.Exit(1);
                }

                // The release is tagged on the Commit the files were built from (not the default branch's latest Commit)
                var commit = GetBuildCommit(config);

                var releaseId = await CreateReleaseDraft(config, githubClient, commit);
                await UploadReleaseFiles(config, githubClient, releaseId, releaseFiles);

                Console.Write("DONE");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("No GitHub Repo Configured. Skipped.");
            }
        }

        static async Task<long> CreateReleaseDraft(Configuration configuration, GitHubClient client, string commit)
        {
            var version = AppVersion.GetVersion(configuration.Id);
            var versionSuffix = AppVersion.GetVersionSuffix(configuration.Id);

            var newRelease = new NewRelease($"v{version}");
            newRelease.Name = $"MTConnect.NET {version}";
            newRelease.Body = $"This is the v{version} Release of MTConnect.NET";
            newRelease.TargetCommitish = commit;
            newRelease.Draft = true;
            newRelease.Prerelease = !string.IsNullOrEmpty(versionSuffix) && !configuration.RemoveVersionSuffix;

            var result = await client.Repository.Release.Create(configuration.GitHubUser, configuration.GitHubRepo, newRelease);
            Console.WriteLine($"Created release draft id {result.Id} (v{version} on {commit})");

            return result.Id;
        }

        static async Task UploadReleaseFiles(Configuration configuration, GitHubClient client, long releaseId, string[] filePaths)
        {
            var release = await client.Repository.Release.Get(configuration.GitHubUser, configuration.GitHubRepo, releaseId);

            foreach (var filePath in filePaths)
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    var assetUpload = new ReleaseAssetUpload()
                    {
                        FileName = Path.GetFileName(filePath),
                        ContentType = GetContentType(filePath),
                        RawData = fileStream
                    };
                    await client.Repository.Release.UploadAsset(release, assetUpload);
                }
                Console.WriteLine($"{Path.GetFileName(filePath)} Uploaded");
            }
        }

        static string[] GetReleaseFiles(Configuration configuration)
        {
            var version = AppVersion.GetVersion(configuration.Id);
            var versionSuffix = AppVersion.GetVersionSuffix(configuration.Id);
            var outputPath = Path.Combine(configuration.Output, version);

            return new[]
            {
                Path.Combine(outputPath, $"{Instance.Installer.GetOutputFilename(configuration, version, versionSuffix)}.exe")
            };
        }

        /// <summary>
        /// The Commit recorded in [output]/[version]/build.json by the Agent build. Exits the Builder if it's missing or
        /// was built with uncommitted changes
        /// </summary>
        static string GetBuildCommit(Configuration configuration)
        {
            var buildInfoPath = Path.Combine(configuration.Output, AppVersion.GetVersion(configuration.Id), Instance.AgentPublish.BuildInfoFilename);
            if (!File.Exists(buildInfoPath))
            {
                Console.WriteLine($"ERROR : {buildInfoPath} not found (build the Agent Installer first)");
                Environment.Exit(1);
            }

            using var buildInfo = JsonDocument.Parse(File.ReadAllText(buildInfoPath));
            if (buildInfo.RootElement.GetProperty("dirty").GetBoolean())
            {
                Console.WriteLine($"ERROR : The Agent was built with uncommitted changes ({buildInfoPath}) : commit them and rebuild");
                Environment.Exit(1);
            }

            return buildInfo.RootElement.GetProperty("commit").GetString();
        }

        static string GetContentType(string filePath)
        {
            switch (Path.GetExtension(filePath).ToLower())
            {
                case ".exe": return "application/vnd.microsoft.portable-executable";
                case ".zip": return "application/zip";
                default: return "application/octet-stream";
            }
        }
    }
}

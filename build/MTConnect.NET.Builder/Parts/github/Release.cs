using Octokit;

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

                var releaseId = await CreateReleaseDraft(config, githubClient);
                await UploadReleaseFiles(config, githubClient, releaseId);

                Console.Write("DONE");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("No GitHub Repo Configured. Skipped.");
            }
        }

        static async Task<long> CreateReleaseDraft(Configuration configuration, GitHubClient client)
        {
            var version = AppVersion.GetVersion(configuration.Id);

            var newRelease = new NewRelease($"v{version}");
            newRelease.Name = $"MTConnect.NET {version}";
            newRelease.Body = $"This is the v{version} Release of MTConnect.NET";
            newRelease.Draft = true;
            newRelease.Prerelease = false;

            var result = await client.Repository.Release.Create(configuration.GitHubUser, configuration.GitHubRepo, newRelease);
            Console.WriteLine("Created release draft id {0}", result.Id);

            return result.Id;
        }

        static async Task UploadReleaseFiles(Configuration configuration, GitHubClient client, long releaseId)
        {
            var version = AppVersion.GetVersion(configuration.Id);
            var outputPath = Path.Combine(Environment.CurrentDirectory, configuration.Output, version);

            // Upload Instance Installer Executable
            var installerFilename = $"{configuration.Agent.Installer.Filename}-{version}-install.exe";
            var installerPath = Path.Combine(outputPath, installerFilename);
            using (var installerExe = File.OpenRead(installerPath))
            {
                var assetUpload = new ReleaseAssetUpload()
                {
                    FileName = installerFilename,
                    ContentType = "application/zip",
                    RawData = installerExe
                };
                var release = await client.Repository.Release.Get(configuration.GitHubUser, configuration.GitHubRepo, releaseId);
                var asset = await client.Repository.Release.UploadAsset(release, assetUpload);
            }
            Console.WriteLine("Instance Installer Uploaded");
        }
    }
}

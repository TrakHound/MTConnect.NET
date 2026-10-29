namespace TrakHound.Builder.Instance
{
    /// <summary>
    /// Builds the Agent Docker Images (one per configured Image, ex. linux-musl-x64 and linux-musl-arm64) and publishes them
    /// with multi-platform [imageName]:[version] and [imageName]:latest manifests
    /// </summary>
    [CommandGroup("agent docker")]
    internal static class Docker
    {
        private static readonly string[] _contextFiles = { "Dockerfile", "docker-healthcheck.sh" };
        private const string _defaultBaseImage = "mcr.microsoft.com/dotnet/runtime";


        [Command("build")]
        public static async Task Build([CommandOption] string configurationId = null, [CommandOption] bool verbose = false)
        {
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            Console.WriteLine($"Build Agent Docker Images : Configuration ID = {configurationId}");
            Console.WriteLine("-----------------------------------------------------");

            CommandLine.Verbose = verbose;

            var config = Configuration.Read(configurationId);
            var images = GetImages(config);

            AgentPublish.Publish(configurationId, "Agent Docker", config.Agent.Docker.ProjectPath, images.Select(o => o.Runtime).Distinct().ToArray(), images.Select(o => o.Framework).Distinct().ToArray());

            await CreateContexts(configurationId, images);
            BuildImages(configurationId, images);

            Console.WriteLine();
        }

        [Command("publish")]
        public static async Task Publish([CommandOption] string configurationId = null, [CommandOption] bool verbose = false)
        {
            await Task.CompletedTask;
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            Console.WriteLine($"Publish Agent Docker Images : Configuration ID = {configurationId}");
            Console.WriteLine("-----------------------------------------------------");

            CommandLine.Verbose = verbose;

            var config = Configuration.Read(configurationId);
            var images = GetImages(config);
            var version = AppVersion.GetVersion(configurationId);

            foreach (var image in images)
            {
                Push($"Pushing Agent Docker Image ({image.Tag})", GetImageTag(config, version, image), GetImageTag(config, "latest", image));
            }

            // Combine the Images into one multi-platform tag for the version and for latest
            foreach (var tag in new[] { version, "latest" })
            {
                var manifest = $"{config.Agent.Docker.ImageName}:{tag}";

                // Remove a manifest left from a previous (failed) publish so it isn't amended with old Images
                CommandLine.Run($"docker manifest rm {manifest}", null);

                var imageTags = string.Join(' ', images.Select(o => GetImageTag(config, tag, o)));
                Run($"Creating Agent Docker Manifest ({manifest})", $"docker manifest create {manifest} {imageTags}", null);
                Run($"Pushing Agent Docker Manifest ({manifest})", $"docker manifest push --purge {manifest}", null);
            }

            Console.WriteLine();
        }


        private static IReadOnlyList<DockerImage> GetImages(Configuration config)
        {
            var images = new List<DockerImage>();

            foreach (var imageConfiguration in config.Agent?.Docker?.Images ?? Array.Empty<DockerImageConfiguration>())
            {
                var runtime = imageConfiguration.Runtime;
                var platform = GetPlatform(runtime);
                if (platform == null) Exit($"Docker Image Runtime ({runtime}) is not supported. Use linux-x64, linux-arm64, linux-arm or their linux-musl-* Runtimes");

                var framework = imageConfiguration.Framework;
                if (string.IsNullOrEmpty(framework)) Exit($"Docker Image Framework is not set ({runtime})");

                var baseImage = GetBaseImage(imageConfiguration.BaseImage, framework);

                // Alpine uses musl, so the Agent must be published for a linux-musl-* Runtime (and only there)
                var isAlpine = baseImage.Contains("alpine", StringComparison.OrdinalIgnoreCase);
                var isMusl = runtime.StartsWith("linux-musl-", StringComparison.OrdinalIgnoreCase);
                if (isAlpine != isMusl) Exit($"Docker Image Base Image ({baseImage}) doesn't match its Runtime ({runtime}). Alpine Base Images need a linux-musl-* Runtime");

                // The Tag separates the Images of each platform (ex. [imageName]:[version]-amd64). A leading '-' is allowed for older Configurations
                var tag = !string.IsNullOrEmpty(imageConfiguration.Tag) ? imageConfiguration.Tag.TrimStart('-') : platform.Replace("linux/", "").Replace("/", "");

                images.Add(new DockerImage(baseImage, tag, runtime, framework, platform));
            }

            if (images.Count == 0) Exit("No Docker Images are configured (agent.docker.images)");
            if (images.Select(o => o.Tag).Distinct(StringComparer.OrdinalIgnoreCase).Count() != images.Count) Exit("Each Docker Image needs a different Tag");
            if (images.Select(o => o.Platform).Distinct().Count() != images.Count) Exit("Each Docker Image needs a different Platform (they're combined into one multi-platform tag)");

            return images;
        }

        /// <summary>
        /// The Docker Platform for a .NET Runtime Identifier (ex. linux-x64 = linux/amd64)
        /// </summary>
        private static string GetPlatform(string runtime)
        {
            switch (runtime?.ToLower())
            {
                case "linux-x64":
                case "linux-musl-x64": return "linux/amd64";
                case "linux-arm64":
                case "linux-musl-arm64": return "linux/arm64";
                case "linux-arm":
                case "linux-musl-arm": return "linux/arm/v7";
            }

            return null;
        }

        /// <summary>
        /// The Base Image, which may be set as just a tag of the .NET Runtime Image (ex. 8.0 or 8.0-alpine).
        /// Defaults to the .NET Runtime Image for the Framework (ex. net8.0 = mcr.microsoft.com/dotnet/runtime:8.0)
        /// </summary>
        private static string GetBaseImage(string baseImage, string framework)
        {
            if (string.IsNullOrEmpty(baseImage)) return $"{_defaultBaseImage}:{framework.Replace("net", "")}";
            if (baseImage.Contains('/') || baseImage.Contains(':')) return baseImage;
            return $"{_defaultBaseImage}:{baseImage}";
        }

        private static string GetImageTag(Configuration config, string version, DockerImage image) => $"{config.Agent.Docker.ImageName}:{version}-{image.Tag}";

        private static string GetContextDirectory(Configuration config, DockerImage image) => Path.Combine(config.Input, config.Agent.Docker.ProjectPath, "bin", "docker", image.Tag);


        /// <summary>
        /// Create the Docker build context for each Image in [projectPath]/bin/docker/[tag] (see the Dockerfile)
        /// </summary>
        private static async Task CreateContexts(string configurationId, IReadOnlyList<DockerImage> images)
        {
            var config = Configuration.Read(configurationId);
            var publishDirectory = AgentPublish.GetPublishDirectory(config, config.Agent.Docker.ProjectPath);

            foreach (var image in images)
            {
                Console.Write($"Creating Agent Docker Context ({image.Tag})..");

                var contextDirectory = GetContextDirectory(config, image);
                if (Directory.Exists(contextDirectory)) Directory.Delete(contextDirectory, true);
                Directory.CreateDirectory(contextDirectory);

                CopyDirectory(Path.Combine(publishDirectory, image.Runtime, image.Framework), Path.Combine(contextDirectory, "agent"));

                foreach (var filename in _contextFiles)
                {
                    // Scripts must have LF line endings to run in the container (the repository may have checked them out with CRLF)
                    var contents = await File.ReadAllTextAsync(BuilderPaths.GetPath("Parts", "agent", "docker", filename));
                    await File.WriteAllTextAsync(Path.Combine(contextDirectory, filename), contents.Replace("\r\n", "\n"));
                }

                Console.Write("DONE");
                Console.WriteLine();
            }
        }

        private static void BuildImages(string configurationId, IReadOnlyList<DockerImage> images)
        {
            var config = Configuration.Read(configurationId);
            var version = AppVersion.GetVersion(configurationId);
            var informationalVersion = AppVersion.GetInformationalVersion(configurationId, AppVersion.GetRevisionVersion(configurationId));

            // The Image for the Docker host's platform is also tagged [imageName]:[version] and [imageName]:latest so it can be run locally
            // (the published tags are multi-platform manifests created by Publish)
            var localImage = images.FirstOrDefault(o => o.Platform == GetDockerPlatform()) ?? images[0];

            foreach (var image in images)
            {
                var parts = new List<string>();
                parts.Add("docker build");
                parts.Add($"--platform {image.Platform}");
                parts.Add($"--build-arg BASE_IMAGE={image.BaseImage}");
                parts.Add($"--build-arg VERSION={informationalVersion}");
                parts.Add($"-t {GetImageTag(config, version, image)}");
                parts.Add($"-t {GetImageTag(config, "latest", image)}");
                if (image == localImage)
                {
                    parts.Add($"-t {config.Agent.Docker.ImageName}:{version}");
                    parts.Add($"-t {config.Agent.Docker.ImageName}:latest");
                }
                parts.Add(".");

                var contextDirectory = GetContextDirectory(config, image);
                Run($"Building Agent Docker Image ({image.Tag} : {image.Platform})", string.Join(' ', parts), contextDirectory);
            }
        }

        /// <summary>
        /// The Docker server's platform (ex. linux/amd64), or null if Docker isn't available
        /// </summary>
        private static string GetDockerPlatform()
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo("docker", "version --format {{.Server.Os}}/{{.Server.Arch}}");
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                using (var process = System.Diagnostics.Process.Start(startInfo))
                {
                    var errorTask = process.StandardError.ReadToEndAsync();
                    var output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();
                    if (process.ExitCode == 0 && !string.IsNullOrEmpty(output)) return output;
                }
            }
            catch { }

            return null;
        }

        private static void Push(string label, params string[] tags)
        {
            foreach (var tag in tags)
            {
                Run($"{label} : {tag}", $"docker image push {tag}", null);
            }
        }

        private static void Run(string label, string cmd, string workingDirectory)
        {
            Console.Write($"{label}..");

            if (CommandLine.Run(cmd, workingDirectory))
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

        private static void Exit(string message)
        {
            Console.WriteLine($"ERROR : {message}");
            Environment.Exit(1);
        }


        private static void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            Directory.CreateDirectory(destinationDirectory);

            foreach (var directory in Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(Path.Combine(destinationDirectory, Path.GetRelativePath(sourceDirectory, directory)));
            }

            foreach (var file in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                File.Copy(file, Path.Combine(destinationDirectory, Path.GetRelativePath(sourceDirectory, file)), true);
            }
        }


        private record DockerImage(string BaseImage, string Tag, string Runtime, string Framework, string Platform);
    }
}

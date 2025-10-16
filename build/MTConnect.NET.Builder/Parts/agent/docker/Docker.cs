namespace TrakHound.Builder.Instance
{
    [CommandGroup("agent docker")]
    internal static class Docker
    {
        [Command("build")]
        public static async Task Build([CommandOption] string configurationId = null, [CommandOption] bool verbose = false)
        {
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            Console.WriteLine($"Build Agent Docker Images : Configuration ID = {configurationId}");

            await BuildProject(configurationId, verbose);
            await BuildImage(configurationId, verbose);
        }

        [Command("publish")]
        public static async Task Publish([CommandOption] string configurationId = null, [CommandOption] bool verbose = false)
        {
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            Console.WriteLine($"Publish Agent Docker Images : Configuration ID = {configurationId}");

            await PushImage(configurationId, verbose);
        }

        private static async Task BuildProject(string configurationId, bool verbose)
        {
            var config = Configuration.Read(configurationId);

            var projectDirectory = Path.Combine(config.Input, config.Agent.Docker.ProjectPath);
            var outputDirectory = Path.Combine(config.Input, config.Agent.Docker.ProjectPath, "bin", "publish");
            if (Directory.Exists(outputDirectory)) Directory.Delete(outputDirectory, true);

            var dockerFilePath = Path.Combine(Environment.CurrentDirectory, "Parts", "agent", "docker", "Dockerfile");

            var version = AppVersion.GetVersion(configurationId);
            var versionSuffix = AppVersion.GetVersionSuffix(configurationId);

            foreach (var image in config.Agent.Docker.Images)
            {
                Console.Write($"Building Agent Docker Project : {image.Runtime} : {image.Framework} : ");

                var publishOutput = Path.Combine(outputDirectory, image.Runtime, image.Framework);

                var parts = new List<string>();
                parts.Add("dotnet publish");
                parts.Add("-c:Release");
                parts.Add($"-r:{image.Runtime}");
                parts.Add($"-f:{image.Framework}");
                parts.Add($"-p:VersionPrefix=\"{version}\"");
                if (config.RemoveVersionSuffix) parts.Add($"-p:VersionSuffix=\"\"");
                parts.Add("-p:WarningLevel=0");
                parts.Add("--no-self-contained");
                parts.Add($"--output \"{publishOutput}/\"");

                var cmd = string.Join(' ', parts);
                if (CommandLine.Run(cmd, projectDirectory, verbose))
                {
                    // Copy DockerFile
                    var imageDockerFilePath = Path.Combine(publishOutput, "Dockerfile");
                    File.Copy(dockerFilePath, imageDockerFilePath, true);

                    var dockerFileContents = await File.ReadAllTextAsync(imageDockerFilePath);
                    dockerFileContents = dockerFileContents.Replace("{DOCKER_BASE_IMAGE}", image.BaseImage);
                    await File.WriteAllTextAsync(imageDockerFilePath, dockerFileContents);

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

        private static async Task BuildImage(string configurationId, bool verbose)
        {
            Console.Write("Building Agent Docker Image : ");

            var config = Configuration.Read(configurationId);
            var outputDirectory = Path.Combine(config.Input, config.Agent.Docker.ProjectPath, "bin", "publish");

            foreach (var image in config.Agent.Docker.Images)
            {
                var version = AppVersion.GetVersion(configurationId);
                var tag = $"{config.Agent.Docker.ImageName}";
                var versionTag = $"{config.Agent.Docker.ImageName}:{version}{image.Tag}";
                var latestTag = $"{config.Agent.Docker.ImageName}:latest{image.Tag}";

                var inputDirectory = Path.Combine(outputDirectory, image.Runtime, image.Framework);

                var parts = new List<string>();
                parts.Add("docker build");
                if (string.IsNullOrEmpty(image.Tag)) parts.Add($"-t {tag}");
                parts.Add($"-t {versionTag}");
                parts.Add($"-t {latestTag}");
                parts.Add(inputDirectory);

                var cmd = string.Join(' ', parts);
                if (CommandLine.Run(cmd, inputDirectory, verbose))
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

        private static async Task PushImage(string configurationId, bool verbose)
        {
            Console.Write("Pushing Agent Docker Image : ");

            var config = Configuration.Read(configurationId);

            foreach (var image in config.Agent.Docker.Images)
            {
                var version = AppVersion.GetVersion(configurationId);
                var tag = $"{config.Agent.Docker.ImageName}";
                var versionTag = $"{config.Agent.Docker.ImageName}:{version}";
                var latestTag = $"{config.Agent.Docker.ImageName}:latest";

                if (CommandLine.Run($"docker image push {tag}", null, verbose))
                {
                    if (CommandLine.Run($"docker image push {versionTag}", null, verbose))
                    {
                        if (CommandLine.Run($"docker image push {latestTag}", null, verbose))
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
                    else
                    {
                        Console.Write("ERROR");
                        Console.WriteLine();
                        Environment.Exit(1);
                    }
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
}

namespace TrakHound.Builder.Libraries
{
    [CommandGroup("libraries nuget")]
    internal static class Nuget
    {
        [Command("build")]
        public static async Task Build([CommandOption] string configurationId = null, [CommandOption] bool verbose = false)
        {
            await Task.CompletedTask;
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            Console.WriteLine($"Build Nuget Libraries : Configuration ID = {configurationId}");
            Console.WriteLine("-----------------------------------------------------");

            CommandLine.Verbose = verbose;

            var config = Configuration.Read(configurationId);
            if (!config.Libraries.IsNullOrEmpty())
            {
                var version = AppVersion.GetVersion(configurationId);

                var outputPath = Path.Combine(config.Output, version, "libraries");
                if (Directory.Exists(outputPath)) Directory.Delete(outputPath, true);

                foreach (var librariesConfiguration in config.Libraries)
                {
                    if (!librariesConfiguration.Packages.IsNullOrEmpty())
                    {
                        PackLibraries(configurationId, config, librariesConfiguration, version, outputPath);
                    }
                }

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Pack all of the Libraries in the group with a single 'dotnet pack' (through a temporary solution) so MSBuild builds
        /// shared dependencies (ex. MTConnect.NET-Common) once and the projects in parallel. Every package gets the same version
        /// ([VersionPrefix].[Revision], see AppVersion.GetSourceInfo), which also makes the dependency versions between the packages correct
        /// (PackageVersion is a global property, so referenced projects are built with it). Exits the Builder if the pack fails
        /// </summary>
        private static void PackLibraries(string configurationId, Configuration config, LibrariesConfiguration librariesConfiguration, string version, string outputPath)
        {
            var packages = librariesConfiguration.Packages;
            var projectConfiguration = !string.IsNullOrEmpty(librariesConfiguration.ProjectConfiguration) ? librariesConfiguration.ProjectConfiguration : "Release";

            var revisionVersion = AppVersion.GetRevisionVersion(configurationId);
            var packageVersion = AppVersion.AddVersionSuffix(configurationId, revisionVersion);

            var solutionDirectory = Path.Combine(Path.GetTempPath(), $"mtconnect-builder-{Guid.NewGuid():N}");
            var solutionPath = Path.Combine(solutionDirectory, "libraries.slnx");

            try
            {
                Directory.CreateDirectory(solutionDirectory);

                // Every project is built with the group's Project Configuration (ex. Package), even if it doesn't list it in its Configurations
                var solution = new System.Text.StringBuilder();
                solution.AppendLine("<Solution>");
                solution.AppendLine("  <Configurations>");
                solution.AppendLine($"    <BuildType Name=\"{projectConfiguration}\" />");
                solution.AppendLine("  </Configurations>");
                foreach (var packageConfiguration in packages)
                {
                    var projectFile = GetProjectFile(Path.Combine(config.Input, packageConfiguration.ProjectPath));
                    solution.AppendLine($"  <Project Path=\"{System.Security.SecurityElement.Escape(projectFile)}\">");
                    solution.AppendLine($"    <BuildType Project=\"{projectConfiguration}\" />");
                    solution.AppendLine("  </Project>");
                }
                solution.AppendLine("</Solution>");
                File.WriteAllText(solutionPath, solution.ToString());

                Console.Write($"Building Nuget Library Projects : {librariesConfiguration.Name} ({packages.Length} Packages) : {packageVersion}..");

                var parts = new List<string>();
                parts.Add("dotnet pack");
                parts.Add($"\"{solutionPath}\"");
                // Referenced projects that aren't in the solution (ex. MTConnect.NET-Common from another group) are built with the
                // Project Configuration too, instead of MSBuild's default (Debug, which targets fewer Frameworks)
                parts.Add("-p:ShouldUnsetParentConfigurationAndPlatform=false");
                parts.Add($"-p:PackageVersion=\"{packageVersion}\"");
                parts.Add($"-p:FileVersion=\"{revisionVersion}\"");
                parts.Add($"-p:InformationalVersion=\"{AppVersion.GetInformationalVersion(configurationId, revisionVersion)}\"");
                if (config.RemoveVersionSuffix) parts.Add($"-p:VersionSuffix=\"\"");
                parts.Add($"-p:Deterministic=\"true\"");
                parts.Add($"-p:Company=\"{librariesConfiguration.Company}\"");
                parts.Add($"-p:Authors=\"{librariesConfiguration.Authors}\"");
                parts.Add($"-p:RepositoryUrl=\"{librariesConfiguration.RepositoryUrl}\"");
                parts.Add($"-p:RepositoryBranch=\"{librariesConfiguration.RepositoryBranch?.Replace("{VERSION}", version)}\"");
                parts.Add($"-p:PackageProjectUrl=\"{librariesConfiguration.PackageProjectUrl}\"");
                parts.Add($"-p:PackageLicenseExpression=\"{librariesConfiguration.PackageLicenseExpression}\"");
                parts.Add($"-p:PackageIcon=\"package-icon.png\"");
                parts.Add($"-p:RepositoryType={librariesConfiguration.RepositoryType}");
                parts.Add($"-p:PublishRepositoryUrl=true");
                parts.Add($"-p:IncludeSymbols=true");
                parts.Add($"-p:IncludeSource=true");
                parts.Add($"-p:EmbedUntrackedSources=true");
                parts.Add($"-p:ProduceReferenceAssembly=true");
                parts.Add($"-p:SymbolPackageFormat=snupkg");
                parts.Add($"-p:DebugSymbols=true");
                parts.Add($"-p:DebugType=portable");
                parts.Add($"--configuration {projectConfiguration}");
                parts.Add($"--output \"{outputPath}/\"");

                var cmd = string.Join(' ', parts);
                if (CommandLine.Run(cmd, config.Input))
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
            finally
            {
                try { if (Directory.Exists(solutionDirectory)) Directory.Delete(solutionDirectory, true); } catch { }
            }
        }

        // The single project file (.csproj) in the Project directory
        private static string GetProjectFile(string projectDirectory)
        {
            var projectFiles = Directory.Exists(projectDirectory) ? Directory.GetFiles(projectDirectory, "*.csproj") : Array.Empty<string>();
            if (projectFiles.Length != 1)
            {
                Console.WriteLine();
                Console.WriteLine($"ERROR : Expected one .csproj file in {projectDirectory} (found {projectFiles.Length})");
                Environment.Exit(1);
            }

            return projectFiles[0];
        }

        [Command("publish")]
        public static async Task Publish([CommandOption] string configurationId = null, [CommandOption] bool verbose = false)
        {
            await Task.CompletedTask;
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            Console.WriteLine($"Publish Nuget Libraries : Configuration ID = {configurationId}");
            Console.WriteLine("-----------------------------------------------------");

            CommandLine.Verbose = verbose;

            var config = Configuration.Read(configurationId);
            if (!config.Libraries.IsNullOrEmpty())
            {
                // The packages are versioned by the Commit (Revision), so uncommitted changes can't be published under it
                AppVersion.EnsureCommitted(configurationId);

                var version = AppVersion.GetVersion(configurationId);
                var packageVersion = AppVersion.AddVersionSuffix(configurationId, AppVersion.GetRevisionVersion(configurationId));

                // Push every package at the same time (each push is network bound)
                var pushes = new List<(string Label, string Command, string WorkingDirectory)>();

                foreach (var librariesConfiguration in config.Libraries)
                {
                    if (!librariesConfiguration.Packages.IsNullOrEmpty())
                    {
                        foreach (var packageConfiguration in librariesConfiguration.Packages)
                        {
                            var projectDirectory = Path.Combine(config.Input, packageConfiguration.ProjectPath);

                            var packageFilename = $"{packageConfiguration.Name}.{packageVersion}.nupkg";

                            var packagePath = Path.Combine(config.Output, version, "libraries", packageFilename);
                            if (!File.Exists(packagePath))
                            {
                                Console.Write($"ERROR : Package Not Found : {packagePath}");
                                Console.WriteLine();
                                Environment.Exit(1);
                            }

                            var parts = new List<string>();
                            parts.Add("dotnet nuget push");
                            parts.Add($"\"{packagePath}\"");
                            parts.Add($"--source \"{librariesConfiguration.NugetServer}\"");
                            parts.Add("--skip-duplicate"); // Allow re-running a Publish (ex. after a later step failed)

                            if (!string.IsNullOrEmpty(librariesConfiguration.NugetApiKey))
                            {
                                parts.Add($"--api-key \"{librariesConfiguration.NugetApiKey}\"");
                            }

                            pushes.Add(($"{packageConfiguration.Name} {packageVersion}", string.Join(' ', parts), projectDirectory));
                        }
                    }
                }

                if (pushes.Count > 0)
                {
                    Console.Write($"Pushing Nuget Packages ({pushes.Count})..");

                    var results = CommandLine.RunParallel(pushes);
                    if (results.All(o => o))
                    {
                        Console.Write("DONE");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.Write($"ERROR : Failed to push {string.Join(", ", pushes.Where((o, i) => !results[i]).Select(o => o.Label))}");
                        Console.WriteLine();
                        Environment.Exit(1);
                    }
                }

                Console.WriteLine();
            }
        }
    }
}

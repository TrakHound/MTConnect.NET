namespace TrakHound.Builder.Libraries
{
    [CommandGroup("libraries nuget")]
    internal static class Nuget
    {
        private const string _productType = "library";


        [Command("build")]
        public static async Task Build([CommandOption] string configurationId = null, [CommandOption] bool verbose = false)
        {
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            Console.WriteLine($"Build Nuget Libraries : Configuration ID = {configurationId}");
            Console.WriteLine("-----------------------------------------------------");

            var config = Configuration.Read(configurationId);
            if (!config.Libraries.IsNullOrEmpty())
            {
                var version = AppVersion.GetVersion(configurationId);
                var versionSuffix = AppVersion.GetVersionSuffix(configurationId);

                var outputPath = Path.Combine(Environment.CurrentDirectory, config.Output, version, "libraries");
                if (Directory.Exists(outputPath)) Directory.Delete(outputPath, true);

                foreach (var librariesConfiguration in config.Libraries)
                {
                    if (!librariesConfiguration.Packages.IsNullOrEmpty())
                    {
                        foreach (var packageConfiguration in librariesConfiguration.Packages)
                        {
                            var projectDirectory = Path.Combine(config.Input, packageConfiguration.ProjectPath);

                            Console.Write($"Building Nuget Library Project : {librariesConfiguration.Name} : {packageConfiguration.Name}..");

                            var versionRevision = AppVersion.GetRevision(configurationId, _productType, packageConfiguration.Name, version) + 1;

                            var packageVersion = $"{version}.{versionRevision}";
                            if (!string.IsNullOrEmpty(versionSuffix) && !config.RemoveVersionSuffix) packageVersion = AppVersion.GenerateFullVersion(packageVersion, versionSuffix);

                            var parts = new List<string>();
                            parts.Add("dotnet pack");
                            parts.Add($"-p:PackageVersion=\"{packageVersion}\"");
                            if (config.RemoveVersionSuffix) parts.Add($"-p:VersionSuffix=\"\"");
                            parts.Add($"-p:Deterministic=\"true\"");
                            parts.Add($"-p:Company=\"{librariesConfiguration.Company}\"");
                            parts.Add($"-p:Authors=\"{librariesConfiguration.Authors}\"");
                            parts.Add($"-p:RepositoryUrl=\"{librariesConfiguration.RepositoryUrl}\"");
                            parts.Add($"-p:RepositoryBranch=\"{librariesConfiguration.RepositoryBranch?.Replace("{VERSION}", version)}\"");
                            parts.Add($"-p:PackageProjectUrl=\"{librariesConfiguration.PackageProjectUrl}\"");
                            parts.Add($"-p:PackageLicenseExpression=\"{librariesConfiguration.PackageLicenseExpression}\"");
                            parts.Add($"-p:PackageIconUrl=\"{librariesConfiguration.PackageIconUrl}\"");
                            parts.Add($"-p:RepositoryType={librariesConfiguration.RepositoryType}");
                            parts.Add($"-p:PublishRepositoryUrl=true");
                            parts.Add($"-p:IncludeSymbols=true");
                            parts.Add($"-p:IncludeSource=true");
                            parts.Add($"-p:EmbedUntrackedSources=true");
                            parts.Add($"-p:ProduceReferenceAssembly=true");
                            parts.Add($"-p:SymbolPackageFormat=snupkg");
                            parts.Add($"-p:DebugSymbols=true");
                            parts.Add($"-p:DebugType=portable");
                            parts.Add($"--configuration {librariesConfiguration.ProjectConfiguration}");
                            parts.Add($"--output {outputPath}/");

                            var cmd = string.Join(' ', parts);
                            if (CommandLine.Run(cmd, projectDirectory, verbose))
                            {
                                AppVersion.SetRevision(configurationId, _productType, packageConfiguration.Name, version, versionRevision);

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

                Console.WriteLine();
            }
        }

        [Command("publish")]
        public static async Task Publish([CommandOption] string configurationId = null, [CommandOption] bool verbose = false)
        {
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            Console.WriteLine($"Publish Nuget Libraries : Configuration ID = {configurationId}");
            Console.WriteLine("-----------------------------------------------------");

            var config = Configuration.Read(configurationId);
            if (!config.Libraries.IsNullOrEmpty())
            {
                var version = AppVersion.GetVersion(configurationId);
                var versionSuffix = AppVersion.GetVersionSuffix(configurationId);

                foreach (var librariesConfiguration in config.Libraries)
                {
                    if (!librariesConfiguration.Packages.IsNullOrEmpty())
                    {
                        foreach (var packageConfiguration in librariesConfiguration.Packages)
                        {
                            var projectDirectory = Path.Combine(config.Input, packageConfiguration.ProjectPath);

                            var versionRevision = AppVersion.GetRevision(configurationId, _productType, packageConfiguration.Name, version);

                            var packageVersion = $"{version}.{versionRevision}";

                            var packageFilename = $"{packageConfiguration.Name}.{packageVersion}.nupkg";
                            if (!string.IsNullOrEmpty(versionSuffix) && !config.RemoveVersionSuffix) packageFilename = $"{packageConfiguration.Name}.{packageVersion}-{versionSuffix}.nupkg";

                            var packagePath = Path.Combine(Environment.CurrentDirectory, config.Output, version, "libraries", packageFilename);
                            if (File.Exists(packagePath))
                            {
                                Console.Write($"Pushing Nuget Package : {librariesConfiguration.Name} : {packageConfiguration.Name} ({librariesConfiguration.NugetServer})..");

                                var parts = new List<string>();
                                parts.Add("dotnet nuget push");
                                parts.Add(packagePath);
                                parts.Add($"--source \"{librariesConfiguration.NugetServer}\"");

                                if (!string.IsNullOrEmpty(librariesConfiguration.NugetApiKey))
                                {
                                    parts.Add($"--api-key \"{librariesConfiguration.NugetApiKey}\"");
                                }

                                var cmd = string.Join(' ', parts);
                                if (CommandLine.Run(cmd, projectDirectory, verbose))
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
                                Console.Write($"ERROR : Package Not Found : {packagePath}");
                                Console.WriteLine();
                                Environment.Exit(1);
                            }
                        }
                    }
                }

                Console.WriteLine();
            }
        }
    }
}

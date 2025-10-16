namespace TrakHound.Builder.Instance
{
    [CommandGroup("agent installer")]
    internal static class Installer
    {
        [Command("build")]
        public static async Task Build([CommandOption] string configurationId = null, [CommandOption] bool verbose = false)
        {
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            Console.WriteLine($"Build Agent Installer : Configuration ID = {configurationId}");
            Console.WriteLine("-----------------------------------------------------");

            await BuildProject(configurationId, verbose);
            await CreateInstaller(configurationId, verbose);

            Console.WriteLine();
        }


        private static async Task BuildProject(string configurationId, bool verbose)
        {
            var config = Configuration.Read(configurationId);

            var projectDirectory = Path.Combine(config.Input, config.Agent.Installer.ProjectPath);
            var outputDirectory = Path.Combine(config.Input, config.Agent.Installer.ProjectPath, "bin", "publish");
            if (Directory.Exists(outputDirectory)) Directory.Delete(outputDirectory, true);

            var version = AppVersion.GetVersion(configurationId);
            var versionSuffix = AppVersion.GetVersionSuffix(configurationId);

            foreach (var runtime in config.Agent.Installer.Runtimes)
            {
                foreach (var framework in config.Agent.Installer.Frameworks)
                {
                    Console.Write($"Building Agent Installer Project ({runtime} : {framework})..");

                    var publishOutput = Path.Combine(outputDirectory, runtime, framework);

                    var parts = new List<string>();
                    parts.Add("dotnet publish");
                    parts.Add("-c:Release");
                    parts.Add($"-r:{runtime}");
                    parts.Add($"-f:{framework}");
                    parts.Add($"-p:VersionPrefix=\"{version}\"");
                    if (config.RemoveVersionSuffix) parts.Add($"-p:VersionSuffix=\"\"");
                    parts.Add("-p:WarningLevel=0");
                    parts.Add("--no-self-contained");
                    parts.Add($"--output \"{publishOutput}/\"");

                    var cmd = string.Join(' ', parts);
                    if (CommandLine.Run(cmd, projectDirectory, verbose))
                    {
                        //// Remove instance.config.yaml file
                        //var instanceConfigurationPath = Path.Combine(publishOutput, "config", "instance.config.yaml");
                        //if (File.Exists(instanceConfigurationPath)) File.Delete(instanceConfigurationPath);

                        //// Remove remotes.config.yaml file
                        //var remotesConfigurationPath = Path.Combine(publishOutput, "config", "remotes.config.yaml");
                        //if (File.Exists(remotesConfigurationPath)) File.Delete(remotesConfigurationPath);

                        //// Remove .user file (Instance Login)
                        //var userLoginPath = Path.Combine(publishOutput, "config", ".user");
                        //if (File.Exists(userLoginPath)) File.Delete(userLoginPath);

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

        private static async Task CreateInstaller(string configurationId, bool verbose)
        {
            Console.Write("Creating Agent Installer..");

            var config = Configuration.Read(configurationId);

            var version = AppVersion.GetVersion(configurationId);
            var versionSuffix = AppVersion.GetVersionSuffix(configurationId);

            var inputPath = Path.Combine(config.Input, config.Agent.Installer.ProjectPath, "bin", "publish");
            var outputPath = Path.Combine(Environment.CurrentDirectory, config.Output, version);

            var installerFile = Path.Combine(Environment.CurrentDirectory, "Parts", "agent", "installer", "installer.iss");
            var tempFile = Path.Combine(Environment.CurrentDirectory, "Parts", "agent", "installer", "installer-temp.iss");
            File.Copy(installerFile, tempFile, true);

            var outputFilename = $"{config.Agent.Installer.Filename}-{version}-install";
            if (!string.IsNullOrEmpty(versionSuffix) && !config.RemoveVersionSuffix) outputFilename = $"{config.Agent.Installer.Filename}-{version}-{versionSuffix}-install";

            var tempContents = await File.ReadAllTextAsync(tempFile);
            tempContents = tempContents.Replace("[ASSEMBLY_VERSION]", version);
            tempContents = tempContents.Replace("[PUBLISHER]", config.Publisher);
            tempContents = tempContents.Replace("[APPLICATION_NAME]", config.Agent.Installer.ApplicationName);
            tempContents = tempContents.Replace("[FILENAME]", config.Agent.Installer.Filename);
            tempContents = tempContents.Replace("[BASE_PATH]", Environment.CurrentDirectory);
            tempContents = tempContents.Replace("[INPUT_PATH]", inputPath);
            tempContents = tempContents.Replace("[OUTPUT_PATH]", outputPath);
            tempContents = tempContents.Replace("[STYLES_PATH]", config.Agent.Styles);
            tempContents = tempContents.Replace("[SCHEMAS_PATH]", config.Agent.Schemas);
            tempContents = tempContents.Replace("[OUTPUT_EXE]", outputFilename);
            await File.WriteAllTextAsync(tempFile, tempContents);

            var parts = new List<string>();
            parts.Add($"\"{config.InnoSetupPath}\"");
            parts.Add(tempFile);

            var cmd = string.Join(' ', parts);
            if (CommandLine.Run(cmd, Environment.CurrentDirectory, verbose))
            {
                File.Delete(tempFile);
                Console.Write("DONE");
                Console.WriteLine();
            }
            else
            {
                File.Delete(tempFile);
                Console.Write("ERROR");
                Console.WriteLine();
                Environment.Exit(1);
            }
        }
    }
}

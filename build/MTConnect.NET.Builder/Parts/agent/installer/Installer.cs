using System.Text;

namespace TrakHound.Builder.Instance
{
    [CommandGroup("agent installer")]
    internal static class Installer
    {
        private const string _componentsFilename = "installer-components.iss";


        [Command("build")]
        public static async Task Build([CommandOption] string configurationId = null, [CommandOption] bool verbose = false)
        {
            await Task.CompletedTask;
            if (string.IsNullOrEmpty(configurationId)) configurationId = Configuration.DefaultId;

            Console.WriteLine($"Build Agent Installer : Configuration ID = {configurationId}");
            Console.WriteLine("-----------------------------------------------------");

            CommandLine.Verbose = verbose;

            var config = Configuration.Read(configurationId);
            var installer = config.Agent.Installer;
            AgentPublish.Publish(configurationId, "Agent Installer", installer.ProjectPath, installer.Runtimes, installer.Frameworks);

            CreateInstaller(configurationId);

            Console.WriteLine();
        }

        /// <summary>
        /// Returns the installer executable name (without the ".exe" extension) that
        /// Inno Setup produces for the given version and suffix.
        /// </summary>
        public static string GetOutputFilename(Configuration config, string version, string versionSuffix)
        {
            if (!string.IsNullOrEmpty(versionSuffix) && !config.RemoveVersionSuffix) return $"{config.Agent.Installer.Filename}-{version}-{versionSuffix}-install";
            return $"{config.Agent.Installer.Filename}-{version}-install";
        }


        private static void CreateInstaller(string configurationId)
        {
            var config = Configuration.Read(configurationId);
            var installer = config.Agent.Installer;

            var version = AppVersion.GetVersion(configurationId);
            var versionSuffix = AppVersion.GetVersionSuffix(configurationId);

            var publishDirectory = AgentPublish.GetPublishDirectory(config, installer.ProjectPath);

            // Files that are identical for every Runtime are installed from [publish]/common so they're only compressed once
            foreach (var framework in installer.Frameworks)
            {
                ParallelPublish.MoveCommonFiles(publishDirectory, installer.Runtimes, framework);
            }

            // Signed after the common files are moved (signatures are timestamped, so identical files wouldn't stay identical)
            SignFiles(config, publishDirectory);

            // The Setup Types, Components and Program Files for each Runtime and Framework
            var componentsPath = Path.Combine(publishDirectory, _componentsFilename);
            File.WriteAllText(componentsPath, CreateComponents(installer, publishDirectory));

            Console.Write("Creating Agent Installer..");

            var defines = new Dictionary<string, string>
            {
                ["MyAppName"] = installer.ApplicationName,
                ["MyAppVersion"] = version,
                ["MyFileVersion"] = AppVersion.GetRevisionVersion(configurationId),
                ["MyAppPublisher"] = config.Publisher,
                ["BasePath"] = BuilderPaths.BaseDirectory,
                ["StylesPath"] = config.Agent.Styles,
                ["SchemasPath"] = config.Agent.Schemas,
                ["ComponentsFile"] = componentsPath
            };

            var scriptPath = BuilderPaths.GetPath("Parts", "agent", "installer", "installer.iss");
            var outputPath = Path.Combine(config.Output, version);

            if (InnoSetup.Compile(config, scriptPath, defines, outputPath, GetOutputFilename(config, version, versionSuffix)))
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

        /// <summary>
        /// Inno Setup [Types], [Components] and [Files] entries for each Runtime and Framework. Setup installs the one Type
        /// selected in the wizard (the first, ex. Windows x64 and the newest Framework, is the default)
        /// </summary>
        private static string CreateComponents(InstallerConfiguration installer, string publishDirectory)
        {
            // x64 first so it's the default Type
            var runtimes = installer.Runtimes.OrderBy(o => o.EndsWith("x64", StringComparison.OrdinalIgnoreCase) ? 0 : 1).ToArray();

            var types = new List<(string Name, string Description, string Runtime, string Framework)>();
            foreach (var framework in installer.Frameworks)
            {
                foreach (var runtime in runtimes)
                {
                    var name = $"{runtime.Replace("-x", "").Replace("-", "")}{framework.Split('.')[0]}";
                    var description = $"Windows {runtime.Substring(runtime.IndexOf('-') + 1)} {InnoSetup.GetFrameworkDisplayName(framework)}";
                    types.Add((name, description, runtime, framework));
                }
            }

            var allTypes = string.Join(' ', types.Select(o => o.Name));

            var sb = new StringBuilder();
            sb.AppendLine("; Generated by the MTConnect.NET Builder from the Configuration's installer runtimes and frameworks");
            sb.AppendLine();

            sb.AppendLine("[Types]");
            foreach (var type in types) sb.AppendLine($"Name: \"{type.Name}\"; Description: \"{type.Description}\"");
            sb.AppendLine();

            sb.AppendLine("[Components]");
            foreach (var type in types) sb.AppendLine($"Name: \"{type.Name}\"; Description: \"{{#MyAppVerName}}\"; Types: {type.Name}");
            sb.AppendLine($"Name: \"configuration\"; Description: \"Configuration Files\"; Types: {allTypes}");
            sb.AppendLine($"Name: \"schemas\"; Description: \"MTConnect XSD Schema Files\"; Types: {allTypes}");
            sb.AppendLine($"Name: \"styles\"; Description: \"XSL Stylesheet Files\"; Types: {allTypes}");
            sb.AppendLine();

            sb.AppendLine("[Files]");
            foreach (var type in types)
            {
                sb.AppendLine($"Source: \"{Path.Combine(publishDirectory, type.Runtime, type.Framework)}\\*\"; DestDir: \"{{app}}\"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: {type.Name};");
            }
            foreach (var framework in installer.Frameworks)
            {
                var frameworkTypes = string.Join(' ', types.Where(o => o.Framework == framework).Select(o => o.Name));
                sb.AppendLine($"Source: \"{Path.Combine(publishDirectory, "common", framework)}\\*\"; DestDir: \"{{app}}\"; Flags: ignoreversion recursesubdirs skipifsourcedoesntexist; Permissions: everyone-full; Components: {frameworkTypes};");
            }

            return sb.ToString();
        }

        private static void SignFiles(Configuration config, string publishDirectory)
        {
            if (!Signing.IsEnabled(config)) return;

            Console.Write("Signing Agent Files..");

            if (Signing.SignFiles(config, publishDirectory))
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
}

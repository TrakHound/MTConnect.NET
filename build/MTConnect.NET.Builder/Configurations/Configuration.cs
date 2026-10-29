using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrakHound.Builder
{
    internal class Configuration
    {
        public const string DefaultId = "default";

        private static readonly Dictionary<string, Configuration> _configurations = new();
        private static readonly object _lock = new();


        public string Id { get; set; }

        public string VersionInfoPath { get; set; }

        public bool RemoveVersionSuffix { get; set; }

        public string Publisher { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }

        public string InnoSetupPath { get; set; }

        public SigningConfiguration Signing { get; set; }

        public string GitHubToken { get; set; }

        public string GitHubUser { get; set; }

        public string GitHubRepo { get; set; }

        public AgentConfiguration Agent { get; set; }

        public LibrariesConfiguration[] Libraries { get; set; }


        /// <summary>
        /// Read config.[ID].yaml from the Builder directory. Relative paths in the file (input, output, versionInfoPath,
        /// agent styles and schemas) are resolved against the Builder directory. Exits the Builder if the file can't be read
        /// </summary>
        public static Configuration Read(string id)
        {
            if (string.IsNullOrEmpty(id)) id = DefaultId;

            lock (_lock)
            {
                if (_configurations.TryGetValue(id, out var cached)) return cached;

                var configurationPath = BuilderPaths.GetPath($"config.{id}.yaml");
                if (!File.Exists(configurationPath))
                {
                    Exit($"Configuration File not Found : {configurationPath} (copy config.template.yaml to config.{id}.yaml)");
                }

                Configuration configuration = null;
                try
                {
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .IgnoreUnmatchedProperties()
                        .Build();

                    configuration = deserializer.Deserialize<Configuration>(File.ReadAllText(configurationPath));
                }
                catch (Exception ex)
                {
                    Exit($"Error reading Configuration File ({configurationPath}) : {ex.Message}");
                }

                if (configuration == null) Exit($"Configuration File is empty : {configurationPath}");
                if (string.IsNullOrEmpty(configuration.Input)) Exit($"'input' not set in {configurationPath}");
                if (string.IsNullOrEmpty(configuration.Output)) Exit($"'output' not set in {configurationPath}");

                configuration.Id = id;
                configuration.Input = BuilderPaths.Resolve(configuration.Input);
                configuration.Output = BuilderPaths.Resolve(configuration.Output);
                configuration.VersionInfoPath = !string.IsNullOrEmpty(configuration.VersionInfoPath) ? BuilderPaths.Resolve(configuration.VersionInfoPath) : Path.Combine(configuration.Input, "Directory.Build.props");
                if (configuration.Agent != null)
                {
                    if (!string.IsNullOrEmpty(configuration.Agent.Styles)) configuration.Agent.Styles = BuilderPaths.Resolve(configuration.Agent.Styles);
                    if (!string.IsNullOrEmpty(configuration.Agent.Schemas)) configuration.Agent.Schemas = BuilderPaths.Resolve(configuration.Agent.Schemas);
                }

                _configurations[id] = configuration;
                return configuration;
            }
        }

        private static void Exit(string message)
        {
            Console.WriteLine($"ERROR : {message}");
            Environment.Exit(1);
        }
    }
}

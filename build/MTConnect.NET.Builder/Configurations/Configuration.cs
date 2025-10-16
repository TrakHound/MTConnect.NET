using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrakHound.Builder
{
    internal class Configuration
    {
        public const string DefaultId = "default";


        public string Id { get; set; }

        public string VersionInfoPath { get; set; }

        public bool RemoveVersionSuffix { get; set; }

        public string Publisher { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }

        public string InnoSetupPath { get; set; }

        public string GitHubToken { get; set; }

        public string GitHubUser { get; set; }

        public string GitHubRepo { get; set; }

        public AgentConfiguration Agent { get; set; }

        public LibrariesConfiguration[] Libraries { get; set; }


        public static Configuration Read(string id)
        {
            var configurationPath = Path.Combine(Environment.CurrentDirectory, $"config.{id}.yaml");

            try
            {
                var text = File.ReadAllText(configurationPath);
                if (!string.IsNullOrEmpty(text))
                {
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .IgnoreUnmatchedProperties()
                        .Build();

                    var configuration = deserializer.Deserialize<Configuration>(text);
                    if (configuration != null)
                    {
                        configuration.Id = id;
                        return configuration;
                    }
                }
            }
            catch { }

            return null;
        }
    }
}

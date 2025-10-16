using System.Text.Json.Serialization;

namespace TrakHound.Builder
{
    internal class InstallerConfiguration
    {
        public string ApplicationName { get; set; }
        public string Filename { get; set; }
        public string ProjectPath { get; set; }
        public string[] Runtimes { get; set; }
        public string[] Frameworks { get; set; }
    }
}

namespace TrakHound.Builder
{
    internal class AgentConfiguration
    {
        public InstallerConfiguration Installer { get; set; }

        public DockerConfiguration Docker { get; set; }

        public string Styles { get; set; }

        public string Schemas { get; set; }
    }
}

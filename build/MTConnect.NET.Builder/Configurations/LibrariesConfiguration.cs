namespace TrakHound.Builder
{
    internal class LibrariesConfiguration
    {
        public string Name { get; set; }
        public string NugetServer { get; set; }
        public string NugetApiKey { get; set; }
        public string ProjectConfiguration { get; set; }
        public string Company { get; set; }
        public string Authors { get; set; }
        public string Copyright { get; set; }
        public string RepositoryType { get; set; }
        public string RepositoryUrl { get; set; }
        public string RepositoryBranch { get; set; }
        public string PackageTags { get; set; }
        public string PackageProjectUrl { get; set; }
        public string PackageLicenseExpression { get; set; }
        public string PackageIconUrl { get; set; }
        public string VersionSuffix { get; set; }
        public PackageConfiguration[] Packages { get; set; }
    }
}

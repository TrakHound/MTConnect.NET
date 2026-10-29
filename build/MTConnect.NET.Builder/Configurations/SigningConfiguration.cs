namespace TrakHound.Builder
{
    /// <summary>
    /// Code Signing with Azure Artifact Signing (signtool with the Artifact Signing Client Tools dlib).
    /// Signing is disabled when not configured
    /// </summary>
    internal class SigningConfiguration
    {
        /// <summary>
        /// signtool.exe from the Windows SDK (defaults to 'signtool' on the PATH)
        /// </summary>
        public string SignToolPath { get; set; }

        /// <summary>
        /// Azure.CodeSigning.Dlib.dll installed by Microsoft.Azure.ArtifactSigningClientTools
        /// </summary>
        public string DlibPath { get; set; }

        /// <summary>
        /// Artifact Signing metadata (Endpoint, CodeSigningAccountName, CertificateProfileName), relative to the Builder directory
        /// </summary>
        public string MetadataPath { get; set; } = "metadata.json";

        public string TimestampUrl { get; set; } = "http://timestamp.acs.microsoft.com";

        /// <summary>
        /// Filename patterns of the published files to sign before they're added to the Installer
        /// </summary>
        public string[] Files { get; set; } = { "MTConnect*.dll", "agent.exe", "agent.dll" };
    }
}

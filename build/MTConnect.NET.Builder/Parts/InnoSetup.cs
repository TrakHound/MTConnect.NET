namespace TrakHound.Builder
{
    /// <summary>
    /// Compiles an Inno Setup script, passing the build values as ISCC preprocessor defines (/D) instead of editing the script
    /// </summary>
    internal static class InnoSetup
    {
        private const string _signToolName = "TrakHound";

        public static bool Compile(Configuration config, string scriptPath, IDictionary<string, string> defines, string outputDirectory, string outputFilename)
        {
            if (!Directory.Exists(outputDirectory)) Directory.CreateDirectory(outputDirectory);

            var parts = new List<string>();
            parts.Add($"\"{config.InnoSetupPath}\"");
            parts.Add("/Q");

            foreach (var define in defines)
            {
                if (define.Value != null) parts.Add(QuoteArgument($"/D{define.Key}={TrimPath(define.Value)}"));
            }

            // Sign Tool used to sign the Setup and Uninstaller (only enabled in the script when SignTool is defined, see installer.iss).
            // Inno Setup replaces $q with a quote and $f with the quoted file to sign
            if (Signing.IsEnabled(config))
            {
                parts.Add(QuoteArgument($"/S{_signToolName}={Signing.GetCommand(config, "$q")} $f"));
                parts.Add(QuoteArgument($"/DSignTool={_signToolName}"));
            }

            parts.Add(QuoteArgument($"/O{TrimPath(outputDirectory)}"));
            parts.Add(QuoteArgument($"/F{outputFilename}"));
            parts.Add(QuoteArgument(scriptPath));

            return CommandLine.Run(string.Join(' ', parts), Path.GetDirectoryName(scriptPath));
        }

        // ex. "net8.0" => ".NET 8", "net48" => ".NET 4.8 Framework"
        public static string GetFrameworkDisplayName(string framework)
        {
            if (!string.IsNullOrEmpty(framework) && framework.StartsWith("net"))
            {
                var versionText = framework.Substring(3);
                if (versionText.Contains('.') && Version.TryParse(versionText, out var version)) return $".NET {version.Major}";

                // .NET Framework monikers have no dots (ex. net48, net461)
                if (versionText.Length > 1 && versionText.All(char.IsDigit)) return $".NET {string.Join('.', versionText.ToCharArray())} Framework";
            }

            return framework;
        }

        // A trailing backslash before the closing quote would escape the quote
        private static string TrimPath(string value) => value.TrimEnd('\\', '/');

        private static string QuoteArgument(string value) => $"\"{value}\"";
    }
}

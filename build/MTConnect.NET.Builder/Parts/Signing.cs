namespace TrakHound.Builder
{
    /// <summary>
    /// Signs files with signtool and Azure Artifact Signing (see SigningConfiguration).
    /// signtool authenticates with the Azure CLI login (az login) of the build server
    /// </summary>
    internal static class Signing
    {
        public static bool IsEnabled(Configuration config) => !string.IsNullOrEmpty(config.Signing?.DlibPath);


        /// <summary>
        /// The signtool command without the files to sign. 'quote' is the quote character ("$q" for an Inno Setup Sign Tool)
        /// </summary>
        public static string GetCommand(Configuration config, string quote = "\"")
        {
            var signing = config.Signing;
            var signToolPath = !string.IsNullOrEmpty(signing.SignToolPath) ? signing.SignToolPath : "signtool";

            return $"{quote}{signToolPath}{quote} sign /fd SHA256 /tr {signing.TimestampUrl} /td SHA256 " +
                $"/dlib {quote}{signing.DlibPath}{quote} /dmdf {quote}{BuilderPaths.Resolve(signing.MetadataPath)}{quote}";
        }

        /// <summary>
        /// Sign the files in the directory (and its subdirectories) that match the configured Files patterns.
        /// Returns true when signing is disabled or there are no files to sign
        /// </summary>
        public static bool SignFiles(Configuration config, string directory)
        {
            if (!IsEnabled(config) || !Directory.Exists(directory)) return true;

            var files = config.Signing.Files
                .SelectMany(pattern => Directory.GetFiles(directory, pattern, SearchOption.AllDirectories))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (files.Count == 0) return true;

            // As few signtool calls as possible, each kept under the Windows command line limit (32767 characters)
            var command = GetCommand(config);
            foreach (var batch in GetBatches(files.Select(o => $"\"{o}\""), 30000 - command.Length))
            {
                if (!CommandLine.Run($"{command} {string.Join(' ', batch)}", directory)) return false;
            }

            return true;
        }

        private static IEnumerable<List<string>> GetBatches(IEnumerable<string> arguments, int maxLength)
        {
            var batch = new List<string>();
            var length = 0;

            foreach (var argument in arguments)
            {
                if (batch.Count > 0 && length + argument.Length + 1 > maxLength)
                {
                    yield return batch;
                    batch = new List<string>();
                    length = 0;
                }

                batch.Add(argument);
                length += argument.Length + 1;
            }

            if (batch.Count > 0) yield return batch;
        }
    }
}

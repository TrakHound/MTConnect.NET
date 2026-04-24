using System;
using System.IO;

namespace MTConnect.SysML
{
    // Resolves Scriban template files relative to the running binary's
    // base directory and fails fast when expected templates are missing.
    //
    // Pre-fix history: every Render*() method in this project did
    // `if (File.Exists(path)) { … } return null;` — a missing template
    // (Linux case-mismatch, missing CopyToOutputDirectory, broken
    // build) caused the renderer to silently no-op rather than tell
    // the operator the template wasn't found. Replaced with explicit
    // throws so the operator sees a clear FileNotFoundException with
    // the resolved path, the relative components used, and a hint
    // about CopyToOutputDirectory.
    internal static class TemplateLoader
    {
        // Loads a Scriban template by its path components, joined under
        // AppDomain.CurrentDomain.BaseDirectory. Throws a descriptive
        // FileNotFoundException if the resolved path doesn't exist.
        //
        // Example:
        //   var content = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Model.scriban");
        public static string LoadOrThrow(params string[] relativeComponents)
        {
            if (relativeComponents == null || relativeComponents.Length == 0)
                throw new ArgumentException("At least one path component is required.", nameof(relativeComponents));

            var components = new string[relativeComponents.Length + 1];
            components[0] = AppDomain.CurrentDomain.BaseDirectory;
            Array.Copy(relativeComponents, 0, components, 1, relativeComponents.Length);
            var resolved = Path.Combine(components);

            if (!File.Exists(resolved))
            {
                throw new FileNotFoundException(
                    $"Scriban template not found at '{resolved}'. " +
                    "Verify the template file is copied to the build output via " +
                    "<CopyToOutputDirectory>Always</CopyToOutputDirectory> in MTConnect.NET-SysML-Import.csproj, " +
                    "and that the path components are case-correct (Linux is case-sensitive).",
                    resolved);
            }

            return File.ReadAllText(resolved);
        }

        // Ensures an output directory exists or creates it. Throws if creation fails.
        public static void EnsureDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentException("Directory path must be non-empty.", nameof(directoryPath));

            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch (Exception ex)
            {
                throw new IOException(
                    $"Failed to create output directory '{directoryPath}': {ex.Message}",
                    ex);
            }
        }
    }
}

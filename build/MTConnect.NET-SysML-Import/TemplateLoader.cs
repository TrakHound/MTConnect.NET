using System;
using System.Collections.Concurrent;
using System.IO;
using Scriban;

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
    //
    // Scriban template-parse cache: Template.Parse is hot enough that
    // re-reading + re-parsing each .scriban file per Render* call shows up
    // in profiles (~2,700 redundant parses for a v2.7 regen). The cache is
    // process-wide and keyed on resolved path. The .scriban files are
    // CopyToOutputDirectory=Always so the resolved path is stable for the
    // life of the process — no invalidation needed.
    internal static class TemplateLoader
    {
        private static readonly ConcurrentDictionary<string, Template> _parseCache = new();

        // Loads a Scriban template by its path components, joined under
        // AppDomain.CurrentDomain.BaseDirectory, and returns its parsed
        // Scriban Template (cached process-wide on resolved path). Throws
        // a descriptive FileNotFoundException if the resolved path doesn't
        // exist.
        //
        // Example:
        //   var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Model.scriban");
        //   var output = template.Render(model);
        public static Template LoadOrThrow(params string[] relativeComponents)
        {
            if (relativeComponents == null || relativeComponents.Length == 0)
                throw new ArgumentException("At least one path component is required.", nameof(relativeComponents));

            var components = new string[relativeComponents.Length + 1];
            components[0] = AppDomain.CurrentDomain.BaseDirectory;
            Array.Copy(relativeComponents, 0, components, 1, relativeComponents.Length);
            var resolved = Path.Combine(components);

            return _parseCache.GetOrAdd(resolved, ParseFromDisk);
        }

        private static Template ParseFromDisk(string resolved)
        {
            if (!File.Exists(resolved))
            {
                throw new FileNotFoundException(
                    $"Scriban template not found at '{resolved}'. " +
                    "Verify the template file is copied to the build output via " +
                    "<CopyToOutputDirectory>Always</CopyToOutputDirectory> in MTConnect.NET-SysML-Import.csproj, " +
                    "and that the path components are case-correct (Linux is case-sensitive).",
                    resolved);
            }

            var contents = File.ReadAllText(resolved);
            return Template.Parse(contents, resolved);
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

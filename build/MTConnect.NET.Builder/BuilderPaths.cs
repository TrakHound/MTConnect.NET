namespace TrakHound.Builder
{
    /// <summary>
    /// Resolves the Builder's own files (config.[ID].yaml, Parts, icons, images, LICENSE.txt) independent of the Current Directory
    /// </summary>
    internal static class BuilderPaths
    {
        private const string _projectFilename = "MTConnect.NET.Builder.csproj";

        private static readonly Lazy<string> _baseDirectory = new Lazy<string>(FindBaseDirectory);


        /// <summary>
        /// The Builder project directory (found by searching up from the application directory),
        /// or the application directory when the Builder is run outside of its project
        /// </summary>
        public static string BaseDirectory => _baseDirectory.Value;


        public static string GetPath(params string[] paths) => Path.Combine(new[] { BaseDirectory }.Concat(paths).ToArray());

        /// <summary>
        /// Resolve a (possibly relative) path from a Configuration file against the Builder directory
        /// </summary>
        public static string Resolve(string path)
        {
            if (string.IsNullOrEmpty(path)) return BaseDirectory;
            return Path.GetFullPath(path, BaseDirectory);
        }


        private static string FindBaseDirectory()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, _projectFilename))) return directory.FullName;
                directory = directory.Parent;
            }

            return AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}

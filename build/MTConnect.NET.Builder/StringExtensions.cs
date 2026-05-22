namespace TrakHound.Builder
{
    /// <summary>
    /// Lenient string-to-primitive converters used by the
    /// release-builder script.
    /// </summary>
    public static class StringFunctions
    {
        /// <summary>
        /// Parses a dotted-quad string into a <see cref="Version"/>,
        /// returning <c>null</c> on failure.
        /// </summary>
        /// <param name="s">Input string, e.g. "6.9.0".</param>
        /// <returns>The parsed version, or <c>null</c>.</returns>
        public static Version ToVersion(this string s)
        {
            if (!string.IsNullOrEmpty(s) && Version.TryParse(s, out var x)) return x;
            else return null;
        }

        /// <summary>
        /// Parses a decimal integer string, returning <c>-1</c> on
        /// failure rather than throwing.
        /// </summary>
        /// <param name="s">Input string.</param>
        /// <returns>The parsed integer, or <c>-1</c>.</returns>
        public static int ToInt(this string s)
        {
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out var x)) return x;
            else return -1;
        }
    }
}

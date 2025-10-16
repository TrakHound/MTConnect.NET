namespace TrakHound.Builder
{
    public static class StringFunctions
    {
        public static Version ToVersion(this string s)
        {
            if (!string.IsNullOrEmpty(s) && Version.TryParse(s, out var x)) return x;
            else return null;
        }

        public static int ToInt(this string s)
        {
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out var x)) return x;
            else return -1;
        }
    }
}

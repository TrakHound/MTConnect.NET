using System.Text.RegularExpressions;

namespace MTConnect.NET_SysML_Import.CSharp
{
    internal static class DescriptionHelper
    {
        public static string GetXmlDescription(string description)
        {
            //if (!string.IsNullOrEmpty(description))
            //{
            //    var regex = new Regex("\\{\\{.*?\\((.*?)\\)\\}\\}");
            //    var result = description;

            //    var matches = regex.Matches(description);
            //    if (matches != null)
            //    {
            //        foreach (Match match in matches)
            //        {
            //            var original = match.Groups[0].Value;
            //            var replace = match.Groups[1].Value;

            //            //if (original.StartsWith("{{block("))
            //            //{
            //            //    // Add 'see' XML documention
            //            //    replace = $"<see cref=\"I{replace}\"></see>";
            //            //}
            //            //else
            //            //{

            //            //}

            //            result = result.Replace(original, replace);
            //        }
            //    }

            //    result = result.Replace("Types::", "");

            //    return result;
            //}

            return description;
        }

        public static string GetTextDescription(string description)
        {
            //if (!string.IsNullOrEmpty(description))
            //{
            //    var regex = new Regex("\\{\\{.*?\\((.*?)\\)\\}\\}");
            //    var result = description;

            //    var matches = regex.Matches(description);
            //    if (matches != null)
            //    {
            //        foreach (Match match in matches)
            //        {
            //            var original = match.Groups[0].Value;
            //            var replace = match.Groups[1].Value;

            //            result = result.Replace(original, replace);
            //        }
            //    }

            //    result = result.Replace("Types::", "");

            //    return result;
            //}

            return description;
        }
    }
}

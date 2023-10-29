using System.Collections.Generic;

namespace MTConnect.SysML.CSharp
{
    internal static class NamespaceHelper
    {
        public static string GetNamespace(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var nsParts = new List<string>();

                var idParts = id.Split('.');
                for (var i = 0; i < idParts.Length - 1; i++) nsParts.Add(idParts[i]);

                return $"MTConnect.{string.Join('.', nsParts)}";
            }

            return null;
        }
    }
}

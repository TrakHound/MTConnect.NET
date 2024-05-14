using MTConnect.SysML.Xmi;
using System;
using System.Linq;

namespace MTConnect.SysML
{
    public class MTConnectVersion
    {
        public static string GetVersionEnum(Version version)
        {
            if (version != null)
            {
                switch (version.Major)
                {
                    case 2:
                        switch (version.Minor)
                        {
                            case 3: return "MTConnectVersions.Version23";
                            case 2: return "MTConnectVersions.Version22";
                            case 1: return "MTConnectVersions.Version21";
                            case 0: return "MTConnectVersions.Version20";
                        }
                        break;

                    case 1:
                        switch (version.Minor)
                        {
                            case 8: return "MTConnectVersions.Version18";
                            case 7: return "MTConnectVersions.Version17";
                            case 6: return "MTConnectVersions.Version16";
                            case 5: return "MTConnectVersions.Version15";
                            case 4: return "MTConnectVersions.Version14";
                            case 3: return "MTConnectVersions.Version13";
                            case 2: return "MTConnectVersions.Version12";
                            case 1: return "MTConnectVersions.Version11";
                            case 0: return "MTConnectVersions.Version10";
                        }
                        break;
                }
            }

            return null;
        }

        public static Version LookupNormative(XmiDocument xmiDocument, string id)
        {
            if (xmiDocument != null && !string.IsNullOrEmpty(id))
            {
                var x = xmiDocument.NormativeIntroductions?.FirstOrDefault(o => o.BaseElement == id);
                if (x != null)
                {
                    if (Version.TryParse(x.Introduced, out var version))
                    {
                        return version;
                    }

                    //if (Version.TryParse(x.Version, out var version))
                    //{
                    //    return version;
                    //}
                }
            }

            return null;
        }

        public static Version LookupDeprecated(XmiDocument xmiDocument, string id)
        {
            if (xmiDocument != null && !string.IsNullOrEmpty(id))
            {
                var x = xmiDocument.Deprecations?.FirstOrDefault(o => o.BaseElement == id);
                if (x != null)
                {
                    if (Version.TryParse(x.Version, out var version))
                    {
                        return version;
                    }
                }
            }

            return null;
        }
    }
}

// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.RegularExpressions;
using System.Xml;

namespace MTConnect
{
    internal static class Namespaces
    {
        public static string Get(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            if (doc != null && doc.DocumentElement != null)
            {
                return doc.DocumentElement.NamespaceURI;
            }

            return null;
        }

        public static string Clear(string xml)
        {
            string regex = @"xmlns(:\w+)?=""(urn:mtconnect[^""]+)""|xsi(:\w+)?=""(urn:mtconnect[^""]+)""";
            return Regex.Replace(xml, regex, "");
        }

        public static class v13
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.3";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.3";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.3";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.3";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }

        public static class v12
        {
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.2";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.2";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.2";

            public static bool Match(string ns)
            {
                return ns == Devices || ns == Error || ns == Streams;
            }
        }

        public static class v11
        {
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.1";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.1";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.1";

            public static bool Match(string ns)
            {
                return ns == Devices || ns == Error || ns == Streams;
            }
        }
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace MTConnect
{
    internal static class Namespaces
    {      
        internal const string DefaultXmlSchemaInstance = "http://www.w3.org/2001/XMLSchema-instance";
        
        private static Regex _streamRemoveNamespaceRegex = new Regex(@"\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""", RegexOptions.Compiled);
        private static Regex _streamNamespaceRegex = new Regex("<MTConnectStreams", RegexOptions.Compiled);


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

        public static string Get(string rootNode, string prefix, byte[] bytes)
        {
            if (bytes != null && bytes.Length > 0)
            {
                try
                {
                    using (var memoryReader = new MemoryStream(bytes))
                    {
                        using (var xmlReader = XmlReader.Create(memoryReader))
                        {
                            xmlReader.ReadStartElement(rootNode);
                            return xmlReader.LookupNamespace(prefix);
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static string GetDevices(int majorVerion, int minorVersion)
        {
            switch (majorVerion)
            {
                case 1:

                    switch (minorVersion)
                    {
                        case 0: return Version10.Devices;                  
                        case 1: return Version11.Devices;                  
                        case 2: return Version12.Devices;                  
                        case 3: return Version13.Devices;                  
                        case 4: return Version14.Devices;                  
                        case 5: return Version15.Devices;                  
                        case 6: return Version16.Devices;                  
                        case 7: return Version17.Devices;                  
                        case 8: return Version18.Devices;                                  
                    }

                    break;

                case 2:

                    switch (minorVersion)
                    {
                        case 0: return Version20.Devices;
                        case 1: return Version21.Devices;
                    }

                    break;
            }

            return null;
        }


        public static string GetStreams(int majorVerion, int minorVersion)
        {
            switch (majorVerion)
            {
                case 1:

                    switch (minorVersion)
                    {
                        case 0: return Version10.Streams;
                        case 1: return Version11.Streams;
                        case 2: return Version12.Streams;
                        case 3: return Version13.Streams;
                        case 4: return Version14.Streams;
                        case 5: return Version15.Streams;
                        case 6: return Version16.Streams;
                        case 7: return Version17.Streams;
                        case 8: return Version18.Streams;
                    }

                    break;

                case 2:

                    switch (minorVersion)
                    {
                        case 0: return Version20.Streams;
                        case 1: return Version21.Streams;
                    }

                    break;
            }

            return null;
        }

        public static string SetStreams(string xml, Version version, IEnumerable<NamespaceConfiguration> extendedSchemas = null)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                var ns = GetStreams(version.Major, version.Minor);

                // Remove the XSD namespace
                xml = _streamRemoveNamespaceRegex.Replace(xml, "");

                var replace = new StringBuilder();
                var extendedNamespaces = new StringBuilder();

                var schemaLocation = new StringBuilder();
                schemaLocation.Append(Schemas.GetStreams(version.Major, version.Minor));

                // Add Extended Schemas
                if (!extendedSchemas.IsNullOrEmpty())
                {
                    schemaLocation.Clear();

                    foreach (var extendedSchema in extendedSchemas)
                    {
                        extendedNamespaces.Append(" xmlns:");
                        extendedNamespaces.Append(extendedSchema.Alias);
                        extendedNamespaces.Append("=\"");
                        extendedNamespaces.Append(extendedSchema.Location);
                        extendedNamespaces.Append('\"');

                        schemaLocation.Append(extendedSchema.Location);
                        schemaLocation.Append(' ');
                        schemaLocation.Append(extendedSchema.Path);
                    }
                }

                // Add Namespaces
                replace.Append("<MTConnectStreams xmlns:m=\"");
                replace.Append(ns);
                replace.Append("\" xmlns=\"");
                replace.Append(ns);
                replace.Append("\" xmlns:xsi=\"");
                replace.Append(DefaultXmlSchemaInstance);
                replace.Append('\"');
                if (extendedNamespaces.Length > 0) replace.Append(extendedNamespaces);
                replace.Append(" xsi:schemaLocation=\"");
                replace.Append(schemaLocation);
                replace.Append('\"');

                xml = _streamNamespaceRegex.Replace(xml, replace.ToString());
            }

            return xml;
        }

        public static string GetAssets(int majorVerion, int minorVersion)
        {
            switch (majorVerion)
            {
                case 1:

                    switch (minorVersion)
                    {
                        case 2: return Version12.Assets;
                        case 3: return Version13.Assets;
                        case 4: return Version14.Assets;
                        case 5: return Version15.Assets;
                        case 6: return Version16.Assets;
                        case 7: return Version17.Assets;
                        case 8: return Version18.Assets;
                    }

                    break;

                case 2:

                    switch (minorVersion)
                    {
                        case 0: return Version20.Assets;
                        case 1: return Version21.Assets;
                    }

                    break;
            }

            return null;
        }

        public static string GetError(int majorVerion, int minorVersion)
        {
            switch (majorVerion)
            {
                case 1:

                    switch (minorVersion)
                    {
                        case 0: return Version10.Error;
                        case 1: return Version11.Error;
                        case 2: return Version12.Error;
                        case 3: return Version13.Error;
                        case 4: return Version14.Error;
                        case 5: return Version15.Error;
                        case 6: return Version16.Error;
                        case 7: return Version17.Error;
                        case 8: return Version18.Error;
                    }

                    break;

                case 2:

                    switch (minorVersion)
                    {
                        case 0: return Version20.Error;
                        case 1: return Version21.Error;
                    }

                    break;
            }

            return null;
        }


        public static string Clear(string xml)
        {
            string regex = @"xmlns(:\w+)?=""(urn:mtconnect[^""]+)""|xsi(:\w+)?=""(urn:mtconnect[^""]+)""";
            return Regex.Replace(xml, regex, "");
        }


        internal static class Version21
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:2.1";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:2.1";
            public const string Error = "urn:mtconnect.org:MTConnectError:2.1";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:2.1";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }


        internal static class Version20
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:2.0";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:2.0";
            public const string Error = "urn:mtconnect.org:MTConnectError:2.0";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:2.0";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }


        internal static class Version18
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.8";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.8";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.8";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.8";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }


        internal static class Version17
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.7";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.7";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.7";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.7";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }

        internal static class Version16
        { 
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.6";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.6";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.6";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.6";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }

        internal static class Version15
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.5";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.5";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.5";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.5";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }

        internal static class Version14
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.4";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.4";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.4";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.4";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }

        internal static class Version13
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

        internal static class Version12
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.2";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.2";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.2";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.2";

            public static bool Match(string ns)
            {
                return ns == Devices || ns == Error || ns == Streams;
            }
        }

        internal static class Version11
        {
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.1";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.1";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.1";

            public static bool Match(string ns)
            {
                return ns == Devices || ns == Error || ns == Streams;
            }
        }

        internal static class Version10
        {
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.0";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.0";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.0";

            public static bool Match(string ns)
            {
                return ns == Devices || ns == Error || ns == Streams;
            }
        }
    }
}
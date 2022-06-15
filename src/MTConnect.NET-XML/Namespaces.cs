// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.RegularExpressions;
using System.Xml;

namespace MTConnect
{
    internal static class Namespaces
    {
        internal const string DefaultXmlSchemaInstance = "http://www.w3.org/2001/XMLSchema-instance";


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
                    }

                    break;
            }

            return null;
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


        internal static class Version20
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:2.0.0";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:2.0.0";
            public const string Error = "urn:mtconnect.org:MTConnectError:2.0.0";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:2.0.0";

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

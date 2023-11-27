// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect
{
    internal static class Schemas
    {
        public static string GetDevices(int majorVerion, int minorVersion)
        {
            switch (majorVerion)
            {
                case 1:

                    switch (minorVersion)
                    {
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


        static class Version21
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:2.1 /schemas/MTConnectAssets_2.1.xsd";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:2.1 /schemas/MTConnectDevices_2.1.xsd";
            public const string Error = "urn:mtconnect.org:MTConnectError:2.1 /schemas/MTConnectError_2.1.xsd";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:2.1 /schemas/MTConnectStreams_2.1.xsd";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }


        static class Version20
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:2.0 /schemas/MTConnectAssets_2.0.xsd";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:2.0 /schemas/MTConnectDevices_2.0.xsd";
            public const string Error = "urn:mtconnect.org:MTConnectError:2.0 /schemas/MTConnectError_2.0.xsd";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:2.0 /schemas/MTConnectStreams_2.0.xsd";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }


        static class Version18
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.8 /schemas/MTConnectAssets_1.8.xsd";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.8 /schemas/MTConnectDevices_1.8.xsd";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.8 /schemas/MTConnectError_1.8.xsd";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.8 /schemas/MTConnectStreams_1.8.xsd";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }


        static class Version17
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.7 /schemas/MTConnectAssets_1.7.xsd";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.7 /schemas/MTConnectDevices_1.7.xsd";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.7 /schemas/MTConnectError_1.7.xsd";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.7 /schemas/MTConnectStreams_1.7.xsd";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }

        static class Version16
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.6 /schemas/MTConnectAssets_1.6.xsd";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.6 /schemas/MTConnectDevices_1.6.xsd";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.6 /schemas/MTConnectError_1.6.xsd";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.6 /schemas/MTConnectStreams_1.6.xsd";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }

        static class Version15
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.5 /schemas/MTConnectAssets_1.5.xsd";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.5 /schemas/MTConnectDevices_1.5.xsd";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.5 /schemas/MTConnectError_1.5.xsd";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.5 /schemas/MTConnectStreams_1.5.xsd";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }

        static class Version14
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.4 /schemas/MTConnectAssets_1.4.xsd";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.4 /schemas/MTConnectDevices_1.4.xsd";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.4 /schemas/MTConnectError_1.4.xsd";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.4 /schemas/MTConnectStreams_1.4.xsd";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }

        static class Version13
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.3 /schemas/MTConnectAssets_1.3.xsd";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.3 /schemas/MTConnectDevices_1.3.xsd";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.3 /schemas/MTConnectError_1.3.xsd";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.3 /schemas/MTConnectStreams_1.3.xsd";

            public static bool Match(string ns)
            {
                return ns == Assets || ns == Devices || ns == Error || ns == Streams;
            }
        }

        static class Version12
        {
            public const string Assets = "urn:mtconnect.org:MTConnectAssets:1.2 /schemas/MTConnectAssets_1.2.xsd";
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.2 /schemas/MTConnectDevices_1.2.xsd";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.2 /schemas/MTConnectError_1.2.xsd";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.2 /schemas/MTConnectStreams_1.2.xsd";

            public static bool Match(string ns)
            {
                return ns == Devices || ns == Error || ns == Streams;
            }
        }

        static class Version11
        {
            public const string Devices = "urn:mtconnect.org:MTConnectDevices:1.1 /schemas/MTConnectDevices_1.1.xsd";
            public const string Error = "urn:mtconnect.org:MTConnectError:1.1 /schemas/MTConnectError_1.1.xsd";
            public const string Streams = "urn:mtconnect.org:MTConnectStreams:1.1 /schemas/MTConnectStreams_1.1.xsd";

            public static bool Match(string ns)
            {
                return ns == Devices || ns == Error || ns == Streams;
            }
        }
    }
}
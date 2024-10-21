// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect
{
    internal static class MTConnectVersion
    {
        /// <summary>
        /// Gets the Version of the MTConnect standard being used based on the XML Namespace that is used
        /// </summary>
        public static Version Get(string xml)
        {
            var ns = Namespaces.Get(xml);
            return GetByNamespace(ns);
        }

        /// <summary>
        /// Gets the Version of the MTConnect standard being used based on the XML Namespace that is used
        /// </summary>
        public static Version GetByNamespace(string ns)
        {
            if (ns != null)
            {
                if (Namespaces.Version23.Match(ns)) return MTConnectVersions.Version24;
                if (Namespaces.Version23.Match(ns)) return MTConnectVersions.Version23;
                if (Namespaces.Version22.Match(ns)) return MTConnectVersions.Version22;
                if (Namespaces.Version21.Match(ns)) return MTConnectVersions.Version21;
                if (Namespaces.Version20.Match(ns)) return MTConnectVersions.Version20;
                if (Namespaces.Version18.Match(ns)) return MTConnectVersions.Version18;
                if (Namespaces.Version17.Match(ns)) return MTConnectVersions.Version17;
                if (Namespaces.Version16.Match(ns)) return MTConnectVersions.Version16;
                if (Namespaces.Version15.Match(ns)) return MTConnectVersions.Version15;
                if (Namespaces.Version14.Match(ns)) return MTConnectVersions.Version14;
                if (Namespaces.Version13.Match(ns)) return MTConnectVersions.Version13;
                if (Namespaces.Version12.Match(ns)) return MTConnectVersions.Version12;
                if (Namespaces.Version11.Match(ns)) return MTConnectVersions.Version11;
                if (Namespaces.Version10.Match(ns)) return MTConnectVersions.Version10;
            }

            return new Version();
        }
    }
}
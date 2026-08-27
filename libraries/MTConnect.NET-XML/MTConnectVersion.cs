// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect
{
    internal static class MTConnectVersion
    {
        /// <summary>
        /// Gets the Version of the MTConnect standard from a raw XML document by extracting the root element's namespace URI and dispatching through <see cref="GetByNamespace"/>.
        /// </summary>
        public static Version Get(string xml)
        {
            var ns = Namespaces.Get(xml);
            return GetByNamespace(ns);
        }

        /// <summary>
        /// Gets the Version of the MTConnect standard from an already-resolved namespace URI. Returns <see cref="MTConnectVersions.Max"/> when the namespace is null, empty, or does not match a declared MTConnect namespace.
        /// </summary>
        public static Version GetByNamespace(string ns)
        {
            if (ns != null)
            {
                if (Namespaces.Version27.Match(ns)) return MTConnectVersions.Version27;
                if (Namespaces.Version26.Match(ns)) return MTConnectVersions.Version26;
                if (Namespaces.Version25.Match(ns)) return MTConnectVersions.Version25;
                if (Namespaces.Version24.Match(ns)) return MTConnectVersions.Version24;
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

            // unknown namespace → default to latest supported version
            return MTConnectVersions.Max;
        }
    }
}

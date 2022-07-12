// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;

namespace MTConnect
{
    public static class MTConnectVersion
    {
        /// <summary>
        /// Gets the Version of the MTConnect standard being used based on the XML Namespace that is used
        /// </summary>
        public static Version Get(string xml)
        {
            var ns = Namespaces.Get(xml);
            if (ns != null)
            {
                if (Namespaces.Version20.Match(ns)) return new Version(2, 0);
                if (Namespaces.Version18.Match(ns)) return new Version(1, 8);
                if (Namespaces.Version17.Match(ns)) return new Version(1, 7);
                if (Namespaces.Version16.Match(ns)) return new Version(1, 6);
                if (Namespaces.Version15.Match(ns)) return new Version(1, 5);
                if (Namespaces.Version14.Match(ns)) return new Version(1, 4);
                if (Namespaces.Version13.Match(ns)) return new Version(1, 3);
                if (Namespaces.Version12.Match(ns)) return new Version(1, 2);
                if (Namespaces.Version11.Match(ns)) return new Version(1, 1);
                if (Namespaces.Version10.Match(ns)) return new Version(1, 0);
            }

            return new Version();
        }
    }
}

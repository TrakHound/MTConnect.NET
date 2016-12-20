// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect
{
    public class Version
    {
        public static double Get(string xml)
        {
            var ns = Namespaces.Get(xml);
            if (ns != null)
            {
                if (Namespaces.v13.Match(ns)) return 1.3;
                if (Namespaces.v12.Match(ns)) return 1.2;
                if (Namespaces.v11.Match(ns)) return 1.1;
            }

            return -1;
        }
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect
{
    public static class MTConnectVersions
    {
        private static IEnumerable<Version> _versions;

        public static Version Max => Version21;

        public static readonly Version Version10 = new Version(1, 0);
        public static readonly Version Version11 = new Version(1, 1);
        public static readonly Version Version12 = new Version(1, 2);
        public static readonly Version Version13 = new Version(1, 3);
        public static readonly Version Version14 = new Version(1, 4);
        public static readonly Version Version15 = new Version(1, 5);
        public static readonly Version Version16 = new Version(1, 6);
        public static readonly Version Version17 = new Version(1, 7);
        public static readonly Version Version18 = new Version(1, 8);
        public static readonly Version Version20 = new Version(2, 0);
        public static readonly Version Version21 = new Version(2, 1);


        public static IEnumerable<Version> Get()
        {
            if (_versions == null)
            {
                var x = new List<Version>();
                x.Add(Version10);
                x.Add(Version11);
                x.Add(Version12);
                x.Add(Version13);
                x.Add(Version14);
                x.Add(Version15);
                x.Add(Version16);
                x.Add(Version17);
                x.Add(Version18);

                x.Add(Version20);
                x.Add(Version21);

                _versions = x;
            }

            return _versions;
        }
    }
}
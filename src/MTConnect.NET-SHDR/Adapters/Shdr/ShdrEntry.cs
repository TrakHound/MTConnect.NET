// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Adapters.Shdr
{
    internal struct ShdrEntry
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public bool Removed { get; set; }

        public ShdrEntry(string key, string value, bool removed = false)
        {
            Key = key;
            Value = value;
            Removed = removed;
        }
    }
}

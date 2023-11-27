// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    public class SubTaskRef
    {
        public bool Parallel { get; set; }

        public string Group { get; set; }

        public int Order { get; set; }

        public bool Optional { get; set; }
    }
}

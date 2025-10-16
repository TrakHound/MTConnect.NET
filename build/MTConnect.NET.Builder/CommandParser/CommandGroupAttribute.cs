// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Builder
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class CommandGroupAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }


        public CommandGroupAttribute(string name, string description = null)
        {
            Name = name;
            Description = description;
        }
    }
}

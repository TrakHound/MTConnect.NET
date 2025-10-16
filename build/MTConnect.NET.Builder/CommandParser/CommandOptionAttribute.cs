// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Builder
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal class CommandOptionAttribute : Attribute
    {
        public string Description { get; set; }


        public CommandOptionAttribute() { }

        public CommandOptionAttribute(string description)
        {
            Description = description;
        }
    }
}

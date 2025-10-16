// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Builder
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal class CommandParameterAttribute : Attribute
    {
        public string Description { get; set; }


        public CommandParameterAttribute() { }

        public CommandParameterAttribute(string description)
        {
            Description = description;
        }
    }
}

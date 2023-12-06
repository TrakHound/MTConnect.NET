// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public class ModuleConfiguration : ShdrAdapterClientConfiguration
    {
        /// <summary>
        /// Gets or Sets whether a Device Model can be sent from an SHDR Adapter
        /// </summary>
        public bool AllowShdrDevice { get; set; }


        public ModuleConfiguration()
        {
            AllowShdrDevice = false;
        }
    }
}
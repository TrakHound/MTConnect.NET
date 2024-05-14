// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public class ShdrAdapterModuleConfiguration : ShdrAdapterClientConfiguration
    {
        /// <summary>
        /// Gets or Sets whether a Device Model can be sent from an SHDR Adapter
        /// </summary>
        public bool AllowShdrDevice { get; set; }


        public ShdrAdapterModuleConfiguration()
        {
            AllowShdrDevice = false;
        }
    }
}
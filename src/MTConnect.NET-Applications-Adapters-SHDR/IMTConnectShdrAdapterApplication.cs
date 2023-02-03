// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Adapters.Shdr;
using MTConnect.Configurations;
using System;

namespace MTConnect.Applications.Adapters
{
    /// <summary>
    /// An interface for an MTConnect SHDR Adapter Application
    /// </summary>
    public interface IMTConnectShdrAdapterApplication
    {
        string ServiceName { get; }

        string ServiceDisplayName { get; }

        string ServiceDescription { get; }


        ShdrAdapter Adapter { get; }

        EventHandler<ShdrAdapterApplicationConfiguration> OnRestart { get; set; }



        /// <summary>
        /// Start the Adapter Application
        /// </summary>
        void StartAdapter(string configurationPath, bool verboseLogging = false);

        /// <summary>
        /// Stop the Adapter Application
        /// </summary>
        void StopAdapter();
    }
}
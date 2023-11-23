// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Input;
using System;

namespace MTConnect.Applications
{
    /// <summary>
    /// An interface for an MTConnect Adapter Application
    /// </summary>
    public interface IMTConnectAdapterApplication
    {
        string ServiceName { get; }

        string ServiceDisplayName { get; }

        string ServiceDescription { get; }

        IMTConnectDataSource DataSource { get; }


        event EventHandler<AdapterApplicationConfiguration> OnRestart;


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
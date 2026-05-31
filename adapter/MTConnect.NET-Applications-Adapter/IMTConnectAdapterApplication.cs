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
        /// <summary>
        /// Windows-service identifier the host registers under (e.g.
        /// <c>MTConnect-Adapter</c>). Ignored on non-Windows platforms.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Human-readable Windows-service display name.
        /// </summary>
        string ServiceDisplayName { get; }

        /// <summary>
        /// Long-form description recorded against the Windows service.
        /// </summary>
        string ServiceDescription { get; }

        /// <summary>
        /// Active data-source instance (the data-pull worker that
        /// reads from the underlying device or simulator and writes
        /// into the adapter).
        /// </summary>
        IMTConnectDataSource DataSource { get; }


        /// <summary>
        /// Raised when the configuration-file watcher detects a change
        /// and the adapter is restarting with the freshly-loaded
        /// <see cref="AdapterApplicationConfiguration"/>.
        /// </summary>
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
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for Static Files that can served from an Http Server
    /// </summary>
    public interface IFileConfiguration
    {
        /// <summary>
        /// The location of the files on the server (where the Agent is running)
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// The path to match in the requested URL
        /// </summary>
        string Location { get; set; }
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an XSLT/CSS stylesheet referenced by MTConnect
    /// response documents so a browser can render the XML response.
    /// </summary>
    public interface IStyleConfiguration
    {
        /// <summary>
        /// The URL location of the stylesheet emitted in the document's
        /// stylesheet processing instruction.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// The local file system path to the stylesheet when it is served by the agent.
        /// </summary>
        string Path { get; }
    }
}
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public interface INamespaceConfiguration
    {
        /// <summary>
        /// The alias that will be used to reference the extended schema
        /// </summary>
        string Alias { get; }

        string Urn { get; }

        /// <summary>
        /// The location of the xsd file relative in the agent namespace
        /// </summary>
        string Location { get; }

        string Path { get; }
    }
}
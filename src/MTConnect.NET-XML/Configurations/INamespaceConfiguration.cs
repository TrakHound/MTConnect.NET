// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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

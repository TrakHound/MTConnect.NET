// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an extended XML schema namespace declared on
    /// MTConnect response documents, allowing custom or vendor schema
    /// extensions to be referenced and validated.
    /// </summary>
    public interface INamespaceConfiguration
    {
        /// <summary>
        /// The alias that will be used to reference the extended schema
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// The URN that uniquely identifies the extended schema namespace.
        /// </summary>
        string Urn { get; }

        /// <summary>
        /// The location of the xsd file relative in the agent namespace
        /// </summary>
        string Location { get; }

        /// <summary>
        /// The local file system path to the xsd file when it is served by the agent.
        /// </summary>
        string Path { get; }
    }
}
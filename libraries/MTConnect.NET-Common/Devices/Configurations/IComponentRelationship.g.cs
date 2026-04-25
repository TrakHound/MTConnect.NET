// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// ConfigurationRelationship that describes the association between a Component or an Asset and another Component.
    /// </summary>
    public interface IComponentRelationship : IConfigurationRelationship
    {
        /// <summary>
        /// Reference to the associated Component.
        /// </summary>
        string IdRef { get; }
    }
}
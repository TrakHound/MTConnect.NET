// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Configurationrelationship that describes the association between two components within a piece of equipment that function independently but together perform a capability or service within a piece of equipment.
    /// </summary>
    public interface IComponentRelationship : IRelationship
    {
        /// <summary>
        /// Reference to the associated Component.
        /// </summary>
        string IdRef { get; }
    }
}
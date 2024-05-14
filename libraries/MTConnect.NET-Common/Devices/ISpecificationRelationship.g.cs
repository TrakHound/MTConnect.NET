// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// IdRef.
    /// </summary>
    public interface ISpecificationRelationship : IAbstractDataItemRelationship
    {
        /// <summary>
        /// Specifies how the Specification is related.
        /// </summary>
        MTConnect.Devices.SpecificationRelationshipType Type { get; }
    }
}
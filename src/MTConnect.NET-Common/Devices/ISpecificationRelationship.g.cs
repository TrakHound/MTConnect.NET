// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Abstractdataitemrelationship that provides a semantic reference to another Specification described by the type and idRef property.
    /// </summary>
    public interface ISpecificationRelationship : IAbstractDataItemRelationship
    {
        /// <summary>
        /// Specifies how the Specification is related.
        /// </summary>
        MTConnect.Devices.SpecificationRelationshipType Type { get; }
    }
}
// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1658942187874_859516_1061

namespace MTConnect.Devices
{
    /// <summary>
    /// Association between a DataItem and another entity.
    /// </summary>
    public abstract partial class AbstractDataItemRelationship : IAbstractDataItemRelationship
    {
        public const string DescriptionText = "Association between a DataItem and another entity.";


        /// <summary>
        /// Reference to the related entity's id.
        /// </summary>
        public string IdRef { get; set; }
        
        /// <summary>
        /// Descriptive name associated with this AbstractDataItemRelationship.
        /// </summary>
        public string Name { get; set; }
    }
}
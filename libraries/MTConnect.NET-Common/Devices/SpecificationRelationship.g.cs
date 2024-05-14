// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605645496116_206752_2923

namespace MTConnect.Devices
{
    /// <summary>
    /// IdRef.
    /// </summary>
    public class SpecificationRelationship : AbstractDataItemRelationship, ISpecificationRelationship
    {
        public new const string DescriptionText = "IdRef.";


        /// <summary>
        /// Specifies how the Specification is related.
        /// </summary>
        public MTConnect.Devices.SpecificationRelationshipType Type { get; set; }
    }
}
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605645496116_206752_2923

namespace MTConnect.Devices
{
    /// <summary>
    /// Abstractdataitemrelationship that provides a semantic reference to another Specification described by the type and idRef property.
    /// </summary>
    public class SpecificationRelationship : AbstractDataItemRelationship, ISpecificationRelationship
    {
        public new const string DescriptionText = "Abstractdataitemrelationship that provides a semantic reference to another Specification described by the type and idRef property.";


        /// <summary>
        /// Specifies how the Specification is related.
        /// </summary>
        public MTConnect.Devices.SpecificationRelationshipType Type { get; set; }
    }
}
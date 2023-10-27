// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605645474430_802116_2875

namespace MTConnect.Devices
{
    /// <summary>
    /// Abstractdataitemrelationship that provides a semantic reference to another DataItem described by the type property.
    /// </summary>
    public class DataItemRelationship : AbstractDataItemRelationship, IDataItemRelationship
    {
        public new const string DescriptionText = "Abstractdataitemrelationship that provides a semantic reference to another DataItem described by the type property.";


        /// <summary>
        /// Specifies how the DataItem is related.
        /// </summary>
        public MTConnect.Devices.DataItemRelationshipType Type { get; set; }
    }
}
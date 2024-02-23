// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605645474430_802116_2875

namespace MTConnect.Devices
{
    /// <summary>
    /// Type.
    /// </summary>
    public class DataItemRelationship : AbstractDataItemRelationship, IDataItemRelationship
    {
        public new const string DescriptionText = "Type.";


        /// <summary>
        /// Specifies how the DataItem is related.
        /// </summary>
        public MTConnect.Devices.DataItemRelationshipType Type { get; set; }
    }
}
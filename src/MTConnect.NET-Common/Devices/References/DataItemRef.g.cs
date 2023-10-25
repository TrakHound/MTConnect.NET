// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1581212127193_199949_213

namespace MTConnect.Devices.References
{
    /// <summary>
    /// Reference that is a pointer to a DataItem associated with another entity defined for a piece of equipment.
    /// </summary>
    public class DataItemRef : Reference, IDataItemRef
    {
        public new const string DescriptionText = "Reference that is a pointer to a DataItem associated with another entity defined for a piece of equipment.";


        /// <summary>
        /// Pointer to the id attribute of the DataItem that contains the information to be associated with this element.
        /// </summary>
        public MTConnect.Devices.IDataItem IdRef { get; set; }
    }
}
// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1760958090309_25486_6198

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Angular range over which the object is designed to move about a fixed axis or pivot
    /// </summary>
    public class SwingAngleDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SWING_ANGLE";
        public const string NameId = "swingAngle";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
        public const string DefaultUnits = Devices.Units.MILLIMETER;     
        public new const string DescriptionText = "Angular range over which the object is designed to move about a fixed axis or pivot";
        
        public override string TypeDescription => DescriptionText;
        
               


        public SwingAngleDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            Units = DefaultUnits;
        }

        public SwingAngleDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation; 
            Units = DefaultUnits;
        }
    }
}
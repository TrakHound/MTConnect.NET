// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1760958089430_389097_6190

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Maximal linear distance from the pivot or axis to the furthest point reached by the object’s swing
    /// </summary>
    public class SwingRadiusDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SWING_RADIUS";
        public const string NameId = "swingRadius";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
        public const string DefaultUnits = Devices.Units.MILLIMETER;     
        public new const string DescriptionText = "Maximal linear distance from the pivot or axis to the furthest point reached by the object’s swing";
        
        public override string TypeDescription => DescriptionText;
        
               


        public SwingRadiusDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            Units = DefaultUnits;
        }

        public SwingRadiusDataItem(string deviceId)
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
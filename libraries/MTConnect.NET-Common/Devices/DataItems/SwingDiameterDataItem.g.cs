// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1760957933398_661917_6176

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Maximal linear width (diameter) of the area described by the object’s movement about an axis
    /// </summary>
    public class SwingDiameterDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SWING_DIAMETER";
        public const string NameId = "swingDiameter";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
        public const string DefaultUnits = Devices.Units.MILLIMETER;     
        public new const string DescriptionText = "Maximal linear width (diameter) of the area described by the object’s movement about an axis";
        
        public override string TypeDescription => DescriptionText;
        
               


        public SwingDiameterDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            Units = DefaultUnits;
        }

        public SwingDiameterDataItem(string deviceId)
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
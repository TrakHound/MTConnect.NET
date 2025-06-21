// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727727419677_160573_24195

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Inability of a material to conduct electricity.
    /// </summary>
    public class ResistivityDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "RESISTIVITY";
        public const string NameId = "resistivity";
             
        public const string DefaultUnits = Devices.Units.OHM_METER;     
        public new const string DescriptionText = "Inability of a material to conduct electricity.";
        
        public override string TypeDescription => DescriptionText;
        
               


        public ResistivityDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public ResistivityDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}
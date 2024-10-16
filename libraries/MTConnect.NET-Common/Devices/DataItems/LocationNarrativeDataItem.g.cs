// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1712320901547_688092_333

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Textual description of the location of an object or activity.
    /// </summary>
    public class LocationNarrativeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "LOCATION_NARRATIVE";
        public const string NameId = "locationNarrative";
             
             
        public new const string DescriptionText = "Textual description of the location of an object or activity.";
        
        public override string TypeDescription => DescriptionText;
        
               


        public LocationNarrativeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public LocationNarrativeDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
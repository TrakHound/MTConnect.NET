// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier for the type of wire used as the cutting mechanism in Electrical Discharge Machining or similar processes.
    /// </summary>
    public class WireDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "WIRE";
        public const string NameId = "wire";
             
        public new const string DescriptionText = "Identifier for the type of wire used as the cutting mechanism in Electrical Discharge Machining or similar processes.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public WireDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public WireDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
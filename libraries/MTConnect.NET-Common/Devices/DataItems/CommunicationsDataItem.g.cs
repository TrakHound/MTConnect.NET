// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication that the piece of equipment has experienced a communications failure.
    /// </summary>
    public class CommunicationsDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "COMMUNICATIONS";
        public const string NameId = "communications";
             
        public new const string DescriptionText = "Indication that the piece of equipment has experienced a communications failure.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public CommunicationsDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public CommunicationsDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
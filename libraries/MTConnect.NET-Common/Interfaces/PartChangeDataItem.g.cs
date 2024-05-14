// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Operating state of the service to change the part or product associated with a piece of equipment to a different part or product.
    /// </summary>
    public class PartChangeDataItem : InterfaceDataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PART_CHANGE";
        public const string NameId = "partChange";
             
        public new const string DescriptionText = "Operating state of the service to change the part or product associated with a piece of equipment to a different part or product.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public PartChangeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public PartChangeDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
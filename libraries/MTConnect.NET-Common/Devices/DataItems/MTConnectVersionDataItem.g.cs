// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Reference version of the MTConnect Standard supported by the adapter.
    /// </summary>
    public class MTConnectVersionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MTCONNECT_VERSION";
        public const string NameId = "mtconnectVersion";
             
             
        public new const string DescriptionText = "Reference version of the MTConnect Standard supported by the adapter.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public MTConnectVersionDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public MTConnectVersionDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
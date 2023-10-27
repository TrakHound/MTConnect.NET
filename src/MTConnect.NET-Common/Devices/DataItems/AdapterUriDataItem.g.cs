// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Uri of the adapter.
    /// </summary>
    public class AdapterUriDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ADAPTER_URI";
        public const string NameId = "";
             
        public new const string DescriptionText = "Uri of the adapter.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public AdapterUriDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public AdapterUriDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
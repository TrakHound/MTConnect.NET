// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// Name of the host computer supplying data.
    /// </summary>
    public class HostNameDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "HOST_NAME";
        public const string NameId = "hostName";
        public new const string DescriptionText = "Name of the host computer supplying data.";
        
        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version21;


        public HostNameDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public HostNameDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
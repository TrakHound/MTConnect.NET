// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The reference version of the MTConnect Standard supported by the Adapter.
    /// </summary>
    public class MTConnectVersionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MTCONNECT_VERSION";
        public const string NameId = "mtconnectVersion";
        public new const string DescriptionText = "The reference version of the MTConnect Standard supported by the Adapter.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public MTConnectVersionDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public MTConnectVersionDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
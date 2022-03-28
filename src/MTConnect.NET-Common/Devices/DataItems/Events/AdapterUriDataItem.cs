// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The URI of the Adapter.
    /// </summary>
    public class AdapterUriDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ADAPTER_URI";
        public const string NameId = "adapterUri";
        public new const string DescriptionText = "The URI of the Adapter.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public AdapterUriDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }

        public AdapterUriDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

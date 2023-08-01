// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Devices.DataItems.Events
{
	/// <summary>
	/// UUID of a feature. ISO 10303 AP 242/239.
	/// </summary>
	public class FeaturePersistentIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "FEATURE_PERSISTENT_ID";
        public const string NameId = "featurePersistentId";
        public new const string DescriptionText = "UUID of a feature. ISO 10303 AP 242/239.";

        public override string TypeDescription => DescriptionText;

        public override Version MinimumVersion => MTConnectVersions.Version22;


        public FeaturePersistentIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public FeaturePersistentIdDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
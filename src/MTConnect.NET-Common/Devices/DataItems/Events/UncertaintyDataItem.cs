// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
	/// <summary>
	/// Uncertainty specified by UncertaintyType.
	/// </summary>
	public class UncertaintyDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "UNCERTAINTY";
        public const string NameId = "uncertainty";
        public new const string DescriptionText = "Uncertainty specified by UncertaintyType.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version22;


        public UncertaintyDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
		}

        public UncertaintyDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
		}
    }
}
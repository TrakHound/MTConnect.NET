// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
	/// <summary>
	/// Tabular representation of assessing elements of a feature.
	/// </summary>
	public class FeatureMeasurementDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "FEATURE_MEASUREMENT";
        public const string NameId = "featureMeasurement";
        public new const string DescriptionText = "Tabular representation of assessing elements of a feature.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version22;


        public FeatureMeasurementDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public FeatureMeasurementDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
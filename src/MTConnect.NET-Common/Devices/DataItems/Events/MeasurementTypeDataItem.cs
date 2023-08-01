// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
	/// <summary>
	/// Class of measurement being performed. QIF 3:2018 Section 6.3 Examples: POINT, RADIUS, ANGLE, LENGTH, etc.
	/// </summary>
	public class MeasurementTypeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MEASUREMENT_TYPE";
        public const string NameId = "measurementType";
        public new const string DescriptionText = "Class of measurement being performed. QIF 3:2018 Section 6.3 Examples: POINT, RADIUS, ANGLE, LENGTH, etc.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version22;


        public MeasurementTypeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public MeasurementTypeDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
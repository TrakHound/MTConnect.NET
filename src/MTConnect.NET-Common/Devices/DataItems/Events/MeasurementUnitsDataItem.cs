// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
	/// <summary>
	/// Engineering units of the measurement.
	/// </summary>
	public class MeasurementUnitsDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MEASUREMENT_UNITS";
        public const string NameId = "measurementUnits";
        public new const string DescriptionText = "Engineering units of the measurement";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version22;


        public MeasurementUnitsDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public MeasurementUnitsDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
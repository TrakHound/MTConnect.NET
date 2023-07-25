// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
	/// <summary>
	/// Detection result of a sensor.
	/// </summary>
	public class SensorStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SENSOR_STATE";
        public const string NameId = "sensorState";
        public new const string DescriptionText = "Detection result of a sensor.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version22;


		public enum SubTypes
		{
			/// <summary>
			/// Detection result of a sensor where the state is Binary.
			/// </summary>
			BINARY,

			/// <summary>
			/// Detection result of a sensor where the state is Boolean.
			/// </summary>
			BOOLEAN,

			/// <summary>
			/// Detection result of a sensor where the state is Detect.
			/// </summary>
			DETECT,

			/// <summary>
			/// Detection result of a sensor where the state is Enumerated.
			/// </summary>
			ENUMERATED
		}


		public SensorStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

		public SensorStateDataItem(
			string parentId,
			SubTypes subType
			)
		{
			Id = CreateId(parentId, NameId, GetSubTypeId(subType));
			Category = CategoryId;
			Type = TypeId;
			SubType = subType.ToString();
			Name = NameId;
			Units = Devices.Units.COUNT;
		}


		public override string SubTypeDescription => GetSubTypeDescription(SubType);

		public static string GetSubTypeDescription(string subType)
		{
			var s = subType.ConvertEnum<SubTypes>();
			switch (s)
			{
				case SubTypes.BINARY: return "Detection result of a sensor where the state is Binary.";
				case SubTypes.BOOLEAN: return "Detection result of a sensor where the state is Boolean.";
				case SubTypes.DETECT: return "Detection result of a sensor where the state is Detect.";
				case SubTypes.ENUMERATED: return "Detection result of a sensor where the state is Enumerated.";
			}

			return null;
		}

		public static string GetSubTypeId(SubTypes subType)
		{
			switch (subType)
			{
				case SubTypes.BINARY: return "bin";
				case SubTypes.BOOLEAN: return "bool";
				case SubTypes.DETECT: return "dct";
				case SubTypes.ENUMERATED: return "enum";
			}

			return null;
		}
	}
}
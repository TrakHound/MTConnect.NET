// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The geometric volume of an object or container.
    /// </summary>
    public class VolumeSpatialDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "VOLUME_SPATIAL";
        public const string NameId = "volSpatial";
        public const string DefaultUnits = Devices.Units.CUBIC_MILLIMETER;
        public new const string DescriptionText = "The geometric volume of an object or container.";

        public override string TypeDescription => DescriptionText;

        public enum SubTypes
        {
            /// <summary>
            /// The measured or reported value of an observation.
            /// </summary>
            ACTUAL,

            /// <summary>
            /// Boundary when an activity or an event commences.
            /// </summary>
            START,

            /// <summary>
            /// Boundary when an activity or an event terminates.
            /// </summary>
            ENDED,

            /// <summary>
            /// Reported or measured value of the amount used in the manufacturing process.
            /// </summary>
            CONSUMED,

            /// <summary>
            /// Reported or measured value of the amount discarded.
            /// </summary>
            WASTE,

            /// <summary>
            /// Reported or measured value of amount included in the Part.
            /// </summary>
            PART
        }


        public VolumeSpatialDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public VolumeSpatialDataItem(
            string parentId,
            SubTypes subType = SubTypes.ACTUAL
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Units = DefaultUnits;
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.ACTUAL: return "The measured or reported value of an observation.";
                case SubTypes.START: return "Boundary when an activity or an event commences.";
                case SubTypes.ENDED: return "Boundary when an activity or an event terminates.";
                case SubTypes.CONSUMED: return "Reported or measured value of the amount used in the manufacturing process.";
                case SubTypes.WASTE: return "Reported or measured value of the amount discarded.";
                case SubTypes.PART: return "Reported or measured value of amount included in the Part.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            { 
                case SubTypes.ACTUAL: return "act";
                case SubTypes.START: return "start";
                case SubTypes.ENDED: return "ended";
                case SubTypes.CONSUMED: return "consumed";
                case SubTypes.WASTE: return "waste";
                case SubTypes.PART: return "part";
            }

            return null;
        }
    }
}

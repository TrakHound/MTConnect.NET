// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Geometric volume of an object or container.
    /// </summary>
    public class VolumeSpatialDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "VOLUME_SPATIAL";
        public const string NameId = "volumeSpatial";
        public const string DefaultUnits = Devices.Units.CUBIC_MILLIMETER;     
        public new const string DescriptionText = "Geometric volume of an object or container.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15;       


        public enum SubTypes
        {
            /// <summary>
            /// Measured or reported value of an observation.
            /// </summary>
            ACTUAL,
            
            /// <summary>
            /// Reported or measured value of the amount used in the manufacturing process.
            /// </summary>
            CONSUMED,
            
            /// <summary>
            /// Reported or measured value of amount included in the part.
            /// </summary>
            PART,
            
            /// <summary>
            /// Reported or measured value of the amount discarded
            /// </summary>
            WASTE,
            
            /// <summary>
            /// Boundary when an activity or an event terminates.
            /// </summary>
            ENDED,
            
            /// <summary>
            /// Boundary when an activity or an event commences.
            /// </summary>
            START
        }


        public VolumeSpatialDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public VolumeSpatialDataItem(
            string parentId,
            SubTypes subType
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
                case SubTypes.ACTUAL: return "Measured or reported value of an observation.";
                case SubTypes.CONSUMED: return "Reported or measured value of the amount used in the manufacturing process.";
                case SubTypes.PART: return "Reported or measured value of amount included in the part.";
                case SubTypes.WASTE: return "Reported or measured value of the amount discarded";
                case SubTypes.ENDED: return "Boundary when an activity or an event terminates.";
                case SubTypes.START: return "Boundary when an activity or an event commences.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTUAL: return "ACTUAL";
                case SubTypes.CONSUMED: return "CONSUMED";
                case SubTypes.PART: return "PART";
                case SubTypes.WASTE: return "WASTE";
                case SubTypes.ENDED: return "ENDED";
                case SubTypes.START: return "START";
            }

            return null;
        }

    }
}
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Rotational speed of the rotary axis.**DEPRECATED** in *Version 1.2*.  Replaced by `ROTARY_VELOCITY`.
    /// </summary>
    public class SpindleSpeedDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "SPINDLE_SPEED";
        public const string NameId = "spindleSpeed";
        public const string DefaultUnits = Devices.Units.REVOLUTION_PER_MINUTE;     
        public new const string DescriptionText = "Rotational speed of the rotary axis.**DEPRECATED** in *Version 1.2*.  Replaced by `ROTARY_VELOCITY`.";
        
        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version12;
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public enum SubTypes
        {
            /// <summary>
            /// Measured or reported value of an observation.**DEPRECATED** in *Version 1.3*.
            /// </summary>
            ACTUAL,
            
            /// <summary>
            /// Directive value including adjustments such as an offset or overrides.**DEPRECATED** in *Version 1.3*.
            /// </summary>
            COMMANDED,
            
            /// <summary>
            /// Operator's overridden value.**DEPRECATED** in *Version 1.3*.
            /// </summary>
            OVERRIDE
        }


        public SpindleSpeedDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public SpindleSpeedDataItem(
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
                case SubTypes.ACTUAL: return "Measured or reported value of an observation.**DEPRECATED** in *Version 1.3*.";
                case SubTypes.COMMANDED: return "Directive value including adjustments such as an offset or overrides.**DEPRECATED** in *Version 1.3*.";
                case SubTypes.OVERRIDE: return "Operator's overridden value.**DEPRECATED** in *Version 1.3*.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTUAL: return "ACTUAL";
                case SubTypes.COMMANDED: return "COMMANDED";
                case SubTypes.OVERRIDE: return "OVERRIDE";
            }

            return null;
        }

    }
}
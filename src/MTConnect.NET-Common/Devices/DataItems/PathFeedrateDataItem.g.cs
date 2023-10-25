// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Feedrate for the axes, or a single axis, associated with a Path component.
    /// </summary>
    public class PathFeedrateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "PATH_FEEDRATE";
        public const string NameId = "";
        public const string DefaultUnits = Devices.Units.MILLIMETER_PER_SECOND;     
        public new const string DescriptionText = "Feedrate for the axes, or a single axis, associated with a Path component.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public enum SubTypes
        {
            /// <summary>
            /// Measured or reported value of an observation.
            /// </summary>
            ACTUAL,
            
            /// <summary>
            /// Directive value including adjustments such as an offset or overrides.
            /// </summary>
            COMMANDED,
            
            /// <summary>
            /// Relating to momentary activation of a function or a movement.**DEPRECATION WARNING**: May be deprecated in the future.
            /// </summary>
            JOG,
            
            /// <summary>
            /// Directive value without offsets and adjustments.
            /// </summary>
            PROGRAMMED,
            
            /// <summary>
            /// Performing an operation faster or in less time than nominal rate.
            /// </summary>
            RAPID,
            
            /// <summary>
            /// Operator's overridden value.**DEPRECATED** in *Version 1.3*.
            /// </summary>
            OVERRIDE
        }


        public PathFeedrateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public PathFeedrateDataItem(
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
                case SubTypes.COMMANDED: return "Directive value including adjustments such as an offset or overrides.";
                case SubTypes.JOG: return "Relating to momentary activation of a function or a movement.**DEPRECATION WARNING**: May be deprecated in the future.";
                case SubTypes.PROGRAMMED: return "Directive value without offsets and adjustments.";
                case SubTypes.RAPID: return "Performing an operation faster or in less time than nominal rate.";
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
                case SubTypes.JOG: return "JOG";
                case SubTypes.PROGRAMMED: return "PROGRAMMED";
                case SubTypes.RAPID: return "RAPID";
                case SubTypes.OVERRIDE: return "OVERRIDE";
            }

            return null;
        }

    }
}
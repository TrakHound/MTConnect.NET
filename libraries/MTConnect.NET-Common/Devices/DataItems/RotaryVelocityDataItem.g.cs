// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Rotational speed of a rotary axis.
    /// </summary>
    public class RotaryVelocityDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "ROTARY_VELOCITY";
        public const string NameId = "rotaryVelocity";
             
        public const string DefaultUnits = Devices.Units.REVOLUTION_PER_MINUTE;     
        public new const string DescriptionText = "Rotational speed of a rotary axis.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


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
            /// Directive value without offsets and adjustments.
            /// </summary>
            PROGRAMMED,
            
            /// <summary>
            /// The operators overridden value.
            /// </summary>
            OVERRIDE
        }


        public RotaryVelocityDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public RotaryVelocityDataItem(
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
                case SubTypes.PROGRAMMED: return "Directive value without offsets and adjustments.";
                case SubTypes.OVERRIDE: return "The operators overridden value.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTUAL: return "ACTUAL";
                case SubTypes.COMMANDED: return "COMMANDED";
                case SubTypes.PROGRAMMED: return "PROGRAMMED";
                case SubTypes.OVERRIDE: return "OVERRIDE";
            }

            return null;
        }

    }
}
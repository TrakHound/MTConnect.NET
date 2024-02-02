// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Amount of water vapor present expressed as a percent to reach saturation at the same temperature.
    /// </summary>
    public class HumidityRelativeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "HUMIDITY_RELATIVE";
        public const string NameId = "humidityRelative";
             
        public const string DefaultUnits = Devices.Units.PERCENT;     
        public new const string DescriptionText = "Amount of water vapor present expressed as a percent to reach saturation at the same temperature.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version16;       


        public enum SubTypes
        {
            /// <summary>
            /// Directive value including adjustments such as an offset or overrides.
            /// </summary>
            COMMANDED,
            
            /// <summary>
            /// Measured or reported value of an observation.
            /// </summary>
            ACTUAL
        }


        public HumidityRelativeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public HumidityRelativeDataItem(
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
                case SubTypes.COMMANDED: return "Directive value including adjustments such as an offset or overrides.";
                case SubTypes.ACTUAL: return "Measured or reported value of an observation.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.COMMANDED: return "COMMANDED";
                case SubTypes.ACTUAL: return "ACTUAL";
            }

            return null;
        }

    }
}
// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Negative rate of change of velocity.
    /// </summary>
    public class DecelerationDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "DECELERATION";
        public const string NameId = "deceleration";
             
        public const string DefaultUnits = Devices.Units.MILLIMETER_PER_SECOND_SQUARED;     
        public new const string DescriptionText = "Negative rate of change of velocity.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public enum SubTypes
        {
            /// <summary>
            /// Directive value without offsets and adjustments.
            /// </summary>
            PROGRAMMED,
            
            /// <summary>
            /// Directive value including adjustments such as an offset or overrides.
            /// </summary>
            COMMANDED,
            
            /// <summary>
            /// Measured or reported value of an observation.
            /// </summary>
            ACTUAL
        }


        public DecelerationDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public DecelerationDataItem(
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
                case SubTypes.PROGRAMMED: return "Directive value without offsets and adjustments.";
                case SubTypes.COMMANDED: return "Directive value including adjustments such as an offset or overrides.";
                case SubTypes.ACTUAL: return "Measured or reported value of an observation.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.PROGRAMMED: return "PROGRAMMED";
                case SubTypes.COMMANDED: return "COMMANDED";
                case SubTypes.ACTUAL: return "ACTUAL";
            }

            return null;
        }

    }
}
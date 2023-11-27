// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Rate of change in spatial volume of material deposited in an additive manufacturing process.
    /// </summary>
    public class DepositionAccelerationVolumetricDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "DEPOSITION_ACCELERATION_VOLUMETRIC";
        public const string NameId = "depositionAccelerationVolumetric";
        public const string DefaultUnits = Devices.Units.CUBIC_MILLIMETER_PER_SECOND_SQUARED;     
        public new const string DescriptionText = "Rate of change in spatial volume of material deposited in an additive manufacturing process.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15;       


        public enum SubTypes
        {
            /// <summary>
            /// Measured or reported value of an observation.
            /// </summary>
            ACTUAL,
            
            /// <summary>
            /// Directive value including adjustments such as an offset or overrides.
            /// </summary>
            COMMANDED
        }


        public DepositionAccelerationVolumetricDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public DepositionAccelerationVolumetricDataItem(
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
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTUAL: return "ACTUAL";
                case SubTypes.COMMANDED: return "COMMANDED";
            }

            return null;
        }

    }
}
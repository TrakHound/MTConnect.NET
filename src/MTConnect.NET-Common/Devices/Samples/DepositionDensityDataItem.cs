// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The density of the material deposited in an additive manufacturing process per unit of volume.
    /// </summary>
    public class DepositionDensityDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "DEPOSITION_DENSITY";
        public const string NameId = "depDensity";
        public const string DefaultUnits = Devices.Units.MILLIGRAM_PER_CUBIC_MILLIMETER;
        public new const string DescriptionText = "The density of the material deposited in an additive manufacturing process per unit of volume.";

        public override string TypeDescription => DescriptionText;

        public enum SubTypes
        {
            /// <summary>
            /// The measured or reported value of an observation.
            /// </summary>
            ACTUAL,

            /// <summary>
            /// Directive value including adjustments such as an offset or overrides.
            /// </summary>
            COMMANDED
        }


        public DepositionDensityDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public DepositionDensityDataItem(
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
                case SubTypes.COMMANDED: return "Directive value including adjustments such as an offset or overrides.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            { 
                case SubTypes.ACTUAL: return "act";
                case SubTypes.COMMANDED: return "cmd";
            }

            return null;
        }
    }
}

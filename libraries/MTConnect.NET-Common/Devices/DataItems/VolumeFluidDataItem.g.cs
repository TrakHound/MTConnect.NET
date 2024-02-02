// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Fluid volume of an object or container.
    /// </summary>
    public class VolumeFluidDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "VOLUME_FLUID";
        public const string NameId = "volumeFluid";
             
        public const string DefaultUnits = Devices.Units.MILLILITER;     
        public new const string DescriptionText = "Fluid volume of an object or container.";
        
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
            /// Reported or measured value of the amount discarded.
            /// </summary>
            WASTE,
            
            /// <summary>
            /// Boundary when an activity or an event commences.
            /// </summary>
            START,
            
            /// <summary>
            /// Boundary when an activity or an event terminates.
            /// </summary>
            ENDED
        }


        public VolumeFluidDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public VolumeFluidDataItem(
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
                case SubTypes.WASTE: return "Reported or measured value of the amount discarded.";
                case SubTypes.START: return "Boundary when an activity or an event commences.";
                case SubTypes.ENDED: return "Boundary when an activity or an event terminates.";
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
                case SubTypes.START: return "START";
                case SubTypes.ENDED: return "ENDED";
            }

            return null;
        }

    }
}
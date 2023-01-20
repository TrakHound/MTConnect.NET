// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// The fluid volume of an object or container.
    /// </summary>
    public class VolumeFluidDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "VOLUME_FLUID";
        public const string NameId = "volFluid";
        public const string DefaultUnits = Devices.Units.MILLILITER;
        public new const string DescriptionText = "The fluid volume of an object or container.";

        public override string TypeDescription => DescriptionText;

        public override Version MinimumVersion => MTConnectVersions.Version15;

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


        public VolumeFluidDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public VolumeFluidDataItem(
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


        protected override IDataItem OnProcess(IDataItem dataItem, Version mtconnectVersion)
        {
            if (SubType == SubTypes.ENDED.ToString() && mtconnectVersion < MTConnectVersions.Version18) return null;
            if (SubType == SubTypes.PART.ToString() && mtconnectVersion < MTConnectVersions.Version18) return null;
            if (SubType == SubTypes.START.ToString() && mtconnectVersion < MTConnectVersions.Version18) return null;
            if (SubType == SubTypes.WASTE.ToString() && mtconnectVersion < MTConnectVersions.Version18) return null;

            return dataItem;
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

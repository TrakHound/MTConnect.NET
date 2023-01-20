// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// Strength of electrical current.
    /// </summary>
    public class AmperageDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "AMPERAGE";
        public const string NameId = "amperage";
        public const string DefaultUnits = Devices.Units.AMPERE;
        public new const string DescriptionText = "Strength of electrical current.";

        public override string TypeDescription => DescriptionText;

        public override Version MaximumVersion => MTConnectVersions.Version16;


        public enum SubTypes
        {
            /// <summary>
            /// The measured or reported value of an observation.
            /// </summary>
            ACTUAL,

            /// <summary>
            /// Measurement of alternating voltage or current. If not specified further in statistic, defaults to RMS voltage.
            /// </summary>
            ALTERNATING,

            /// <summary>
            /// Measurement of DC current or voltage.
            /// </summary>
            DIRECT,

            /// <summary>
            /// Goal of the operation or process.
            /// </summary>
            TARGET
        }


        public AmperageDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }
        public AmperageDataItem(
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
            if (SubType == SubTypes.ACTUAL.ToString() && mtconnectVersion < MTConnectVersions.Version14) return null;
            if (SubType == SubTypes.ALTERNATING.ToString() && mtconnectVersion < MTConnectVersions.Version12) return null;
            if (SubType == SubTypes.DIRECT.ToString() && mtconnectVersion < MTConnectVersions.Version12) return null;
            if (SubType == SubTypes.TARGET.ToString() && mtconnectVersion < MTConnectVersions.Version14) return null;

            return dataItem;
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.ACTUAL: return "The measured or reported value of an observation.";
                case SubTypes.ALTERNATING: return "Measurement of alternating voltage or current. If not specified further in statistic, defaults to RMS voltage.";
                case SubTypes.DIRECT: return "Measurement of DC current or voltage.";
                case SubTypes.TARGET: return "Goal of the operation or process.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTUAL: return "act";
                case SubTypes.ALTERNATING: return "ac";
                case SubTypes.DIRECT: return "dc";
                case SubTypes.TARGET: return "tgt";
            }

            return null;
        }
    }
}
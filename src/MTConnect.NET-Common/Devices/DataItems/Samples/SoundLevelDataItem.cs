// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// The measurement of a sound level or sound pressure level relative to atmospheric pressure.
    /// </summary>
    public class SoundLevelDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "SOUND_LEVEL";
        public const string NameId = "soundlvl";
        public const string DefaultUnits = Devices.Units.DECIBEL;
        public new const string DescriptionText = "The measurement of a sound level or sound pressure level relative to atmospheric pressure.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version12;

        public enum SubTypes
        {
            /// <summary>
            /// No weighting factor on the frequency scale
            /// </summary>
            NO_SCALE,

            /// <summary>
            /// A Scale weighting factor. This is the default weighting factor if no factor is specified
            /// </summary>
            A_SCALE,

            /// <summary>
            /// B Scale weighting factor
            /// </summary>
            B_SCALE,

            /// <summary>
            /// C Scale weighting factor
            /// </summary>
            C_SCALE,

            /// <summary>
            /// D Scale weighting factor
            /// </summary>
            D_SCALE
        }


        public SoundLevelDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public SoundLevelDataItem(
            string parentId,
            SubTypes subType = SubTypes.NO_SCALE
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
                case SubTypes.NO_SCALE: return "No weighting factor on the frequency scale";
                case SubTypes.A_SCALE: return "A Scale weighting factor. This is the default weighting factor if no factor is specified";
                case SubTypes.B_SCALE: return "B Scale weighting factor";
                case SubTypes.C_SCALE: return "C Scale weighting factor";
                case SubTypes.D_SCALE: return "D Scale weighting factor";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            { 
                case SubTypes.NO_SCALE: return "";
                case SubTypes.A_SCALE: return "A";
                case SubTypes.B_SCALE: return "B";
                case SubTypes.C_SCALE: return "C";
                case SubTypes.D_SCALE: return "D";
            }

            return null;
        }
    }
}

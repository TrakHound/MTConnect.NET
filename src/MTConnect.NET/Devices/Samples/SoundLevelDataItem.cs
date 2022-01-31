// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The measurement of a sound level or sound pressure level relative to atmospheric pressure.
    /// </summary>
    public class SoundLevelDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "SOUND_LEVEL";
        public const string NameId = "soundlvl";

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
            DataItemCategory = CategoryId;
            Type = TypeId;
            Units = Devices.Units.DECIBEL;
        }

        public SoundLevelDataItem(
            string parentId,
            SubTypes subType = SubTypes.NO_SCALE
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            DataItemCategory = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Units = Devices.Units.DECIBEL;
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

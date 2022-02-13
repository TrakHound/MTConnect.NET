// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The embedded software of a component.
    /// </summary>
    public class FirmwareDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "FIRMWARE";
        public const string NameId = "firmware";
        public new const string DescriptionText = "The embedded software of a component.";

        public override string TypeDescription => DescriptionText;

        public enum SubTypes
        {
            /// <summary>
            /// The date the hardware or software was installed.
            /// </summary>
            INSTALL_DATE,

            /// <summary>
            /// The license code to validate or activate the hardware or software.
            /// </summary>
            LICENSE,

            /// <summary>
            /// The corporate identity for the maker of the hardware or software.
            /// </summary>
            MANUFACTURER,

            /// <summary>
            /// The date the hardware or software was released for general use.
            /// </summary>
            RELEASE_DATE,

            /// <summary>
            /// The version of the hardware or software.
            /// </summary>
            VERSION
        }


        public FirmwareDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public FirmwareDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }

        public override string GetSubTypeDescription() => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.INSTALL_DATE: return "The date the hardware or software was installed.";
                case SubTypes.LICENSE: return "The license code to validate or activate the hardware or software.";
                case SubTypes.MANUFACTURER: return "The corporate identity for the maker of the hardware or software.";
                case SubTypes.RELEASE_DATE: return "The date the hardware or software was released for general use.";
                case SubTypes.VERSION: return "The version of the hardware or software.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.INSTALL_DATE: return "installDate";
                case SubTypes.LICENSE: return "license";
                case SubTypes.MANUFACTURER: return "manufacturer";
                case SubTypes.RELEASE_DATE: return "relDate";
                case SubTypes.VERSION: return "version";
            }

            return null;
        }
    }
}

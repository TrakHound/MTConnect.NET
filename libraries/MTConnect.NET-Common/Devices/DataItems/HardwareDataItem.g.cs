// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_91b028d_1587751253562_643001_2625

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Hardware of a Component.
    /// </summary>
    public class HardwareDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "HARDWARE";
        public const string NameId = "hardware";
             
             
        public new const string DescriptionText = "Hardware of a Component.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version16;       


        public enum SubTypes
        {
            /// <summary>
            /// Version of the hardware or software.
            /// </summary>
            VERSION,
            
            /// <summary>
            /// Date the hardware or software was released for general use.
            /// </summary>
            RELEASE_DATE,
            
            /// <summary>
            /// Corporate identity for the maker of the hardware or software.
            /// </summary>
            MANUFACTURER,
            
            /// <summary>
            /// License code to validate or activate the hardware or software.
            /// </summary>
            LICENSE,
            
            /// <summary>
            /// Date the hardware or software was installed.
            /// </summary>
            INSTALL_DATE,
            
            /// <summary>
            /// Model info of the hardware or software.
            /// </summary>
            MODEL
        }


        public HardwareDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public HardwareDataItem(
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

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.VERSION: return "Version of the hardware or software.";
                case SubTypes.RELEASE_DATE: return "Date the hardware or software was released for general use.";
                case SubTypes.MANUFACTURER: return "Corporate identity for the maker of the hardware or software.";
                case SubTypes.LICENSE: return "License code to validate or activate the hardware or software.";
                case SubTypes.INSTALL_DATE: return "Date the hardware or software was installed.";
                case SubTypes.MODEL: return "Model info of the hardware or software.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.VERSION: return "VERSION";
                case SubTypes.RELEASE_DATE: return "RELEASE_DATE";
                case SubTypes.MANUFACTURER: return "MANUFACTURER";
                case SubTypes.LICENSE: return "LICENSE";
                case SubTypes.INSTALL_DATE: return "INSTALL_DATE";
                case SubTypes.MODEL: return "MODEL";
            }

            return null;
        }

    }
}
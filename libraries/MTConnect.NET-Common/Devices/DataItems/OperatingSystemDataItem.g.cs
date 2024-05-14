// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_91b028d_1587749356759_918178_2174

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Operating System (OS) of a Component.
    /// </summary>
    public class OperatingSystemDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "OPERATING_SYSTEM";
        public const string NameId = "operatingSystem";
             
             
        public new const string DescriptionText = "Operating System (OS) of a Component.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version16;       


        public enum SubTypes
        {
            /// <summary>
            /// License code to validate or activate the hardware or software.
            /// </summary>
            LICENSE,
            
            /// <summary>
            /// Version of the hardware or software.
            /// </summary>
            VERSION,
            
            /// <summary>
            /// Date the hardware or software was released for general use.
            /// </summary>
            RELEASE_DATE,
            
            /// <summary>
            /// Date the hardware or software was installed.
            /// </summary>
            INSTALL_DATE,
            
            /// <summary>
            /// Corporate identity for the maker of the hardware or software.
            /// </summary>
            MANUFACTURER
        }


        public OperatingSystemDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public OperatingSystemDataItem(
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
                case SubTypes.LICENSE: return "License code to validate or activate the hardware or software.";
                case SubTypes.VERSION: return "Version of the hardware or software.";
                case SubTypes.RELEASE_DATE: return "Date the hardware or software was released for general use.";
                case SubTypes.INSTALL_DATE: return "Date the hardware or software was installed.";
                case SubTypes.MANUFACTURER: return "Corporate identity for the maker of the hardware or software.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.LICENSE: return "LICENSE";
                case SubTypes.VERSION: return "VERSION";
                case SubTypes.RELEASE_DATE: return "RELEASE_DATE";
                case SubTypes.INSTALL_DATE: return "INSTALL_DATE";
                case SubTypes.MANUFACTURER: return "MANUFACTURER";
            }

            return null;
        }

    }
}
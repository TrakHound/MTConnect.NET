// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218288_775940_1776

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication of whether the end of a piece of bar stock being feed by a bar feeder has been reached.
    /// </summary>
    public class EndOfBarDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "END_OF_BAR";
        public const string NameId = "endOfBar";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
             
        public new const string DescriptionText = "Indication of whether the end of a piece of bar stock being feed by a bar feeder has been reached.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public enum SubTypes
        {
            /// <summary>
            /// Specific applications **MAY** reference one or more locations on a piece of bar stock as the indication for the EndOfBar. The main or most important location **MUST** be designated as the PRIMARY indication for the EndOfBar.If no subType is specified, PRIMARY **MUST** be the default EndOfBar indication.
            /// </summary>
            PRIMARY,
            
            /// <summary>
            /// When multiple locations on a piece of bar stock are referenced as the indication for the EndOfBar, the additional location(s) **MUST** be designated as `AUXILIARY` indication(s) for the EndOfBar.
            /// </summary>
            AUXILIARY
        }


        public EndOfBarDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public EndOfBarDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Representation = DefaultRepresentation; 
            
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.PRIMARY: return "Specific applications **MAY** reference one or more locations on a piece of bar stock as the indication for the EndOfBar. The main or most important location **MUST** be designated as the PRIMARY indication for the EndOfBar.If no subType is specified, PRIMARY **MUST** be the default EndOfBar indication.";
                case SubTypes.AUXILIARY: return "When multiple locations on a piece of bar stock are referenced as the indication for the EndOfBar, the additional location(s) **MUST** be designated as `AUXILIARY` indication(s) for the EndOfBar.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.PRIMARY: return "PRIMARY";
                case SubTypes.AUXILIARY: return "AUXILIARY";
            }

            return null;
        }

    }
}
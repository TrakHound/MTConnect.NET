// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1760961558820_305572_316

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Dimension or distance as measured downwards from the top
    /// </summary>
    public class DepthDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "DEPTH";
        public const string NameId = "depth";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
        public const string DefaultUnits = Devices.Units.MILLIMETER;     
        public new const string DescriptionText = "Dimension or distance as measured downwards from the top";
        
        public override string TypeDescription => DescriptionText;
        
               


        public enum SubTypes
        {
            /// <summary>
            /// PLUNGE
            /// </summary>
            PLUNGE,
            
            /// <summary>
            /// PECK
            /// </summary>
            PECK,
            
            /// <summary>
            /// CUT
            /// </summary>
            CUT,
            
            /// <summary>
            /// LAYER
            /// </summary>
            LAYER
        }


        public DepthDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            Units = DefaultUnits;
        }

        public DepthDataItem(
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
            Units = DefaultUnits;
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.PLUNGE: return "PLUNGE";
                case SubTypes.PECK: return "PECK";
                case SubTypes.CUT: return "CUT";
                case SubTypes.LAYER: return "LAYER";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.PLUNGE: return "PLUNGE";
                case SubTypes.PECK: return "PECK";
                case SubTypes.CUT: return "CUT";
                case SubTypes.LAYER: return "LAYER";
            }

            return null;
        }

    }
}
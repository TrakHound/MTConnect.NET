// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Operating state of a mechanism represented by a Composition entity.
    /// </summary>
    public class CompositionStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "COMPOSITION_STATE";
        public const string NameId = "compositionState";
             
        public new const string DescriptionText = "Operating state of a mechanism represented by a Composition entity.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public enum SubTypes
        {
            /// <summary>
            /// Indication of the operating state of a mechanism.
            /// </summary>
            ACTION,
            
            /// <summary>
            /// Indication of the position of a mechanism that may move in a lateral direction.
            /// </summary>
            LATERAL,
            
            /// <summary>
            /// Indication of the open or closed state of a mechanism.
            /// </summary>
            MOTION,
            
            /// <summary>
            /// Indication of the activation state of a mechanism.
            /// </summary>
            SWITCHED,
            
            /// <summary>
            /// Indication of the position of a mechanism that may move in a vertical direction.
            /// </summary>
            VERTICAL
        }


        public CompositionStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public CompositionStateDataItem(
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
                case SubTypes.ACTION: return "Indication of the operating state of a mechanism.";
                case SubTypes.LATERAL: return "Indication of the position of a mechanism that may move in a lateral direction.";
                case SubTypes.MOTION: return "Indication of the open or closed state of a mechanism.";
                case SubTypes.SWITCHED: return "Indication of the activation state of a mechanism.";
                case SubTypes.VERTICAL: return "Indication of the position of a mechanism that may move in a vertical direction.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTION: return "ACTION";
                case SubTypes.LATERAL: return "LATERAL";
                case SubTypes.MOTION: return "MOTION";
                case SubTypes.SWITCHED: return "SWITCHED";
                case SubTypes.VERTICAL: return "VERTICAL";
            }

            return null;
        }

    }
}
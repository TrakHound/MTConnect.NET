// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Value of a signal or calculation issued to adjust the feedrate for the axes associated with a Path component that may represent a single axis or the coordinated movement of multiple axes.
    /// </summary>
    public class PathFeedrateOverrideDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PATH_FEEDRATE_OVERRIDE";
        public const string NameId = "pathFeedrateOverride";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
             
        public new const string DescriptionText = "Value of a signal or calculation issued to adjust the feedrate for the axes associated with a Path component that may represent a single axis or the coordinated movement of multiple axes.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public enum SubTypes
        {
            /// <summary>
            /// Relating to momentary activation of a function or a movement.**DEPRECATION WARNING**: May be deprecated in the future.
            /// </summary>
            JOG,
            
            /// <summary>
            /// Directive value without offsets and adjustments.
            /// </summary>
            PROGRAMMED,
            
            /// <summary>
            /// Performing an operation faster or in less time than nominal rate.
            /// </summary>
            RAPID
        }


        public PathFeedrateOverrideDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public PathFeedrateOverrideDataItem(
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
                case SubTypes.JOG: return "Relating to momentary activation of a function or a movement.**DEPRECATION WARNING**: May be deprecated in the future.";
                case SubTypes.PROGRAMMED: return "Directive value without offsets and adjustments.";
                case SubTypes.RAPID: return "Performing an operation faster or in less time than nominal rate.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.JOG: return "JOG";
                case SubTypes.PROGRAMMED: return "PROGRAMMED";
                case SubTypes.RAPID: return "RAPID";
            }

            return null;
        }

    }
}
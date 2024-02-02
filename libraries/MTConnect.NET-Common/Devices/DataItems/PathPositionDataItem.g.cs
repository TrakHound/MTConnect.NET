// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Position of a control point associated with a Controller or a Path.
    /// </summary>
    public class PathPositionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "PATH_POSITION";
        public const string NameId = "pathPosition";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
        public const string DefaultUnits = Devices.Units.MILLIMETER_3D;     
        public new const string DescriptionText = "Position of a control point associated with a Controller or a Path.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public enum SubTypes
        {
            /// <summary>
            /// Measured or reported value of an observation.
            /// </summary>
            ACTUAL,
            
            /// <summary>
            /// Directive value including adjustments such as an offset or overrides.
            /// </summary>
            COMMANDED,
            
            /// <summary>
            /// Goal of the operation or process.
            /// </summary>
            TARGET,
            
            /// <summary>
            /// Position provided by a measurement probe.**DEPRECATION WARNING**: May be deprecated in the future.
            /// </summary>
            PROBE
        }


        public PathPositionDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            Units = DefaultUnits;
        }

        public PathPositionDataItem(
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
                case SubTypes.ACTUAL: return "Measured or reported value of an observation.";
                case SubTypes.COMMANDED: return "Directive value including adjustments such as an offset or overrides.";
                case SubTypes.TARGET: return "Goal of the operation or process.";
                case SubTypes.PROBE: return "Position provided by a measurement probe.**DEPRECATION WARNING**: May be deprecated in the future.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTUAL: return "ACTUAL";
                case SubTypes.COMMANDED: return "COMMANDED";
                case SubTypes.TARGET: return "TARGET";
                case SubTypes.PROBE: return "PROBE";
            }

            return null;
        }

    }
}
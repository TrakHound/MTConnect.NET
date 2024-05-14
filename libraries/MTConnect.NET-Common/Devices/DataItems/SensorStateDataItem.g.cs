// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1677588817278_345680_780

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Detection result of a sensor.
    /// </summary>
    public class SensorStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SENSOR_STATE";
        public const string NameId = "sensorState";
             
             
        public new const string DescriptionText = "Detection result of a sensor.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version22;       


        public enum SubTypes
        {
            /// <summary>
            /// BINARY
            /// </summary>
            BINARY,
            
            /// <summary>
            /// BOOLEAN
            /// </summary>
            BOOLEAN,
            
            /// <summary>
            /// ENUMERATED
            /// </summary>
            ENUMERATED,
            
            /// <summary>
            /// DETECT
            /// </summary>
            DETECT
        }


        public SensorStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public SensorStateDataItem(
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
                case SubTypes.BINARY: return "BINARY";
                case SubTypes.BOOLEAN: return "BOOLEAN";
                case SubTypes.ENUMERATED: return "ENUMERATED";
                case SubTypes.DETECT: return "DETECT";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.BINARY: return "BINARY";
                case SubTypes.BOOLEAN: return "BOOLEAN";
                case SubTypes.ENUMERATED: return "ENUMERATED";
                case SubTypes.DETECT: return "DETECT";
            }

            return null;
        }

    }
}